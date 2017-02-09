namespace XServices.Common.Models
{
    public class TemplateEmailSenderInformation
    {
        public string Subject { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        /// <summary>
        /// if you don’t want them to see each other’s information, set the preserve_recipients option to false
        /// </summary>
        public bool PreserveRecipients { set; get; }
    }
}