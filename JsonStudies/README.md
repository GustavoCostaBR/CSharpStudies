# JsonStudies

Benchmarks and demos for page/section/card/field hierarchies across System.Text.Json and a custom in-memory index.

## Project layout
- `Models` – Mutable domain classes for pages, sections, cards, fields.
- `SampleData` – Deterministic hierarchy factory to seed demos/benchmarks.
- `Adapters` – System.Text.Json adapter for (de)serialization.
- `Helpers` – Traversal, mutation, and indexing helpers shared by benchmarks.
- `Demo` – Quick console demo exercising each adapter.
- `Benchmarks` – BenchmarkDotNet suite comparing traversal and mutation costs.

## Prerequisites
- .NET 9 SDK installed.

## Run demo
```powershell
cd JsonStudies
dotnet run -- Demo
```

## Run benchmarks
```powershell
cd JsonStudies
dotnet run -c Release -- Benchmarks [hierarchy|massive|all]
```

Omit the optional selector (or pass `hierarchy`) to run the small hierarchy suite, use `massive` for the large dataset benchmarks, or `all` to execute both.

BenchmarkDotNet will emit results under `JsonStudies/BenchmarkDotNet.Artifacts`. Adjust `Sections`, `CardsPerSection`, and `FieldsPerContainer` params inside `HierarchyBenchmarks` to scale the dataset.

