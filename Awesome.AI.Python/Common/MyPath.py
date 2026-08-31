from pathlib import Path
import os


class MyPath:
    # Python package root equivalent to the C# DEBUG Root property.
    Root = str(Path(__file__).resolve().parents[2]) + os.sep

    @staticmethod
    def GetPath(*parts): return str(Path(__file__).resolve().parents[2].joinpath(*parts))
