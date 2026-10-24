namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Url
{
    public class Root
    {
        public int total { get; set; }
        public List<Sucess> sucess { get; set; }
    }

    public class Sucess
    {
        public string host { get; set; }
        public string path { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
    }
}