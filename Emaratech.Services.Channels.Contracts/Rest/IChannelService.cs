using System.ServiceModel;
using Emaratech.Services.WcfCommons.Cors;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IChannelService : 
        IProfileService,
        IUserService,
        IApplicationService,
        IVisaInquiryService,
        ICorsAwareRestWcfService,
        IReportService,
        IRenewPasswordService, 
        IEstablishmentService,
        IHappinessMeterService
    {
    }
}
