using Ninject;
using TradeTwits.Data.Helpers;
using TradeTwits.Data.Services;

namespace TradeTwits.Data.Installers
{
    public class ServiceInstaller
    {
        public void Install(IKernel kernel)
        {
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<ITwitsService>().To<TwitsService>();
            kernel.Bind<IStockDataService>().To<StockDataService>();
            kernel.Bind<IDocumentHelper>().To<DocumentHelper>();
        }
    }
}
