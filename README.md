# Socio-Economic Simulator

Headless socio-economic simulator. Specification lives in `docs/`.

## Layout

```
core/                 single source of truth, compiled twice
  package.json        makes this folder a Unity local package
  Sim.Core.asmdef     Unity assembly, noEngineReferences: true
  Sim.Core.csproj     harness and CI build, netstandard2.1, C# 9
  Runtime/
    Primitives/       EntityId, fixed point, remainder accumulator, hash
    Time/             tick, calendar, cadence, staggered buckets
    State/            the arrays, snapshot, serialiser, state hash
    Data/             table types and loader for goods, recipes, mestieri, crops
    Systems/          one file per phase of the settlement update
    Chronicle/        entries, causes, propagation
    Commands/         command queue in, event stream out
  Tests/              NUnit, runs under dotnet test and under Unity
harness/              CLI runner, may use anything
data/                 tables as CSV or JSON, not code
docs/                 SIM-REQ, SIM-DEC, SIM-OBS, SIM-LOOPS, SIM-STATE, SIM-ECON
```

## Why the core is a package, not a folder in Assets

The consumer imports source rather than a prebuilt assembly, so the barrier
against host dependencies has to come from project configuration. As a local
package with `noEngineReferences: true`, calling into the engine from the core
fails to compile inside the editor rather than working there and breaking
headless. The folder also stays owned by this repository rather than by a
Unity project.

Reference it from the consumer's `Packages/manifest.json`:

```json
"com.fedegame.sim.core": "file:../../sim/core"
```

## Rules

- The core references no engine, no rendering, no input, no asset API.
- The core takes no dependency outside the base class library. A
  `PackageReference` in `Sim.Core.csproj` is a decision, not a convenience.
- No reflection-based serialisation, no reflection emit, no runtime codegen.
- No `async` in the tick. No unordered parallelism.
- No floating point in simulation state.
- No iteration over hash-ordered collections in simulation logic.
- The harness may use anything. The core may not.

## Structure follows what the code is, not what it simulates

State lives in one place as parallel arrays. Folders named after subsystems
would suggest an isolation that does not exist, since every system writes to
the same arrays. `Systems/` therefore holds one file per phase of the
settlement update, in the order given by SIM-ECON, and those files are the
only things that mutate state.

## CI

`core` is a release-blocking job. It builds the core with no editor present
and runs the suite twice, once with one wealth band and once with three.
