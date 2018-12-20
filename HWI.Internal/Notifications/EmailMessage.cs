using System.Collections.Generic;

namespace HWI.Internal.Notifications
{
    public class EmailMessage
    {
        private readonly IDictionary<string, string> _toAddresses;
        private readonly IDictionary<string, string> _ccAddresses;
        private readonly IDictionary<string, string> _bccAddresses;
        private readonly ICollection<string> _attachments;

        public EmailMessage()
        {
            _toAddresses = new Dictionary<string, string>();
            _ccAddresses = new Dictionary<string, string>();
            _bccAddresses = new Dictionary<string, string>();
            _attachments = new List<string>();
        }

        public string From { get; set; }
        public IEnumerable<KeyValuePair<string, string>> To => _toAddresses;
        public IEnumerable<KeyValuePair<string, string>> Cc => _ccAddresses;
        public IEnumerable<KeyValuePair<string, string>> Bcc => _bccAddresses;
        public string Subject { get; set; }
        public string Body { get; set; }
        public IEnumerable<string> Attachments => _attachments;
        public bool IsHtml { get; set; }
        public int Priority { get; set; }

        public void AddRecipientTo(string address, string displayName)
        {
            _toAddresses[address] = displayName;
        }

        public void AddRecipientCc(string address, string displayName)
        {
            _ccAddresses[address] = displayName;
        }

        public void AddRecipientBcc(string address, string displayName)
        {
            _bccAddresses[address] = displayName;
        }

        public void AddAttachment(string pathToAttachment)
        {
            _attachments.Add(pathToAttachment);
        }
    }
}