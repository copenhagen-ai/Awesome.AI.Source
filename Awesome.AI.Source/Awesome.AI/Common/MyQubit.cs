﻿namespace Awesome.AI.Common
{
    public class MyQubit
    {
        private Complex alpha;
        private Complex beta;
        private Random random = new Random();

        public QUsage usage;

        public MyQubit()
        {
            usage = new QUsage();

            // Initialize to |0> state
            alpha = new Complex(1, 0);
            beta = new Complex(0, 0);
        }
    
        public void ApplyHadamard()
        {
            Complex newAlpha = Complex.Divide(Complex.Add(alpha, beta), Math.Sqrt(2));
            Complex newBeta = Complex.Divide(Complex.Subtract(alpha, beta), Math.Sqrt(2));
            alpha = newAlpha;
            beta = newBeta;
        }

        public void ApplyPauliX()
        {
            var temp = alpha;
            alpha = beta;
            beta = temp;
        }

        public void ApplyCNOT(MyQubit control)
        {
            Complex newAlpha = alpha;
            Complex newBeta = Complex.Add(Complex.Multiply(beta, control.alpha.MagnitudeSquared()), Complex.Multiply(alpha, control.beta.MagnitudeSquared()));
            alpha = newAlpha;
            beta = newBeta;
        }

        public void ApplyToffoli(MyQubit control1, MyQubit control2)
        {
            if (control1.alpha.MagnitudeSquared() < 0.5 && control2.alpha.MagnitudeSquared() < 0.5)
            {
                ApplyPauliX();
            }
        }

        public void ApplySuperposition()
        {
            ApplyHadamard();
        }

        public void ApplyAND(MyQubit control1, MyQubit control2)
        {
            ApplyToffoli(control1, control2);
        }

        public void ApplyOR(MyQubit control1, MyQubit control2)
        {
            control1.ApplyPauliX();
            control2.ApplyPauliX();
            ApplyToffoli(control1, control2);
            ApplyPauliX();
            control1.ApplyPauliX();
            control2.ApplyPauliX();
        }

        public void ApplyXOR(MyQubit control)
        {
            ApplyCNOT(control);
        }

        public int Measure()
        {
            double probabilityZero = alpha.MagnitudeSquared();
            double randValue = random.NextDouble();
            return randValue < probabilityZero ? 0 : 1;
        }

        public override string ToString()
        {
            return $"|ψ> = ({alpha})|0> + ({beta})|1>";
        }

        public class Entanglement
        {
            // |Φ+⟩ = (|00⟩ + |11⟩) / sqrt(2) - Always the same result
            public static (MyQubit, MyQubit) CreatePhiPlus()
            {
                MyQubit qubit1 = new MyQubit();
                MyQubit qubit2 = new MyQubit();
                qubit1.ApplyHadamard();
                qubit2.ApplyCNOT(qubit1);
                return (qubit1, qubit2);
            }

            // |Φ-⟩ = (|00⟩ - |11⟩) / sqrt(2) - Always the same result, but with a phase difference
            public static (MyQubit, MyQubit) CreatePhiMinus()
            {
                var (q1, q2) = CreatePhiPlus();
                q2.ApplyPauliX(); // Apply Pauli-X to introduce the phase difference
                return (q1, q2);
            }

            // |Ψ+⟩ = (|01⟩ + |10⟩) / sqrt(2) - Always opposite results
            public static (MyQubit, MyQubit) CreatePsiPlus()
            {
                MyQubit qubit1 = new MyQubit();
                MyQubit qubit2 = new MyQubit();
                qubit1.ApplyHadamard();
                qubit2.ApplyPauliX(); // Flip second qubit before entangling
                qubit2.ApplyCNOT(qubit1);
                return (qubit1, qubit2);
            }

            // |Ψ-⟩ = (|01⟩ - |10⟩) / sqrt(2) - Always opposite results, with phase difference
            public static (MyQubit, MyQubit) CreatePsiMinus()
            {
                var (q1, q2) = CreatePsiPlus();
                q2.ApplyPauliX(); // Apply Pauli-X to introduce phase difference
                return (q1, q2);
            }
        }
    }

    public struct Complex
    {
        public double Real { get; }
        public double Imaginary { get; }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public static Complex Add(Complex a, Complex b) =>
            new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);

        public static Complex Subtract(Complex a, Complex b) =>
            new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);

        public static Complex Multiply(Complex a, double scalar) =>
            new Complex(a.Real * scalar, a.Imaginary * scalar);

        public static Complex Multiply(Complex a, Complex b) =>
            new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Real * b.Imaginary + a.Imaginary * b.Real);

        public static Complex Divide(Complex a, double scalar) =>
            new Complex(a.Real / scalar, a.Imaginary / scalar);

        public static Complex Negate(Complex a) =>
            new Complex(-a.Real, -a.Imaginary);

        public double MagnitudeSquared() => Real * Real + Imaginary * Imaginary;

        public override string ToString() => $"{Real} + {Imaginary}i";
    }

    // Example Usage
    public class QUsage
    {
        public QUsage()
        {
            //var superpositionQubit = MySuperposition();
            //Console.WriteLine("Superposition Qubit: " + superpositionQubit);

            //var xorResult = MyXOR(true, false);
            //Console.WriteLine("XOR Result (true XOR false): " + xorResult);

            //var (phiPlus1, phiPlus2) = MyQubit.Entanglement.CreatePhiPlus();
            //Console.WriteLine("|Φ+⟩ State:");
            //Console.WriteLine("Qubit 1: " + phiPlus1);
            //Console.WriteLine("Qubit 2: " + phiPlus2);
            //Console.WriteLine("Measurement: " + phiPlus1.Measure() + " " + phiPlus2.Measure());

            //var (psiPlus1, psiPlus2) = MyQubit.Entanglement.CreatePsiPlus();
            //Console.WriteLine("\n|Ψ+⟩ State:");
            //Console.WriteLine("Qubit 1: " + psiPlus1);
            //Console.WriteLine("Qubit 2: " + psiPlus2);
            //Console.WriteLine("Measurement: " + psiPlus1.Measure() + " " + psiPlus2.Measure());
        }

        public (MyQubit, MyQubit) CreateEntangledPair()
        {
            MyQubit qubitA = new MyQubit();
            MyQubit qubitB = new MyQubit();
            qubitA.ApplyHadamard();
            qubitB.ApplyCNOT(qubitA);
            return (qubitA, qubitB);
        }


        public bool MySuperposition()
        {
            MyQubit qubit = new MyQubit();

            qubit.ApplySuperposition();

            int measurement1 = qubit.Measure();

            return measurement1 > 0;
        }

        public bool MyXOR(bool a, bool b)
        {
            MyQubit qubitA = new MyQubit();
            MyQubit qubitB = new MyQubit();

            if (a) qubitA.ApplyPauliX(); // Set to |1> if a is true
            if (b) qubitB.ApplyPauliX(); // Set to |1> if b is true

            //qubitA.ApplySuperposition();  // Superposition: (|0⟩ + |1⟩)/√2

            qubitA.ApplyXOR(qubitB);

            int measurement1 = qubitB.Measure();

            return measurement1 > 0;
        }

        // Quantum XOR using entangled qubits, unconventional
        public bool DontQuantumXOR(bool a, bool b)
        {
            var (qubitA, qubitB) = CreateEntangledPair();

            // Set initial states based on inputs
            if (a) qubitA.ApplyPauliX();
            if (b) qubitB.ApplyPauliX();

            // Measurement collapses the state, but entanglement ensures XOR behavior
            int measurementA = qubitA.Measure();
            int measurementB = qubitB.Measure();

            // XOR logic: a ⊕ b
            return measurementA != measurementB;
        }

        // Quantum XOR using entangled qubits, correct
        public bool DoQuantumXOR(bool a, bool b)
        {
            MyQubit control = new MyQubit();
            MyQubit target = new MyQubit();

            if (a) control.ApplyPauliX();
            if (b) target.ApplyPauliX();

            control.ApplyCNOT(target);

            int result = target.Measure();
            return result > 0;
        }
    }
}