﻿using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
{
    public class Position
    {
        private bool is_no { get; set; }
        
        private TheMind mind;
        private Position() { }

        public Position(TheMind mind)
        {
            this.mind = mind;
        }

        private double up = 0.1d;
        private double down = 0.1d;
        private double pos = 5.0d;
        private double pos_prev = 0.0d;
        public MOOD Mood { get; set; }
        public double Pos { get { return pos == 0.0d ? Constants.VERY_LOW : pos; } }

        public void Update(bool _pro)
        {
            if (!_pro)
                return;

            Schedule();

            pos += is_no ? up : -down;
            up = Up();
            down += Down() / 2;

            Mood = pos > pos_prev ? MOOD.GOOD : MOOD.BAD;

            if (pos <= 0.0d)
                pos = 0.0d;

            if (pos >= 10.0d)
                pos = 10.0d;

            pos_prev = pos;
        }

        private double Up()
        {
            double rand = (mind.rand.MyRandomDouble(2)[0] / 10.0d) * 2.0d;

            return rand;
        }

        private double Down()
        {
            double rand = (mind.rand.MyRandomDouble(2)[1] / 10.0d) * 2.0d;

            return rand;
        }
        
        private void Schedule()
        {
            if (mind.goodbye.IsYes()) {
                is_no = false;
                return;
            }

            if (pos < mind.parms.schedule_low) {
                if (!is_no)
                    is_no = true;
                return;
            }

            if (pos < mind.parms.schedule_mid) {
                if (!is_no)
                    is_no = mind.dir.Choise.IsNo();
                return;
            }

            if (pos >= mind.parms.schedule_high) {
                is_no = false;
                down = 0.1d;
                return;
            }
        }        
    }
}
