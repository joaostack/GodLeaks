namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Username
{
    public class DatabaseUrl
    {
        public string host { get; set; }
        public string path { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
    }

    public class Root
    {
        public string time { get; set; }
        public List<object> stealer_json { get; set; }
        public List<DatabaseUrl> database_url { get; set; }
        public List<object> bigcomboCombolist { get; set; }
        public List<object> DatabaseOsintLeaks { get; set; }
    }
}