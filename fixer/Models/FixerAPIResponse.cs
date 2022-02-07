namespace fixer.Models
{
    public class FixerAPIResponse
    {
        public bool success { get; set; }
        public int timestamp { get; set; }
        public bool historical { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Rates rates { get; set; }
    }
    public class Rates
    {
        public double USD { get; set; }
        public double EUR { get; set; }
        public double CAD { get; set; }
    }
}
