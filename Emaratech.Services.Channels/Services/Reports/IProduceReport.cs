using System.IO;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Services.Reports
{
    public interface IProduceReport
    {
        Stream AsStream(string reportData, WebOperationContext currentContext, short fileType);
        IProduceReport SetTypeExcel();
        IProduceReport SetTypePdf();
    }
}