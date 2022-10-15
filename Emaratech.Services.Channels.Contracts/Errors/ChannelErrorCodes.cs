using Emaratech.Services.WcfCommons.Faults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels.Contracts.Errors
{
    public class ChannelErrorCodes : ErrorCodes
    {
        #region Fields
        public static readonly ChannelErrorCodes InvalidSystem = new ChannelErrorCodes(nameof(InvalidSystem), HttpStatusCode.BadGateway);
        public static readonly ChannelErrorCodes ServiceNotFound = new ChannelErrorCodes(nameof(ServiceNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IncorrectAnswer = new ChannelErrorCodes(nameof(IncorrectAnswer), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IncorrectMobileNumber = new ChannelErrorCodes(nameof(IncorrectMobileNumber), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes UserValidationError = new ChannelErrorCodes(nameof(UserValidationError), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes UsernameNotAvailable = new ChannelErrorCodes(nameof(UsernameNotAvailable), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserVisaExpired = new ChannelErrorCodes(nameof(IndividualUserVisaExpired), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes UserAlreadyRegisteredOnThisProfile = new ChannelErrorCodes(nameof(UserAlreadyRegisteredOnThisProfile), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserResInfoNotFound = new ChannelErrorCodes(nameof(IndividualUserResInfoNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserPartner = new ChannelErrorCodes(nameof(IndividualUserPartner), HttpStatusCode.BadRequest);


        public static readonly ChannelErrorCodes ProfileNotFound = new ChannelErrorCodes(nameof(ProfileNotFound), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes IndividualUserMobileNotFound = new ChannelErrorCodes(nameof(IndividualUserMobileNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserNotFound = new ChannelErrorCodes(nameof(IndividualUserNotFound), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualLocalUserPassportNotFromUae = new ChannelErrorCodes(nameof(IndividualLocalUserPassportNotFromUae), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserNotSupported = new ChannelErrorCodes(nameof(IndividualUserNotSupported), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes IndividualUserDepartmentNotSupported = new ChannelErrorCodes(nameof(IndividualUserDepartmentNotSupported), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes TokenMismatch = new ChannelErrorCodes(nameof(TokenMismatch), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes PasswordStrengthFailed = new ChannelErrorCodes(nameof(PasswordStrengthFailed), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes EmailNotAvailable = new ChannelErrorCodes(nameof(EmailNotAvailable), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes InvalidFormData = new ChannelErrorCodes(nameof(InvalidFormData), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes eVisaInquiryResultNotfound = new ChannelErrorCodes(nameof(eVisaInquiryResultNotfound), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes individualUserProfileIsViolated = new ChannelErrorCodes(nameof(individualUserProfileIsViolated), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes passportExpired = new ChannelErrorCodes(nameof(passportExpired), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes applicationAlreadyExist = new ChannelErrorCodes(nameof(applicationAlreadyExist), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes InvalidArgumentsForPasswordReset = new ChannelErrorCodes(nameof(InvalidArgumentsForPasswordReset), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes InvalidInformationForRefund = new ChannelErrorCodes(nameof(InvalidInformationForRefund), HttpStatusCode.BadRequest);
        public static readonly ChannelErrorCodes InvalidTravelStatus = new ChannelErrorCodes(nameof(InvalidTravelStatus), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes InvalidPaymentMerchantConfiguration = new ChannelErrorCodes(nameof(InvalidPaymentMerchantConfiguration), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes InvalidEmiratesId = new ChannelErrorCodes(nameof(InvalidEmiratesId), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes InvalidSmartAmerFee = new ChannelErrorCodes(nameof(InvalidSmartAmerFee), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes RefundFailed = new ChannelErrorCodes(nameof(RefundFailed), HttpStatusCode.Accepted);

        public static readonly ChannelErrorCodes ReportStillGenerating = new ChannelErrorCodes(nameof(ReportStillGenerating), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes PleaseTryAgain = new ChannelErrorCodes(nameof(PleaseTryAgain), HttpStatusCode.BadRequest);

        public static readonly ChannelErrorCodes PermitNumberNotFound = new ChannelErrorCodes(nameof(PermitNumberNotFound), HttpStatusCode.BadRequest);

        #endregion

        private ChannelErrorCodes(string code, HttpStatusCode status)
            : base(code, status)
        {
        }
    }

    public class ConstantMessageCodes
    {
        #region Dasboard Actions Message Codes
        public static readonly string NoActionAllowedDefault = nameof(NoActionAllowedDefault);
        public static readonly string NoCancelDependentsExist = nameof(NoCancelDependentsExist);
        public static readonly string NoActionAllowedAccompaniedExist = nameof(NoActionAllowedAccompaniedExist);
        public static readonly string NoActionNotDubaiVisa = nameof(NoActionNotDubaiVisa);
        public static readonly string NoRenewLessThan180Days = nameof(NoRenewLessThan180Days);
        public static readonly string NoCancelLessThan180DaysOutside = nameof(NoCancelLessThan180DaysOutside);
        public static readonly string NoActionInvestorSponsor = nameof(NoActionInvestorSponsor);
        public static readonly string NotAllowedPersonOutSideCountry = nameof(NotAllowedPersonOutSideCountry);
        #endregion

        #region Refund Error Codes

        public static readonly string TransactionAlreadyRefunded = "ERR_TRANSACTION_ALREADY_REFUND";
        public static readonly string BeneficiaryDoesNotExist = "ERR_BENE_DOESNOT_EXIST";
        public static readonly string BeneficiaryAmountShouldLessEqual = "ERR_BENE_AMOUNT_SHOULD_BE_LESS_OR_EQUAL";
        public static readonly string BeneficiaryPaymentDoesNotExist = "ERR_BENE_PAYMENT_DOESNOT_EXIST";
        public static readonly string BeneficiaryAmountShouldBeEqual = "ERR_BENE_AMOUNT_SHOULD_BE_EQUAL";
        public static readonly string BeneficiaryInvalidCodeFormat = "ERR_BENE_CODE_FORMAT";
        public static readonly string BeneficiaryInvalidAmountFormat = "ERR_BENE_AMOUNT_FORMAT";
        public static readonly string BeneficiaryAmountNegative = "ERR_BENE_AMOUNT_NEGATIVE";
        public static readonly string BeneficiaryBackwardCompatibility = "ERR_BENE_BACKWARD_COMPATIBILITY_1";
        public static readonly string BeneficiaryRefundFailure = "ERR_REFUND_MIGS_FAILURE";
        public static readonly string InvalidBankCountry = "EMPTY_BANK_COUNTRY";
        public static readonly string InvalidIBANNo = "EMPTY_IBAN_NO";
        public static readonly string InvalidBankName = "EMPTY_BANK_NAME";
        public static readonly string InvalidBeneficiaryName = "EMPTY_BENE_NAME";
        public static readonly string BankMismatch = "ERR_BNK_MISMATCH";
        public static readonly string InvalidLengthBeneficiaryName = "INVALID_LENGTH_BENE_NAME";

        public static readonly string IssueWithRefundRequest = nameof(IssueWithRefundRequest);
        public static readonly string RefundRequestSuccessful = nameof(RefundRequestSuccessful);
        public static readonly string RefundSuccessful = nameof(RefundSuccessful);
        #endregion
    }
}