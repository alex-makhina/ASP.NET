using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Messages;
using Pcf.GivingToCustomer.WebHost.Hubs;

namespace Pcf.GivingToCustomer.WebHost.Notification
{
    public class PromoCodeNotifier
        : IPromoCodeNotifier
    {
        private readonly IHubContext<PromoCodeHub> _hubContext;

        public PromoCodeNotifier(IHubContext<PromoCodeHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPromoCodeIssuedAsync(PromoCodeIssuedMessage message)
        {
            await _hubContext.Clients.All.SendAsync("PromoCodeIssued", message);
        }
    }
}
