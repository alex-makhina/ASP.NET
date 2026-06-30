using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Data;

namespace Pcf.GivingToCustomer.IntegrationTests.Data
{
    public class MongoTestDbInitializer
        : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoTestDbInitializer(IMongoDatabase database)
        {
            _database = database;
        }
        
        public void InitializeDb()
        {
            var preferencesCollection = _database.GetCollection<Preference>(nameof(Preference));
            var customersCollection = _database.GetCollection<Customer>(nameof(Customer));

            preferencesCollection.DeleteMany(FilterDefinition<Preference>.Empty);
            customersCollection.DeleteMany(FilterDefinition<Customer>.Empty);

            preferencesCollection.InsertMany(TestDataFactory.Preferences);
            customersCollection.InsertMany(TestDataFactory.Customers);
        }
    }
}
