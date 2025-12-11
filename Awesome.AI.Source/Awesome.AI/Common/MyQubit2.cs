using Awesome.AI.Core;

namespace Awesome.AI.Common
{
    // Simple Complex struct (keeps the earlier API, with a small addition)
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

        public static readonly Complex One = new Complex(1.0, 0.0);
        public static readonly Complex Zero = new Complex(0.0, 0.0);
    }

    // A proper small state-vector based quantum register
    public class QuantumRegister
    {
        private Complex[] state;      // amplitude vector length = 2^nQubits
        public int Qubits { get; }
        private Random random;

        public QuantumRegister(int nQubits, Random rand)
        {
            if (nQubits <= 0) throw new ArgumentException(nameof(nQubits));
            Qubits = nQubits;
            state = new Complex[1 << Qubits];
            // initialize to |0...0>
            state[0] = Complex.One;
            for (int i = 1; i < state.Length; ++i) state[i] = Complex.Zero;
            random = rand;
        }

        // Normalizes the whole state vector
        private void Normalize()
        {
            double sum = 0.0;
            for (int i = 0; i < state.Length; ++i) sum += state[i].MagnitudeSquared();
            if (sum == 0.0) throw new InvalidOperationException("State norm is zero.");
            double inv = 1.0 / Math.Sqrt(sum);
            for (int i = 0; i < state.Length; ++i)
                state[i] = Complex.Multiply(state[i], inv);
        }

        // Apply single-qubit Pauli-X (bit flip) to qubit 'target' (0-based)
        public void ApplyPauliX(int target)
        {
            ValidateQubit(target);
            int mask = 1 << target;
            // swap amplitudes for pairs differing in target bit
            for (int i = 0; i < state.Length; ++i)
            {
                if ((i & mask) == 0)
                {
                    int j = i | mask;
                    // swap state[i] and state[j]
                    var tmp = state[i];
                    state[i] = state[j];
                    state[j] = tmp;
                }
            }
        }

        // Apply single-qubit Hadamard to qubit 'target'
        public void ApplyHadamard(int target)
        {
            ValidateQubit(target);
            int mask = 1 << target;
            double invSqrt2 = 1.0 / Math.Sqrt(2.0);

            // For each pair (i, i|mask) where target bit is 0
            for (int i = 0; i < state.Length; ++i)
            {
                if ((i & mask) == 0)
                {
                    int j = i | mask;
                    Complex a = state[i];
                    Complex b = state[j];
                    // new_i = (a + b)/sqrt(2)
                    // new_j = (a - b)/sqrt(2)
                    state[i] = Complex.Multiply(Complex.Add(a, b), invSqrt2);
                    state[j] = Complex.Multiply(Complex.Subtract(a, b), invSqrt2);
                }
            }
        }

        // Apply CNOT with given control and target qubit indices (0-based)
        public void ApplyCNOT(int control, int target)
        {
            ValidateQubit(control);
            ValidateQubit(target);
            if (control == target) throw new ArgumentException("Control and target must be different.");

            int controlMask = 1 << control;
            int targetMask = 1 << target;

            // We must flip the target bit for all basis indices where control bit == 1.
            // To avoid double-swapping, iterate only over indices where control=1 and target=0,
            // swapping with index with target=1 (index ^ targetMask).
            for (int i = 0; i < state.Length; ++i)
            {
                if ((i & controlMask) != 0 && (i & targetMask) == 0)
                {
                    int j = i ^ targetMask; // flip target bit
                    // swap state[i] and state[j]
                    var tmp = state[i];
                    state[i] = state[j];
                    state[j] = tmp;
                }
            }
        }

        // Measure a single qubit: returns 0 or 1, collapses and renormalizes the state.
        public int Measure(int target)
        {
            ValidateQubit(target);
            int mask = 1 << target;

            // compute probability of measuring 0 on 'target' (sum of |amplitude|^2 where bit=0)
            double p0 = 0.0;
            for (int i = 0; i < state.Length; ++i)
            {
                if ((i & mask) == 0) p0 += state[i].MagnitudeSquared();
            }

            double r = random.NextDouble();
            int outcome = (r < p0) ? 0 : 1;

            // collapse: zero out amplitudes inconsistent with outcome
            for (int i = 0; i < state.Length; ++i)
            {
                if (((i & mask) == 0 && outcome == 1) || ((i & mask) != 0 && outcome == 0))
                {
                    state[i] = Complex.Zero;
                }
            }

            // renormalize remaining amplitudes
            Normalize();

            return outcome;
        }

        // Utility for printing amplitudes (for small registers)
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < state.Length; ++i)
            {
                if (state[i].MagnitudeSquared() > 1e-12)
                {
                    string basis = Convert.ToString(i, 2).PadLeft(Qubits, '0');
                    sb.AppendLine($"{state[i]} |{basis}> (prob={state[i].MagnitudeSquared():F4})");
                }
            }
            return sb.ToString();
        }

        private void ValidateQubit(int q)
        {
            if (q < 0 || q >= Qubits) throw new ArgumentOutOfRangeException(nameof(q));
        }
    }

    // Example usage / test
    public class QUsage
    {
        /*
         * proof of concept
         * */

        private Random random;

        private TheMind mind;
        private QUsage() { }
        public QUsage(TheMind mind)
        {
            this.mind = mind;

            random = new Random(1234);
        }

        public int Run()
        {
            // Build 2-qubit register (qubit 0 is the least-significant bit in this implementation)
            QuantumRegister qr = new QuantumRegister(2, random);

            // Create Bell state: (|00> + |11>) / sqrt(2)
            qr.ApplyHadamard(0);     // H on qubit 0
            qr.ApplyCNOT(0, 1);      // CNOT control=0, target=1

            //Console.WriteLine("Amplitudes after H(0) then CNOT(0->1):");
            //Console.WriteLine(qr.ToString());

            // Measure both qubits (note: measuring one will collapse the other due to entanglement)
            int m0 = qr.Measure(0);
            int m1 = qr.Measure(1);
            //Console.WriteLine($"Measured: q0={m0}, q1={m1}");

            return m0;
        }
    }
}