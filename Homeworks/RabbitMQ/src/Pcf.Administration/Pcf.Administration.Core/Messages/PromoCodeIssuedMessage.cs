using System;

namespace Pcf.Administration.Core.Messages
{
    public class PromoCodeIssuedMessage
    {
        public Guid PartnerId { get; set; }
        public string PromoCode { get; set; }
        public string ServiceInfo { get; set; }
        public Guid PreferenceId { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public Guid? PartnerManagerId { get; set; }
    }
}
