using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using log4net;

namespace KindleSender.Service
{
  public class FileSender : IFileSender
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(FolderWatcher));

    private readonly IConfiguration _configuration;

    private const short FileReadTries = 5;

    public FileSender(IConfiguration configuration)
    {
      if (configuration == null)
      {
        throw new ArgumentNullException("configuration");
      }

      _configuration = configuration;
    }

    public void Send(string filePath)
    {
      var fileName = filePath.Replace(_configuration.FolderPath, string.Empty);

      Log.Info(string.Format("Sending file {0} ... ", fileName));

      var tryOpenFileCounter = 0;

      do
      {
        if (IsFileLocked(filePath))
        {
          Log.Error("File is locked. Try counter: " + tryOpenFileCounter);
          Thread.Sleep(5000);
        }

        tryOpenFileCounter++;
      } while (tryOpenFileCounter > FileReadTries);

      try
      {
        SendEmail(filePath);

        Log.Info(string.Format("Sended file {0} ... ", fileName));
      }
      catch (Exception ex)
      {
        Log.Error(ex.Message);
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
