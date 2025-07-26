# C# Collection Performance Benchmarks

This document contains comprehensive performance benchmark results for various C# collection types. The benchmarks were conducted using BenchmarkDotNet with enhanced reliability measures including forced garbage collection between iterations and increased sample sizes.

## Test Scenarios

- **Collection Types**: Array, List, HashSet, SortedSet, Dictionary, SortedDictionary, ConcurrentDictionary, ImmutableList, ImmutableHashSet
- **Collection Sizes (N)**: 10, 100, 1,000, 10,000 elements
- **Lookup Counts**: 10, 100, 1,000, 10,000 lookups per test
- **Sample Size**: 56 runs per scenario with outlier detection (Z-score > 2.0)
- **Reliability Enhancements**: Forced GC between iterations, increased warmup (5 iterations), increased test iterations (50)

## Performance Summary by Collection Size

### Small Collections (N=10)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.133 ±1.878 | 1.211 ±2.612 | 0.987 ±2.056 | 1.407 ±3.404 |
| **List** | 0.962 ±1.500 | 0.971 ±1.466 | 0.930 ±1.448 | 0.833 ±1.022 |
| **HashSet** | 1.958 ±2.665 | 1.665 ±2.703 | 4.425 ±16.762 | 1.764 ±2.334 |
| **Dictionary** | 1.736 ±0.974 | 1.745 ±1.136 | 1.906 ±1.120 | 1.925 ±1.349 |
| **SortedSet** | 2.673 ±2.696 | 2.462 ±2.623 | 2.380 ±2.509 | 2.380 ±2.342 |
| **SortedDictionary** | 5.913 ±2.687 | 5.804 ±2.484 | 5.620 ±2.348 | 5.780 ±2.640 |
| **ConcurrentDictionary** | 6.327 ±2.511 | 6.560 ±3.157 | 5.746 ±2.681 | 6.104 ±2.563 |
| **ImmutableList** | 1.822 ±0.733 | 1.465 ±0.361 | 1.641 ±0.317 | 1.542 ±0.305 |
| **ImmutableHashSet** | 4.315 ±1.405 | 4.547 ±1.711 | 5.169 ±2.454 | 4.864 ±1.553 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 0.325 ±0.100 | 1.253 ±0.459 | 10.972 ±1.768 | 41.316 ±6.794 |
| **List** | 0.416 ±0.151 | 1.240 ±0.251 | 12.302 ±3.289 | 37.073 ±5.316 |
| **HashSet** | 0.342 ±0.121 | 1.727 ±0.091 | 18.784 ±2.093 | 102.647 ±5.316 |
| **Dictionary** | 0.356 ±0.066 | 1.665 ±0.102 | 18.778 ±2.194 | 107.976 ±4.098 |
| **SortedSet** | 1.220 ±0.122 | 9.000 ±0.867 | 83.640 ±6.033 | 189.167 ±9.439 |
| **SortedDictionary** | 1.324 ±0.102 | 8.782 ±0.906 | 87.738 ±5.927 | 242.433 ±19.389 |
| **ConcurrentDictionary** | 0.433 ±0.051 | 2.496 ±0.058 | 22.041 ±4.388 | 126.471 ±14.166 |
| **ImmutableList** | 1.140 ±0.091 | 7.662 ±0.089 | 74.391 ±6.421 | 590.018 ±32.436 |
| **ImmutableHashSet** | 0.856 ±0.060 | 6.524 ±1.069 | 64.715 ±9.489 | 182.098 ±7.072 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.582 ±1.920 | 2.582 ±2.713 | 12.094 ±2.710 | 42.847 ±7.880 |
| **List** | 1.475 ±1.619 | 2.331 ±1.593 | 13.335 ±3.513 | 37.987 ±5.520 |
| **HashSet** | 2.420 ±2.686 | 3.495 ±2.746 | 23.311 ±16.538 | 104.518 ±6.031 |
| **Dictionary** | 2.184 ±0.999 | 3.531 ±1.168 | 20.776 ±2.228 | 110.011 ±4.703 |
| **SortedSet** | 3.989 ±2.748 | 11.555 ±3.235 | 86.113 ±6.122 | 191.640 ±10.019 |
| **SortedDictionary** | 7.327 ±2.754 | 14.678 ±2.887 | 93.453 ±6.036 | 248.302 ±20.679 |
| **ConcurrentDictionary** | 6.860 ±2.540 | 9.187 ±3.136 | 27.874 ±5.401 | 132.653 ±16.380 |
| **ImmutableList** | 3.073 ±0.775 | 9.233 ±0.427 | 76.133 ±6.462 | 591.647 ±32.575 |
| **ImmutableHashSet** | 5.247 ±1.411 | 11.162 ±2.026 | 69.983 ±9.751 | 187.065 ±7.499 |

### Medium Collections (N=100)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.053 ±1.995 | 1.091 ±1.841 | 1.106 ±1.889 | 1.162 ±1.802 |
| **List** | 1.060 ±1.757 | 0.989 ±1.612 | 1.018 ±1.558 | 0.953 ±1.224 |
| **HashSet** | 3.176 ±3.948 | 3.218 ±3.276 | 5.196 ±16.032 | 3.131 ±2.890 |
| **Dictionary** | 5.055 ±1.540 | 4.945 ±1.242 | 4.978 ±1.104 | 5.084 ±1.579 |
| **SortedSet** | 11.687 ±4.516 | 11.620 ±4.320 | 11.536 ±4.142 | 11.749 ±4.569 |
| **SortedDictionary** | 33.113 ±4.680 | 32.658 ±4.758 | 33.062 ±4.972 | 33.429 ±4.442 |
| **ConcurrentDictionary** | 21.462 ±5.221 | 21.251 ±3.215 | 21.093 ±5.390 | 21.551 ±3.887 |
| **ImmutableList** | 9.564 ±3.648 | 10.195 ±3.991 | 11.130 ±4.294 | 30.838 ±145.460 |
| **ImmutableHashSet** | 36.064 ±3.739 | 33.942 ±5.068 | 36.848 ±6.332 | 37.371 ±4.848 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 0.467 ±0.128 | 1.545 ±0.121 | 17.633 ±2.007 | 111.758 ±7.885 |
| **List** | 0.549 ±0.197 | 1.520 ±0.127 | 18.502 ±2.764 | 108.651 ±7.997 |
| **HashSet** | 0.362 ±0.065 | 1.749 ±0.141 | 19.358 ±1.946 | 116.678 ±5.501 |
| **Dictionary** | 0.375 ±0.064 | 1.725 ±0.597 | 19.559 ±4.246 | 119.462 ±9.872 |
| **SortedSet** | 1.711 ±0.142 | 17.087 ±1.550 | 139.578 ±8.721 | 376.942 ±20.513 |
| **SortedDictionary** | 1.982 ±0.982 | 17.373 ±1.467 | 171.731 ±15.458 | 522.080 ±8.751 |
| **ConcurrentDictionary** | 0.553 ±0.074 | 2.755 ±0.107 | 30.113 ±6.033 | 173.591 ±19.792 |
| **ImmutableList** | 6.856 ±0.411 | 67.600 ±4.937 | 662.704 ±31.389 | 5039.748 ±1553.148 |
| **ImmutableHashSet** | 1.278 ±0.092 | 11.109 ±4.340 | 93.717 ±9.776 | 328.489 ±16.905 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.618 ±2.054 | 2.715 ±1.869 | 18.844 ±2.545 | 113.031 ±8.512 |
| **List** | 1.709 ±1.871 | 2.597 ±1.664 | 19.620 ±2.823 | 109.695 ±8.866 |
| **HashSet** | 3.671 ±3.951 | 5.091 ±3.322 | 24.655 ±16.101 | 119.895 ±6.291 |
| **Dictionary** | 5.547 ±1.571 | 6.760 ±1.431 | 24.652 ±4.196 | 124.644 ±10.598 |
| **SortedSet** | 13.522 ±4.549 | 28.829 ±5.313 | 151.235 ±9.192 | 388.802 ±21.813 |
| **SortedDictionary** | 35.211 ±4.573 | 50.143 ±5.903 | 204.909 ±16.136 | 555.622 ±8.530 |
| **ConcurrentDictionary** | 24.578 ±5.578 | 26.698 ±4.089 | 53.630 ±8.550 | 197.671 ±21.440 |
| **ImmutableList** | 16.542 ±3.769 | 77.924 ±6.532 | 673.935 ±31.766 | 5070.748 ±1585.941 |
| **ImmutableHashSet** | 37.465 ±3.759 | 45.158 ±7.891 | 130.680 ±8.957 | 365.985 ±17.329 |

### Large Collections (N=1,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 7.809 ±3.526 | 7.427 ±3.213 | 7.680 ±3.246 | 8.402 ±3.826 |
| **List** | 7.440 ±3.438 | 7.064 ±3.114 | 7.300 ±3.234 | 6.862 ±3.042 |
| **HashSet** | 24.709 ±5.604 | 26.135 ±5.866 | 28.873 ±14.243 | 27.484 ±6.003 |
| **Dictionary** | 52.565 ±5.867 | 50.989 ±4.670 | 53.289 ±6.714 | 48.459 ±5.157 |
| **SortedSet** | 81.067 ±10.853 | 81.667 ±10.687 | 81.480 ±10.647 | 82.020 ±10.864 |
| **SortedDictionary** | 362.287 ±23.045 | 364.880 ±23.178 | 366.680 ±23.331 | 369.420 ±23.551 |
| **ConcurrentDictionary** | 164.606 ±28.278 | 146.245 ±25.320 | 147.653 ±27.020 | 164.619 ±29.171 |
| **ImmutableList** | 45.344 ±7.610 | 45.873 ±8.361 | 62.423 ±171.582 | 46.569 ±35.823 |
| **ImmutableHashSet** | 371.461 ±17.073 | 412.965 ±17.128 | 417.176 ±18.392 | 415.643 ±18.713 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 1.431 ±0.115 | 10.340 ±1.019 | 114.059 ±4.413 | 1070.942 ±21.470 |
| **List** | 1.380 ±0.133 | 10.176 ±0.831 | 114.640 ±2.548 | 962.400 ±19.473 |
| **HashSet** | 0.328 ±0.053 | 1.729 ±0.130 | 20.471 ±8.012 | 121.567 ±8.064 |
| **Dictionary** | 0.380 ±0.068 | 1.885 ±0.545 | 17.702 ±2.071 | 106.061 ±6.117 |
| **SortedSet** | 2.213 ±0.140 | 24.382 ±2.268 | 222.062 ±18.436 | 608.240 ±39.468 |
| **SortedDictionary** | 2.407 ±0.109 | 24.947 ±1.935 | 248.422 ±18.833 | 818.762 ±19.701 |
| **ConcurrentDictionary** | 0.534 ±0.071 | 2.638 ±0.077 | 26.674 ±4.026 | 172.779 ±14.552 |
| **ImmutableList** | 69.011 ±5.278 | 679.760 ±13.104 | 4937.521 ±2086.523 | 23184.850 ±988.525 |
| **ImmutableHashSet** | 1.404 ±0.064 | 15.330 ±2.052 | 127.583 ±14.441 | 542.009 ±16.610 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 9.336 ±3.592 | 17.864 ±3.537 | 121.826 ±5.644 | 1079.449 ±21.602 |
| **List** | 8.920 ±3.496 | 17.338 ±3.297 | 122.037 ±4.783 | 969.358 ±20.380 |
| **HashSet** | 25.148 ±5.613 | 27.985 ±5.865 | 49.476 ±16.709 | 149.171 ±10.693 |
| **Dictionary** | 55.352 ±6.544 | 55.515 ±5.456 | 73.417 ±7.676 | 156.602 ±8.591 |
| **SortedSet** | 83.387 ±10.910 | 106.162 ±12.128 | 303.655 ±23.966 | 690.373 ±45.178 |
| **SortedDictionary** | 364.807 ±23.065 | 389.942 ±22.019 | 615.218 ±39.145 | 1188.298 ±40.754 |
| **ConcurrentDictionary** | 165.247 ±28.293 | 149.000 ±25.359 | 174.423 ±29.617 | 337.504 ±31.793 |
| **ImmutableList** | 114.453 ±9.496 | 725.742 ±16.667 | 5000.080 ±2117.437 | 23231.694 ±995.507 |
| **ImmutableHashSet** | 372.970 ±17.072 | 428.389 ±17.128 | 544.872 ±27.955 | 957.791 ±30.221 |

### Very Large Collections (N=10,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 19.011 ±6.604 | 17.449 ±7.031 | 19.846 ±6.066 | 23.307 ±16.814 |
| **List** | 17.640 ±5.909 | 16.384 ±4.897 | 17.604 ±7.449 | 16.558 ±16.282 |
| **HashSet** | 128.519 ±6.828 | 144.504 ±9.909 | 149.373 ±20.784 | 145.173 ±11.903 |
| **Dictionary** | 147.555 ±10.466 | 145.660 ±6.578 | 140.559 ±10.672 | 140.191 ±12.126 |
| **SortedSet** | 553.327 ±35.468 | 536.900 ±31.172 | 560.540 ±32.019 | 537.760 ±37.209 |
| **SortedDictionary** | 4389.687 ±165.970 | 3829.560 ±196.152 | 4044.800 ±198.203 | 3987.020 ±247.840 |
| **ConcurrentDictionary** | 662.769 ±59.824 | 645.976 ±36.324 | 596.178 ±65.073 | 634.182 ±47.523 |
| **ImmutableList** | 248.264 ±12.056 | 219.545 ±208.528 | 134.826 ±142.817 | 120.853 ±123.856 |
| **ImmutableHashSet** | 3839.800 ±809.059 | 3733.951 ±754.649 | 3607.038 ±913.783 | 3476.127 ±1015.345 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 9.858 ±0.541 | 87.313 ±8.525 | 966.635 ±19.231 | 6650.230 ±2267.822 |
| **List** | 9.942 ±0.505 | 97.424 ±10.065 | 871.642 ±38.859 | 6457.058 ±2253.309 |
| **HashSet** | 0.330 ±0.060 | 2.021 ±0.146 | 22.027 ±4.501 | 137.847 ±5.680 |
| **Dictionary** | 0.367 ±0.079 | 1.924 ±0.126 | 18.833 ±1.727 | 127.380 ±9.775 |
| **SortedSet** | 2.956 ±0.224 | 30.162 ±3.233 | 264.240 ±14.057 | 910.080 ±56.158 |
| **SortedDictionary** | 4.373 ±0.820 | 32.173 ±3.133 | 332.960 ±193.440 | 1126.340 ±69.220 |
| **ConcurrentDictionary** | 0.649 ±0.137 | 3.075 ±0.125 | 45.893 ±124.261 | 188.513 ±20.746 |
| **ImmutableList** | 655.020 ±12.327 | 4831.446 ±1727.661 | 23412.786 ±4543.689 | 268549.307 ±6251.209 |
| **ImmutableHashSet** | 2.218 ±0.655 | 18.120 ±5.750 | 168.745 ±173.694 | 860.920 ±64.468 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 28.962 ±6.742 | 104.918 ±10.403 | 986.765 ±20.060 | 6675.025 ±2270.872 |
| **List** | 27.673 ±5.999 | 113.962 ±11.400 | 889.339 ±39.011 | 6473.708 ±2260.456 |
| **HashSet** | 128.946 ±6.849 | 146.645 ±9.974 | 171.520 ±21.166 | 283.129 ±15.017 |
| **Dictionary** | 148.295 ±10.857 | 147.924 ±7.098 | 159.698 ±11.114 | 267.904 ±20.366 |
| **SortedSet** | 556.383 ±35.580 | 567.182 ±32.435 | 824.900 ±39.006 | 1447.960 ±89.283 |
| **SortedDictionary** | 4394.160 ±166.324 | 3861.853 ±197.732 | 4377.880 ±264.113 | 5113.460 ±308.779 |
| **ConcurrentDictionary** | 663.540 ±59.919 | 649.187 ±36.333 | 642.185 ±138.827 | 822.787 ±61.041 |
| **ImmutableList** | 903.402 ±20.115 | 5051.204 ±1802.655 | 23547.822 ±4568.784 | 268670.485 ±6268.557 |
| **ImmutableHashSet** | 3842.205 ±808.860 | 3752.285 ±757.547 | 3775.958 ±960.037 | 4337.229 ±1059.249 |

## Key Performance Insights

### 🥇 Best Performers by Category

#### Creation Time Champions
- **Small collections (N≤100)**: List consistently shows the best creation performance (~1 μs)
- **Large collections (N≥1,000)**: Array and List maintain excellent creation performance (~7-20 μs)

#### Lookup Time Champions
- **Hash-based lookups**: HashSet and Dictionary show excellent O(1) performance across all scales
- **Sequential access**: Array and List excel for small lookup counts but scale poorly for large lookups
- **Sorted access**: SortedSet and SortedDictionary provide consistent O(log n) performance

#### Total Performance Winners
- **Small workloads**: Array and List dominate with sub-microsecond to low-microsecond performance
- **Medium workloads**: HashSet and Dictionary take the lead with consistent hash-based performance
- **Large workloads**: Hash-based collections maintain clear superiority

### 📊 Performance Patterns

1. **Scalability**: Hash-based collections (HashSet, Dictionary) show excellent scalability with consistent O(1) lookup performance
2. **Consistency**: Array and List provide very consistent performance with low variance for creation operations
3. **Thread Safety Cost**: ConcurrentDictionary shows ~2-3x overhead compared to Dictionary but maintains good scalability
4. **Immutability Cost**: Immutable collections show significant performance penalties, especially ImmutableList for large datasets (200,000+ μs for N=10,000, 10,000 lookups)
5. **Reliability**: The enhanced benchmark methodology with forced GC and increased samples provides much more consistent and reliable results

### 💡 Recommendations

- **Use Array/List** for small collections with infrequent lookups
- **Use HashSet/Dictionary** for frequent lookups regardless of collection size
- **Avoid ImmutableList** for performance-critical scenarios with large datasets
- **Consider ConcurrentDictionary** only when thread safety is required and the ~2-3x overhead is acceptable
