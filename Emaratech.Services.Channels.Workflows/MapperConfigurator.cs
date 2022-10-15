using AutoMapper;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Document.Model;
using Emaratech.Services.Vision.Model;

namespace Emaratech.Services.Channels.Workflows

{
    public static class MapperConfigurator
    {
        public static IMapper Configure()
        {
            var mapperConfig = new MapperConfiguration(ConfigureInternal);
            return mapperConfig.CreateMapper();
        }

        private static void ConfigureInternal(IMapperConfiguration configurationExp)
        {
            configurationExp.CreateMap<ApplicationDocument, RestDocument>().ReverseMap();
            configurationExp.CreateMap<RestDependentVisaInfo, RestIndividualDependentsInfoWrapper>().ReverseMap().AfterMap((src, dest) =>
            {
                dest.VisaType = src.IndividualVisaInformation?.VisaType;
                dest.VisaTypeId = src?.IndividualVisaInformation?.VisaTypeId.ToString();
                dest.VisaNumber = src.IndividualVisaInformation?.VisaNumber;
                dest.ExpiryDate = src.IndividualVisaInformation?.VisaExpiryDate;
                dest.FirstName = new Contracts.Rest.Models.RestName();
                dest.LastName = new Contracts.Rest.Models.RestName();
                dest.FirstName.En = src?.IndividualProfileInformation?.LastNameEn.ToString();
                dest.FirstName.Ar = src?.IndividualProfileInformation?.FirstNameAr.ToString();

                dest.LastName.En = src?.IndividualProfileInformation?.LastNameEn.ToString();
                dest.LastName.Ar = src?.IndividualProfileInformation?.LastNameAr.ToString();
                dest.Nationality = src?.IndividualProfileInformation?.NationalityId.ToString();
                dest.Relationship = src?.IndividualVisaInformation?.RelationshipId.ToString();
                dest.Status = src?.IndividualVisaInformation.VisaStatusId.ToString();


            }); 
        }
    }
}
