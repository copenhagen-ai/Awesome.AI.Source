using Awesome.AI.Core;
using Microsoft.Quantum.Simulation.Core;

public class MyQubit
{
    private Complex alpha;
    private Complex beta;
    private static Random random = new Random();

    private TheMind mind;
    private MyQubit() { }
    public MyQubit(TheMind mind)
    {
        this.mind = mind;
        // Initialize to |0> state
        alpha = new Complex(1, 0);
        beta = new Complex(0, 0);
    }

    public async Task<bool> MySuperposition()
    {
        MyQubit qubit1 = new MyQubit();

        qubit1.ApplySuperposition();
        
        int measurement1 = qubit1.Measure();

        //MyQubit qubit1 = new MyQubit();

        //Console.WriteLine("Initial State:");
        //Console.WriteLine($"Qubit 1: {qubit1}");

        // Apply Hadamard to put qubit1 into superposition
        //qubit1.ApplySuperposition();
        //Console.WriteLine("\nAfter Superposition:");
        //Console.WriteLine($"Qubit 1: {qubit1}");

        // Measure both qubits
        //int measurement1 = qubit1.Measure();
        //Console.WriteLine($"\nMeasurement results: Qubit 1 = {measurement1}");

        return measurement1 > 0;
    }

    public async Task<bool> MyXOR(bool _a, bool _b)
    {
        MyQubit qubit1 = new MyQubit();
        MyQubit qubit2 = new MyQubit();

        qubit1.ApplySuperposition();
        qubit1.ApplyXOR(qubit2);
        
        int measurement1 = qubit1.Measure();
        int measurement2 = qubit2.Measure();

        //MyQubit qubit1 = new MyQubit();
        //MyQubit qubit2 = new MyQubit();

        //Console.WriteLine("Initial State:");
        //Console.WriteLine($"Qubit 1: {qubit1}");
        //Console.WriteLine($"Qubit 2: {qubit2}");

        // Apply Hadamard to put qubit1 into superposition
        //qubit1.ApplySuperposition();
        //Console.WriteLine("\nAfter Superposition:");
        //Console.WriteLine($"Qubit 1: {qubit1}");

        // Apply XOR gate
        //qubit1.ApplyXOR(qubit2);
        //Console.WriteLine("\nAfter XOR with Qubit 2:");
        //Console.WriteLine($"Qubit 1: {qubit1}");

        // Measure both qubits
        //int measurement1 = qubit1.Measure();
        //int measurement2 = qubit2.Measure();
        //Console.WriteLine($"\nMeasurement results: Qubit 1 = {measurement1}, Qubit 2 = {measurement2}");

        return measurement1 > 0;
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
        // Swaps alpha and beta
        var temp = alpha;
        alpha = beta;
        beta = temp;
    }

    public void ApplyPauliY()
    {
        // Multiplies beta by i and alpha by -i and swaps
        var temp = alpha;
        alpha = Complex.Multiply(new Complex(0, -1), beta);
        beta = Complex.Multiply(new Complex(0, 1), temp);
    }

    public void ApplyPauliZ()
    {
        // Multiplies beta by -1
        beta = Complex.Negate(beta);
    }

    public void ApplyAND(MyQubit other)
    {
        alpha = Complex.Multiply(alpha, other.alpha);
        beta = new Complex(0, 0);
    }

    public void ApplyOR(MyQubit other)
    {
        alpha = Complex.Subtract(Complex.Add(alpha, other.alpha), Complex.Multiply(alpha, other.alpha));
        beta = Complex.Subtract(new Complex(1, 0), alpha);
    }

    public void ApplyXOR(MyQubit other)
    {
        Complex tempAlpha = Complex.Add(Complex.Multiply(alpha, other.beta), Complex.Multiply(beta, other.alpha));
        Complex tempBeta = Complex.Add(Complex.Multiply(alpha, other.alpha), Complex.Multiply(beta, other.beta));
        alpha = tempAlpha;
        beta = tempBeta;
    }

    public void ApplySuperposition()
    {
        ApplyHadamard();
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
