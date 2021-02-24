using ProjektAspNetStudia.Models.Database;

namespace ProjektAspNetStudia.Utilities
{
    public static class MessageExtensions
    {
        public static object CreateMessageForJsClient(this Message message, string userName)
        {
            return new {sent = message.Sent.ToString("h:mm tt | MMMM d"), sentShort = message.Sent.ToString("MMM dd"), text = message.Text, userName};
        }
    }
}
