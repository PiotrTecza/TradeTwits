using Microsoft.Azure.WebJobs;
using StockDataWebJob.Ninject;
using Ninject;

namespace StockDataWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var kernel = new StandardKernel(new Installer());
            var config = new JobHostConfiguration
            {
                JobActivator = new Activator(kernel)

            };
            var host = new JobHost(config);
            // The following code will invoke a function called ManualTrigger and 
            // pass in data (value in this case) to the function
            host.Call(typeof(Functions).GetMethod("ManualTrigger"), new { value = 20 });
        }
    }
}
