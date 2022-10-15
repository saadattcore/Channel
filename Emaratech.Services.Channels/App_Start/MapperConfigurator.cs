using AutoMapper;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using Emaratech.Services.Lookups.Model;
using Emaratech.Services.Payment.Model;
using RestApplicationStatus = Emaratech.Services.Channels.Contracts.Rest.Models.Application.RestApplicationStatus;
using RestApplicationStatusWrapper = Emaratech.Services.Channels.Contracts.Rest.Models.Application.RestApplicationStatusWrapper;
using Emaratech.Services.WcfCommons.Dynamic;

namespace Emaratech.Services.Channels
{

    [MapperConfigurationClass]
    public static class MapperConfigurator
    {
        [MapperConfigurationMethod]
        public static void Configure(this IMapperConfiguration configurationExp)
        {

            configurationExp.CreateMap<RestApplicationStatusWrapper, Application.Model.RestApplicationStatusWrapper>().ReverseMap();
            configurationExp.CreateMap<RestApplicationStatus, Application.Model.RestApplicationStatus>().ReverseMap()
            .AfterMap((src, dest) =>
            {
                dest.StatusDesc = new RestName { En = src.StatusDescE, Ar = src.StatusDescA };
                dest.Status_Code = src.StatusCode;
            });
            configurationExp.CreateMap<RestDocumentsRequiredResponse, Document.Model.DocumentType>().ReverseMap();
            configurationExp.CreateMap<RestReportInfo, RestReportsHistory>()
                .ForMember(dest => dest.PaymentDate, opts => opts.MapFrom(src => src.PaymentDate.HasValue ? src.PaymentDate.Value.ToString("s") : null))
                .ForMember(dest => dest.ApplicationId, opts => opts.MapFrom(src => src.ApplicationId))
                .ForMember(dest => dest.ServiceResourceKey, opts => opts.MapFrom(src => src.ResourceKey))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => new RestName { En = src.NameEn, Ar = src.NameAr }))
                .ReverseMap();


            configurationExp.CreateMap<RestIndividualDependentSearchCriteria, RestDashboardSearchRequest>().ReverseMap();
            configurationExp.CreateMap<RestEstablishmentDependentsSummary, RestEstablishmentSummary>().ReverseMap();
            configurationExp.CreateMap<RestRefundResponse, RestPaymentStatus>().ReverseMap();
            configurationExp.CreateMap<RestReportingRequest, ReportingRequest>().ReverseMap();
            configurationExp.CreateMap<Contracts.Rest.Models.RestKeyValue, Lookups.Model.RestKeyValue>().ReverseMap();
            configurationExp.CreateMap<Contracts.Rest.Models.RestKeyValueList, Lookups.Model.RestKeyValueList>().ReverseMap();
            configurationExp.CreateMap<RestReportingResponse, ReportingResponse>().ReverseMap();
            configurationExp.CreateMap<RestUserDetailedProfile, RestUserProfile>().ReverseMap();
            configurationExp.CreateMap<RestEstablishmentSearchCriteria, RestEstablishmentSearchRequest>().ReverseMap();
        }

    }
}