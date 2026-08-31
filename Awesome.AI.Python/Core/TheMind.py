"""Behavioral port of ``Awesome.AI.CS/Core/TheMind.cs``.

ARC and the C# micro-timer/process-pass path are deliberately not part of the
Python mind.  A processing pass is instead the established ``cycles_all & 50``
cadence used by the simulation service.
"""
from __future__ import annotations
import asyncio
import random
from awesome_ai.Common import GPTProbability, MyCalc, MyOutJson, MyOutVars, MyRandom, MyStats, MyMeters
from awesome_ai.Core.Internals.Filters import Filters
from awesome_ai.Core.Internals.Operators import Down
from awesome_ai.Core.Spaces.HubSpace import HubSpace
from awesome_ai.Core.Spaces.Unit import UNIT
from awesome_ai.Core.Spaces.USAccess import USAccess
from awesome_ai.Core.Spaces.USSetup import USSetup
from awesome_ai.Core.Spaces.USSoup import USSoup
from awesome_ai.CoreSystems.Environment import MyExternal, MyInternal
from awesome_ai.CoreSystems.GoalMath import GoalMath
from awesome_ai.CoreSystems.LongDecision import LongDecision
from awesome_ai.CoreSystems.Monologue1 import Monologue1
from awesome_ai.CoreSystems.Monologue2 import Monologue2
from awesome_ai.CoreSystems.Mood import Mood
from awesome_ai.CoreSystems.QuickDecision import QuickDecision
from awesome_ai.CoreSystems.Whistle import Whistle
from awesome_ai.Factorys.BotFactory import BotFactory
from awesome_ai.Factorys.MechFactory import MechFactory
from awesome_ai.Generators.WordGenerator import WordGenerator
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import ENV, LONGTYPE, MINDS, PATTERN, STATE, UNITTYPE


class TheMind:
    def __init__(self, mindtype: MINDS, env: ENV):
        self.mindtype, self.environment = mindtype, env
        self.json = ""
        self.result_whistle = ""
        self.result_math = ""
        self.stats, self.meters = MyStats(), MyMeters()
        self.theanswer = None
        self.q_u_whistle = None
        self.q_u_mathlearn = None
        self.q_u_mathsolve = None
        self.goodbye = self.chat_answer = self.chat_asked = self.reward = False
        self.ok = True
        self.epochs, self.cycles, self.cycles_all = 1, 0, 0
        self.pain_truth_something = 0.0
        self.STATE = STATE.JUSTRUNNING
        self.unit_corridor, self.unit_current, self.unit_actual = [], None, None
        self.count, self._pro = 0, False
        self.bot = BotFactory(self).GetBot()
        self.lng_dec = self.bot.lng_dec
        self.mech = MechFactory(self).GetMech(self.bot.mech_low, self.bot.props)
        self.down = Down(self)
        # MyCalc depends on MyRandom through the mind, but C# constructs calc first.
        self.calc, self.rand = MyCalc(self), MyRandom(self)
        self._internal, self._external = MyInternal(self), MyExternal(self)
        self.filters = Filters(self)
        self.o_vars, self.o_json = MyOutVars(self), MyOutJson(self)
        self._long = LongDecision(self, self.lng_dec)
        self._quick = QuickDecision(self)
        self.mood, self.word = Mood(self), WordGenerator(self)
        self.mono1, self.mono2 = Monologue1(self), Monologue2(self)
        self.prob, self.soup = GPTProbability(), USSoup(self)
        self.memory, self.access = USSetup(self), USAccess(self)
        self.hub, self.whistle = HubSpace(self), Whistle(self)
        self.g_math = GoalMath(self)
        # The corresponding C# ARC quick units are intentionally omitted.
        self.q_u_whistle = UNIT.CreateQuick(self, "WHISTLE", [50, 50, 50, 50, 50, 50])
        self.q_u_mathlearn = UNIT.CreateQuick(self, "MATHLEARN", [51, 50, 50, 50, 50, 50])
        self.q_u_mathsolve = UNIT.CreateQuick(self, "MATHSOLVE", [52, 50, 50, 50, 50, 50])
        all_units = self.access.UNITS_ALL()
        if not all_units: raise RuntimeError("USSetup created no running units")
        picks = [random.randrange(len(all_units)) for _ in range(3)]
        from awesome_ai.Core.Core import Core
        self.core = Core(self, *picks)
        self.unit_current = all_units[len(all_units) // 2]
        self.Pre(True); self.Post(True)
        self.theanswer = UNIT.Create(self, "GUID", [-1] * 6, "I dont Know", "SPECIAL", UNITTYPE.JUSTAUNIT, LONGTYPE.NONE)
        self.ok = True

    def SetAccess(self, _pro):
        self.count = 0 if _pro else self.count + 1

    def HasAccess(self, access):
        return self.count == access

    async def Run(self):
        while self.ok:
            self.Cycle()
            await asyncio.sleep(CONST.AGENT_DELAY_MS / 1000)

    def Cycle(self, *args):
        try:
            self.cycles += 1
            self.cycles_all += 1
            if not self.ok:
                return

            # Replaces the removed C# do_process/ProcessPass timer.
            self._pro = (self.cycles_all & 50) == 0
            if self._pro:
                self.epochs += 1

            self.SetAccess(self._pro)
            self.Pre(self._pro)
            if not self.Core(self._pro):
                self.ok = False

            self.CorePost(self._pro)
            # Systems remains intentionally disabled for the simulation path.
            # self.Systems(self._pro)
            self.Post(self._pro)
            if self._pro:
                self.cycles = 0
        except Exception:
            self.ok = False

    def Pre(self, _pro):
        self.rand.SaveDeltaVel(self.mech.ms.dv_sym_curr)
        if not _pro:
            return
        self._internal.Reset()
        self._external.Reset()

    def Post(self, _pro):
        self.o_vars.SetOut()
        self.json = self.o_json.GetJson(_pro)

    def Core(self, _pro):
        self.core.UpdateCredit(); self.core.StopCondition()
        if self.unit_current.IsIDLE(): return True
        for pattern in (PATTERN.MOODGENERAL, PATTERN.MOODGOOD, PATTERN.MOODBAD): self.mech.Calculate(pattern, self.cycles)
        self.down.Modify(); self.mech.mp.mprops.Update()
        result, pain = self.core.OK(self.pain_truth_something)
        self.pain_truth_something = pain
        return result

    def CorePost(self, _pro):
        self.soup.CurrentUnit(_pro)
        self.core.History()
        self.core.ActualUnit(_pro)
        self.core.Stats(_pro)

    def Systems(self, _pro):
        quick_names = ("QYES", "QNO", "WHISTLE", "MATHLEARN", "MATHSOLVE")
        if self.unit_current.Data in quick_names:
            self._quick.Decide(self.unit_current, self.unit_current.Data)
        self.whistle.Do(_pro)
        self.g_math.Learn(self.g_math.GetProblem(-1), _pro)
        self.g_math.Solve(self.g_math.GetProblem(-1), _pro)
        if self.STATE == STATE.QUICKDECISION: return
        for decision_type in self.lng_dec: self._long.Decide(_pro, decision_type)
        self.mood.Generate(_pro); self.mono1.Create(_pro); self.mono2.Create(_pro)
