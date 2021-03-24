namespace USLModule.Models
{
    public class SocketData
    {
        public string Event;
        public string Data;
        
        public SocketData(string eEvent, string eData)
        {
            Event = eEvent;
            Data = eData;
        }
    }
}