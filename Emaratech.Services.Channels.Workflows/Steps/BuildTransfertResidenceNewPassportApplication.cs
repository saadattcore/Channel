using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildTransfertResidenceNewPassportApplication : BuildResidenceApplication
    {
        protected override async Task<JObject> BuildApplicantDetails(RestIndividualUserFormInfo visionData)
        {
            var applicant = await base.BuildApplicantDetails(visionData);
            applicant["PassportIssueDate"] = null;
            applicant["PassportExpiryDate"] = null;
            applicant["PassportPlaceE"] = null;
            return applicant;
        }
    }
}
