using Microsoft.Azure.WebJobs.Host;
using Ninject;

namespace StockDataWebJob.Ninject
{
    public class Activator :  IJobActivator
    {
        private readonly IKernel kernel;

        public Activator(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public T CreateInstance<T>()
        {
            return kernel.Get<T>();
        }
    }
}
