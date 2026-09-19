# Awesome.AI

**An experimental, dynamics-based framework for thought simulation and autonomous cognitive control.**

[![License: Apache 2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![Languages](https://img.shields.io/badge/implementations-C%23%20%7C%20Python-3776AB)](#implementations)

Awesome.AI explores whether thought-like processes can emerge from a continuously evolving system governed by dynamics inspired by physics. Instead of treating cognition as a sequence of isolated prompts, the framework models competing candidates, context, internal state, feedback, and selection over time.

> This is an open research project and work in progress. It presents a hypothesis and an experimental architecture—not a complete model of cognition or a production-ready AI system.

## Why this project?

The project investigates a simple question: **can controlled dynamics organize stochastic internal states into stable, useful thought patterns?**

Its architecture centers on two abstractions:

- **UNIT** — a candidate thought, decision, action, or answer evaluated by the system.
- **HUB** — the context associated with a UNIT and used during evaluation.

At each cycle, the state of UNIT and HUB space evolves. Mechanics, filters, properties, internal state, and feedback influence which UNIT becomes current and which remains dominant over time. Higher-level systems—including mood, monologue, quick decisions, long decisions, and goal solving—are built on top of that core loop.

## Key ideas

- Dynamics-based thought simulation inspired by concepts such as force, gravity, velocity, and mass
- Continuous state evolution rather than one-shot inference
- Controlled randomness and emergent behavior
- Context-aware selection across UNIT and HUB spaces
- Feedback, memory, mood, and internal/external state
- Extensible mechanics and cognitive subsystems
- Parallel reference implementations in C# and Python

## Learn the framework

The conceptual white paper explains the motivation, mathematical framing, architecture, mechanics, feedback loop, and higher-level systems:

- [Read the illustrated white paper](docs/Dynamics_Based_Framework_Thought_Simulation.htm)
- [Download the PDF](docs/Dynamics-Based-Framework-for-Thought-Simulation.pdf)

## Implementations

### C# / .NET 9

The primary implementation is a .NET 9 class library in [`Awesome.AI.Source`](Awesome.AI.Source/). Its central lifecycle is coordinated by `TheMind`, with supporting modules for mechanics, UNIT/HUB spaces, state, decisions, mood, monologue, goals, and ARC tasks.

Build it with:

```bash
git clone https://github.com/copenhagen-ai/Awesome.AI.Source.git
cd Awesome.AI.Source
dotnet build Awesome.AI.Source.sln
```

### Python

[`Awesome.AI.Python`](Awesome.AI.Python/) contains a source-level Python port of the core algorithm. It mirrors the main C# architecture for experimentation and comparison; some C# features, including ARC and the micro-timer path, are intentionally omitted.

See the [Python implementation notes](Awesome.AI.Python/README.md) for its folder structure and current scope.

## Repository structure

```text
Awesome.AI.Source/
├── Awesome.AI.Source/       # C#/.NET implementation
│   └── Awesome.AI/
│       ├── Core/            # Main lifecycle, UNIT/HUB spaces, mechanics
│       ├── CoreSystems/     # Decisions, mood, monologue, goals, ARC
│       ├── Common/          # Math, vectors, probability, output utilities
│       ├── Factorys/        # Bot and mechanics factories
│       └── Variables/       # Constants and enums
├── Awesome.AI.Python/       # Python port of the framework
└── docs/                    # White paper and supporting images
```

## Project status

Awesome.AI is exploratory software intended for research, discussion, and prototyping. APIs and internal models may change as the hypothesis is tested and refined. The repository currently provides source implementations and documentation rather than a packaged application or hosted service.

## Contributing

Discussion, critique, experiments, and code contributions are welcome. Useful areas include:

- Reviewing the cognitive and mathematical assumptions
- Creating reproducible experiments and benchmarks
- Comparing the C# and Python implementations
- Improving tests, documentation, performance, and API design
- Exploring alternative mechanics, filters, and selection strategies

Please open an issue before starting a large change so the approach can be discussed.

## Ethics

Please review the project's [ethical standards](ETHICAL) before using or distributing the software. Responsible experimentation, transparency, human oversight, privacy, and harm prevention are core expectations of the project.

## License

Copyright © 2023 Joakim Jacobsen and the Copenhagen-AI Group.

Licensed under the [Apache License 2.0](LICENSE). See the [ethical standards](ETHICAL) that accompany this project. Licensing questions can be directed to [Copenhagen AI](https://www.copenhagen-ai.com/).

---

If the approach interests you, consider starring the repository, opening a discussion, or sharing an experiment built with it.
