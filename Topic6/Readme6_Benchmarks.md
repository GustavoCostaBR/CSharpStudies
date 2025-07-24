# Topic 6: Collection Performance

This topic explores the performance differences between various collection types in C#.

## `List<T>` vs. `HashSet<T>` for Lookups

When you need to check if a collection contains a specific item, the choice of collection type can have a significant impact on performance, especially for large datasets.

### Benchmark Results

We benchmarked the `Contains` method for both `List<T>` and `HashSet<T>` with collections of varying sizes.

| Collection Type | Number of Elements | Average Time (ns) |
| :--- | :--- | :--- |
| `List<T>` | 100 | 5.711 |
| `HashSet<T>` | 100 | **1.456** |
| `List<T>` | 1,000 | 39.353 |
| `HashSet<T>` | 1,000 | **1.468** |
| `List<T>` | 10,000 | 249.127 |
| `HashSet<T>` | 10,000 | **1.932** |

### Analysis

- **`List<T>.Contains`**: Performs a linear search with **O(n)** time complexity. The time to find an item increases proportionally with the size of the list.
- **`HashSet<T>.Contains`**: Performs a hash-based lookup with an average time complexity of **O(1)**. The time to find an item is nearly constant, regardless of the collection's size.

### Recommendation

For frequent lookups (i.e., calling the `Contains` method), **`HashSet<T>` is the clear winner**. Use it when you need fast, efficient checks for the existence of items in a collection that does not contain duplicates.

# Collection Performance Benchmarks

This document contains comprehensive performance analysis of `List<T>` vs `HashSet<T>` for real-world scenarios that include both creation and lookup operations.

## Creation + Lookup Performance Results

The benchmarks measure the combined time of creating a collection and performing multiple lookups, which represents more realistic usage patterns.

### Summary Table

| Collection Size | Lookups | List<T> (μs) | HashSet<T> (μs) | Winner | Performance Ratio |
|:---|:---|---:|---:|:---|---:|
| 100 | 10 | 0.094 | 0.536 | **List** | 5.7x faster |
| 100 | 100 | 0.658 | 0.775 | **List** | 1.2x faster |
| 100 | 1000 | 7.130 | 2.856 | **HashSet** | 2.5x faster |
| 1,000 | 10 | 0.760 | 4.882 | **List** | 6.4x faster |
| 1,000 | 100 | 5.787 | 5.296 | **HashSet** | 1.1x faster |
| 1,000 | 1000 | 56.721 | 7.253 | **HashSet** | 7.8x faster |
| 10,000 | 10 | 6.740 | 100.061 | **List** | 14.9x faster |
| 10,000 | 100 | 49.063 | 100.478 | **List** | 2.0x faster |
| 10,000 | 1000 | 450.416 | 110.033 | **HashSet** | 4.1x faster |
| 100,000 | 10 | 79.343 | 1142.859 | **List** | 14.4x faster |
| 100,000 | 100 | 617.020 | 1170.574 | **List** | 1.9x faster |
| 100,000 | 1000 | 6039.243 | 1196.177 | **HashSet** | 5.0x faster |

### Key Findings

#### Break-Even Points
Based on the benchmark results, here are the break-even points where `HashSet<T>` becomes more efficient:

- **100 elements**: HashSet wins when performing **≥ 1000 lookups**
- **1,000 elements**: HashSet wins when performing **≥ 100 lookups**
- **10,000 elements**: HashSet wins when performing **≥ 1000 lookups**
- **100,000 elements**: HashSet wins when performing **≥ 1000 lookups**

#### Memory Usage
- `List<T>` uses significantly less memory (456B for 100 items vs 1,864B for HashSet)
- HashSet memory overhead increases substantially with size (1.7MB vs 400KB for 100,000 items)
- HashSet triggers more garbage collection, especially for larger collections

#### Performance Patterns

1. **Small Collections (≤ 1,000 items)**:
   - List is faster for few lookups due to lower creation cost
   - HashSet overhead dominates performance for small lookup counts

2. **Medium Collections (1,000 - 10,000 items)**:
   - Break-even point occurs around 100-1000 lookups
   - HashSet creation cost becomes more justified with frequent lookups

3. **Large Collections (≥ 100,000 items)**:
   - HashSet creation cost is very high but pays off with many lookups
   - List performance degrades significantly with lookup count

### Recommendations

#### Use `List<T>` when:
- Collection size < 1,000 items AND lookup count < 100
- Collection size < 10,000 items AND lookup count < 1,000
- Memory usage is a primary concern
- You need indexed access to elements

#### Use `HashSet<T>` when:
- You need to perform many lookups (>100 for medium collections, >1000 for large collections)
- Lookup performance is more critical than creation time
- You can tolerate higher memory usage
- You don't need duplicate values

#### Special Considerations
- For one-time operations with few lookups, `List<T>` is almost always better
- For long-lived collections with frequent Contains() calls, `HashSet<T>` is superior
- Consider the total lifetime of your collection when making the choice

---

## Extended Collection Performance Analysis

This comprehensive benchmark compares performance across multiple collection types with different data types, providing insights for real-world scenarios.

### Test Configuration
- **Collection Sizes**: 1,000 and 10,000 elements
- **Lookup Counts**: 100 and 1,000 lookups per test
- **Data Types**: int, string, Guid, custom struct (ProductId), custom class (Customer)
- **Collections Tested**: List, HashSet, SortedSet, Dictionary, SortedDictionary, ConcurrentDictionary, ImmutableList, ImmutableHashSet, Array

### Performance Summary by Collection Type

#### Hash-Based Collections (Best for Lookups)
| Collection | 1K items, 100 lookups | 1K items, 1K lookups | 10K items, 100 lookups | 10K items, 1K lookups |
|:---|---:|---:|---:|---:|
| **HashSet<int>** | 252ns | 2.5μs | 254ns | 3.0μs |
| **Dictionary<int,bool>** | 241ns | 2.4μs | 251ns | 3.0μs |
| **ConcurrentDictionary<int,bool>** | 184ns | 2.0μs | 203ns | 2.1μs |

#### Linear Search Collections (Worst for Lookups)
| Collection | 1K items, 100 lookups | 1K items, 1K lookups | 10K items, 100 lookups | 10K items, 1K lookups |
|:---|---:|---:|---:|---:|
| **List<int>** | 5.8μs | 61μs | 48μs | 480μs |
| **Array<int>** | 5.9μs | 60μs | 48μs | 473μs |
| **ImmutableList<int>** | 234μs | 2.4ms | 2.8ms | 28ms |

#### Tree-Based Collections (Moderate Performance)
| Collection | 1K items, 100 lookups | 1K items, 1K lookups | 10K items, 100 lookups | 10K items, 1K lookups |
|:---|---:|---:|---:|---:|
| **SortedSet<int>** | 1.5μs | 38μs | 1.6μs | 68μs |
| **SortedDictionary<int,bool>** | 1.5μs | 40μs | 2.0μs | 68μs |

### Performance by Data Type

#### Integer Collections (Baseline - Fastest)
```
HashSet<int>: ~250ns per 100 lookups (regardless of collection size)
List<int>: 5.8μs → 480μs (scales linearly with size)
```

#### String Collections (Moderate Overhead)
```
HashSet<string>: ~613ns per 100 lookups (2.4x slower than int)
List<string>: 173μs → 46ms (string comparison overhead)
SortedSet<string>: 33μs → 551μs (string comparison + tree traversal)
```

#### Guid Collections (Efficient Structs)
```
HashSet<Guid>: ~277ns per 100 lookups (similar to int performance)
List<Guid>: 52μs → 5.6ms (struct comparison overhead)
```

#### Custom Struct Collections (ProductId)
```
HashSet<ProductId>: ~250ns per 100 lookups (excellent performance)
List<ProductId>: 25μs → 2.5ms (custom equality comparison)
```

#### Custom Class Collections (Customer - Reference Types)
```
HashSet<Customer>: ~1.1μs → 14μs (reference equality + hash computation)
List<Customer>: 378μs → 66ms (worst performance due to reference comparison)
```

### Key Performance Insights

#### 1. Hash Collections Dominate for Lookups
- **HashSet/Dictionary**: Consistent O(1) performance regardless of collection size
- **Performance advantage**: 100-1000x faster than linear search for large collections
- **ConcurrentDictionary**: Surprisingly fastest, even for single-threaded scenarios

#### 2. Data Type Impact Rankings (Best to Worst)
1. **Primitive types (int)**: Fastest across all collection types
2. **Small structs (Guid, ProductId)**: Near-primitive performance
3. **Strings**: ~2-3x overhead due to string hashing/comparison
4. **Custom classes**: Highest overhead due to reference equality

#### 3. Collection Type Performance Tiers

**Tier 1 - Excellent (Sub-microsecond)**
- HashSet<T>
- Dictionary<TKey, TValue>
- ConcurrentDictionary<TKey, TValue>

**Tier 2 - Good (Low microseconds)**
- SortedSet<T> (for small collections)
- SortedDictionary<TKey, TValue> (for small collections)
- ImmutableHashSet<T>

**Tier 3 - Poor (High microseconds to milliseconds)**
- List<T>.Contains()
- Array search
- SortedSet<T> (for large collections)

**Tier 4 - Terrible (Milliseconds)**
- ImmutableList<T>.Contains()

### Scalability Analysis

#### Hash Collections: O(1) - Constant Time
```
HashSet performance remains constant:
- 1K items: ~250ns
- 10K items: ~250ns
- Performance difference: <5%
```

#### Linear Collections: O(n) - Linear Growth
```
List performance scales linearly:
- 1K items: 61μs (1K lookups)
- 10K items: 480μs (1K lookups)
- Performance degradation: 8x worse
```

#### Tree Collections: O(log n) - Logarithmic Growth
```
SortedSet performance scales logarithmically:
- 1K items: 38μs (1K lookups)
- 10K items: 68μs (1K lookups)
- Performance degradation: 1.8x worse
```

### Memory Considerations

#### Memory Efficiency Rankings
1. **Array/List**: Most memory efficient
2. **SortedSet/SortedDictionary**: Moderate overhead (tree structure)
3. **HashSet/Dictionary**: Higher overhead (hash buckets)
4. **ConcurrentDictionary**: Highest overhead (thread-safety structures)

### Real-World Recommendations

#### Use HashSet<T>/Dictionary<TKey,TValue> when:
- You need frequent lookups (>10 lookups per collection lifetime)
- Collection size > 100 items
- Lookup performance is critical
- You don't need ordered data

#### Use List<T> when:
- Few lookups (<10 per collection lifetime)
- Small collections (<100 items)
- Memory usage is critical
- You need indexed access
- You need ordered data

#### Use SortedSet<T>/SortedDictionary<TKey,TValue> when:
- You need both lookups AND sorted iteration
- Moderate lookup performance is acceptable
- Collection size is small-to-medium (<10K items)

#### Avoid ImmutableList<T> for lookups:
- 100-10,000x slower than alternatives
- Only use when immutability is absolutely required
- Consider ImmutableHashSet<T> as alternative

#### Consider ConcurrentDictionary<TKey,TValue> when:
- You need thread-safe operations
- Surprisingly good performance even for single-threaded scenarios
- Can tolerate higher memory usage

### Performance Impact Examples

#### Small Collection (1K items, 100 lookups)
```csharp
// Excellent choices (sub-microsecond)
HashSet<int>.Contains()     // 252ns
Dictionary<int,bool>        // 241ns
ConcurrentDictionary        // 184ns

// Poor choices (multiple microseconds)
List<int>.Contains()        // 5.8μs (23x slower)
SortedSet<int>.Contains()   // 1.5μs (6x slower)

// Terrible choice
ImmutableList<int>          // 234μs (1000x slower)
```

#### Large Collection (10K items, 1K lookups)
```csharp
// The performance gap widens dramatically:
HashSet<int>                // 3.0μs
List<int>                   // 480μs (160x slower!)
ImmutableList<int>          // 28ms (9,300x slower!)
```

### Conclusion

For lookup operations, **hash-based collections (HashSet, Dictionary) are almost always the right choice** unless you have very specific requirements. The performance difference becomes more dramatic as collection size and lookup frequency increase, making the choice of collection type one of the most impactful performance decisions in C# development.
