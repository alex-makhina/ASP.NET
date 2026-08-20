using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Messages;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IPromoCodeNotifier
    {
        Task NotifyPromoCodeIssuedAsync(PromoCodeIssuedMessage message);
    }
}
