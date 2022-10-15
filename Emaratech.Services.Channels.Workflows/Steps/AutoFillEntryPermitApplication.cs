using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Models;
using log4net;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class AutoFillEntryPermitApplication : BuildApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FetchIndividualProfileByPassportInfo));

        public InputParameter<string> Platform { get; set; }
        public InputParameter<string> ApplicationId { get; set; }
        public OutputParameter View { get; set; }
        public OutputParameter FormId { get; set; }
        public OutputParameter FormConfiguration { get; set; }
        public ReferenceParameter<string> ApplicationData { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            UnifiedApplication = new OutputParameter(nameof(UnifiedApplication));
            View = new OutputParameter(nameof(View));
            FormId = new OutputParameter(nameof(FormId));
            FormConfiguration = new OutputParameter(nameof(FormConfiguration));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            try
            {
                await base.Execute();

                //Mark step as done in case of application edit
                if (ApplicationId != null && ApplicationId.Get() != null)
                {
                    return StepState = WorkflowStepState.Done;
                }

                CheckRequiredInput(ApplicationData);
                if (ParametersRequiringInput.Any())
                {
                    View.Set(ViewEnum.Form);
                    FormId.Set(Constants.Forms.EntryPermitAutofillFormId);

                    var data = await ServicesHelper.RenderFormForPlatform(Constants.Forms.EntryPermitAutofillFormId, Platform.Get());
                    //DataHelper.FilterLookup("CurrentNationalityId", GetAllowedNationalities(), data);
                    FormConfiguration.Set(data);

                    Log.Debug($"Input Required {nameof(ApplicationData)}");
                    return WorkflowStepState.InputRequired;
                }

                var formData = JObject.Parse(ApplicationData.Value);
                var fetchEntryPermitDataAction = formData["Application"]["ApplicationDetails"]["FetchEntryPermitDataAction"].Value<string>();

                if (fetchEntryPermitDataAction == "1")
                {
                    FormId.Set(null);
                    FormConfiguration.Set(null);
                    ApplicationData.Value = null;
                    return StepState = WorkflowStepState.Done;
                }

                RestIndividualUserFormInfo visionData = null;
                var uid = formData["Application"]["ApplicantDetails"]["UnifiedNo"].Value<string>();
                var passportNumber = formData["Application"]["ApplicantDetails"]["PassportNo"].Value<string>();
                var birthYear = formData["Application"]["ApplicantDetails"]["YearOfBirth"].Value<string>();
                var nationalityId = formData["Application"]["ApplicantDetails"]["CurrentNationalityId"].Value<string>();
                
                if ((passportNumber != null || uid != null) && birthYear != null && nationalityId != null)
                {
                    visionData = await ServicesHelper.GetIndividualDetailedAutoFill(passportNumber, uid, birthYear, nationalityId);
                    //visionData = await ServicesHelper.GetIndividualProfileByPassportInfo(passportNumber, birthYear, nationalityId);

                    if (visionData == null)
                    {
                        throw ChannelErrorCodes.ProfileNotFound.ToWebFault($"Profile not found for passport number {passportNumber}");
                    }

                    //if (visionData.IndividualUserInfo.IndividualPassportInformation.ExpiryDate.Value.Date <= DateTime.Now.Date)
                    //{
                    //    throw ChannelErrorCodes.passportExpired.ToWebFault($"Expired Passport for passport number {passportNumber}");
                    //}

                    //PermitNo.Set(visionData.IndividualUserInfo.IndividualPermitInfo?.PermitNo);

                    //if (await ServicesHelper.IsApplicationExist(ServiceId.Get(), visionData.IndividualUserInfo.IndividualPermitInfo?.PermitNo))
                    //{
                    //    throw ChannelErrorCodes.applicationAlreadyExist.ToWebFault($"This application already exist.");
                    //}

                    await BuildUnifiedApplicationDocument(visionData);
                }

                FormId.Set(null);
                FormConfiguration.Set(null);
                ApplicationData.Value = null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            Log.Debug($"END - All Inputs Provided.");
            return StepState = WorkflowStepState.Done;
        }
    }
}