using Awesome.AI.Core;

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

        qubitA.ApplyXOR(qubitB);

        int measurement1 = qubitA.Measure();

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
