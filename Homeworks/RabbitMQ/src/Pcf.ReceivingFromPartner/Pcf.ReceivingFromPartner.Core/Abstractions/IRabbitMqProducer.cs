using Pcf.ReceivingFromPartner.Core.Messages;

namespace Pcf.ReceivingFromPartner.Core.Abstractions
{
    public interface IRabbitMqProducer
    {
        void PublishPromoCodeIssued(PromoCodeIssuedMessage message);
    }
}
