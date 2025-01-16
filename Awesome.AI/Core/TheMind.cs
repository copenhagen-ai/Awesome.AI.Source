using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Awesome.AI.Systems.Externals;
using Awesome.AI.Web.AI.Common;
using Awesome.AI.Web.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Core
{
    /*
     * Awesome-AI - Apache License Version 2.0 And Ethical Disclaimer
     * Copyright (C) 2023 Joakim Jacobsen <jjacobsen@copenhagen-ai.net>
     * https://www.copenhagen-ai.net
     * 
     * This software is licensed under the Apache License, Version 2.0 (the "LICENCE"); you may not use this file except in compliance with the License. 
     * You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES 
     * OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
     *
     * Additional Ethical Standards
     *
     * By using this software, you agree to adhere to the ethical standards as defined and maintained by the AWESOME.AI Group. These standards are outlined 
     * in the ETHICAL file included with the software distribution and can also be reviewed at the following URL: https://www.copenhagen-ai.net/docs/ethical.txt

     * These ethical standards are intended to ensure that the software is used in ways that align with principles of fairness, safety, accountability, and 
     * respect for human rights. By accepting this license, you commit to upholding these principles in your use, modification, and redistribution of the software.

     * Future Modifications to the License

     * The AWESOME.AI Group reserves the right to modify the formulation of this license in the future. Any such modifications will be communicated through 
     * updates to the LICENSE file accompanying the software and published at the URL mentioned above.

     * Acknowledgment

     * By using this software, you acknowledge that:
     * You have reviewed and accepted the ethical standards established by the AWESOME.AI Group.
     * You understand that failure to comply with these ethical standards may result in the revocation of your rights under this license.

     * If you have any questions or concerns about this license or the ethical standards, please contact the AWESOME.AI Group through the channels provided in 
     * the accompanying documentation.
     * */


    /*
     * Notes:
     * Maybe its not about creating a replica of the brain, but building upon the basics ie. TheMatrix, Mechanics etc
     * Maybe the question should be: "What can be done with this setup"
     * */
    public class TheMind
    {
        public TheMatrix matrix;
        public Core core;
        public TheCurve curve;
        public Memory mem;
        public Params parms;
        public Calc calc;
        public Process process;
        public Filters filters;
        public Out _out;
        public MyInternal _internal;
        public MyExternal _external;
        public Common.Common common;
                
        public HUB curr_hub;
        public UNIT curr_unit;
        public UNIT theanswer;

        public MINDS mindtype;
        public MECHANICS mech;
        public THECHOISE goodbye = THECHOISE.NO;

        private bool do_process = false;
                
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public int correct_thinking = 0;
        public int not_correct_thinking = 0;
        public int _near_death = 0;
        public double pain = 0.0f;
        public int valid_units = 0;

        //public bool theme_on = false;
        //public string theme = "none";
        //public string theme_old = "";
        public string hobby = "socializing";

        public List<KeyValuePair<string, int>> themes_stat = new List<KeyValuePair<string, int>>();
        public Stats stats = new Stats();
        
        public TheMind(MECHANICS mech, MINDS mindtype)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;

                this.mech = mech;
                this.mindtype = mindtype;

                parms = new Params(this, mech);
                common = new Common.Common(this);
                matrix = new TheMatrix(this);
                calc = new Calc(this);
                process = new Process(this);
                _internal = new MyInternal(this);
                _external = new MyExternal(this);
                filters = new Filters(this);
                core = new Core(this);
                curve = new TheCurve(this);
                _out = new Out(this);

                mem = new Memory(this, parms.number_of_units);

                parms.UpdateLowCut();

                //if (mindtype == MINDS.STANDARD)
                //    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_love1").FirstOrDefault();
                if (mindtype == MINDS.ANDREW)
                    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_fembots1").FirstOrDefault();
                if (mindtype == MINDS.ROBERTA)
                    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_macho machines1").FirstOrDefault();

                curr_hub = curr_unit.HUB;

                PreRun(true);
                PostRun(true);

                theanswer = UNIT.Create(this, -1.0d, "I dont Know", "null", "SPECIAL", TYPE.JUSTAUNIT);//set it to "It does not", and the program terminates
            
                ProcessPass();
                        
                Lists();
            }
            catch (Exception _e)
            {
                string msg = "themind - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);                
            }
        }
        
        private void Lists()
        {
            List<UNIT> list = mem.UNITS_VAL();

            List<Tuple<string, bool, double>> units_force = new List<Tuple<string, bool, double>>();
            foreach (UNIT u in list.OrderBy(x => x.Variable).ToList())
                units_force.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.Variable));

            List<Tuple<string, bool, double>> units_mass = new List<Tuple<string, bool, double>>();
            foreach (UNIT u in list.OrderBy(x => x.HighAtZero).ToList())
                units_mass.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.HighAtZero));

            Dictionary<string, double> units_dist = new Dictionary<string, double>();
            foreach (UNIT u in list.OrderBy(x => x.LengthFromZero).ToList())
                units_dist.Add(u.root, u.LengthFromZero);

            List<UNIT> list1 = list.OrderBy(x => x.Index).ToList();
            List<UNIT> list2 = list.OrderBy(x => x.Variable).ToList();
            List<UNIT> list3 = list.Where(x => !filters.LowCut(x)).OrderBy(x => x.Variable).ToList();

            valid_units = units_force.Count;

            ;
        }

        //public void StatClear()
        //{
        //    epochs = 1;
        //    stats = new Stats();
        //    themes_stat = new List<KeyValuePair<string, int>>();
        //    themes_stat.Add(new KeyValuePair<string, int>("none", 1));
        //}

        public bool run = true;
        public bool ok = true;
        public void Run(object sender, MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            try
            {                
                if (!ok)
                    return;
                
                if (!run)
                    return;

                run = false;

                Lists();

                if (do_process)
                    epochs++;

                bool _pro = do_process;
                do_process = false;
                
                PreRun(_pro);

                if (!Core(_pro))//the basics
                    ok = false;

                TheSoup();//find new curr_unit/curr_hub
                PostRun(_pro);
                Process(_pro);
                Systems(_pro);
                //Output(_pro);

                if (_pro) _out.Set();
                if (_pro) cycles = 0;   

                run = true;
            }
            catch (Exception _e) 
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);
            }
        }

        private void PreRun(bool _pro)
        {
            if (!_pro)
                return;

            common.Reset();            
        }

        private void PostRun(bool _pro)
        {
            if (!_pro)
                return;

            _internal.Reset();
            _external.Reset();
        }

        private bool Core(bool _pro)
        {
            /*
             * This is the algorithm for producing thought/making the choise
             * - maybe Core() + TheSoup() could be made into at neural net all by it self, since "almost all" it does is choosing up or down 
             * */

            cycles++;
            cycles_all++;

            IMechanics mech = parms.GetMechanics(MECHANICS.NONE);

            core.UpdateCredit();
            core.AnswerQuestion();
            
            if (curr_unit.IsIDLE())
                return true;

            mech.Calculate();
            mech.Position();

            //if (curr_hub.IsIDLE())
            //    core.SetTheme(_pro);
            
            if (!curve.OK(out pain))
                return false;
            return true;
        }

        private void TheSoup() 
        {
            curr_hub = curr_unit.HUB;
            curr_unit = matrix.NextUnit(curr_unit, parms._mech.dir);
        }

        private void Process(bool _pro)
        {
            process.History(this);
            process.CommonUnit(this);
            process.Stats(this, _pro);
        }

        private void Systems(bool _pro)
        {
            
        }

        //public string output_topic = "";
        //public string output_sub_th = "";
        //private void Output(bool _pro)
        //{
        //    if (curr_unit.root.Split(':')[0] == "subject")
        //        return;

        //    output_topic = process.StreamTop().HUB.GetSubject();

        //    output_sub_th = "currently subprocessing:\t" + curr_unit.HUB.GetSubject() + "\n" +
        //                    "- actual:\t\t\t" + curr_unit.root + "";

        //    if (_pro)
        //        _out.Set();
            
        //    if (_pro)
        //        cycles = 0;           
        //}

        private async void ProcessPass()
        {
            while (true)
            {
                do_process = true;
                await Task.Delay(2000);
            }
        }                
    }
}
