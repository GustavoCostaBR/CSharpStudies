# C# Collection Performance Benchmarks - Integer Data

This document contains comprehensive performance benchmark results for various C# collection types using **integer data**. The benchmarks were conducted using our custom high-precision benchmarking framework with single-core execution and statistical analysis.

## Test Scenarios

- **Collection Types**: Array, List, HashSet, SortedSet, Dictionary, SortedDictionary, ConcurrentDictionary, ImmutableList, ImmutableHashSet
- **Collection Sizes (N)**: 10, 100, 1,000, 10,000 elements
- **Lookup Counts**: 10, 100, 1,000, 10,000 lookups per test
- **Data Type**: Integer values and keys
- **Sample Size**: 50 runs per scenario with outlier detection (Z-score > 2.0)
- **Execution**: Single CPU core, high priority process for maximum consistency

## Performance Summary by Collection Size

### Small Collections (N=10)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 0.498 ±0.092 | 0.548 ±0.096 | 0.849 ±0.439 | 0.664 ±0.126 |
| **Array** | 0.828 ±0.215 | 0.912 ±0.207 | 1.698 ±0.924 | 1.287 ±0.400 |
| **HashSet** | 1.588 ±0.243 | 1.954 ±0.334 | 2.924 ±1.233 | 2.506 ±0.358 |
| **ImmutableList** | 1.611 ±0.105 | 2.010 ±0.443 | 3.143 ±1.429 | 2.485 ±0.560 |
| **Dictionary** | 1.731 ±0.219 | 1.939 ±0.246 | 3.053 ±1.888 | 2.078 ±0.344 |
| **SortedSet** | 2.110 ±0.270 | 2.446 ±0.380 | 3.324 ±1.409 | 2.560 ±0.436 |
| **ImmutableHashSet** | 5.202 ±0.272 | 6.070 ±1.000 | 9.929 ±3.406 | 8.465 ±1.025 |
| **SortedDictionary** | 5.490 ±0.343 | 6.312 ±0.907 | 8.931 ±3.851 | 6.921 ±1.180 |
| **ConcurrentDictionary** | 5.857 ±0.473 | 6.534 ±0.763 | 8.684 ±2.927 | 7.761 ±1.759 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 0.357 ±0.065 | 1.308 ±0.111 | 35.092 ±15.558 | 107.947 ±5.663 |
| **List** | 0.383 ±0.067 | 1.302 ±0.080 | 35.769 ±19.058 | 110.153 ±5.067 |
| **HashSet** | 0.408 ±0.079 | 1.920 ±0.150 | 36.739 ±11.055 | 167.602 ±8.292 |
| **Dictionary** | 0.441 ±0.091 | 1.771 ±0.087 | 36.982 ±10.980 | 167.565 ±9.053 |
| **ConcurrentDictionary** | 0.484 ±0.080 | 3.036 ±0.144 | 37.804 ±11.574 | 139.661 ±7.825 |
| **ImmutableHashSet** | 0.959 ±0.070 | 7.874 ±1.295 | 106.892 ±29.169 | 183.133 ±6.839 |
| **ImmutableList** | 1.130 ±0.072 | 10.098 ±2.542 | 265.522 ±81.882 | 1575.053 ±79.406 |
| **SortedDictionary** | 1.292 ±0.099 | 12.229 ±1.801 | 163.584 ±31.659 | 288.332 ±13.484 |
| **SortedSet** | 1.331 ±0.096 | 10.473 ±1.225 | 106.237 ±17.517 | 210.611 ±6.168 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 0.960 ±0.108 | 1.915 ±0.149 | 36.720 ±19.457 | 110.889 ±5.098 |
| **Array** | 1.250 ±0.226 | 2.290 ±0.256 | 36.892 ±16.308 | 109.334 ±5.756 |
| **HashSet** | 2.073 ±0.279 | 3.943 ±0.408 | 39.769 ±12.147 | 170.188 ±8.314 |
| **Dictionary** | 2.224 ±0.238 | 3.800 ±0.273 | 40.122 ±12.549 | 169.724 ±9.242 |
| **ImmutableList** | 2.804 ±0.138 | 12.188 ±2.874 | 268.786 ±82.898 | 1577.683 ±79.345 |
| **SortedSet** | 3.508 ±0.283 | 12.998 ±1.369 | 109.671 ±18.638 | 213.273 ±6.013 |
| **ImmutableHashSet** | 6.249 ±0.316 | 14.026 ±2.008 | 116.933 ±32.053 | 191.689 ±7.047 |
| **SortedDictionary** | 6.852 ±0.405 | 18.594 ±2.423 | 172.612 ±34.515 | 295.323 ±13.713 |
| **ConcurrentDictionary** | 6.414 ±0.520 | 9.674 ±0.839 | 46.602 ±14.042 | 147.504 ±7.834 |

### Medium Collections (N=100)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 0.640 ±0.088 | 0.657 ±0.157 | 0.779 ±0.165 | 0.449 ±0.157 |
| **Array** | 0.986 ±0.272 | 1.172 ±0.319 | 1.460 ±0.422 | 0.920 ±0.301 |
| **HashSet** | 4.133 ±0.596 | 5.145 ±0.644 | 5.964 ±0.501 | 4.761 ±1.442 |
| **Dictionary** | 7.778 ±0.772 | 7.622 ±0.474 | 7.237 ±0.617 | 4.992 ±0.706 |
| **ImmutableList** | 9.915 ±0.733 | 10.150 ±1.245 | 10.302 ±0.823 | 3.012 ±1.531 |
| **ConcurrentDictionary** | 20.777 ±1.067 | 22.979 ±1.024 | 22.809 ±1.461 | 15.873 ±1.944 |
| **ImmutableHashSet** | 68.237 ±3.633 | 66.631 ±2.984 | 61.279 ±3.025 | 20.256 ±5.707 |
| **SortedDictionary** | 49.547 ±2.137 | 48.788 ±1.425 | 47.173 ±1.848 | 20.582 ±6.668 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **ConcurrentDictionary** | 0.596 ±0.092 | 2.644 ±0.120 | 23.370 ±1.241 | 73.371 ±3.927 |
| **Dictionary** | 0.492 ±0.061 | 2.298 ±0.103 | 22.924 ±1.396 | 108.310 ±5.827 |
| **HashSet** | 0.512 ±0.084 | 2.431 ±0.100 | 23.716 ±0.741 | 114.024 ±6.751 |
| **Array** | 1.531 ±0.261 | 6.850 ±0.657 | 48.490 ±3.245 | 80.300 ±4.615 |
| **List** | 1.479 ±0.193 | 6.857 ±0.650 | 49.442 ±2.901 | 68.308 ±4.398 |
| **ImmutableHashSet** | 1.544 ±0.109 | 10.624 ±0.604 | 94.779 ±5.269 | 262.617 ±21.001 |
| **SortedSet** | 2.135 ±0.110 | 16.309 ±0.893 | 147.815 ±7.047 | 332.886 ±21.421 |
| **SortedDictionary** | 2.915 ±0.819 | 23.780 ±1.002 | 231.409 ±9.232 | 310.718 ±17.310 |
| **ImmutableList** | 18.052 ±0.954 | 172.885 ±6.960 | 1648.725 ±72.332 | 3016.508 ±181.175 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 2.211 ±0.223 | 7.628 ±0.579 | 50.306 ±2.917 | 68.841 ±4.453 |
| **Array** | 2.639 ±0.477 | 8.117 ±0.679 | 50.060 ±3.411 | 81.300 ±4.676 |
| **HashSet** | 4.725 ±0.599 | 7.686 ±0.676 | 29.782 ±0.856 | 118.880 ±6.794 |
| **Dictionary** | 8.363 ±0.764 | 10.027 ±0.511 | 30.282 ±1.674 | 113.376 ±6.057 |
| **ConcurrentDictionary** | 21.488 ±1.110 | 25.725 ±1.065 | 46.272 ±2.240 | 89.320 ±4.520 |
| **ImmutableList** | 28.056 ±1.055 | 183.140 ±7.492 | 1659.140 ±72.356 | 3019.578 ±180.846 |
| **SortedSet** | 9.554 ±0.266 | 25.089 ±0.996 | 157.428 ±7.161 | 345.141 ±21.888 |
| **SortedDictionary** | 52.560 ±2.171 | 72.673 ±1.739 | 278.698 ±9.643 | 331.371 ±18.332 |
| **ImmutableHashSet** | 69.860 ±3.660 | 77.380 ±3.037 | 156.172 ±7.396 | 282.952 ±22.824 |

### Large Collections (N=1,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 0.800 ±0.523 | 0.665 ±0.201 | 0.667 ±0.115 | 0.819 ±0.272 |
| **Array** | 0.937 ±0.189 | 0.822 ±0.131 | 0.865 ±0.156 | 1.588 ±2.020 |
| **ImmutableList** | 10.655 ±1.286 | 9.744 ±0.637 | 9.586 ±0.920 | 9.519 ±0.955 |
| **HashSet** | 17.886 ±10.656 | 10.285 ±0.368 | 10.439 ±0.634 | 10.648 ±1.863 |
| **Dictionary** | 20.604 ±12.724 | 11.735 ±1.038 | 11.389 ±0.663 | 11.561 ±1.200 |
| **SortedSet** | 68.212 ±44.984 | 39.051 ±1.970 | 39.031 ±2.460 | 37.077 ±2.159 |
| **ConcurrentDictionary** | 71.033 ±24.504 | 45.090 ±1.679 | 45.384 ±2.677 | 44.612 ±2.621 |
| **SortedDictionary** | 136.157 ±137.040 | 102.915 ±3.171 | 99.916 ±5.693 | 96.019 ±5.730 |
| **ImmutableHashSet** | 163.492 ±18.126 | 149.504 ±5.469 | 148.909 ±6.333 | 165.800 ±138.402 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **HashSet** | 0.210 ±0.079 | 1.238 ±0.084 | 11.073 ±0.823 | 105.292 ±29.371 |
| **Dictionary** | 0.226 ±0.080 | 1.178 ±0.074 | 10.268 ±0.600 | 94.435 ±4.649 |
| **ConcurrentDictionary** | 0.276 ±0.534 | 0.965 ±0.086 | 7.414 ±0.523 | 63.641 ±3.908 |
| **ImmutableHashSet** | 0.743 ±0.163 | 5.452 ±0.383 | 49.920 ±2.674 | 443.213 ±18.142 |
| **SortedDictionary** | 0.786 ±0.205 | 6.374 ±0.816 | 55.182 ±4.198 | 476.011 ±20.425 |
| **SortedSet** | 0.837 ±0.175 | 5.891 ±0.360 | 51.492 ±2.861 | 440.302 ±11.415 |
| **Array** | 0.946 ±0.132 | 6.871 ±0.766 | 71.398 ±3.997 | 619.151 ±187.299 |
| **List** | 0.924 ±0.109 | 6.242 ±0.330 | 60.792 ±3.612 | 571.587 ±23.491 |
| **ImmutableList** | 30.006 ±1.598 | 295.260 ±6.753 | 2843.647 ±146.602 | 26584.733 ±1097.389 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 1.869 ±0.714 | 6.981 ±0.366 | 61.508 ±3.639 | 572.460 ±23.585 |
| **Array** | 1.946 ±0.306 | 7.749 ±0.808 | 72.310 ±4.073 | 620.786 ±187.253 |
| **HashSet** | 18.192 ±10.711 | 11.629 ±0.412 | 21.624 ±1.214 | 116.004 ±29.526 |
| **Dictionary** | 20.886 ±12.809 | 12.961 ±1.049 | 21.768 ±1.344 | 106.049 ±5.116 |
| **ImmutableList** | 40.743 ±2.390 | 305.053 ±6.806 | 2853.329 ±146.945 | 26594.379 ±1097.533 |
| **ConcurrentDictionary** | 71.376 ±24.657 | 46.116 ±1.730 | 52.847 ±2.948 | 108.308 ±5.995 |
| **SortedSet** | 69.110 ±45.146 | 44.987 ±2.137 | 90.587 ±4.779 | 477.418 ±11.797 |
| **SortedDictionary** | 137.022 ±137.214 | 109.361 ±3.172 | 155.143 ±7.958 | 572.087 ±22.898 |
| **ImmutableHashSet** | 164.298 ±18.283 | 154.992 ±5.670 | 198.887 ±8.172 | 609.096 ±146.091 |

### Very Large Collections (N=10,000)

#### Creation Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **Array** | 2.408 ±0.190 | 2.606 ±0.485 | 3.980 ±4.906 | 3.377 ±1.083 |
| **List** | 2.450 ±0.191 | 2.473 ±0.421 | 3.302 ±4.632 | 3.504 ±1.988 |
| **ImmutableList** | 99.335 ±6.845 | 99.770 ±18.222 | 95.030 ±10.809 | 97.117 ±8.471 |
| **HashSet** | 110.820 ±3.302 | 109.410 ±6.901 | 108.435 ±10.543 | 112.514 ±12.140 |
| **Dictionary** | 123.476 ±3.895 | 120.686 ±6.956 | 116.085 ±3.885 | 120.700 ±7.531 |
| **SortedSet** | 461.574 ±22.191 | 446.735 ±25.175 | 432.674 ±15.672 | 449.553 ±20.322 |
| **ConcurrentDictionary** | 492.004 ±18.110 | 479.906 ±28.969 | 471.133 ±29.234 | 461.415 ±14.093 |
| **SortedDictionary** | 1285.320 ±43.693 | 1237.204 ±67.900 | 1186.137 ±47.385 | 1196.335 ±59.166 |
| **ImmutableHashSet** | 1972.986 ±95.804 | 1906.450 ±105.765 | 1871.750 ±220.254 | 1925.192 ±430.456 |

#### Lookup Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **HashSet** | 0.198 ±0.077 | 1.421 ±0.120 | 11.724 ±0.604 | 115.598 ±7.338 |
| **Dictionary** | 0.200 ±0.037 | 1.396 ±0.104 | 11.794 ±0.931 | 113.300 ±9.612 |
| **ConcurrentDictionary** | 0.227 ±0.054 | 1.096 ±0.127 | 8.567 ±2.345 | 76.017 ±8.323 |
| **SortedSet** | 1.010 ±0.093 | 8.039 ±3.011 | 71.693 ±5.476 | 704.349 ±31.521 |
| **ImmutableHashSet** | 1.042 ±0.078 | 8.118 ±0.503 | 79.994 ±38.360 | 749.367 ±64.686 |
| **SortedDictionary** | 1.063 ±0.173 | 8.866 ±0.526 | 83.386 ±8.652 | 819.092 ±54.296 |
| **List** | 4.978 ±0.188 | 47.029 ±3.102 | 447.092 ±23.599 | 4433.057 ±317.486 |
| **Array** | 6.310 ±0.292 | 57.394 ±4.973 | 467.312 ±22.632 | 4515.868 ±532.950 |
| **ImmutableList** | 279.140 ±13.174 | 2689.021 ±153.933 | 25452.267 ±650.448 | 252022.628 ±3467.115 |

#### Total Time Performance (μs)
| Collection Type | 10 Lookups | 100 Lookups | 1,000 Lookups | 10,000 Lookups |
|----------------|------------|--------------|---------------|----------------|
| **List** | 7.485 ±0.272 | 49.553 ±3.301 | 450.453 ±24.609 | 4436.663 ±318.193 |
| **Array** | 8.769 ±0.434 | 60.049 ±4.999 | 471.345 ±25.131 | 4519.372 ±533.437 |
| **HashSet** | 111.113 ±3.307 | 110.915 ±6.975 | 120.206 ±10.913 | 228.167 ±17.268 |
| **Dictionary** | 123.711 ±3.905 | 122.127 ±7.014 | 127.917 ±4.254 | 234.024 ±13.587 |
| **ImmutableList** | 378.546 ±18.153 | 2788.887 ±155.075 | 25547.491 ±652.600 | 252119.911 ±3465.356 |
| **ConcurrentDictionary** | 492.292 ±18.116 | 481.071 ±29.033 | 479.765 ±29.699 | 537.487 ±17.029 |
| **SortedSet** | 462.644 ±22.265 | 454.820 ±25.232 | 504.426 ±17.112 | 1153.955 ±41.184 |
| **ImmutableHashSet** | 1974.108 ±95.872 | 1914.696 ±106.145 | 1951.821 ±221.909 | 2674.629 ±450.876 |
| **SortedDictionary** | 1286.459 ±43.731 | 1246.130 ±68.339 | 1269.571 ±49.199 | 2015.486 ±105.542 |

## Key Performance Insights - Integer Data

### 🥇 Best Performers by Category

#### Creation Time Champions
- **All collection sizes**: **List** consistently shows the best creation performance (0.45-3.5μs)
- **Small to medium collections**: **Array** provides competitive creation times (0.8-3.4μs)
- **Hash-based collections**: **HashSet** and **Dictionary** scale well with size

#### Lookup Time Champions
- **Hash-based lookups**: **HashSet** and **Dictionary** demonstrate excellent O(1) performance
- **Thread-safe operations**: **ConcurrentDictionary** matches or exceeds regular Dictionary performance
- **Sequential access**: **List** and **Array** show similar O(n) behavior with List slightly faster

#### Total Performance Winners
- **Small workloads (N≤100)**: **List** dominates for low lookup counts; **HashSet/Dictionary** excel with high lookups
- **Large workloads (N≥1,000)**: Hash-based collections maintain clear superiority for any significant lookup volume

### 📊 Performance Patterns - Integer Specific

1. **O(1) vs O(n) Scaling**: Clear performance separation between hash-based (constant time) and sequential (linear time) collections
2. **Creation Overhead**: Hash-based collections have higher upfront costs but amortize quickly with lookups
3. **Memory Efficiency**: Integer data shows excellent cache locality for arrays and lists
4. **Concurrent Safety**: **ConcurrentDictionary** surprisingly matches or exceeds regular **Dictionary** performance
5. **Immutable Collections**: Show dramatic performance penalties, especially **ImmutableList** at scale

### ⚠️ Performance Warnings - Integer Data

- **ImmutableList**: Catastrophic performance degradation (252ms for 10k×10k scenario)
- **Large sequential collections**: Array/List become extremely slow for high lookup volumes (4.4-4.5ms for 10k lookups)
- **SortedDictionary**: High creation costs (1.2-1.3ms) with moderate lookup performance
- **Memory vs Speed**: Hash-based collections use more memory but provide dramatically better lookup performance

### 🎯 Recommendations by Use Case

#### Small Collections (N≤100)
- **Creation-heavy workloads**: Use **List** for minimal overhead (0.45-0.78μs)
- **Lookup-heavy workloads**: Use **HashSet/Dictionary** when lookup count > collection size
- **Mixed workloads**: **List** provides excellent balance for small datasets

#### Medium Collections (N=1,000)
- **Creation-focused**: **List** for minimal creation overhead (0.67-0.82μs)
- **Lookup-focused**: **HashSet/Dictionary** for consistent O(1) performance (10-11μs creation, 0.2-1.2μs lookups)
- **Thread-safe requirements**: **ConcurrentDictionary** with minimal performance penalty

#### Large Collections (N=10,000)
- **Any significant lookup volume**: **HashSet/Dictionary** are essential (110-124μs creation, 0.2-1.4μs lookups)
- **Sorted access needed**: **SortedSet/SortedDictionary** acceptable for moderate lookup volumes
- **Immutable requirements**: Consider performance trade-offs; **ImmutableHashSet** over **ImmutableList**

### 📈 Performance Scale Analysis

#### Creation Time Scaling
- **List/Array**: Excellent linear scaling (2-4x increase from N=10 to N=10,000)
- **Hash-based**: Predictable scaling with size (70-80x increase for large collections)
- **Sorted collections**: Higher overhead but manageable scaling

#### Lookup Time Scaling  
- **Hash-based**: Maintains near-constant time regardless of collection size
- **Sequential**: Clear O(n) behavior - lookup time scales directly with collection size
- **Sorted**: Logarithmic scaling provides middle-ground performance

---

*Statistics format: Mean ±Standard Deviation (all times in microseconds)*  
*Outliers removed using Z-score > 2.0 threshold across 50 samples per scenario*  
*Benchmarks executed with single CPU core affinity and high process priority for maximum consistency*
