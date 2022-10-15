using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Errors;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [SwaggerWcf("/")]
    public class ChannelServiceV2 : ChannelService
    {
        public override Task<Stream> GetImageStream(string documentId)
        {
            Log.Debug($"Going to get image stream for document id {documentId}");
            var userId = ClaimUtil.GetAuthenticatedUserId();

            if (string.IsNullOrEmpty(documentId) || documentId == "null")
                return null;

            int? count = ApiFactory.Default.GetApplicationApi().ValidateDocument(userId, documentId);

            if (count == null || count == 0)
                throw ChannelErrorCodes.BadRequest.ToWebFault("Unauthorized document accessed");
            
            var document = ApiFactory.Default.GetDocumentApi().GetDocument(documentId);

            if (document == null)
                Log.Debug($"Document not found for document id {documentId}");

            MemoryStream documentStream = new MemoryStream(Convert.FromBase64String(document.DocumentStream));
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            return Task.FromResult((Stream)documentStream);
        }
    }
}
