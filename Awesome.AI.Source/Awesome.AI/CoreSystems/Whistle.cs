using Awesome.AI.Core;

namespace Awesome.AI.CoreSystems
{
    public class Whistle
    {
        private TheMind mind;
        public Whistle(TheMind mind)
        {
            this.mind = mind;
            this.mind.result_whistle = "";
        }

        private string[] gimmick = { "[.??]", "[??.]" };
        private int count = 0;
        public void Do(bool _pro)
        {
            if (!_pro)
                return;

            if (count > 1)
                count = 0;

            bool res = mind._quick.Result("WHISTLE");

            if (res)
                mind.result_whistle = "[Whistling to my self..]";
            
            if (!res)
                mind.result_whistle = gimmick[count];

            count++;
        }
    }
}
