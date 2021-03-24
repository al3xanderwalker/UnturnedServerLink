namespace USLModule.Models
{
    public class ChatMessage
    {
        public string Sender;
        public string SenderId;
        public string Content;
        public string Icon;

        public ChatMessage(string sender, string senderId, string content, string icon)
        {
            Sender = sender;
            SenderId = senderId;
            Content = content;
            Icon = icon;
        }
    }
}