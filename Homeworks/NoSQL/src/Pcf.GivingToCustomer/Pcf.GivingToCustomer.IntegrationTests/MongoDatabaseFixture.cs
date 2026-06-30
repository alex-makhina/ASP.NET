using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Pcf.GivingToCustomer.IntegrationTests.Data;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class MongoDatabaseFixture: IDisposable
    {
        private readonly MongoTestDbInitializer _dbInitializer;
        public IMongoDatabase Database { get; private set; }

        public MongoDatabaseFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connString = configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb")
                ?? "mongodb://localhost:27017";

            var mongoClient = new MongoClient(connString);
            Database = mongoClient.GetDatabase("promocode_factory_givingToCustomer_test_db_components");

            _dbInitializer = new MongoTestDbInitializer(Database);
            _dbInitializer.InitializeDb();
        }

        public void Dispose()
        {
        }
    }
}
