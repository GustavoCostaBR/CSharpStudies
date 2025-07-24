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
