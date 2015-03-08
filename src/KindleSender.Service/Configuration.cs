using System;
using System.Configuration;
using System.Diagnostics;

namespace KindleSender.Service
{
  public class Configuration : IConfiguration
  {
    private readonly IEventLogger _eventLogger;

    public Configuration(IEventLogger eventLogger)
    {
      if (eventLogger == null)
      {
        throw new ArgumentNullException("eventLogger");
      }

      _eventLogger = eventLogger;
    }

    public string FolderPath { get; set; }

    public string FileFilter { get; set; }

    public string KindleMail { get; set; }

    public int SmtpPort { get; set; }

    public string SmtpServer { get; set; }

    public string SmtpUserName { get; set; }

    public string SmtpPassword { get; set; }


    public void Load()
    {
      _eventLogger.Write("Loading configuration ... ", EventLogEntryType.Information);

      try
      {
        FolderPath = ConfigurationManager.AppSettings["FolderPath"];
        FileFilter = ConfigurationManager.AppSettings["FileFilter"];
        KindleMail = ConfigurationManager.AppSettings["KindleMail"];

        SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        SmtpUserName = ConfigurationManager.AppSettings["SmtpUserName"];
        SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

        _eventLogger.Write("Configuration loaded.", EventLogEntryType.Information);
      }
      catch (Exception e)
      {
        _eventLogger.Write(e.Message, EventLogEntryType.Error);
      }
    }
  }
}
