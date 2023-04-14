using System;
using System.Threading;

namespace LBPUnion.ProjectLighthouse.Types.Synchronization;

public class LighthouseGlobalMutex : ILighthouseMutex
{
    private readonly Mutex mutex;

    public LighthouseGlobalMutex(string name)
    {
        this.mutex = new Mutex(false, $"Global\\{name}");
    }

    public void Dispose()
    {
        this.mutex.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool WaitOne() => this.mutex.WaitOne();

    public void ReleaseMutex() => this.mutex.ReleaseMutex();
}