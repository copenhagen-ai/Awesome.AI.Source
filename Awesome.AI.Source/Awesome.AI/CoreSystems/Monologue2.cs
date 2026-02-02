using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    public class Monologue2
    {
        private TheMind mind;
        private Monologue2() { }

        public Monologue2(TheMind mind)
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

        private string Index(double _in)
        {
            _in = mind.calc.Normalize(_in, -1.0d, 1.0d, 0.0d, 100.0d);

            if(_in < 10.0d)
                return "" + 0;

            string idx = $"{_in}"[..1];

            return idx;
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

                string _base = mind.mindtype == MINDS.ROBERTA ? "base" : "base";
                mt = mind.mindtype;
                sub = mind?.unit_actual?.HUB?.subject ?? "";
                idx = $"{Index(mind.mech_high.mp.props.PropsOut[_base])}";
                //idx = $"{Index(mind.down.Props["noise"])}";
                //idx = $"{mind.mech_current.mp.p_100}"[..1];

                if (sub == "")
                    return;

                if (CONST.DECI_SUBJECT_CONTAINS(sub))
                    return;

                curr = mind.mindtype == MINDS.ROBERTA ? mind.word.Generate(idx, sub) : mind.word.Generate(idx, sub);
                curr = curr.Trim().ToLower()
                    .Replace(".", "").Replace("?", "")
                    .Replace("[9]", "").Replace("[8]", "")
                    .Replace("[7]", "").Replace("[6]", "")
                    .Replace("[5]", "").Replace("[4]", "")
                    .Replace("[3]", "").Replace("[2]", "")
                    .Replace("[1]", "").Replace("[0]", "");
                curr += $" [{idx}]";

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
