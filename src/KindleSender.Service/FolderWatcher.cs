using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KindleSender.Service
{
  public class FolderWatcher : IFolderWatcher, IDisposable
  {
    private readonly IConfiguration _configuration;

    private readonly IFileSender _fileSender;

    private readonly IEventLogger _eventLogger;

    private FileSystemWatcher _fileSystemWatcher;

    public FolderWatcher(IConfiguration configuration, IFileSender fileSender, IEventLogger eventLogger)
    {
      if (configuration == null)
      {
        throw new ArgumentNullException("configuration");
      }

      if (fileSender == null)
      {
        throw new ArgumentNullException("fileSender");
      }

      if (eventLogger == null)
      {
        throw new ArgumentNullException("eventLogger");
      }

      _configuration = configuration;
      _fileSender = fileSender;
      _eventLogger = eventLogger;

      InitilaizeFileSystemWatcher();
    }

    public void Start()
    {
      _fileSystemWatcher.EnableRaisingEvents = true;
    }
    public void Stop()
    {
      _fileSystemWatcher.EnableRaisingEvents = false;
    }

    private void InitilaizeFileSystemWatcher()
    {
      if (!Directory.Exists(_configuration.FolderPath))
      {
        Directory.CreateDirectory(_configuration.FolderPath);
        _eventLogger.Write("Created directory: " + _configuration.FolderPath, EventLogEntryType.Information);
      }

      _fileSystemWatcher = new FileSystemWatcher
      {
        Path = _configuration.FolderPath,
        Filter = _configuration.FileFilter,
        EnableRaisingEvents = false
      };

      _fileSystemWatcher.Created += OnFileCreated;

      _eventLogger.Write("Watching folder: " + _fileSystemWatcher.Path, EventLogEntryType.Information);
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
      _eventLogger.Write("New file found: " + e.FullPath, EventLogEntryType.Information);
      _fileSender.Send(e.FullPath);
    }

    public void Dispose()
    {
      _fileSystemWatcher.Created -= OnFileCreated;
      _fileSystemWatcher.Dispose();
    }


  }
}
