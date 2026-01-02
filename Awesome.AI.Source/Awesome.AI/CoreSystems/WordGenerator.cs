using Awesome.AI.Common;
using Awesome.AI.Core;
using System.Globalization;

namespace Awesome.AI.CoreSystems
{
    public class WordGenerator
    {
        private string path {get;set;}
        private List<string> lines;
        private Random rand = new Random();

        private TheMind mind;
        private WordGenerator() { }

        public WordGenerator(TheMind mind)
        {
            this.mind = mind;

        }

        private void LoadData()
        {
            lines ??= new List<string>();

            if (lines.Any())
                return;

            string root = MyPath.Root;
            string path = root + "Data\\latin.txt";

            lines = File.ReadAllLines(path).ToList();
        }

        private string PickWord()
        {
            List<string> l1 = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X" };
            int c1 = l1.Count;
            int r1 = rand.Next(c1);

            string letter = l1[r1];

            List<string> l2 = lines.Where(x => x[..1].ToLower() == letter.ToLower()).ToList();
            int c2 = l2.Count;
            int r2 = rand.Next(c2);

            if (l2.Count == 0)
                return "...";

            string res = "";
            res = l2[r2].Split(":")[0];
            res = res.Split(",")[0];

            if (res.Count() == 1)
                return "...";

            return res;
        }

        private string Format(string str)
        {
            string test = "";
            bool _do = false;
            foreach(char c in str)
            {
                if (c == '(') {
                    //test += "(";
                    _do = true;
                }

                if (c == ')') {
                    test += ")";
                    _do = false;
                }

                if (_do)
                    test += "" + c;
            }

            if (str.Contains("(") && str.Contains(")"))
                str = str.Replace(test, "");

            return str;
        }

        public string Generate(string idx, string sub)
        {
            LoadData();

            string word1 = "", word2 = "", word3 = "";

            while (word1 == "" || word2 == "" || word3 == "")
            {
                word1 = PickWord();
                word2 = PickWord();
                word3 = PickWord();
            }

            string res = $"[{idx}]{word1} {word2} {word3}";

            res = Format(res);

            return res;
        }
    }
}
