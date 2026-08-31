# Awesome.AI algorithm source

This folder contains the reusable Python source for the Awesome.AI framework.
It can be copied beside another project, including the C# repository,
as source/reference material. Nothing runs merely because these files are in a
Git repository.

## Core algorithm folders

The framework logic is spread across these folders:

- `Core/` — the mind and its core lifecycle.
- `CoreSystems/` — supporting systems used by the mind.
- `Common/` — shared output and utility code.
- `Variables/` — constants, enums, and runtime variables.
- `Access/`, `Factorys/`, `Generators/`, and `Interfaces/` — supporting
  building blocks imported by the core algorithm.

Keep these folders together when copying the algorithm, because they import
one another through the `awesome_ai` package.
