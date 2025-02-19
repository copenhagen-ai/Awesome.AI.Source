namespace Awesome.AI.Common
{
    public class Stat
    {
        //public int hits { get; set; }
        public string _name { get; set; }
        public double _var { get; set; }
        public double _index { get; set; }
        public int hits {  get; set; }
    }

    public class Stats
    {
        public string curr_name { get; set; }
        //public int curr_value { get; set; }
        public string reset_name { get; set; }
        //public int reset_value { get; set; }

        public List<Stat> list = new List<Stat>();        
    }

}
