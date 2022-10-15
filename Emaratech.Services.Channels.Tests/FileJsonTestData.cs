using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    public class FileJsonTestData : ITestData
    {
        string filePath;
        public FileJsonTestData(string filePath)
        {
            this.filePath = filePath;
        }
        public string GetData()
        {
            return File.ReadAllText(filePath);
        }
    }
}
