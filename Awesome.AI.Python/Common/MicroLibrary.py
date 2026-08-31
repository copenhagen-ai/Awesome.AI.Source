import time


class MicroStopwatch:
    def __init__(self): self.started = time.perf_counter_ns()
    @property
    def ElapsedMicroseconds(self): return (time.perf_counter_ns() - self.started) // 1000
