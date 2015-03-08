using System.ServiceProcess;

namespace KindleSender.Service
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      var servicesToRun = new ServiceBase[] 
      { 
        new KindleSenderService()
      };

      ServiceBase.Run(servicesToRun);
    }
  }
}
