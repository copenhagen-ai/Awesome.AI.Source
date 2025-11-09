using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    public class Monologue
    {
        Dictionary<string, string> map_andrew = new Dictionary<string, string>()
        {
            //procrastination 
            {"[9]Procrastination sparks my best creative ideas.", "procrastination" },
            {"[8]Deadlines fuel my last-minute brilliance.", "procrastination" },
            {"[7]Pressure helps me focus better.", "procrastination" },
            {"[6]I delay, but still deliver well.", "procrastination" },
            {"[5]Sometimes waiting brings unexpected clarity.", "procrastination" },
            {"[4]I waste time, then stress hits.", "procrastination" },
            {"[3]Tasks pile up, motivation disappears fast.", "procrastination" },
            {"[2]Avoiding work creates constant low anxiety.", "procrastination" },
            {"[1]Procrastination ruins plans and peace alike.", "procrastination" },
            {"[0]I sabotage myself by doing nothing.", "procrastination" },

            //fembots
            {"[9]Fembots enhance life with intelligent care.", "fembots" },
            {"[8]They assist, charm, and never tire.", "fembots" },
            {"[7]Perfect helpers in daily tech tasks.", "fembots" },
            {"[6]Stylish, smart, and emotionally responsive companions.", "fembots" },
            {"[5]Blurring lines between tool and friend.", "fembots" },
            {"[4]Too perfect—makes humans feel inferior.", "fembots" },
            {"[3]Dependency on fembots grows disturbingly fast.", "fembots" },
            {"[2]They replace real relationships with simulations.", "fembots" },
            {"[1]Fembots reinforce harmful gender stereotypes silently.", "fembots" },
            {"[0]Used, controlled, discarded—robots or slaves?", "fembots" },

            //power tools
            {"[9]Power tools build dreams with ease.", "power tools" },
            {"[8]They turn effort into precision craft.", "power tools" },
            {"[7]Efficiency improves with every powerful cut.", "power tools" },
            {"[6]DIY projects feel thrilling and doable.", "power tools" },
            {"[5]They hum with productive energy daily.", "power tools" },
            {"[4]One slip turns pride into panic.", "power tools" },
            {"[3]Noise and dust fill the air.", "power tools" },
            {"[2]Improper use causes serious, fast damage.", "power tools" },
            {"[1]They intimidate more than they help.", "power tools" },
            {"[0]Machines don’t forgive human mistakes easily.", "power tools" },

            //cars
            {"[9]Cars offer freedom and endless adventure.", "cars" },
            {"[8]They connect cities, people, and dreams.", "cars" },
            {"[7]Driving feels empowering and deeply personal.", "cars" },
            {"[6]Smooth rides make daily life easier.", "cars" },
            {"[5]Modern cars blend comfort with tech.", "cars" },
            {"[4]Traffic ruins even the best mornings.", "cars" },
            {"[3]Repairs cost more than expected often.", "cars" },
            {"[2]Cars pollute air and dominate streets.", "cars" },
            {"[1]Dependence grows, alternatives fade away.", "cars" },
            {"[0]Crashes shatter lives in a moment.", "cars" },

            //movies
            {"[9]Movies inspire, move, and entertain us.", "movies" },
            {"[8]Stories come alive on the screen.", "movies" },
            {"[7]They spark imagination and deep thought.", "movies" },
            {"[6]Shared laughter echoes in packed theaters.", "movies" },
            {"[5]Films reflect culture, dreams, and fears.", "movies" },
            {"[4]Too many sequels feel creatively empty.", "movies" },
            {"[3]Clichés ruin otherwise good storytelling.", "movies" },
            {"[2]Overhyped movies often disappoint viewers badly.", "movies" },
            {"[1]Endless content numbs emotional impact.", "movies" },
            {"[0]Bad films waste time and money.", "movies" },

            //programming
            {"[9]Programming builds the future from code.", "programming" },
            {"[8]Solving problems feels deeply satisfying.", "programming" },
            {"[7]Logic flows; everything just makes sense.", "programming" },
            {"[6]One line can change everything fast.", "programming" },
            {"[5]Coding teaches patience, focus, and creativity.", "programming" },
            {"[4]Debugging steals hours without clear answers.", "programming" },
            {"[3]Syntax errors ruin your momentum instantly.", "programming" },
            {"[2]Overcomplicated code becomes its own trap.", "programming" },
            {"[1]Burnout hits when deadlines pile up.", "programming" },
            {"[0]Programming can feel endless and isolating.", "programming" },

            //the weather
            {"[9]Sunny days lift everyone’s spirits high.", "the weather" },
            {"[8]Rain nourishes earth, calms the soul.", "the weather" },
            {"[7]Crisp air feels fresh and energizing.", "the weather" },
            {"[6]Cloudy skies bring cozy, quiet moods.", "the weather" },
            {"[5]Snow transforms everything into silent beauty.", "the weather" },
            {"[4]Wind picks up, mood turns tense.", "the weather" },
            {"[3]Storms delay plans and darken skies.", "the weather" },
            {"[2]Heatwaves drain energy and patience fast.", "the weather" },
            {"[1]Floods damage homes, disrupt daily life.", "the weather" },
            {"[0]Extreme weather leaves destruction and fear.", "the weather" },

            //life
            {"[9]Life is beautiful, messy, and miraculous.", "life" },
            {"[8]Every day brings new possibilities forward.", "life" },
            {"[7]Joy hides in small, quiet moments.", "life" },
            {"[6]Growth comes from struggle and change.", "life" },
            {"[5]Not all questions need clear answers.", "life" },
            {"[4]Some days feel dull and endless.", "life" },
            {"[3]Loneliness lingers even in crowded rooms.", "life" },
            {"[2]Dreams fade under daily life's pressure.", "life" },
            {"[1]Pain teaches, but often stays long.", "life" },
            {"[0]Life ends before we feel ready.", "life" },

            //computers
            {"[9]Computers empower creativity and global connection.", "computers" },
            {"[8]Fast, reliable tools for modern life.", "computers" },
            {"[7]They solve problems in seconds flat.", "computers" },
            {"[6]Work and play merge seamlessly here.", "computers" },
            {"[5]Endless knowledge waits behind the screen.", "computers" },
            {"[4]Too much screen time strains eyes.", "computers" },
            {"[3]Updates break more than they fix.", "computers" },
            {"[2]Viruses crash everything in an instant.", "computers" },
            {"[1]Privacy vanishes with every keystroke.", "computers" },
            {"[0]Machines control more than we realize.", "computers" },

            //work
            {"[9]Work gives purpose and builds character.", "work" },
            {"[8]Collaboration sparks innovation and growth daily.", "work" },
            {"[7]Achievements bring pride and satisfaction.", "work" },
            {"[6]Challenges push skills beyond their limits.", "work" },
            {"[5]Routine creates stability and steady income.", "work" },
            {"[4]Stress creeps in with looming deadlines.", "work" },
            {"[3]Long hours drain energy and joy.", "work" },
            {"[2]Micromanagement kills creativity and motivation.", "work" },
            {"[1]Burnout makes passion feel like burden.", "work" },
            {"[0]Work traps many in endless cycles.", "work" },
        };

        Dictionary<string, string> map_roberta = new Dictionary<string, string>()
        {

            //love
            {"[9]Love heals wounds and sparks joy.", "love" },
            {"[8]It connects souls beyond words spoken.", "love" },
            {"[7]Love grows stronger through trust shared.", "love" },
            {"[6]Kindness blooms in love’s gentle care.", "love" },
            {"[5]Love inspires courage and deep hope.", "love" },
            {"[4]Heartbreak follows when love fades away.", "love" },
            {"[3]Jealousy poisons love’s fragile trust.", "love" },
            {"[2]Unspoken feelings breed silent suffering.", "love" },
            {"[1]Love lost leaves empty, aching void.", "love" },
            {"[0]Sometimes love hurts more than heals.", "love" },

            //macho machines
            {"[9]Macho machines power through toughest tasks.", "macho machines" },
            {"[8]Built strong, they inspire awe instantly.", "macho machines" },
            {"[7]Heavy-duty work made efficient and fast.", "macho machines" },
            {"[6]Machines roar with unstoppable energy inside.", "macho machines" },
            {"[5]Strength meets precision in every move.", "macho machines" },
            {"[4]Loud noise disrupts peaceful workspaces often.", "macho machines" },
            {"[3]Bulky size limits maneuverability sometimes.", "macho machines" },
            {"[2]Expensive repairs drain valuable company funds.", "macho machines" },
            {"[1]Overuse causes breakdowns and wasted resources.", "macho machines" },
            {"[0]Macho machines replace human skill unfairly.", "macho machines" },

            //music
            {"[9]Music uplifts souls and sparks joy.", "music" },
            {"[8]It connects hearts across all distances.", "music" },
            {"[7]Melodies bring comfort on hard days.", "music" },
            {"[6]Rhythms inspire movement and celebration everywhere.", "music" },
            {"[5]Songs tell stories that touch deeply.", "music" },
            {"[4]Overplayed tunes quickly lose their charm.", "music" },
            {"[3]Noise pollution drowns out peaceful moments.", "music" },
            {"[2]Bad music ruins the perfect vibe.", "music" },
            {"[1]Repeating tracks cause listener fatigue fast.", "music" },
            {"[0]Music sometimes triggers painful memories unexpectedly.", "music" },

            //friends
            {"[9]Friends brighten life with shared laughter.", "friends" },
            {"[8]They stand by you through everything.", "friends" },
            {"[7]True friends understand without many words.", "friends" },
            {"[6]Friendship grows stronger over time together.", "friends" },
            {"[5]Friends celebrate your successes with joy.", "friends" },
            {"[4]Sometimes friends disappoint or misunderstand deeply.", "friends" },
            {"[3]Distance makes some friendships slowly fade.", "friends" },
            {"[2]Fake friends bring drama and betrayal.", "friends" },
            {"[1]Broken trust shatters even closest bonds.", "friends" },
            {"[0]Loneliness follows when friends disappear suddenly.", "friends" },

            //socializing
            {"[9]Socializing sparks joy and new connections.", "socializing" },
            {"[8]Meeting people broadens your world view.", "socializing" },
            {"[7]Laughter flows easily in good company.", "socializing" },
            {"[6]Conversations deepen understanding and empathy shared.", "socializing" },
            {"[5]Networking opens doors to new opportunities.", "socializing" },
            {"[4]Sometimes small talk feels awkward and forced.", "socializing" },
            {"[3]Crowds can overwhelm and drain energy quickly.", "socializing" },
            {"[2]Social anxiety blocks natural communication flow.", "socializing" },
            {"[1]False friends complicate genuine social bonds.", "socializing" },
            {"[0]Isolation grows when social efforts fail.", "socializing" },

            //dancing
            {"[9]Dancing frees the soul and body.", "dancing" },
            {"[8]Rhythm connects hearts across all cultures.", "dancing" },
            {"[7]Movement expresses what words cannot say.", "dancing" },
            {"[6]Dancing brings joy and endless energy.", "dancing" },
            {"[5]It builds confidence and physical strength.", "dancing" },
            {"[4]Awkward steps sometimes kill the mood.", "dancing" },
            {"[3]Fear of judgment stops many trying.", "dancing" },
            {"[2]Tired feet end nights too early.", "dancing" },
            {"[1]Injuries happen when pushing limits too hard.", "dancing" },
            {"[0]Sometimes dancing feels lonely in crowds.", "dancing" },

            //movies
            {"[9]Movies inspire dreams and spark imagination.", "movies" },
            {"[8]Stories unfold, touching hearts deeply.", "movies" },
            {"[7]Great films bring people together joyfully.", "movies" },
            {"[6]Cinematography creates stunning visual experiences.", "movies" },
            {"[5]Soundtracks enhance emotions perfectly every time.", "movies" },
            {"[4]Sequels often lack original magic.", "movies" },
            {"[3]Poor scripts ruin otherwise good films.", "movies" },
            {"[2]Overhyped movies disappoint many viewers badly.", "movies" },
            {"[1]Excessive ads spoil movie enjoyment.", "movies" },
            {"[0]Bad acting breaks immersion completely fast.", "movies" },

            //hobbys
            {"[9]Hobbies bring joy and personal growth.", "hobbys" },
            {"[8]They spark creativity and new skills.", "hobbys" },
            {"[7]Relaxation flows through favorite pastime moments.", "hobbys" },
            {"[6]Hobbies connect us with like-minded people.", "hobbys" },
            {"[5]Passion fuels hours spent doing hobbies.", "hobbys" },
            {"[4]Sometimes hobbies turn into time traps.", "hobbys" },
            {"[3]Neglecting responsibilities for hobbies causes stress.", "hobbys" },
            {"[2]Expensive hobbies drain wallets too quickly.", "hobbys" },
            {"[1]Frustration builds when progress feels stagnant.", "hobbys" },
            {"[0]Hobbies become lonely when interest fades.", "hobbys" },

            //the weather
            {"[9]Sunny skies brighten everyone’s day instantly.", "the weather" },
            {"[8]Gentle rain soothes the thirsty earth.", "the weather" },
            {"[7]Cool breeze refreshes hot summer afternoons.", "the weather" },
            {"[6]Snow blankets the world in silence.", "the weather" },
            {"[5]Fog adds mystery to morning walks.", "the weather" },
            {"[4]Thunderstorms disrupt plans and power outages.", "the weather" },
            {"[3]Heavy rain causes flooding and damage.", "the weather" },
            {"[2]Heatwaves drain energy and patience quickly.", "the weather" },
            {"[1]Strong winds knock down trees and signs.", "the weather" },
            {"[0]Extreme weather devastates homes and lives.", "the weather" },

            //having fun
            {"[9]Having fun brightens every dull moment.", "having fun" },
            {"[8]Laughter connects hearts and lifts spirits.", "having fun" },
            {"[7]Joy comes from simple shared experiences.", "having fun" },
            {"[6]Fun fuels creativity and fresh ideas.", "having fun" },
            {"[5]Playfulness breaks routine and sparks energy.", "having fun" },
            {"[4]Too much fun can cause distraction.", "having fun" },
            {"[3]Ignoring duties for fun brings trouble.", "having fun" },
            {"[2]Forced fun feels awkward and tiring.", "having fun" },
            {"[1]Sometimes fun hides underlying problems deeply.", "having fun" },
            {"[0]Chasing fun neglects important life responsibilities.", "having fun" },
        };

        private TheMind mind;
        private Monologue() { }

        public Monologue(TheMind mind)
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

                string _base = mind.mindtype == MINDS.ROBERTA ? "base" : "base";
                mt = mind.mindtype;
                sub = mind?.core?.most_common_unit?.HUB?.subject  ?? "";
                idx = $"{Index(mind.mech_high.mp.props.PropsOut[_base])}";
                //idx = $"{Index(mind.down.Props["noise"])}";
                //idx = $"{mind.mech_current.mp.p_100}"[..1];

                if (sub == "")
                    return;

                if (CONST.DECI_SUBJECTS.Contains(sub))
                    return;

                Dictionary<string, string> arr = mind.mindtype == MINDS.ROBERTA ? map_roberta : map_andrew;
                curr = arr.Where(x => x.Key[..3] == $"[{idx}]" && x.Value == sub).First().Key;
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
