using System.Linq;
using System.Text;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class DateUtility
    {
        public static string ConvertToArabicNumbers(string str)
        {
            char[] arabicChars = { '\u0660', '\u0661', '\u0662', '\u0663', '\u0664', '\u0665', '\u0666', '\u0667', '\u0668', '\u0669' };
            StringBuilder builder = new StringBuilder();
            if (null != str)
            {
                for (int i = 0; i < str.Count(); i++)
                {
                    if (char.IsDigit(str[i]))
                    {
                        builder.Append(arabicChars[(int)(str[i]) - 48]);
                    }
                    else
                    {
                        builder.Append(str[i]);
                    }
                }
            }
            else
            {
                return str;
            }          
            return builder.ToString();
        }
    }
}
