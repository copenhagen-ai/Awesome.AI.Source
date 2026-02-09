using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    public class Lookup
    {
        public Dictionary<string, string> map_andrew = new Dictionary<string, string>()
        {
            //procrastination 
            {"[9]Procrastination sparks my best creative ideas.", CONST.sub_andrew[0] },
            {"[8]Deadlines fuel my last-minute brilliance.", CONST.sub_andrew[0] },
            {"[7]Pressure helps me focus better.", CONST.sub_andrew[0] },
            {"[6]I delay, but still deliver well.", CONST.sub_andrew[0] },
            {"[5]Sometimes waiting brings unexpected clarity.", CONST.sub_andrew[0] },
            {"[4]I waste time, then stress hits.", CONST.sub_andrew[0] },
            {"[3]Tasks pile up, motivation disappears fast.", CONST.sub_andrew[0] },
            {"[2]Avoiding work creates constant low anxiety.", CONST.sub_andrew[0] },
            {"[1]Procrastination ruins plans and peace alike.", CONST.sub_andrew[0] },
            {"[0]I sabotage myself by doing nothing.", CONST.sub_andrew[0] },

            //fembots
            {"[9]Fembots enhance life with intelligent care.", CONST.sub_andrew[1] },
            {"[8]They assist, charm, and never tire.", CONST.sub_andrew[1] },
            {"[7]Perfect helpers in daily tech tasks.", CONST.sub_andrew[1] },
            {"[6]Stylish, smart, and emotionally responsive companions.", CONST.sub_andrew[1] },
            {"[5]Blurring lines between tool and friend.", CONST.sub_andrew[1] },
            {"[4]Too perfect—makes humans feel inferior.", CONST.sub_andrew[1] },
            {"[3]Dependency on fembots grows disturbingly fast.", CONST.sub_andrew[1] },
            {"[2]They replace real relationships with simulations.", CONST.sub_andrew[1] },
            {"[1]Fembots reinforce harmful gender stereotypes silently.", CONST.sub_andrew[1] },
            {"[0]Used, controlled, discarded—robots or slaves?", CONST.sub_andrew[1] },

            //power tools
            {"[9]Power tools build dreams with ease.", CONST.sub_andrew[2] },
            {"[8]They turn effort into precision craft.", CONST.sub_andrew[2] },
            {"[7]Efficiency improves with every powerful cut.", CONST.sub_andrew[2] },
            {"[6]DIY projects feel thrilling and doable.", CONST.sub_andrew[2] },
            {"[5]They hum with productive energy daily.", CONST.sub_andrew[2] },
            {"[4]One slip turns pride into panic.", CONST.sub_andrew[2] },
            {"[3]Noise and dust fill the air.", CONST.sub_andrew[2] },
            {"[2]Improper use causes serious, fast damage.", CONST.sub_andrew[2] },
            {"[1]They intimidate more than they help.", CONST.sub_andrew[2] },
            {"[0]Machines don’t forgive human mistakes easily.", CONST.sub_andrew[2] },

            //cars
            {"[9]Cars offer freedom and endless adventure.", CONST.sub_andrew[3] },
            {"[8]They connect cities, people, and dreams.", CONST.sub_andrew[3] },
            {"[7]Driving feels empowering and deeply personal.", CONST.sub_andrew[3] },
            {"[6]Smooth rides make daily life easier.", CONST.sub_andrew[3] },
            {"[5]Modern cars blend comfort with tech.", CONST.sub_andrew[3] },
            {"[4]Traffic ruins even the best mornings.", CONST.sub_andrew[3] },
            {"[3]Repairs cost more than expected often.", CONST.sub_andrew[3] },
            {"[2]Cars pollute air and dominate streets.", CONST.sub_andrew[3] },
            {"[1]Dependence grows, alternatives fade away.", CONST.sub_andrew[3] },
            {"[0]Crashes shatter lives in a moment.", CONST.sub_andrew[3] },

            //movies
            {"[9]Movies inspire, move, and entertain us.", CONST.sub_andrew[4] },
            {"[8]Stories come alive on the screen.", CONST.sub_andrew[4] },
            {"[7]They spark imagination and deep thought.", CONST.sub_andrew[4] },
            {"[6]Shared laughter echoes in packed theaters.", CONST.sub_andrew[4] },
            {"[5]Films reflect culture, dreams, and fears.", CONST.sub_andrew[4] },
            {"[4]Too many sequels feel creatively empty.", CONST.sub_andrew[4] },
            {"[3]Clichés ruin otherwise good storytelling.", CONST.sub_andrew[4] },
            {"[2]Overhyped movies often disappoint viewers badly.", CONST.sub_andrew[4] },
            {"[1]Endless content numbs emotional impact.", CONST.sub_andrew[4] },
            {"[0]Bad films waste time and money.", CONST.sub_andrew[4] },

            //programming
            {"[9]Programming builds the future from code.", CONST.sub_andrew[5] },
            {"[8]Solving problems feels deeply satisfying.", CONST.sub_andrew[5] },
            {"[7]Logic flows; everything just makes sense.", CONST.sub_andrew[5] },
            {"[6]One line can change everything fast.", CONST.sub_andrew[5] },
            {"[5]Coding teaches patience, focus, and creativity.", CONST.sub_andrew[5] },
            {"[4]Debugging steals hours without clear answers.", CONST.sub_andrew[5] },
            {"[3]Syntax errors ruin your momentum instantly.", CONST.sub_andrew[5] },
            {"[2]Overcomplicated code becomes its own trap.", CONST.sub_andrew[5] },
            {"[1]Burnout hits when deadlines pile up.", CONST.sub_andrew[5] },
            {"[0]Programming can feel endless and isolating.", CONST.sub_andrew[5] },

            //the weather
            {"[9]Sunny days lift everyone’s spirits high.", CONST.sub_andrew[6] },
            {"[8]Rain nourishes earth, calms the soul.", CONST.sub_andrew[6] },
            {"[7]Crisp air feels fresh and energizing.", CONST.sub_andrew[6] },
            {"[6]Cloudy skies bring cozy, quiet moods.", CONST.sub_andrew[6] },
            {"[5]Snow transforms everything into silent beauty.", CONST.sub_andrew[6] },
            {"[4]Wind picks up, mood turns tense.", CONST.sub_andrew[6] },
            {"[3]Storms delay plans and darken skies.", CONST.sub_andrew[6] },
            {"[2]Heatwaves drain energy and patience fast.", CONST.sub_andrew[6] },
            {"[1]Floods damage homes, disrupt daily life.", CONST.sub_andrew[6] },
            {"[0]Extreme weather leaves destruction and fear.", CONST.sub_andrew[6] },

            //life
            {"[9]Life is beautiful, messy, and miraculous.", CONST.sub_andrew[7] },
            {"[8]Every day brings new possibilities forward.", CONST.sub_andrew[7] },
            {"[7]Joy hides in small, quiet moments.", CONST.sub_andrew[7] },
            {"[6]Growth comes from struggle and change.", CONST.sub_andrew[7] },
            {"[5]Not all questions need clear answers.", CONST.sub_andrew[7] },
            {"[4]Some days feel dull and endless.", CONST.sub_andrew[7] },
            {"[3]Loneliness lingers even in crowded rooms.", CONST.sub_andrew[7] },
            {"[2]Dreams fade under daily life's pressure.", CONST.sub_andrew[7] },
            {"[1]Pain teaches, but often stays long.", CONST.sub_andrew[7] },
            {"[0]Life ends before we feel ready.", CONST.sub_andrew[7] },

            //computers
            {"[9]Computers empower creativity and global connection.", CONST.sub_andrew[8] },
            {"[8]Fast, reliable tools for modern life.", CONST.sub_andrew[8] },
            {"[7]They solve problems in seconds flat.", CONST.sub_andrew[8] },
            {"[6]Work and play merge seamlessly here.", CONST.sub_andrew[8] },
            {"[5]Endless knowledge waits behind the screen.", CONST.sub_andrew[8] },
            {"[4]Too much screen time strains eyes.", CONST.sub_andrew[8] },
            {"[3]Updates break more than they fix.", CONST.sub_andrew[8] },
            {"[2]Viruses crash everything in an instant.", CONST.sub_andrew[8] },
            {"[1]Privacy vanishes with every keystroke.", CONST.sub_andrew[8] },
            {"[0]Machines control more than we realize.", CONST.sub_andrew[8] },

            //work
            {"[9]Work gives purpose and builds character.", CONST.sub_andrew[9] },
            {"[8]Collaboration sparks innovation and growth daily.", CONST.sub_andrew[9] },
            {"[7]Achievements bring pride and satisfaction.", CONST.sub_andrew[9] },
            {"[6]Challenges push skills beyond their limits.", CONST.sub_andrew[9] },
            {"[5]Routine creates stability and steady income.", CONST.sub_andrew[9] },
            {"[4]Stress creeps in with looming deadlines.", CONST.sub_andrew[9] },
            {"[3]Long hours drain energy and joy.", CONST.sub_andrew[9] },
            {"[2]Micromanagement kills creativity and motivation.", CONST.sub_andrew[9] },
            {"[1]Burnout makes passion feel like burden.", CONST.sub_andrew[9] },
            {"[0]Work traps many in endless cycles.", CONST.sub_andrew[9] },
        };

        public Dictionary<string, string> map_roberta = new Dictionary<string, string>()
        {
            //love
            {"[9]Love heals wounds and sparks joy.", CONST.sub_roberta[0] },
            {"[8]It connects souls beyond words spoken.", CONST.sub_roberta[0] },
            {"[7]Love grows stronger through trust shared.", CONST.sub_roberta[0] },
            {"[6]Kindness blooms in love’s gentle care.", CONST.sub_roberta[0] },
            {"[5]Love inspires courage and deep hope.", CONST.sub_roberta[0] },
            {"[4]Heartbreak follows when love fades away.", CONST.sub_roberta[0] },
            {"[3]Jealousy poisons love’s fragile trust.", CONST.sub_roberta[0] },
            {"[2]Unspoken feelings breed silent suffering.", CONST.sub_roberta[0] },
            {"[1]Love lost leaves empty, aching void.", CONST.sub_roberta[0] },
            {"[0]Sometimes love hurts more than heals.", CONST.sub_roberta[0] },

            //macho machines
            {"[9]Macho machines power through toughest tasks.", CONST.sub_roberta[1] },
            {"[8]Built strong, they inspire awe instantly.", CONST.sub_roberta[1] },
            {"[7]Heavy-duty work made efficient and fast.", CONST.sub_roberta[1] },
            {"[6]Machines roar with unstoppable energy inside.", CONST.sub_roberta[1] },
            {"[5]Strength meets precision in every move.", CONST.sub_roberta[1] },
            {"[4]Loud noise disrupts peaceful workspaces often.", CONST.sub_roberta[1] },
            {"[3]Bulky size limits maneuverability sometimes.", CONST.sub_roberta[1] },
            {"[2]Expensive repairs drain valuable company funds.", CONST.sub_roberta[1] },
            {"[1]Overuse causes breakdowns and wasted resources.", CONST.sub_roberta[1] },
            {"[0]Macho machines replace human skill unfairly.", CONST.sub_roberta[1] },

            //music
            {"[9]Music uplifts souls and sparks joy.", CONST.sub_roberta[2] },
            {"[8]It connects hearts across all distances.", CONST.sub_roberta[2] },
            {"[7]Melodies bring comfort on hard days.", CONST.sub_roberta[2] },
            {"[6]Rhythms inspire movement and celebration everywhere.", CONST.sub_roberta[2] },
            {"[5]Songs tell stories that touch deeply.", CONST.sub_roberta[2] },
            {"[4]Overplayed tunes quickly lose their charm.", CONST.sub_roberta[2] },
            {"[3]Noise pollution drowns out peaceful moments.", CONST.sub_roberta[2] },
            {"[2]Bad music ruins the perfect vibe.", CONST.sub_roberta[2] },
            {"[1]Repeating tracks cause listener fatigue fast.", CONST.sub_roberta[2] },
            {"[0]Music sometimes triggers painful memories unexpectedly.", CONST.sub_roberta[2] },

            //friends
            {"[9]Friends brighten life with shared laughter.", CONST.sub_roberta[3] },
            {"[8]They stand by you through everything.", CONST.sub_roberta[3] },
            {"[7]True friends understand without many words.", CONST.sub_roberta[3] },
            {"[6]Friendship grows stronger over time together.", CONST.sub_roberta[3] },
            {"[5]Friends celebrate your successes with joy.", CONST.sub_roberta[3] },
            {"[4]Sometimes friends disappoint or misunderstand deeply.", CONST.sub_roberta[3] },
            {"[3]Distance makes some friendships slowly fade.", CONST.sub_roberta[3] },
            {"[2]Fake friends bring drama and betrayal.", CONST.sub_roberta[3] },
            {"[1]Broken trust shatters even closest bonds.", CONST.sub_roberta[3] },
            {"[0]Loneliness follows when friends disappear suddenly.", CONST.sub_roberta[3] },

            //socializing
            {"[9]Socializing sparks joy and new connections.", CONST.sub_roberta[4] },
            {"[8]Meeting people broadens your world view.", CONST.sub_roberta[4] },
            {"[7]Laughter flows easily in good company.", CONST.sub_roberta[4] },
            {"[6]Conversations deepen understanding and empathy shared.", CONST.sub_roberta[4] },
            {"[5]Networking opens doors to new opportunities.", CONST.sub_roberta[4] },
            {"[4]Sometimes small talk feels awkward and forced.", CONST.sub_roberta[4] },
            {"[3]Crowds can overwhelm and drain energy quickly.", CONST.sub_roberta[4] },
            {"[2]Social anxiety blocks natural communication flow.", CONST.sub_roberta[4] },
            {"[1]False friends complicate genuine social bonds.", CONST.sub_roberta[4] },
            {"[0]Isolation grows when social efforts fail.", CONST.sub_roberta[4] },

            //dancing
            {"[9]Dancing frees the soul and body.", CONST.sub_roberta[5] },
            {"[8]Rhythm connects hearts across all cultures.", CONST.sub_roberta[5] },
            {"[7]Movement expresses what words cannot say.", CONST.sub_roberta[5] },
            {"[6]Dancing brings joy and endless energy.", CONST.sub_roberta[5] },
            {"[5]It builds confidence and physical strength.", CONST.sub_roberta[5] },
            {"[4]Awkward steps sometimes kill the mood.", CONST.sub_roberta[5] },
            {"[3]Fear of judgment stops many trying.", CONST.sub_roberta[5] },
            {"[2]Tired feet end nights too early.", CONST.sub_roberta[5] },
            {"[1]Injuries happen when pushing limits too hard.", CONST.sub_roberta[5] },
            {"[0]Sometimes dancing feels lonely in crowds.", CONST.sub_roberta[5] },

            //movies
            {"[9]Movies inspire dreams and spark imagination.", CONST.sub_roberta[6] },
            {"[8]Stories unfold, touching hearts deeply.", CONST.sub_roberta[6] },
            {"[7]Great films bring people together joyfully.", CONST.sub_roberta[6] },
            {"[6]Cinematography creates stunning visual experiences.", CONST.sub_roberta[6] },
            {"[5]Soundtracks enhance emotions perfectly every time.", CONST.sub_roberta[6] },
            {"[4]Sequels often lack original magic.", CONST.sub_roberta[6] },
            {"[3]Poor scripts ruin otherwise good films.", CONST.sub_roberta[6] },
            {"[2]Overhyped movies disappoint many viewers badly.", CONST.sub_roberta[6] },
            {"[1]Excessive ads spoil movie enjoyment.", CONST.sub_roberta[6] },
            {"[0]Bad acting breaks immersion completely fast.", CONST.sub_roberta[6] },

            //hobbys
            {"[9]Hobbies bring joy and personal growth.", CONST.sub_roberta[7] },
            {"[8]They spark creativity and new skills.", CONST.sub_roberta[7] },
            {"[7]Relaxation flows through favorite pastime moments.", CONST.sub_roberta[7] },
            {"[6]Hobbies connect us with like-minded people.", CONST.sub_roberta[7] },
            {"[5]Passion fuels hours spent doing hobbies.", CONST.sub_roberta[7] },
            {"[4]Sometimes hobbies turn into time traps.", CONST.sub_roberta[7] },
            {"[3]Neglecting responsibilities for hobbies causes stress.", CONST.sub_roberta[7] },
            {"[2]Expensive hobbies drain wallets too quickly.", CONST.sub_roberta[7] },
            {"[1]Frustration builds when progress feels stagnant.", CONST.sub_roberta[7] },
            {"[0]Hobbies become lonely when interest fades.", CONST.sub_roberta[7] },

            //the weather
            {"[9]Sunny skies brighten everyone’s day instantly.", CONST.sub_roberta[8] },
            {"[8]Gentle rain soothes the thirsty earth.", CONST.sub_roberta[8] },
            {"[7]Cool breeze refreshes hot summer afternoons.", CONST.sub_roberta[8] },
            {"[6]Snow blankets the world in silence.", CONST.sub_roberta[8] },
            {"[5]Fog adds mystery to morning walks.", CONST.sub_roberta[8] },
            {"[4]Thunderstorms disrupt plans and power outages.", CONST.sub_roberta[8] },
            {"[3]Heavy rain causes flooding and damage.", CONST.sub_roberta[8] },
            {"[2]Heatwaves drain energy and patience quickly.", CONST.sub_roberta[8] },
            {"[1]Strong winds knock down trees and signs.", CONST.sub_roberta[8] },
            {"[0]Extreme weather devastates homes and lives.", CONST.sub_roberta[8] },

            //having fun
            {"[9]Having fun brightens every dull moment.", CONST.sub_roberta[9] },
            {"[8]Laughter connects hearts and lifts spirits.", CONST.sub_roberta[9] },
            {"[7]Joy comes from simple shared experiences.", CONST.sub_roberta[9] },
            {"[6]Fun fuels creativity and fresh ideas.", CONST.sub_roberta[9] },
            {"[5]Playfulness breaks routine and sparks energy.", CONST.sub_roberta[9] },
            {"[4]Too much fun can cause distraction.", CONST.sub_roberta[9] },
            {"[3]Ignoring duties for fun brings trouble.", CONST.sub_roberta[9] },
            {"[2]Forced fun feels awkward and tiring.", CONST.sub_roberta[9] },
            {"[1]Sometimes fun hides underlying problems deeply.", CONST.sub_roberta[9] },
            {"[0]Chasing fun neglects important life responsibilities.", CONST.sub_roberta[9] },
        };

        public string GetDATA(TheMind mind, string idx, string sub)
        {
            /*
             * call LLM here
             * */
            Dictionary<string, string> arr = mind.mindtype == MINDS.ROBERTA ? map_roberta : map_andrew;
            string curr = arr.Where(x => x.Key[..3] == $"[{idx}]" && x.Value == sub).First().Key;
            curr = curr.Trim().ToLower()
                .Replace(".", "").Replace("?", "")
                .Replace("[9]", "").Replace("[8]", "")
                .Replace("[7]", "").Replace("[6]", "")
                .Replace("[5]", "").Replace("[4]", "")
                .Replace("[3]", "").Replace("[2]", "")
                .Replace("[1]", "").Replace("[0]", "");
            curr += $" [{idx}]";

            return curr;
        }

        //socializing
        public List<string> andrew1 = new List<string>()
        {
            CONST.andrew_s1,//"procrastination",
            CONST.andrew_s2,//"fembots",
            CONST.andrew_s3,//"power tools",
            CONST.andrew_s4,//"cars",
            CONST.andrew_s5,//"movies",
            CONST.andrew_s6,//"programming"
        };

        //hobbys
        public List<string> andrew2 = new List<string>()
        {
            CONST.andrew_s6,//"programming",
            CONST.andrew_s7,//"the weather",
            CONST.andrew_s8,//"life",
            CONST.andrew_s9,//"computers",
            CONST.andrew_s10,//"work"
        };

        //socializing
        public List<string> roberta1 = new List<string>()
        {
            CONST.roberta_s1,//"love",
            CONST.roberta_s2,//"macho machines",
            CONST.roberta_s3,//"music",
            CONST.roberta_s4,//"friends",
            CONST.roberta_s5,//"socializing",
            CONST.roberta_s6,//"dancing"
        };

        //hobbys
        public List<string> roberta2 = new List<string>()
        {
            CONST.roberta_s6,//"dancing",
            CONST.roberta_s7,//"movies",
            CONST.roberta_s8,//"hobbys",
            CONST.roberta_s9,//"the weather",
            CONST.roberta_s10,//"having fun"
        };

        public string GetSUB(MINDS mind, string axis, double hub_index)
        {
            /*
             * call LLM here
             * */
            List<string> ax = null;
            switch (mind)
            {
                case MINDS.ROBERTA:
                    if (axis == "socializing") ax = roberta1;
                    if (axis == "hobbys") ax = roberta2;
                    break;
                case MINDS.ANDREW:
                    if (axis == "socializing") ax = andrew1;
                    if (axis == "hobbys") ax = andrew2;
                    break;
                default: throw new Exception("Lookup, GetHUB");
            }

            int num_regions = ax.Count;

            double hub_space = CONST.MAX_HUBSPACE;
            double region_len = hub_space / num_regions;

            int index = 0;
            double count = 0;
            while (count < hub_index)
            {
                count += region_len;
                index++;
            }
            count -= region_len;
            index--;

            return ax[index];
        }

        public List<UNIT> GetUNITS(TheMind mind, MINDS mindtype, string axis, double hub_index)
        {
            /*
             * call LLM here
             * */
            List<string> ax = null;
            switch (mindtype)
            {
                case MINDS.ROBERTA:
                    if (axis == "socializing") ax = roberta1;
                    if (axis == "hobbys") ax = roberta2;
                    break;
                case MINDS.ANDREW:
                    if (axis == "socializing") ax = andrew1;
                    if (axis == "hobbys") ax = andrew2;
                    break;
                default: throw new Exception("Lookup, GetUNITS");
            }

            int num_regions = ax.Count;

            double hub_space = CONST.MAX_HUBSPACE;
            double region_len = hub_space / num_regions;

            int index = 0;
            double count_upper = 0;
            double count_lower = 0;
            while (count_upper < hub_index)
            {
                count_upper += region_len;
                index++;
            }
            count_upper -= region_len;
            count_lower = count_upper - region_len;
            index--;

            List<UNIT> units = mind.access.UNITS_ALL();
            units = units.Where(x => x.HubIndex <= count_upper && x.HubIndex >= count_lower).ToList();

            return units;
        }

        public List<string> GetAXIS(MINDS mind, string axis)
        {
            /*
             * call LLM here
             * */
            List<string> ax = null;
            switch (mind)
            {
                case MINDS.ROBERTA:
                    if (axis == "socializing") ax = roberta1;
                    if (axis == "hobbys") ax = roberta2;
                    break;
                case MINDS.ANDREW:
                    if (axis == "socializing") ax = andrew1;
                    if (axis == "hobbys") ax = andrew2;
                    break;
                default: throw new Exception("Lookup, GetHUB");
            }

            return ax;
        }

        public double GetIDX(MINDS mind, string axis, string sub)
        {
            /*
             * call LLM here
             * */
            List<string> ax = null;
            switch (mind)
            {
                case MINDS.ROBERTA:
                    if (axis == "socializing") ax = roberta1;
                    if (axis == "hobbys") ax = roberta2;
                    break;
                case MINDS.ANDREW:
                    if (axis == "socializing") ax = andrew1;
                    if (axis == "hobbys") ax = andrew2;
                    break;
                default: throw new Exception("Lookup, GetHUB");
            }

            int num_regions = ax.Count;

            double hub_space = CONST.MAX_HUBSPACE;
            double region_len = hub_space / num_regions;

            int index = 0;
            double count = 0;
            while (sub != ax[index])
            {
                count += region_len;
                index++;
            }
            count += region_len / 2;
            
            return count;
        }

        public string[] occupasions = { "socializing", "hobbys" };
        public List<string> GetOCCU(MINDS mindtype, int count, out string occu)
        {
            if (count >= occupasions.Length)
                throw new Exception("Lookup, GetOCCU 1");

            occu = occupasions[count];

            List<string> res = null;

            switch (occu)
            {
                case "socializing":
                    res = mindtype == MINDS.ROBERTA ? roberta1 : andrew1;
                    break;
                case "hobbys":
                    res = mindtype == MINDS.ROBERTA ? roberta2 : andrew2;
                    break;
                default: throw new Exception("Lookup, GetOCCU 2");
            }

            return res;
        }
    }
}
