using System.ComponentModel;
using System.Configuration.Install;

namespace KindleSender.Service
{
  [RunInstaller(true)]
  public partial class ProjectInstaller : Installer
  {
    public ProjectInstaller()
    {
      InitializeComponent();
    }
  }
}
