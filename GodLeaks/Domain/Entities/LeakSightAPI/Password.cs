namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Password
{
    public class MachinesStealer
    {
        public string host { get; set; }
        public string path { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string domain_email { get; set; }
    }

    public class Root
    {
        public string time { get; set; }
        public List<object> stealer { get; set; }
        public List<MachinesStealer> MachinesStealer { get; set; }
    }
}