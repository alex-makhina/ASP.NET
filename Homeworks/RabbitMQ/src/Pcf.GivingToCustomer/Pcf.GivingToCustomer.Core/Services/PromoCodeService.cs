using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Messages;

namespace Pcf.GivingToCustomer.Core.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;
        private readonly IPromoCodeNotifier _promoCodeNotifier;

        public PromoCodeService(IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository,
            IPromoCodeNotifier promoCodeNotifier)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
            _promoCodeNotifier = promoCodeNotifier;
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

            await _promoCodeNotifier.NotifyPromoCodeIssuedAsync(new PromoCodeIssuedMessage
            {
                PartnerId = promocode.PartnerId,
                PromoCode = promocode.Code,
                ServiceInfo = promocode.ServiceInfo,
                PreferenceId = promocode.PreferenceId,
                BeginDate = promocode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promocode.EndDate.ToString("yyyy-MM-dd")
            });
        }
    }
}
