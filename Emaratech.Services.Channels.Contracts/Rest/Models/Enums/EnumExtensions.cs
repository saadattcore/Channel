using System;
using System.Linq;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Enums
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this QuestionCode value)
            where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }

        public static T GetAttribute<T>(this VisaStatus value)
            where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }

        public static T GetAttribute<T>(this VisaType value)
            where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }

        public static T GetAttribute<T>(this DashboardSearchType value)
        where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }

        public static T GetAttribute<T>(this BarCodeType value)
        where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }
    }
}