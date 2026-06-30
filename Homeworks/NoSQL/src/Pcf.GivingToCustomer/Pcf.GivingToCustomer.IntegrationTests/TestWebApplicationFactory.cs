using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.IntegrationTests.Data;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<INotificationGateway, NotificationGateway>();

                var tempSp = services.BuildServiceProvider();
                var configuration = tempSp.GetRequiredService<IConfiguration>();
                var connString = configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb");

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(IMongoDatabase));

                services.Remove(descriptor);

                var mongoClient = new MongoClient(connString);
                var mongoDatabase = mongoClient.GetDatabase("promocode_factory_givingToCustomer_test_db_api");
                services.AddSingleton(mongoDatabase);

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var database = scopedServices.GetRequiredService<IMongoDatabase>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();
                
                try
                {
                    new MongoTestDbInitializer(database).InitializeDb();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Проблема во время заполнения тестовой базы. " +
                                        "Ошибка: {Message}", ex.Message);
                }
            });
        }
    }
}