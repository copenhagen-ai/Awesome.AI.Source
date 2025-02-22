using Awesome.AI.Core;
using Microsoft.Quantum.Simulation.Simulators;
using QuantumApp;

namespace Awesome.AI.Common
{
    public class MyQuantum
    {
        private TheMind mind;
        private MyQuantum() { }
        public MyQuantum(TheMind mind)
        {
            this.mind = mind;
        }

        public async Task<bool> Superposition()
        {
            using (var simulator = new QuantumSimulator())  // Quantum simulator
            {
                var result = await QuantumSuperposition.Run(simulator);
                
                Console.WriteLine($"Qubit measurement result: {result}");

                bool res = "" + result != "Zero";

                return res;
            }
        }

        public async Task<bool> XOR(bool _a, bool _b)
        {
            using (var simulator = new QuantumSimulator())  // Quantum simulator
            {
                var result = await QuantumXOR.Run(simulator, _a, _b);

                Console.WriteLine($"Qubit measurement result: {result}");

                bool res = "" + result != "Zero";

                return res;
            }
        }
    }
}
