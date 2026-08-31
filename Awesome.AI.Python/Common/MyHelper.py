import sys


class MyHelper:
    @staticmethod
    def IsDebug(): return sys.gettrace() is not None

