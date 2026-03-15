using System.Text.Json;

namespace Awesome.AI.Common
{
    //public class Stat
    //{
    //    public string _name { get; set; }
    //    public double _index { get; set; }
    //    public int hits {  get; set; }       
    //}

    public class MyStats
    {
        public MyStats()
        {
            hits = new Dictionary<int, int>();
            units = new Dictionary<int, int>();
        }

        public Dictionary<int, int> hits { get; set; }
        public Dictionary<int, int> units { get; set; }

        public string HitsJson()
        {
            string json = JsonSerializer.Serialize(hits);

            return json;
        }

        public string UnitsJson()
        {
            string json = JsonSerializer.Serialize(units);

            return json;
        }
    }

    public class MyMeters
    {
        public int units_added {  get; set; }
        public int units_removed { get; set; }
    }
}
