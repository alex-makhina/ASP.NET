using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services
{
    public interface IPromoCodeService
    {
        Task GivePromoCodeToCustomersAsync(Guid preferenceId, string promoCode, Guid partnerId,
            string serviceInfo, string beginDate, string endDate);
    }
}
