using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emaratech.Services.UserManagement.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.UserManagement.Api;
using Emaratech.Services.UserManagement.Model;
using System.IO;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Extensions
{
    internal static class Mapper
    {

        public static RestUserProfile MapUserProfile(UserAll userAllObj)
        {
            RestUserProfile resUserprofile = new RestUserProfile();

            foreach (var pro in userAllObj.Profile)
            {
                setData(pro, resUserprofile);
            }

            var nameObj = new name
            {
                ar = resUserprofile.FirstNameAr,
                en = resUserprofile.FirstNameEn
            };

            resUserprofile.name = nameObj;
            resUserprofile.Username = userAllObj.Username;

            return resUserprofile;
        }

        private static void setData(Profile profile, RestUserProfile restUserProfileObj)
        {
            switch (profile.Name)
            {
                case UserProfileEnum.DateOfBirth:
                    {
                        DateTime dob;
                        if (DateTime.TryParse(profile.Value,out dob))
                        {
                            restUserProfileObj.DateOfBirth = dob;
                        }
                    }
                    break;

                case UserProfileEnum.Username:
                    restUserProfileObj.Username = profile.Value;
                    break;

                case UserProfileEnum.Email:
                    restUserProfileObj.Email = profile.Value;
                    break;

                case UserProfileEnum.EmiratesId:
                    restUserProfileObj.EmiratesId = profile.Value;
                    break;               

                case UserProfileEnum.FileTypeId:
                    restUserProfileObj.FileTypeId = profile.Value;
                    break;

                case UserProfileEnum.FirstNameAr:
                    restUserProfileObj.FirstNameAr = profile.Value;
                    break;

                case UserProfileEnum.FirstNameEn:
                    restUserProfileObj.FirstNameEn = profile.Value;
                    break;

                case UserProfileEnum.GenderId:
                    restUserProfileObj.GenderId = profile.Value;
                    break;

                case UserProfileEnum.Mobile:
                    restUserProfileObj.Mobile = profile.Value;
                    break;

                case UserProfileEnum.NationalityId:
                    restUserProfileObj.NationalityId = profile.Value;
                    break;

                //case UserProfile.PPSID:
                //    restUserProfileObj = profile.Value;
                //    break;

                case UserProfileEnum.PassportNo:
                    restUserProfileObj.PassportNumber = profile.Value;
                    break;

                case UserProfileEnum.SponsorNo:
                    restUserProfileObj.SponsorNo = profile.Value;
                    break;

                case UserProfileEnum.UserType:
                    restUserProfileObj.UserType = profile.Value;
                    break;

                case UserProfileEnum.VisaNumber:
                    restUserProfileObj.VisaNumber = profile.Value;
                    break;

                case UserProfileEnum.VisaExpiryDate:
                    restUserProfileObj.VisaExpiryDate = DateTime.Parse(profile.Value);
                    break;

                case UserProfileEnum.ImageId:
                    restUserProfileObj.Image = profile.Value;
                    break;
            }
        }

        public static string GetUserProfileDocId(List<Profile> lstProfile)
        {
            string documentId = string.Empty;
            foreach (var pro in lstProfile)
            {
                switch (pro.Name)
                {
                    case UserProfileEnum.ImageId:
                        documentId = pro.Value;
                        return documentId;
                        break;
                }
                
            }
            return documentId;
        }
    }
}