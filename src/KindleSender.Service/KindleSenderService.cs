using System.Diagnostics;
using System.ServiceProcess;

namespace KindleSender.Service
{
  public partial class KindleSenderService : ServiceBase
  {
    private IFolderWatcher _folderWatcher;

    private readonly IEventLogger _eventLogger;
    public KindleSenderService()
    {
      _eventLogger = new EventLogger();

      _eventLogger.Write("Initializing Kindle Sender Service.", EventLogEntryType.Information);

      InitializeComponent();

      InitializeService();

      _eventLogger.Write("Initialized Kindle Sender Service.", EventLogEntryType.Information);
    }

    protected override void OnStart(string[] args)
    {
      _folderWatcher.Start();

      _eventLogger.Write("Kindle Sender Service started.", EventLogEntryType.Information);
    }

    protected override void OnStop()
    {
      _folderWatcher.Stop();

      _eventLogger.Write("Kindle Sender Service stoped.", EventLogEntryType.Information);
    }

    private void InitializeService()
    {
      var configuration = new Configuration(_eventLogger);
      configuration.Load();

      var fileSender = new FileSender(configuration, _eventLogger);

      _folderWatcher = new FolderWatcher(configuration, fileSender, _eventLogger);
    }
  }
}
