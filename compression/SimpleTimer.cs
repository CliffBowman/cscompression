using System.Diagnostics;

namespace compression;

public class SimpleTimer : IDisposable
{
    private readonly Stopwatch _sw;
    private readonly string _msg;
    private readonly TimeSpan? _threshold;

    public SimpleTimer(string msg, TimeSpan? threshold = null)
    {
        _msg = msg;
        _threshold = threshold;
        _sw = new Stopwatch();
        _sw.Start();
    }

    public void Dispose()
    {
        _sw.Stop();

        if (!_threshold.HasValue || _sw.Elapsed > _threshold)
            Console.WriteLine($"{_msg} {_sw.Elapsed} elapsed");
    }
}
