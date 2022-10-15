using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Tests;

namespace Emaratech.Services.Channels.Tests.Steps
{
    abstract class RegressionTestStepBase : IRegressionTestStep
    {
        protected ITestData testData;
        private IRegressionTestStep _PreviousStep;
        public IRegressionTestStep PreviousStep
        {
            get
            {
                return _PreviousStep;
            }

            set
            {
                _PreviousStep = value;
            }
        }
        protected string successResponse;
        public string SuccessResponse
        {
            get
            {
                return successResponse;
            }
        }

        protected RegressionTestStepBase() { }
        protected RegressionTestStepBase(string filePath)
        {
            testData = new FileJsonTestData(filePath);
        }

        protected RegressionTestStepResponse HttpRequestGet(string url)
        {
            var request = WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["BaseUrl"] + url);

            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();

            return new RegressionTestStepResponse { Request = url, Response = responseFromServer, StatusCode = ((HttpWebResponse)response).StatusCode };
        }

        protected RegressionTestStepResponse HttpRequestPost(string jsonData, string url)
        {
            var request = WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["BaseUrl"] + url);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();
            return new RegressionTestStepResponse { Request = jsonData, Response = responseFromServer, StatusCode = ((HttpWebResponse)response).StatusCode };
        }

        public abstract RegressionTestStepResponse Execute();

        public void Init(ITestData testData)
        {
            this.testData = testData;
        }

        protected RegressionTestStepResponse Post(string data, string url)
        {
            var result = HttpRequestPost(data, url);
            if (result.StatusCode == HttpStatusCode.OK)
                successResponse = result.Response;
            return result;
        }

        protected RegressionTestStepResponse Get(string url)
        {
            var result = HttpRequestGet(url);
            if (result.StatusCode == HttpStatusCode.OK)
                successResponse = result.Response;
            return result;
        }
    }
}