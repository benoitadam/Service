using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace DotAdam.Service
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        public ServiceProcessInstaller ProcInstaller { get; private set; }
        public ServiceInstaller SrvInstaller { get; private set; }

        public WindowsServiceInstaller()
        {
            // Installer avec le compte Administrateur : LocalSystem
            ProcInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem,
                Password = null,
                Username = null,
            };

            // Nom et description du service. Attention "ServiceName" doit être identique avec le service.
            SrvInstaller = new ServiceInstaller()
            {
                ServiceName = WindowsService.Name,
                Description = WindowsService.Description,
                DisplayName = WindowsService.Name,
            };

            // Lance le service au démarrage de Windows
            SrvInstaller.StartType = ServiceStartMode.Automatic;

            Installers.AddRange(new Installer[] { ProcInstaller, SrvInstaller });
        }
    }
}
