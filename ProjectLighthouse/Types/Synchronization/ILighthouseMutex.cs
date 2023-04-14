using System;

namespace LBPUnion.ProjectLighthouse.Types.Synchronization;

public interface ILighthouseMutex : IDisposable
{
    public bool WaitOne();
    public void ReleaseMutex();
}