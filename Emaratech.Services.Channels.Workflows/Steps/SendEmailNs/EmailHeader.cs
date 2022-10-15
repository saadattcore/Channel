using System.Collections.Generic;

namespace Emaratech.Services.Channels.Workflows.Steps.SendEmailNs
{
    public class EmailHeader
    {
        public IList<EmailDestination> DestinationList;
        public string Subject { get; set; }

        public static EmailHeader Create(string argName, string argEmailAddress, string argSubject)
        {
            var emData = new EmailHeader
            {
                DestinationList = new List<EmailDestination>()
                {
                    new EmailDestination() {Name = argName, Address = argEmailAddress}
                },
                Subject = argSubject
            };
            return emData;
        }
    }
}