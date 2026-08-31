from abc import ABC, abstractmethod
import ast
import operator
import re


class IMathStrategy(ABC):
    @abstractmethod
    def CanHandle(self, input): ...
    @abstractmethod
    def Solve(self, input): ...


class ExampleMemory:
    def __init__(self): self.examples = {}
    def Learn(self, problem): self.examples[problem] = problem
    def TrySolve(self, problem): return self.examples.get(problem)


class LinearEquationStrategy(IMathStrategy):
    _pattern = re.compile(r"^\s*([+-]?\d*\.?\d*)x\s*([+-]\s*\d+(?:\.\d+)?)?\s*=\s*([+-]?\d+(?:\.\d+)?)\s*$")
    def CanHandle(self, input): return bool(self._pattern.match(input))
    def Solve(self, input):
        match = self._pattern.match(input)
        if not match: raise ValueError("Unsupported linear equation")
        raw_a, raw_b, raw_c = match.groups()
        a = -1.0 if raw_a == "-" else 1.0 if raw_a in ("", "+") else float(raw_a)
        b = float((raw_b or "0").replace(" ", "")); c = float(raw_c)
        if a == 0: raise ValueError("Equation has no unique solution")
        return str((c - b) / a)


class SolveExpressionStrategy(IMathStrategy):
    _ops = {ast.Add: operator.add, ast.Sub: operator.sub, ast.Mult: operator.mul,
            ast.Div: operator.truediv, ast.Pow: operator.pow, ast.USub: operator.neg,
            ast.UAdd: operator.pos}
    def CanHandle(self, expression):
        try: self._eval(ast.parse(expression, mode="eval").body); return True
        except (SyntaxError, TypeError, ValueError, ZeroDivisionError): return False
    def Solve(self, expression): return str(self._eval(ast.parse(expression, mode="eval").body))
    def _eval(self, node):
        if isinstance(node, ast.Constant) and isinstance(node.value, (int, float)): return node.value
        if isinstance(node, ast.BinOp) and type(node.op) in self._ops: return self._ops[type(node.op)](self._eval(node.left), self._eval(node.right))
        if isinstance(node, ast.UnaryOp) and type(node.op) in self._ops: return self._ops[type(node.op)](self._eval(node.operand))
        raise ValueError("Unsafe or unsupported expression")


class GoalMath:
    def __init__(self, mind=None):
        self.mind = mind
        self.mem = ExampleMemory()
        self.strategies = [LinearEquationStrategy(), SolveExpressionStrategy()]
        self.problems = ["calc: 5 * (3 + 2)", "solve: 2x + 4 = 10", "calc: 3 * (3 + 2)", "solve: 2x + 2 = 10", "calc: 2 * (3 + 2)", "solve: 2x + 6 = 10"]
        if mind is not None: mind.result_math = "nothing yet.."
    def GetProblem(self, index):
        if index == -1 and self.mind is not None: index = self.mind.rand.MyRandomInt(1, 59)[0] // 10
        return self.problems[index]
    def AddStrategy(self, strategy): self.strategies.append(strategy)
    def Solve(self, problem, _pro=True):
        prefix, _, body = problem.partition(":")
        candidate = body.strip() if body else problem
        if self.mind is not None and (not _pro or not self.mind._quick.Result("MATHSOLVE")): return None
        remembered = self.mem.TrySolve(problem)
        if remembered is not None: return remembered
        for strategy in self.strategies:
            if strategy.CanHandle(candidate):
                result = strategy.Solve(candidate)
                if self.mind is not None: self.mind.result_math = result
                return result
        if self.mind is not None: self.mind.result_math = "i don't know how to solve this yet."; return None
        raise ValueError(f"No strategy can solve: {problem}")
    def Learn(self, problem, _pro=True):
        if self.mind is not None and (not _pro or not self.mind._quick.Result("MATHLEARN")): return
        prefix = problem.partition(":")[0]
        self.mem.examples[problem] = prefix
        if prefix == "calc": self.AddStrategy(SolveExpressionStrategy())
        elif prefix == "solve": self.AddStrategy(LinearEquationStrategy())
    def Knows(self, strategy): return any(isinstance(item, type(strategy)) for item in self.strategies)
