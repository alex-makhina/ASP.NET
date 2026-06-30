using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public class MongoDbInitializer
        : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoDbInitializer(IMongoDatabase database)
        {
            _database = database;
        }
        
        public void InitializeDb()
        {
            var preferencesCollection = _database.GetCollection<Preference>(nameof(Preference));
            var customersCollection = _database.GetCollection<Customer>(nameof(Customer));

            preferencesCollection.DeleteMany(FilterDefinition<Preference>.Empty);
            customersCollection.DeleteMany(FilterDefinition<Customer>.Empty);

            preferencesCollection.InsertMany(FakeDataFactory.Preferences);
            customersCollection.InsertMany(FakeDataFactory.Customers);
        }
    }
}
