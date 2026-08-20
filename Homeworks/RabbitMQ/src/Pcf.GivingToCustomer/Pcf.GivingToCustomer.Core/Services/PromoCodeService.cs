using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;

        public PromoCodeService(IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
        }

        public async Task GivePromoCodeToCustomersAsync(Guid preferenceId, string promoCode,
            Guid partnerId, string serviceInfo, string beginDate, string endDate)
        {
            var preference = await _preferencesRepository.GetByIdAsync(preferenceId);
            if (preference == null)
                throw new ArgumentException("Предпочтение не найдено");

            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            var promocode = new PromoCode
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Code = promoCode,
                ServiceInfo = serviceInfo,
                BeginDate = DateTime.Parse(beginDate),
                EndDate = DateTime.Parse(endDate),
                Preference = preference,
                PreferenceId = preference.Id
            };

            promocode.Customers = customers.Select(c => new PromoCodeCustomer
            {
                CustomerId = c.Id,
                Customer = c,
                PromoCodeId = promocode.Id,
                PromoCode = promocode
            }).ToList();

            await _promoCodesRepository.AddAsync(promocode);
        }
    }
}
