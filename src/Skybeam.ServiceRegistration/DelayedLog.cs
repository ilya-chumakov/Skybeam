using Microsoft.Extensions.Logging;

namespace Skybeam;

internal class DelayedLog
{
    private List<Action<ILogger>> _logs = [];

    internal void Add(Action<ILogger> log)
    {
        Logs.Add(log);
    }

    private List<Action<ILogger>> Logs
    {
        get
        {
            if (_logs != null) return _logs;

            throw new NotSupportedException("Logs can't be accessed after Release() has been called.");
        }
    }

    public void Replay(ILogger logger)
    {
        foreach (var action in Logs)
        {
            action(logger);
        }
    }

    public void Release()
    {
        _logs = null;
    }

    public int Count()
    {
        return Logs.Count;
    }
}