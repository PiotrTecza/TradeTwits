using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using Ninject;
using System.Configuration;
using System.Linq;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Installers
{
    public class DBInstaller
    {
        private IMongoDatabase database;
        private readonly string usersCollectionName = "AspNetUsers";
        private readonly string twitsCollectionName = "twits";
        private readonly string stocksDataCollectionName = "stocksData";

        public DBInstaller()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString;
            string databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        public void Install(IKernel kernel)
        {
            RegisterMongoCollection<TwitModel>(kernel, twitsCollectionName);
            RegisterMongoCollection<ApplicationUser>(kernel, usersCollectionName);
            RegisterMongoCollection<StockDataModel>(kernel, stocksDataCollectionName);

            RegisterUserManager(kernel);
       }

        private void RegisterMongoCollection<T>(IKernel kernel, string collectionName)
        {
            kernel.Bind<IMongoCollection<T>>().ToConstant(database.GetCollection<T>(collectionName));
            kernel.Bind<IQueryable<T>>().ToConstant(database.GetCollection<T>(collectionName).AsQueryable());
        }

        private void RegisterUserManager(IKernel kernel)
        {
            var userCollection = database.GetCollection<ApplicationUser>(usersCollectionName);


            kernel.Bind<UserManager<ApplicationUser>>().ToMethod(c =>
            {
                var userStore = new UserStore<ApplicationUser>(userCollection);
                var userManager = new UserManager<ApplicationUser>(userStore);
                return userManager;
            });
        }
    }
}
