#nullable enable
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LBPUnion.ProjectLighthouse.Types.Synchronization;

public class LighthouseFileMutex : ILighthouseMutex
{

    private readonly string fileName;
    private readonly FileSystemWatcher fileSystemWatcher;
    private readonly SemaphoreSlim notifySemaphore;

    private FileStream? heldFileStream;

    public LighthouseFileMutex(string name)
    {
        this.fileName = name;
        this.fileSystemWatcher = new FileSystemWatcher(this.fileName);
        this.fileSystemWatcher.Filter = name;
        this.fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime;
        this.fileSystemWatcher.Deleted += this.OnFileDeleted;
        this.notifySemaphore = new SemaphoreSlim(0, 1);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        this.notifySemaphore.Release();
    }

    public bool WaitOne()
    {
        if (File.Exists(this.fileName))
        {
            this.notifySemaphore.Wait();
        }
        else
        {
            this.heldFileStream = File.Create(this.fileName);
        }
        return true;
    }

    public async Task<bool> WaitOneAsync()
    {
        if (File.Exists(this.fileName))
        {
            await this.notifySemaphore.WaitAsync();
        }
        else
        {
            this.heldFileStream = File.Create(this.fileName);
        }
        return true;
    }


    public void ReleaseMutex()
    {
        if (this.heldFileStream == null)
        {
            throw new NullReferenceException("Tried to release a mutex that was not held");
        }
        File.Delete(this.fileName);
        this.heldFileStream = null;
    }
}