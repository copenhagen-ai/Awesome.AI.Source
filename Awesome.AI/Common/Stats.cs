namespace Awesome.AI.Common
{
    public class Stat
    {
        //public int hits { get; set; }
        public string name { get; set; }
        public double force { get; set; }
        public double index { get; set; }
    }

    public class Stats
    {
        public string curr_name { get; set; }
        public int curr_value { get; set; }
        public string reset_name { get; set; }
        public int reset_value { get; set; }

        public List<Stat> list = new List<Stat>();

        public void Reset()
        {
            list = new List<Stat>();
        }
    }

}
