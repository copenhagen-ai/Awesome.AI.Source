from abc import ABC, abstractmethod


class IMechanics(ABC):
    @abstractmethod
    def PosXY(self) -> float: ...

    @abstractmethod
    def Calc(self, curr, cycles: int) -> None: ...

    @abstractmethod
    def Calculate(self, match, cycles: int) -> None: ...

