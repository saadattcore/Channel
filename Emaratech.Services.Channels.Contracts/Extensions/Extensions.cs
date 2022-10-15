using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Extensions
{
    public static class Extensions
    {
        public static Boolean ToBoolean(this string str)
        {
            if (string.IsNullOrEmpty((str)))
                return false;

            else if (str.Trim() == "0")
                return false;

            else if (str.Trim() == "1")
                return true;

            else
                throw new Exception("Unexpected string to convert to boolean");
        }

        public static int? EvaluateArithmeticExpression(this string expression)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            return Convert.ToInt32(table.Compute(expression, String.Empty));
        }

        public static int ToInt32(this string expression)
        {
            int result = 0;
            int.TryParse(expression, out result);
            return result;
        }      
    }
}
