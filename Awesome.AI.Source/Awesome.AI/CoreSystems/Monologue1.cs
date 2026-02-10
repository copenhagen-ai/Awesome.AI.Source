using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    public class Monologue1
    {
        private TheMind mind;
        private Monologue1() { }

        public Monologue1(TheMind mind)
        {
            this.mind = mind;
        }

        public string Result { get; set; }
        public string Subject { get; set; }
        public string Relevance { get; set; }

        private string prev = "im so happy";
        private string curr = "";
        private int counter = 0;

        private string GetRelevance(string str1, string str2)
        {
            string num1 = new string(str1.Where(char.IsDigit).ToArray());
            string num2 = new string(str2.Where(char.IsDigit).ToArray());
            
            try
            {
                if(str1 == "im so happy")
                    return ", but ";

                int _num1 = int.Parse(num1);
                bool upper1 = _num1 > 4;

                int _num2 = int.Parse(num2);
                bool upper2 = _num2 > 4;

                if (upper1 && upper2)
                    return ", and ";

                if (!upper1 && !upper2)
                    return ", and ";

                return ", but ";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Create(bool _pro)
        {
            MINDS mt;
            string idx = "";
            string sub = "";

            try
            {
                if (!_pro)
                    return;

                counter++;

                if (counter < 2)
                    return;

                counter = 0;

                if (mind.STATE == STATE.QUICKDECISION)
                    return;

                if (mind.unit_actual.IsQUICKDECISION())
                    return;

                if (mind.unit_actual.IsDECISION())
                    return;

                //if (!CONST.SAMPLE20.RandomSample(mind))
                //    return;

                sub = mind.hub.GetSubject(mind.unit_actual) ?? "";
                curr = mind?.unit_actual?.Data ?? "";

                if (sub == "")
                    return;

                Relevance = GetRelevance(prev, curr);

                string res = prev + "||" + curr;
                prev = curr;

                Result = res;
                Subject = sub;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
