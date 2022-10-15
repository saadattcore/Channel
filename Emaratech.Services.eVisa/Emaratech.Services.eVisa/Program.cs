using System;
using System.Linq;
using log4net;
using System.Configuration;
using System.Reflection;
using log4net.Config;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("eVisa Service");
        static void Main(string[] args)
        {
            try
            {
                ConfigurationSystem.Load("eVisa");
                XmlConfigurator.Configure();
                Log.Debug($"eVisa App Started ");

                var processors = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => typeof(StatusProcessor).IsAssignableFrom(x) && !x.IsAbstract);

                foreach (var processorType in processors)
                {
                    Log.Debug($"Processor Type = {processorType.Name}");
                    var processor = (StatusProcessor)Activator.CreateInstance(processorType);
                    processor.Process();
                }

                Log.Debug($"eVisa App Ended ");

                // AddNewTemplate();
            }
            catch (Exception ex)
            {
                Log.Error($"exception while processing eVisa ==> {ex.Message.ToString()}");
                Log.Error(ex.ToString());
                //   throw ex;
            }


        }



        private static void AddNewTemplate()
        {
            Emaratech.Services.Template.Api.CRUDApi templateApi = new Template.Api.CRUDApi(ConfigurationSystem.AppSettings["TemplateApi"]);
            Emaratech.Services.Template.Model.RestTemplate templateModel = new Template.Model.RestTemplate();

            templateModel.TemplateId = Guid.NewGuid().ToString();
            templateModel.Name = "eVisa Template";
            templateModel.Description = "eVisa Template for approved application";
            templateModel.TemplateTypeId = "00000000000000000000000000000001";
            templateModel.Content = "";

            templateApi.AddTemplate(templateModel);

        }
    }
}
