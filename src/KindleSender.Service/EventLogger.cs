using System;
using System.Diagnostics;

namespace KindleSender.Service
{
  public class EventLogger : IEventLogger
  {
    private const string LogSource = "KindleSenderService";

    private const string LogName = "KindleSenderServiceLog";

    private readonly EventLog _log;

    public EventLogger()
    {
      if (EventLog.SourceExists(LogSource))
      {
        EventLog.Delete(LogName);
        EventLog.DeleteEventSource(LogSource);
      }

      EventLog.CreateEventSource(LogSource, LogName);

      _log = new EventLog { Source = LogSource, Log = LogName };
    }

    public void Write(string message, EventLogEntryType type)
    {
      _log.WriteEntry(message, type);
    }
  }

  public interface IEventLogger
  {
    void Write(string message, EventLogEntryType type);
  }
}
