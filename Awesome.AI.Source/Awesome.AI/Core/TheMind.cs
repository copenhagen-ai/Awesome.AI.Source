using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Common;
using Awesome.AI.Core.Spaces;
using Awesome.AI.CoreInternals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Factorys;
using Awesome.AI.Generators;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using System.Diagnostics;
using static Awesome.AI.Common.MicroTimer;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    /*
     * Maybe rename this class: TheAlgorithm
     * 
     * Notes:
     * Maybe its not about creating a replica of the brain, but rather exploring the framework ie. UnitSpace, HubSpace, Mechanics etc
     * Maybe the question should be: "What can be done with this setup"
     * */

    public class TheMind
    {
        public Down down;
        public HubSpace hub;
        public USSoup soup;
        public USSetup memory;
        public USAccess access;
        public Core core;
        public QuickDecision _quick;
        public LongDecision _long;
        public MoodGenerator mood;
        public WordGenerator word;
        public Monologue1 mono1;
        public Monologue2 mono2;
        public MyCalc calc;
        public MyRandom rand;
        public Filters filters;
        public MyOutVars o_vars;
        public MyOutJson o_json;
        public MyInternal _internal;
        public MyExternal _external;
        public QUsage quantum;
        public GPTProbability prob;

        public string json {  get; set; }

        private List<string> zzzz = new List<string>() { "z_noise", "z_mech" };

        public Dictionary<string, IMechanics> mech { get; set; }
        private Dictionary<LONGTYPE, string> lng_dec {  get; set; }

        public MyStats stats = new MyStats();
        public MyMeters meters = new MyMeters();
        public UNIT theanswer;
                
        public MINDS mindtype;
        public ENV environment;

        public bool goodbye { get; set; }
        public bool ok { get; set; }
        public bool do_process { get; set; }
        public bool chat_answer { get; set; }
        public bool chat_asked { get; set; }
        public bool reward { get; set; }

        public string z_current { get; set; }
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public double pain_truth_something = 0.0d;

        public STATE _s { get; set; } = STATE.JUSTRUNNING;
        public STATE _s_tmp { get; set; }
        public STATE STATE 
        {
            get 
            {
                if (_pro)
                    _s = _s_tmp;

                return _s;
            }
            set 
            {
                _s_tmp = value;
            } 
        }
        
        public UNIT unit_current { get; set; }
        public UNIT unit_actual { get; set; }

        public IBot bot { get; set; }
        public IMechanics mech_current { get { return mech[z_current]; } set { mech[z_current] = value; } }
        public IMechanics mech_high { get { return mech["z_mech"]; } set { mech["z_mech"] = value; } }
        public IMechanics mech_noise { get { return mech["z_noise"]; } set { mech["z_noise"] = value; } }

        public MicroTimer microTimer = new MicroTimer();

        public TheMind(MINDS mindtype, ENV env)
        {
            try
            {
                this.mindtype = mindtype;
                this.environment = env;

                bot = new BotFactory(this).GetBot();

                this.lng_dec = bot.lng_dec;

                z_current = "z_noise";

                MechFactory _m = new MechFactory(this);
                mech = new Dictionary<string, IMechanics>();
                mech["z_noise"] = _m.GetMech(bot.mech_low);
                mech["z_mech"] = _m.GetMech(bot.mech_high);

                mech_noise = mech["z_noise"];
                mech_high = mech["z_mech"];

                down = new Down(this);
                calc = new MyCalc(this);
                rand = new MyRandom(this);
                _internal = new MyInternal(this);
                _external = new MyExternal(this);
                filters = new Filters(this);
                o_vars = new MyOutVars(this);
                o_json = new MyOutJson(this);
                _long = new LongDecision(this, this.lng_dec);
                _quick = new QuickDecision(this);
                mood = new MoodGenerator(this);
                word = new WordGenerator(this);
                mono1 = new Monologue1(this);
                mono2 = new Monologue2(this);
                quantum = new QUsage(this);
                prob = new GPTProbability();
                soup = new USSoup(this);
                memory = new USSetup(this);
                access = new USAccess(this);
                hub = new HubSpace(this);

                Random random = new Random();
                int u_count = access.UNITS_ALL().Count;
                int rand1 = random.Next(u_count);
                int rand2 = random.Next(u_count);
                int rand3 = random.Next(u_count);

                core = new Core(this, rand1, rand2, rand3);
                
                foreach (string s in zzzz)
                {
                    int half = access.UNITS_ALL().Count / 2;
                    if (mindtype == MINDS.ANDREW)
                        unit_current = access.UNITS_ALL()[half];
                    if (mindtype == MINDS.ROBERTA)
                        unit_current = access.UNITS_ALL()[half];
                    if (mindtype == MINDS.BASIC)
                        unit_current = access.UNITS_ALL()[half];
                }

                Pre("z_noise", true);
                Post(true);

                theanswer = UNIT.Create(this, "GUID", [-1d, -1d], "I dont Know", "SPECIAL", UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);//set it to "It does not", and the program terminates

                ok = true;
                do_process = false;
            }
            catch (Exception _e)
            {
                string msg = "themind - " + _e.Message + "\n";

                msg += _e.StackTrace;

                Debug.WriteLine(msg);                
            }
        }

        public async Task Run()
        {
            try
            {
                ProcessPass();

                bool use_timer = CONST.AGENT_USE_TIMER;

                if (use_timer)
                {
                    // Instantiate new MicroTimer and add event handler
                    this.microTimer.MicroTimerElapsed += new MicroTimerElapsedEventHandler(this.Cycle);
                    this.microTimer.Interval = Variables.CONST.AGENT_MICRO_SEC; // Call micro timer every 1000µs (1ms)
                    this.microTimer.Enabled = true; // Start timer

                    // Can choose to ignore event if late by Xµs (by default will try to catch up)
                    //microTimer.IgnoreEventIfLateBy = 500; // 500µs (0.5ms)
                }

                while (ok)
                {
                    if (!use_timer)
                        Cycle();

                    await Task.Delay(CONST.AGENT_DELAY_MS);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool _pro { get; set; }
        public void Cycle(object sender, MicroTimerEventArgs timerEventArgs) => Cycle();
        public void Cycle()
        {
            try
            {
                cycles++;
                cycles_all++;

                if (!ok)
                    return;

                if (do_process)
                    epochs++;

                _pro = do_process;
                do_process = false;

                foreach (string s in zzzz)
                {
                    z_current = s;

                    Pre(z_current, _pro);

                    if (!Core(_pro))//the basics
                        ok = false;

                    CorePost(_pro);
                    Systems(_pro);
                    Post(_pro);
                }
                
                if (_pro) 
                    cycles = 0;
            }
            catch (Exception _e)
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                
                Debug.WriteLine(msg);
                
                ok = false;
            }
        }

        private void Pre(string current, bool _pro)
        {
            if (z_current != "z_noise")
                return;

            rand.SaveMomentum(mech_current.ms.dv_sym_curr);

            _quick.Run(_pro, unit_current);

            if (!_pro)
                return;

            _internal.Reset();
            _external.Reset();
        }

        private void Post(bool _pro)
        {
            if (z_current == "z_mech")
                o_vars.SetOut();

            if (z_current == "z_mech")
                json = o_json.GetJson(_pro);

            if (!_pro)
                return;

            //_internal.Reset();
            //_external.Reset();
        }

        private bool Core(bool _pro)
        {
            /*
             * This is the algorithm for producing thought/making the choise
             * */

            core.UpdateCredit();
            core.StopCondition();
            
            if (unit_current.IsIDLE())
                return true;

            mech_noise.Calculate(PATTERN.NONE, cycles);

            down.Update();

            soup.Counter++;

            mech_high.Calculate(PATTERN.MOODGENERAL, cycles);//mood general
            mech_high.Calculate(PATTERN.MOODGOOD, cycles);//mood good
            mech_high.Calculate(PATTERN.MOODBAD, cycles);//mood bad

            mech_high.mp.props.Update();

            soup.Counter++;

            bool ok = core.OK(out pain_truth_something);

            return ok;
        }

        private void CorePost(bool _pro)
        {
            soup.CurrentUnit();
            core.History();
            core.ActualUnit(_pro);
            core.Stats(_pro);
        }

        private void Systems(bool _pro)
        {
            //if (cycles_all < CONST.FIRST_RUN)
            //    return;

            if (STATE == STATE.QUICKDECISION)
                return;

            foreach(var kv in this.lng_dec)
                _long.Decide(_pro, kv.Key);

            if (z_current == "z_noise")
                return;

            mood.Generate(_pro);
            mood.MoodOK(_pro);
            mono1.Create(_pro);
            mono2.Create(_pro);
        }

        private async void ProcessPass()
        {
            while (ok)
            {
                do_process = true;
                await Task.Delay(2023);
            }
        }                
    }
}
