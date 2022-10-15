using System.Net;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels.Workflows.Errors
{
    public class ChannelWorkflowErrorCodes : ErrorCodes
    {
        protected ChannelWorkflowErrorCodes(string code, HttpStatusCode status)
            : base(code, status)
        {
        }

        public static readonly ChannelWorkflowErrorCodes ResidenceNumberNotFound =
            new ChannelWorkflowErrorCodes(nameof(ResidenceNumberNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes PermitNumberNotFound =
            new ChannelWorkflowErrorCodes(nameof(PermitNumberNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes SystemNotFound =
            new ChannelWorkflowErrorCodes(nameof(SystemNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidApplicationData =
            new ChannelWorkflowErrorCodes(nameof(InvalidApplicationData), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidFormConfiguration =
            new ChannelWorkflowErrorCodes(nameof(InvalidFormConfiguration), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes PlatformNotFound =
            new ChannelWorkflowErrorCodes(nameof(PlatformNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ServiceNotResolved =
            new ChannelWorkflowErrorCodes(nameof(ServiceNotResolved), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidServiceConfiguration =
            new ChannelWorkflowErrorCodes(nameof(InvalidServiceConfiguration), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidFeeConfiguration =
            new ChannelWorkflowErrorCodes(nameof(InvalidFeeConfiguration), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidDocumentConfiguration =
            new ChannelWorkflowErrorCodes(nameof(InvalidDocumentConfiguration), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes HealthTestResultNotFound =
            new ChannelWorkflowErrorCodes(nameof(HealthTestResultNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes HealthTestResultFail =
            new ChannelWorkflowErrorCodes(nameof(HealthTestResultFail), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ResidentCancelFailed =
            new ChannelWorkflowErrorCodes(nameof(ResidentCancelFailed), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes TravelInfoNotFound =
            new ChannelWorkflowErrorCodes(nameof(TravelInfoNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes SponsorFileNotActive =
            new ChannelWorkflowErrorCodes(nameof(SponsorFileNotActive), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes SponsorResidenceExpiry3Months =
            new ChannelWorkflowErrorCodes(nameof(SponsorResidenceExpiry3Months), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes OverstayViolationFound =
            new ChannelWorkflowErrorCodes(nameof(OverstayViolationFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ResidenceCancelFailPersonOutsideCountry =
            new ChannelWorkflowErrorCodes(nameof(ResidenceCancelFailPersonOutsideCountry), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ResidentCancelFailedRelatedSponsored =
            new ChannelWorkflowErrorCodes(nameof(ResidentCancelFailedRelatedSponsored), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes PersonOutsideCountry =
            new ChannelWorkflowErrorCodes(nameof(PersonOutsideCountry), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidNumberOfWifes =
            new ChannelWorkflowErrorCodes(nameof(InvalidNumberOfWifes), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidDaughterMaritalStatus =
            new ChannelWorkflowErrorCodes(nameof(InvalidDaughterMaritalStatus), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidSponsoredGender =
            new ChannelWorkflowErrorCodes(nameof(InvalidSponsoredGender), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ApplicationAlreadyExists =
            new ChannelWorkflowErrorCodes(nameof(ApplicationAlreadyExists), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidSponsorFileNumber =
            new ChannelWorkflowErrorCodes(nameof(InvalidSponsorFileNumber), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes InvalidNumberOfYears =
            new ChannelWorkflowErrorCodes(nameof(InvalidNumberOfYears), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ResidencyNotValid =
            new ChannelWorkflowErrorCodes(nameof(ResidencyNotValid), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ResidencyExpiringSoon =
            new ChannelWorkflowErrorCodes(nameof(ResidencyExpiringSoon), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes PassportRequestNotFound =
            new ChannelWorkflowErrorCodes(nameof(PassportRequestNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes PassportRequestAlreadyExists =
            new ChannelWorkflowErrorCodes(nameof(PassportRequestAlreadyExists), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes LegalAdviceNotFound =
            new ChannelWorkflowErrorCodes(nameof(LegalAdviceNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes WifeNationalitySyria =
            new ChannelWorkflowErrorCodes(nameof(WifeNationalitySyria), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes ViolationPaymentHistoryFound =
            new ChannelWorkflowErrorCodes(nameof(ViolationPaymentHistoryFound), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes DeletePassportInvalid =
            new ChannelWorkflowErrorCodes(nameof(DeletePassportInvalid), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes FormFieldNotValid =
            new ChannelWorkflowErrorCodes(nameof(FormFieldNotValid), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaAccessOk =
            new ChannelWorkflowErrorCodes(nameof(RsaAccessOk), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaAccessDenied =
            new ChannelWorkflowErrorCodes(nameof(RsaAccessDenied), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaNewPinRequired =
            new ChannelWorkflowErrorCodes(nameof(RsaNewPinRequired), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaPinAccepted =
            new ChannelWorkflowErrorCodes(nameof(RsaPinAccepted), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaPinRejected =
            new ChannelWorkflowErrorCodes(nameof(RsaPinRejected), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaNextCodeRequired =
            new ChannelWorkflowErrorCodes(nameof(RsaNextCodeRequired), HttpStatusCode.BadRequest);
        public static readonly ChannelWorkflowErrorCodes RsaNextCodeBad =
            new ChannelWorkflowErrorCodes(nameof(RsaNextCodeBad), HttpStatusCode.BadRequest);
    }
}