using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;

namespace KindleSender.Service
{
  public class FileSender : IFileSender
  {
    private readonly IConfiguration _configuration;

    private readonly IEventLogger _eventLogger;

    private const short FileReadTries = 5;

    public FileSender(IConfiguration configuration, IEventLogger eventLogger)
    {
      if (configuration == null)
      {
        throw new ArgumentNullException("configuration");
      }

      if (eventLogger == null)
      {
        throw new ArgumentNullException("eventLogger");
      }

      _configuration = configuration;
      _eventLogger = eventLogger;
    }

    public void Send(string filePath)
    {
      var fileName = filePath.Replace(_configuration.FolderPath, string.Empty);

      _eventLogger.Write(string.Format("Sending file {0} ... ", fileName), EventLogEntryType.Information);

      var tryOpenFileCounter = 0;

      do
      {
        if (IsFileLocked(filePath))
        {
          _eventLogger.Write("File is locked. Try counter: " + tryOpenFileCounter, EventLogEntryType.Error);
          Thread.Sleep(5000);
        }

        tryOpenFileCounter++;
      } while (tryOpenFileCounter > FileReadTries);

      try
      {
        SendEmail(filePath);

        _eventLogger.Write(string.Format("Sended file {0} ... ", fileName), EventLogEntryType.Information);
      }
      catch (Exception ex)
      {
        _eventLogger.Write(ex.Message, EventLogEntryType.Error);
      }
    }

    private void SendEmail(string filePath)
    {
      var mail = new MailMessage(_configuration.SmtpUserName, _configuration.KindleMail);
      mail.Attachments.Add(GetAtachment(filePath));

      var client = new SmtpClient
      {
        Port = _configuration.SmtpPort,
        UseDefaultCredentials = false,
        Host = _configuration.SmtpServer,
        Credentials = new System.Net.NetworkCredential(_configuration.SmtpUserName, _configuration.SmtpPassword),
        Timeout = 0
      };

      client.Send(mail);
    }

    private static Attachment GetAtachment(string filePath)
    {
      var attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);

      var disposition = attachment.ContentDisposition;
      disposition.CreationDate = File.GetCreationTime(filePath);
      disposition.ModificationDate = File.GetLastWriteTime(filePath);
      disposition.ReadDate = File.GetLastAccessTime(filePath);
      disposition.FileName = Path.GetFileName(filePath);
      disposition.Size = new FileInfo(filePath).Length;
      disposition.DispositionType = DispositionTypeNames.Attachment;

      return attachment;
    }

    private bool IsFileLocked(string filePath)
    {
      try
      {
        using (File.Open(filePath, FileMode.Open))
        {

        }
      }
      catch (IOException e)
      {
        var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

        return errorCode == 32 || errorCode == 33;
      }

      return false;
    }
  }
}
