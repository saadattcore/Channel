using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateHealthTestInfo : ChannelWorkflowStep
    {
        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<DateTime> BirthDate { get; set; }
        public InputParameter<string> PermitNo { get; set; }
        public InputParameter<string> VisaType { get; set; }
        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(BirthDate);
            CheckRequiredInput(VisaType);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if (VisaTypeId.Residence == (VisaTypeId)Enum.Parse(typeof(VisaTypeId), VisaType.Get()))
            {
                string visaNumber = PermitNo?.Get() ?? ResidenceNo?.Get();

                int age = GetAge(BirthDate.Get());
                if (age > 18)
                {
                    var healthTestInfo = await ServicesHelper.GetIndividualHealthTestInformation(visaNumber);

                    if (healthTestInfo == null)
                        throw ChannelWorkflowErrorCodes.HealthTestResultNotFound.ToWebFault($"Health test result not found for visa number {visaNumber}");

                    if (healthTestInfo.FitFlag != "5")
                        throw ChannelWorkflowErrorCodes.HealthTestResultFail.ToWebFault($"Health test result fail for visa number {visaNumber}");
                }
            }
            return StepState = WorkflowStepState.Done;
        }

        private int GetAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;
            return (a - b) / 10000;
        }
    }
}