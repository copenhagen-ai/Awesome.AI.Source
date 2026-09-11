import random
import math

from awesome_ai.Variables.Constants import CONST


class MyRandom:
    def __init__(self, mind=None, seed=None):
        self.mind = mind
        self.random = random.Random(seed)
        self.saves = [self.RandomDouble(0.0, 1.0) for _ in range(450)]
        self.delta_velocities = self.saves  # compatibility name used by early Python callers
        self.shift_a = 0
        self.shift_b = 0

    def SaveDeltaVel(self, dv):
        dv = float(dv)
        if math.isnan(dv) or math.isinf(dv):
            raise RuntimeError("SaveMomentum")
        if self.mind is not None and self.mind.cycles_all < CONST.FIRST_RUN:
            dv = self.saves[self.mind.cycles_all]
        if dv == 0.0 or dv in self.saves:
            return
        self.saves.append(dv)
        if len(self.saves) > 500:
            self.saves.pop(0)

    def Rand(self, index):
        if index + 1 >= len(self.saves):
            raise RuntimeError("Rand")
        first = str(self.saves[index])
        second = str(self.saves[index + 1])
        if len(first) < 10:
            first = second
        first = first.split("e", 1)[0].split("E", 1)[0]
        return "".join(character for character in reversed(first) if character.isdigit())

    def MyRandomDouble(self, count):
        try:
            result = []
            for index in range(count):
                digits = self.Rand(index + self.shift_b)
                result.append(float(f"0.{digits[:10]}"))
            self.shift_b = (self.shift_b + 1) % 100
            return result
        except Exception as error:
            raise RuntimeError("MyRandomDouble") from error

    def MyRandomInt(self, count, i_max):
        if i_max > 9999:
            raise RuntimeError("MyRandomInt")
        try:
            result = []
            for index in range(count):
                digits = self.Rand(index + self.shift_a)
                try:
                    if len(digits) < 4:
                        raise IndexError("MyRandomInt requires four digits")
                    decimal = int(digits[0] + digits[1] + digits[2] + digits[3]) / 10000.0
                    result.append(int((i_max + 1) * decimal))
                except (ValueError, IndexError):
                    result.append(0)
            self.shift_a = (self.shift_a + 1) % 100
            return result
        except Exception as error:
            raise RuntimeError("MyRandomInt") from error

    def RandomInt(self, *args): return self.random.randrange(*args)
    def RandomDouble(self, minimum, maximum): return self.random.random() * (maximum - minimum) + minimum
    def Next(self, minimum=0, maximum=None):
        if maximum is None: minimum, maximum = 0, minimum
        return self.random.randrange(minimum, maximum)
    def NextDouble(self): return self.random.random()
