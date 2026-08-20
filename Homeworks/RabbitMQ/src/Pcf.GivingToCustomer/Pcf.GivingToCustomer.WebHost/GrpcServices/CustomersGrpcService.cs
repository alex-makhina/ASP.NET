using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Grpc;
using DomainCustomer = Pcf.GivingToCustomer.Core.Domain.Customer;
using GrpcCustomer = Pcf.GivingToCustomer.WebHost.Grpc.Customer;
using GrpcPreference = Pcf.GivingToCustomer.WebHost.Grpc.Preference;

namespace Pcf.GivingToCustomer.WebHost.GrpcServices
{
    public class CustomersGrpcService
        : CustomersService.CustomersServiceBase
    {
        private readonly IRepository<DomainCustomer> _customerRepository;

        public CustomersGrpcService(IRepository<DomainCustomer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override async Task<GetCustomersReply> GetCustomers(GetCustomersRequest request,
            ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();

            var reply = new GetCustomersReply();
            reply.Customers.AddRange(customers.Select(MapCustomer));

            return reply;
        }

        public override async Task<GetCustomerReply> GetCustomer(GetCustomerRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный идентификатор клиента"));
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Клиент не найден"));
            }

            return new GetCustomerReply
            {
                Customer = MapCustomer(customer)
            };
        }

        private static GrpcCustomer MapCustomer(DomainCustomer customer)
        {
            var result = new GrpcCustomer
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

            result.Preferences.AddRange((customer.Preferences ?? Enumerable.Empty<CustomerPreference>())
                .Select(x => new GrpcPreference
                {
                    Id = x.Preference.Id.ToString(),
                    Name = x.Preference.Name
                }));

            return result;
        }
    }
}
