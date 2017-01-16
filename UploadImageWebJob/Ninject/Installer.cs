using Ninject.Modules;
using TradeTwits.Data.Installers;

namespace UploadImageWebJob.Ninject
{
    public class Installer : NinjectModule
    {
        public override void Load()
        {
            new DBInstaller().Install(Kernel);
            new ServiceInstaller().Install(Kernel);
        }
    }
}
