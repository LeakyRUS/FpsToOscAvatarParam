using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;
using System.Reflection;

namespace FpsToOscAvatarParam;

public sealed class WindowsEventFpsProvider : IFpsProvider, IDisposable
{
    //https://stackoverflow.com/questions/18340347/monitoring-the-fps-of-a-direct-x-application

    public const ushort EventID_D3D9PresentStart = 1;
    public const ushort EventID_DxgiPresentStart = 42;

    //ETW provider codes
    public static readonly Guid DXGI_provider = Guid.Parse("{CA11C036-0102-4A2D-A6AD-F03CFED5D3C9}");

    private readonly TraceEventSession _etwSession = new($"{Assembly.GetExecutingAssembly().FullName}:{Environment.CurrentManagedThreadId}");
    private readonly Dictionary<int, TimestampCollection> _frames = [];
    private readonly Dictionary<string, double> _progFps = [];
    private readonly Stopwatch _watch = new();
    private readonly object _sync = new();

    private Thread _etwThread = null!;
    private Thread _outputThread = null!;

    private volatile bool _run = true;

    public WindowsEventFpsProvider()
    {
        Run();
    }

    private void Run()
    {
        //create ETW session and register providers
        _etwSession.StopOnDispose = true;
        _etwSession.EnableProvider("Microsoft-Windows-DXGI");

        //handle event
        _etwSession.Source.AllEvents += data =>
        {
            // filter out frame presentation events
            if ((ushort)data.ID == EventID_DxgiPresentStart && data.ProviderGuid == DXGI_provider)
            {
                int pid = data.ProcessID;

                lock (_sync)
                {
                    var t = _watch.ElapsedMilliseconds;

                    //if process is not yet in Dictionary, add it
                    if (!_frames.ContainsKey(pid))
                    {
                        _frames[pid] = new TimestampCollection();

                        string name = "";

                        Process? proc = null;
                        try
                        {
                            proc = Process.GetProcessById(pid);
                        }
                        catch { }

                        if (proc != null)
                        {
                            using (proc)
                            {
                                name = proc.ProcessName;
                            }
                        }
                        else
                            name = pid.ToString();

                        _frames[pid].Name = name;
                    }

                    //store frame timestamp in collection
                    _frames[pid].Add(t);
                }
            }
        };

        _watch.Start();

        _etwThread = new(EtwThreadProc);
        _etwThread.IsBackground = true;
        _etwThread.Start();

        _outputThread = new(OutputThreadProc);
        _outputThread.IsBackground = true;
        _outputThread.Start();
    }

    void EtwThreadProc()
    {
        _etwSession.Source.Process();
    }

    void OutputThreadProc()
    {
        while (_run)
        {
            long t1, t2;
            long dt = 2000;

            lock (_sync)
            {
                t2 = _watch.ElapsedMilliseconds;
                t1 = t2 - dt;

                foreach (var x in _frames.Values)
                {
                    int count = x.QueryCount(t1, t2);

                    _progFps[x.Name] = (double)count / dt * 1000.0;
                }
            }

            Thread.Sleep(1000);
        }
    }

    private class TimestampCollection
    {
        const int MAXNUM = 1000;

        public string Name { get; set; } = null!;

        List<long> timestamps = new(MAXNUM + 1);
        object sync = new();

        //add value to the collection
        public void Add(long timestamp)
        {
            lock (sync)
            {
                timestamps.Add(timestamp);
                if (timestamps.Count > MAXNUM) timestamps.RemoveAt(0);
            }
        }

        //get the number of timestamps withing interval
        public int QueryCount(long from, long to)
        {
            int c = 0;

            lock (sync)
            {
                foreach (var ts in timestamps)
                {
                    if (ts >= from && ts <= to) c++;
                }
            }
            return c;
        }
    }

    public void Dispose()
    {
        _run = false;
        _etwSession.Dispose();
        GC.SuppressFinalize(this);
    }

    public float GetFps(string key)
    {
        double result;

        lock (_sync)
        {
            if (!_progFps.TryGetValue(key, out result))
                result = 0;
        }

        return (float) result;
    }
}
