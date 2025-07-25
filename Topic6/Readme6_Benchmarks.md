# C# Collection Performance Benchmarks

This document contains comprehensive performance benchmark results for various C# collection types. The benchmarks were conducted using BenchmarkDotNet with multiple scenarios testing creation time, lookup time, and total time performance.

## Test Scenarios

- **Collection Types**: Array, List, HashSet, SortedSet, Dictionary, SortedDictionary, ConcurrentDictionary, ImmutableList, ImmutableHashSet
- **Collection Sizes (N)**: 10, 100, 1,000, 10,000 elements
- **Lookup Counts**: 10, 100, 1,000, 10,000 lookups per test
- **Sample Size**: 34 runs per scenario with outlier detection (Z-score > 2.0)

## Performance Summary by Collection Size

### Small Collections (N=10)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.18 ±2.44 | 1.12 ±2.19 | 0.93 ±1.92 | 1.07 ±1.99 |
| **List** | 0.99 ±1.64 | 0.86 ±1.27 | 0.98 ±1.71 | 0.92 ±1.65 |
| **HashSet** | 1.93 ±3.43 | 1.79 ±3.00 | 4.67 ±15.80 | 1.74 ±3.21 |
| **Dictionary** | 1.77 ±1.33 | 1.82 ±1.96 | 1.91 ±3.09 | 1.90 ±1.29 |
| **SortedSet** | 2.62 ±2.71 | 2.43 ±3.12 | 2.49 ±3.87 | 2.22 ±2.33 |
| **SortedDictionary** | 6.16 ±3.60 | 5.19 ±2.73 | 5.38 ±2.71 | 6.08 ±3.03 |
| **ConcurrentDictionary** | 5.22 ±2.65 | 5.63 ±3.84 | 5.89 ±4.69 | 6.62 ±3.75 |
| **ImmutableList** | 1.63 ±0.34 | 1.40 ±0.21 | 1.84 ±1.37 | 2.11 ±2.77 |
| **ImmutableHashSet** | 4.69 ±2.21 | 4.07 ±1.57 | 4.31 ±2.16 | 4.41 ±2.30 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 0.30 ±0.12 | 1.09 ±0.15 | 10.59 ±1.66 | 37.22 ±2.80 |
| **List** | 0.38 ±0.17 | 1.18 ±0.13 | 12.36 ±2.23 | 37.44 ±2.35 |
| **HashSet** | 0.27 ±0.07 | 1.49 ±0.12 | 17.17 ±1.96 | 98.02 ±4.55 |
| **Dictionary** | 0.33 ±0.05 | 1.59 ±0.17 | 17.00 ±1.96 | 106.30 ±7.50 |
| **SortedSet** | 1.22 ±0.12 | 8.88 ±1.05 | 81.35 ±5.73 | 186.58 ±9.28 |
| **SortedDictionary** | 1.32 ±0.10 | 8.69 ±0.89 | 86.10 ±5.76 | 234.73 ±18.96 |
| **ConcurrentDictionary** | 0.38 ±0.05 | 2.18 ±0.12 | 25.20 ±4.62 | 129.88 ±21.05 |
| **ImmutableList** | 1.04 ±0.12 | 6.38 ±0.34 | 71.66 ±3.33 | 550.93 ±34.79 |
| **ImmutableHashSet** | 0.79 ±0.08 | 5.48 ±0.22 | 93.23 ±195.38 | 162.55 ±8.12 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.58 ±2.52 | 2.36 ±2.25 | 11.59 ±2.28 | 38.38 ±4.42 |
| **List** | 1.48 ±1.78 | 2.14 ±1.33 | 13.48 ±2.64 | 38.46 ±3.68 |
| **HashSet** | 2.29 ±3.50 | 3.37 ±3.04 | 21.92 ±15.83 | 99.83 ±7.23 |
| **Dictionary** | 2.20 ±1.35 | 3.53 ±2.06 | 19.03 ±3.18 | 108.30 ±7.69 |
| **SortedSet** | 3.94 ±2.79 | 11.43 ±3.72 | 83.95 ±5.50 | 188.90 ±9.95 |
| **SortedDictionary** | 7.60 ±3.62 | 13.98 ±2.78 | 91.59 ±5.55 | 240.92 ±20.62 |
| **ConcurrentDictionary** | 5.70 ±2.67 | 7.90 ±3.91 | 31.18 ±6.17 | 136.58 ±23.97 |
| **ImmutableList** | 2.77 ±0.41 | 7.88 ±0.46 | 73.61 ±2.92 | 553.12 ±35.81 |
| **ImmutableHashSet** | 5.57 ±2.22 | 9.66 ±1.64 | 97.63 ±195.37 | 167.06 ±9.32 |

### Medium Collections (N=100)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.18 ±2.48 | 1.22 ±2.37 | 1.07 ±2.14 | 1.38 ±2.47 |
| **List** | 1.10 ±1.85 | 0.81 ±1.34 | 1.06 ±1.62 | 1.04 ±1.41 |
| **HashSet** | 2.71 ±3.09 | 3.02 ±4.05 | 5.81 ±16.27 | 3.70 ±5.72 |
| **Dictionary** | 4.36 ±1.30 | 4.64 ±1.67 | 4.81 ±3.71 | 4.90 ±1.11 |
| **SortedSet** | 11.40 ±4.69 | 12.52 ±4.79 | 10.02 ±3.59 | 11.88 ±4.55 |
| **SortedDictionary** | 32.69 ±4.61 | 32.32 ±5.26 | 35.73 ±5.72 | 37.36 ±4.22 |
| **ConcurrentDictionary** | 17.42 ±2.95 | 18.55 ±5.06 | 18.70 ±5.10 | 21.58 ±6.03 |
| **ImmutableList** | 7.02 ±3.43 | 7.77 ±3.67 | 9.75 ±4.05 | 42.65 ±201.03 |
| **ImmutableHashSet** | 32.44 ±9.10 | 31.09 ±3.59 | 34.41 ±3.61 | 32.48 ±4.73 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 0.41 ±0.11 | 1.47 ±0.14 | 16.80 ±2.36 | 119.82 ±6.86 |
| **List** | 0.59 ±0.20 | 1.45 ±0.15 | 18.53 ±2.77 | 108.64 ±7.97 |
| **HashSet** | 0.31 ±0.07 | 1.54 ±0.17 | 17.75 ±2.10 | 117.18 ±4.14 |
| **Dictionary** | 0.32 ±0.06 | 1.54 ±0.10 | 17.34 ±2.25 | 119.86 ±4.44 |
| **SortedSet** | 1.71 ±0.14 | 16.96 ±1.55 | 137.26 ±8.66 | 372.12 ±20.36 |
| **SortedDictionary** | 1.96 ±0.98 | 17.25 ±1.45 | 170.13 ±15.31 | 515.59 ±8.63 |
| **ConcurrentDictionary** | 0.46 ±0.07 | 2.39 ±0.14 | 28.79 ±4.44 | 177.26 ±22.26 |
| **ImmutableList** | 5.98 ±0.29 | 56.72 ±5.95 | 611.68 ±27.09 | 4409.84 ±1663.93 |
| **ImmutableHashSet** | 1.08 ±0.08 | 9.56 ±1.36 | 98.19 ±10.76 | 300.62 ±30.76 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.69 ±2.54 | 2.79 ±2.43 | 17.98 ±2.67 | 121.31 ±7.56 |
| **List** | 1.79 ±1.96 | 2.34 ±1.43 | 19.69 ±2.83 | 109.79 ±8.90 |
| **HashSet** | 3.08 ±3.10 | 4.66 ±4.15 | 23.65 ±16.15 | 121.00 ±7.63 |
| **Dictionary** | 4.79 ±1.30 | 6.28 ±1.71 | 22.25 ±3.91 | 124.85 ±4.98 |
| **SortedSet** | 13.22 ±4.71 | 29.58 ±5.22 | 147.35 ±9.16 | 384.11 ±21.77 |
| **SortedDictionary** | 34.78 ±4.54 | 49.67 ±5.93 | 205.95 ±16.06 | 553.06 ±8.43 |
| **ConcurrentDictionary** | 19.64 ±3.27 | 22.36 ±5.56 | 50.04 ±6.58 | 200.97 ±25.56 |
| **ImmutableList** | 13.10 ±3.53 | 64.58 ±6.28 | 621.53 ±29.69 | 4452.63 ±1722.88 |
| **ImmutableHashSet** | 33.62 ±9.13 | 40.75 ±4.01 | 132.71 ±9.82 | 333.18 ±30.86 |

### Large Collections (N=1,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 9.42 ±5.87 | 5.95 ±3.07 | 6.41 ±3.93 | 8.37 ±4.99 |
| **List** | 7.39 ±4.58 | 5.93 ±2.90 | 7.90 ±3.39 | 6.48 ±3.51 |
| **HashSet** | 26.26 ±6.14 | 26.13 ±6.42 | 29.93 ±16.99 | 27.78 ±6.00 |
| **Dictionary** | 51.48 ±6.71 | 52.21 ±7.51 | 55.53 ±10.28 | 69.00 ±88.14 |
| **SortedSet** | 76.41 ±10.74 | 81.39 ±8.77 | 88.96 ±13.16 | 106.19 ±155.55 |
| **SortedDictionary** | 357.69 ±22.88 | 363.27 ±22.99 | 405.17 ±25.59 | 455.99 ±264.17 |
| **ConcurrentDictionary** | 170.44 ±24.89 | 182.49 ±175.07 | 194.56 ±206.45 | 216.94 ±247.72 |
| **ImmutableList** | 39.38 ±7.49 | 40.48 ±7.30 | 80.61 ±217.69 | 38.86 ±17.37 |
| **ImmutableHashSet** | 376.04 ±23.62 | 391.73 ±31.70 | 510.66 ±521.26 | 387.23 ±27.47 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.46 ±0.23 | 9.94 ±0.10 | 114.43 ±2.62 | 1075.43 ±35.20 |
| **List** | 1.30 ±0.15 | 10.12 ±0.82 | 114.63 ±2.54 | 962.67 ±19.42 |
| **HashSet** | 0.32 ±0.09 | 1.65 ±0.13 | 19.29 ±2.42 | 118.55 ±5.40 |
| **Dictionary** | 0.41 ±0.06 | 1.88 ±0.13 | 19.24 ±3.91 | 121.38 ±6.33 |
| **SortedSet** | 2.21 ±0.14 | 24.26 ±2.25 | 220.51 ±18.33 | 602.66 ±39.28 |
| **SortedDictionary** | 2.40 ±0.11 | 24.84 ±1.91 | 246.93 ±18.71 | 811.43 ±19.58 |
| **ConcurrentDictionary** | 0.53 ±0.07 | 2.41 ±0.11 | 29.36 ±4.43 | 177.94 ±23.42 |
| **ImmutableList** | 55.12 ±3.93 | 545.80 ±9.32 | 4427.22 ±1622.43 | 21691.81 ±2126.67 |
| **ImmutableHashSet** | 1.37 ±0.11 | 14.43 ±1.91 | 134.31 ±20.62 | 491.20 ±27.48 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 10.99 ±6.03 | 15.97 ±3.08 | 120.95 ±3.78 | 1083.92 ±33.72 |
| **List** | 8.79 ±4.64 | 16.14 ±2.80 | 122.63 ±4.78 | 969.24 ±20.35 |
| **HashSet** | 26.68 ±6.13 | 27.92 ±6.41 | 49.32 ±17.42 | 146.41 ±7.43 |
| **Dictionary** | 54.11 ±7.36 | 56.50 ±8.42 | 77.34 ±12.72 | 192.78 ±88.87 |
| **SortedSet** | 78.69 ±10.81 | 105.73 ±9.96 | 309.57 ±23.88 | 708.95 ±168.44 |
| **SortedDictionary** | 360.21 ±22.88 | 388.20 ±21.99 | 652.22 ±38.91 | 1267.56 ±264.41 |
| **ConcurrentDictionary** | 171.09 ±24.92 | 184.98 ±175.12 | 224.01 ±206.06 | 394.99 ±254.26 |
| **ImmutableList** | 94.59 ±10.23 | 586.38 ±10.52 | 4507.97 ±1678.56 | 21730.91 ±2124.38 |
| **ImmutableHashSet** | 377.52 ±23.68 | 406.25 ±30.56 | 645.12 ±537.61 | 878.53 ±51.39 |

### Very Large Collections (N=10,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 16.36 ±8.46 | 17.98 ±6.27 | 16.83 ±6.81 | 21.22 ±13.84 |
| **List** | 17.47 ±5.88 | 16.14 ±4.87 | 17.40 ±7.40 | 16.38 ±16.20 |
| **HashSet** | 143.58 ±12.18 | 138.79 ±11.76 | 135.04 ±20.94 | 130.00 ±9.28 |
| **Dictionary** | 148.32 ±9.31 | 131.94 ±12.39 | 145.49 ±6.13 | 130.86 ±7.98 |
| **SortedSet** | 549.51 ±35.26 | 533.23 ±30.93 | 557.61 ±31.90 | 534.08 ±37.02 |
| **SortedDictionary** | 4338.02 ±163.32 | 3789.63 ±194.53 | 4003.85 ±196.62 | 3946.77 ±246.12 |
| **ConcurrentDictionary** | 699.76 ±94.36 | 589.26 ±53.70 | 654.51 ±49.36 | 590.72 ±47.79 |
| **ImmutableList** | 244.16 ±6.78 | 234.58 ±265.78 | 151.08 ±159.87 | 171.08 ±204.33 |
| **ImmutableHashSet** | 3981.00 ±228.55 | 3641.79 ±813.67 | 3765.74 ±842.47 | 3358.86 ±863.95 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 8.98 ±0.83 | 96.71 ±7.11 | 854.99 ±14.03 | 7745.85 ±2014.02 |
| **List** | 9.89 ±0.50 | 97.19 ±10.01 | 869.81 ±38.74 | 6455.15 ±2250.71 |
| **HashSet** | 0.32 ±0.09 | 1.86 ±0.12 | 19.71 ±2.57 | 122.18 ±7.40 |
| **Dictionary** | 0.40 ±0.07 | 1.81 ±0.15 | 21.67 ±6.24 | 119.36 ±3.00 |
| **SortedSet** | 2.95 ±0.22 | 30.03 ±3.21 | 262.58 ±13.96 | 904.04 ±55.88 |
| **SortedDictionary** | 4.36 ±0.81 | 32.04 ±3.11 | 331.24 ±192.02 | 1120.07 ±68.75 |
| **ConcurrentDictionary** | 0.89 ±0.43 | 2.62 ±0.10 | 60.91 ±163.83 | 174.76 ±22.24 |
| **ImmutableList** | 594.55 ±17.11 | 4332.47 ±1460.85 | 21239.72 ±1006.84 | 198042.57 ±4948.98 |
| **ImmutableHashSet** | 2.24 ±0.51 | 18.65 ±7.14 | 196.45 ±237.84 | 842.16 ±62.56 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 25.66 ±8.90 | 114.85 ±9.11 | 871.92 ±15.31 | 7767.38 ±2016.53 |
| **List** | 27.44 ±5.94 | 113.51 ±11.33 | 887.31 ±38.91 | 6472.14 ±2258.04 |
| **HashSet** | 144.00 ±12.21 | 140.76 ±11.83 | 154.87 ±21.39 | 252.27 ±14.47 |
| **Dictionary** | 149.24 ±9.22 | 134.08 ±12.65 | 167.65 ±8.45 | 250.47 ±9.87 |
| **SortedSet** | 552.56 ±35.37 | 563.34 ±32.14 | 820.26 ±38.82 | 1438.20 ±88.70 |
| **SortedDictionary** | 4342.99 ±163.66 | 3821.83 ±196.13 | 4335.23 ±262.39 | 5067.04 ±306.56 |
| **ConcurrentDictionary** | 700.83 ±94.79 | 591.98 ±53.78 | 715.52 ±166.90 | 765.60 ±67.83 |
| **ImmutableList** | 838.86 ±20.71 | 4567.22 ±1574.60 | 21391.03 ±1052.32 | 198214.07 ±4928.54 |
| **ImmutableHashSet** | 3983.41 ±228.63 | 3660.62 ±817.27 | 3962.36 ±922.61 | 4201.21 ±862.65 |

## Key Performance Insights

### 🥇 Best Performers by Category

#### Creation Time Champions
- **Small collections (N≤100)**: Array and List (sub-microsecond performance)
- **Large collections (N≥1,000)**: Array and List maintain excellent creation performance

#### Lookup Time Champions
- **Hash-based lookups**: HashSet and Dictionary show O(1) performance across all scales
- **Sequential access**: Array and List excel for small lookup counts
- **Sorted access**: SortedSet and SortedDictionary provide O(log n) performance

#### Total Performance Winners
- **Small workloads**: Array and List dominate
- **Medium workloads**: HashSet and Dictionary take the lead
- **Large workloads**: Hash-based collections maintain superiority

### 📊 Performance Patterns

1. **Scalability**: Hash-based collections (HashSet, Dictionary) show excellent scalability
2. **Consistency**: Array and List provide very consistent performance with low variance
3. **Thread Safety Cost**: ConcurrentDictionary shows ~2-3x overhead compared to Dictionary
4. **Immutability Cost**: Immutable collections show significant performance penalties, especially for large datasets
5. **Sorted Structure Cost**: SortedSet and SortedDictionary have higher overhead but provide ordering guarantees

### ⚠️ Performance Warnings

- **ImmutableList**: Shows exponential degradation with size, unsuitable for large collections
- **Large sorted collections**: SortedDictionary creation time becomes prohibitive at N=10,000
- **High lookup counts**: Sequential collections (Array, List) become inefficient for many lookups

---

*Statistics format: Mean ±Standard Deviation (all times in microseconds)*
*Outliers removed using Z-score > 2.0 threshold*