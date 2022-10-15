using System;
using System.Xml.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.PassportService
{
    [Serializable]
    public class PassportServiceErrors
    {
        [XmlElement("invalidValue")]
        public string InvalidValue { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("messageTemplate")]
        public string MessageTemplate { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }
    }

    [Serializable]
    [XmlRoot("validationErrors")]
    public class PassportServiceValidationErrors
    {

        [XmlElement("errors")]
        public PassportServiceErrors[] Errors { get; set; }
    }
}