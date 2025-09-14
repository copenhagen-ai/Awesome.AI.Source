namespace Awesome.AI.Common
{
    //public class Stat
    //{
    //    public string _name { get; set; }
    //    public double _index { get; set; }
    //    public int hits {  get; set; }       
    //}

    public class Stats
    {
        public Stats() 
        {
            hits = new Dictionary<int, int>();
            units = new Dictionary<int, int>();
        }

        public Dictionary<int, int> hits { get; set; }
        public Dictionary<int, int> units { get; set; }
    }
}
