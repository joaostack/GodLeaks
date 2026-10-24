namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Email
{
    public class BigData
    {
        public Search search { get; set; }
        public Breachbase breachbase { get; set; }
        public Hackcheck hackcheck { get; set; }
    }

    public class Brazil15B
    {
        public string search_term { get; set; }
        public string path { get; set; }
        public List<object> results { get; set; }
        public int total_time_ms { get; set; }
        public int collections_with_results { get; set; }
    }

    public class Breachbase
    {
        public string error { get; set; }
    }

    public class DatabaseUrl
    {
        public string host { get; set; }
        public string path { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
    }

    public class Hackcheck
    {
        public string error { get; set; }
    }

    public class Osint
    {
        public string status { get; set; }
        public string value { get; set; }
    }

    public class Osint2
    {
        public string email { get; set; }
        public int total { get; set; }
        public List<object> dados { get; set; }
    }

    public class Root
    {
        public string time { get; set; }
        public List<object> stealer_json { get; set; }
        public List<DatabaseUrl> database_url { get; set; }
        public List<object> bigcomboCombolist { get; set; }
        public List<object> DatabaseOsintLeaks { get; set; }
        public Osint osint { get; set; }
        public BigData BigData { get; set; }
        public Brazil15B Brazil15B { get; set; }
    }

    public class Search
    {
        public string error { get; set; }
    }
}
