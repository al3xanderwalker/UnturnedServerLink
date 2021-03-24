namespace USLModule.Models
{
    public class RemoteCommand
    {
        public string Sender;
        public string Command;

        public RemoteCommand(string sender, string command)
        {
            Sender = sender;
            Command = command;
        }
    }
}