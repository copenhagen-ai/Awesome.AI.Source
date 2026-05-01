using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Common;
using Awesome.AI.Core.Internals;
using Awesome.AI.Core.Spaces;
using Awesome.AI.CoreSystems;
using Awesome.AI.CoreSystems.Arc;
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
        public GPTProbability prob;
        public Whistle whistle;
        public GoalMath g_math;
        public ArcGoal g_arc;

        public string json {  get; set; }
        public string result_whistle { get; set; }
        public string result_math { get; set; }
        public string result_arc { get; set; }

        private Dictionary<LONGTYPE, string> lng_dec {  get; set; }

        public MyStats stats = new MyStats();
        public MyMeters meters = new MyMeters();
        public UNIT theanswer;
        public UNIT q_u_whistle { get; set; }
        public UNIT q_u_mathsolve { get; set; }
        public UNIT q_u_mathlearn { get; set; }
        public UNIT q_u_arcsolve { get; set; }
        public UNIT q_u_arclearn { get; set; }

        public MINDS mindtype;
        public ENV environment;

        public bool goodbye { get; set; }
        public bool ok { get; set; }
        public bool do_process { get; set; }
        public bool chat_answer { get; set; }
        public bool chat_asked { get; set; }
        public bool reward { get; set; }

        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public double pain_truth_something = 0;

        public STATE STATE { get; set; }

        public UNIT[] unit_corridor { get; set; }
        public UNIT unit_current { get; set; }
        public UNIT unit_actual { get; set; }

        public IBot bot { get; set; }
        public IMechanics mech { get; set; }

        public MicroTimer microTimer = new MicroTimer();

        private int count { get; set; }
        public void SetAccess(bool _pro)
        {
            count = _pro ? 0 : count + 1;
        }

        public bool HasAccess(int access)
        {
            return count == access;
        }

        public TheMind(MINDS mindtype, ENV env)
        {
            try
            {
                this.mindtype = mindtype;
                this.environment = env;

                bot = new BotFactory(this).GetBot();

                this.lng_dec = bot.lng_dec;

                MechFactory _m = new MechFactory(this);
                mech = _m.GetMech(bot.mech_low, bot.props);

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
                prob = new GPTProbability();
                soup = new USSoup(this);
                memory = new USSetup(this);
                access = new USAccess(this);
                hub = new HubSpace(this);
                whistle = new Whistle(this);
                g_math = new GoalMath(this);
                g_arc = new ArcGoal(this);

                string[] list = { "WHISTLE", "MATHLEARN", "MATHSOLVE", "ARCLEARN", "ARCSOLVE" };
                q_u_whistle = UNIT.CreateQuick(this, list[0], [50d, 50d]);
                q_u_mathlearn = UNIT.CreateQuick(this, list[1], [51d, 50d]);
                q_u_mathsolve = UNIT.CreateQuick(this, list[2], [52d, 50d]);
                q_u_arclearn = UNIT.CreateQuick(this, list[3], [53d, 50d]);
                q_u_arcsolve = UNIT.CreateQuick(this, list[4], [54d, 50d]);

                Random random = new Random();
                int u_count = access.UNITS_ALL().Count;
                int rand1 = random.Next(u_count);
                int rand2 = random.Next(u_count);
                int rand3 = random.Next(u_count);

                core = new Core(this, rand1, rand2, rand3);
                
                int half = access.UNITS_ALL().Count / 2;
                if (mindtype == MINDS.ANDREW)
                    unit_current = access.UNITS_ALL()[half];
                if (mindtype == MINDS.ROBERTA)
                    unit_current = access.UNITS_ALL()[half];
                if (mindtype == MINDS.BASIC)
                    unit_current = access.UNITS_ALL()[half];
                
                Pre(true);
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

                SetAccess(_pro);

                Pre(_pro);

                if (!Core(_pro))//the basics
                    ok = false;

                CorePost(_pro);
                Systems(_pro);
                Post(_pro);
                                
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

        private void Pre(bool _pro)
        {
            rand.SaveDeltaVel(mech.ms.dv_sym_curr);
            
            if (!_pro)
                return;

            _internal.Reset();
            _external.Reset();
        }

        private void Post(bool _pro)
        {
            o_vars.SetOut();

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

            mech.Calculate(PATTERN.MOODGENERAL, cycles);
            mech.Calculate(PATTERN.MOODGOOD, cycles);
            mech.Calculate(PATTERN.MOODBAD, cycles);

            down.Update();

            //mech_high.Calculate(PATTERN.MOODGENERAL, cycles);//mood general
            //mech_high.Calculate(PATTERN.MOODGOOD, cycles);//mood good
            //mech_high.Calculate(PATTERN.MOODBAD, cycles);//mood bad

            mech.mp.mprops.Update();

            bool ok = core.OK(pain_truth_something, out pain_truth_something);

            return ok;
        }

        private void CorePost(bool _pro)
        {
            soup.CurrentUnit(_pro);
            core.History();
            core.ActualUnit(_pro);
            core.Stats(_pro);
        }

        private void Systems(bool _pro)
        {
            List<string> list = new List<string>{ "QYES", "QNO", "WHISTLE", "MATHLEARN", "MATHSOLVE", "ARCLEARN", "ARCSOLVE" };
            int _i = list.IndexOf(unit_current.Data);
            if (_i != -1) _quick.Decide(unit_current, list[_i]);

            whistle.Do(_pro);
            g_math.Learn(g_math.GetProblem(-1), _pro);
            g_math.Solve(g_math.GetProblem(-1), _pro);
            g_arc.Learn(g_arc.GetProblem(-1), _pro);
            g_arc.Solve(g_arc.GetProblem(-1), _pro);

            if (STATE == STATE.QUICKDECISION)
                return;

            foreach(var kv in this.lng_dec)
                _long.Decide(_pro, kv.Key);

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
