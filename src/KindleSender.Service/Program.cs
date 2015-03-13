﻿using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace KindleSender.Service
{
  static class Program
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      XmlConfigurator.Configure();

      var servicesToRun = new ServiceBase[] 
      { 
        new KindleSenderService()
      };

      ServiceBase.Run(servicesToRun);
    }
  }
}
