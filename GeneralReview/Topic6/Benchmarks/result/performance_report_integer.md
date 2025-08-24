# Statistical Performance Analysis Report
Generated on: 2025-07-26 15:26:29

## Creation Time Variance Analysis

### Theory: Why Creation Time Varies with Lookup Count
Even though creation should be independent of lookup count, several factors cause variance:

1. **Memory Layout Optimization**: JIT compiler may optimize differently based on expected usage patterns
2. **CPU Cache State**: Previous iterations affect CPU cache state, influencing memory allocation
3. **Garbage Collection Timing**: Different GC pressure from previous tests affects allocation patterns
4. **Heap Fragmentation**: Memory fragmentation from previous iterations affects new allocations
5. **Branch Prediction**: CPU branch predictor state varies based on previous operations
6. **Memory Pre-allocation**: Some collections pre-allocate based on detected usage patterns

### Creation Time Variance by Collection Type:
- **Array**: High Variance (CV: 56.4%)
- **List**: High Variance (CV: 75.3%)
- **Dictionary**: Extremely Variable (CV: 137.8%)
- **ImmutableList**: Extremely Variable (CV: 137.8%)
- **HashSet**: Extremely Variable (CV: 139.2%)
- **ConcurrentDictionary**: Extremely Variable (CV: 143.2%)
- **SortedSet**: Extremely Variable (CV: 149.3%)
- **SortedDictionary**: Extremely Variable (CV: 151.3%)
- **ImmutableHashSet**: Extremely Variable (CV: 154.7%)

## Scenario: N=10, Lookups=10

### Total Time Performance Ranking:
🥇 **List**: 1.1 μs (±0.1, 2 outliers removed)
🥈 **Array**: 1.4 μs (±0.3, 1 outliers removed)
🥉 **HashSet**: 2.1 μs (±0.3, 1 outliers removed)
   **Dictionary**: 2.2 μs (±0.2, 1 outliers removed)
   **ImmutableList**: 2.9 μs (±0.1, 5 outliers removed)
   **SortedSet**: 3.6 μs (±0.3, 1 outliers removed)
   **ImmutableHashSet**: 6.3 μs (±0.3, 3 outliers removed)
   **ConcurrentDictionary**: 6.3 μs (±0.4, 3 outliers removed)
   **SortedDictionary**: 6.9 μs (±0.3, 2 outliers removed)

### Reliability Analysis:
- **List**: CV = 13.5% (Moderate)
- **Array**: CV = 18.7% (Moderate)
- **HashSet**: CV = 13.1% (Moderate)

## Scenario: N=10, Lookups=100

### Total Time Performance Ranking:
🥇 **List**: 1.9 μs (±0.1, 3 outliers removed)
🥈 **Array**: 2.4 μs (±0.3, 1 outliers removed)
🥉 **HashSet**: 3.5 μs (±0.3, 2 outliers removed)
   **Dictionary**: 3.6 μs (±0.3, 1 outliers removed)
   **ConcurrentDictionary**: 8.8 μs (±0.4, 2 outliers removed)
   **ImmutableList**: 9.2 μs (±0.2, 2 outliers removed)
   **SortedSet**: 12.6 μs (±1.2, 1 outliers removed)
   **ImmutableHashSet**: 12.9 μs (±0.9, 3 outliers removed)
   **SortedDictionary**: 16.9 μs (±0.9, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 6.5% (Reliable)
- **Array**: CV = 13.8% (Moderate)
- **HashSet**: CV = 9.2% (Reliable)

## Scenario: N=10, Lookups=1000

### Total Time Performance Ranking:
🥇 **List**: 29.1 μs (±7.6, 1 outliers removed)
🥈 **Array**: 29.2 μs (±6.5, 1 outliers removed)
🥉 **Dictionary**: 33.5 μs (±5.7, 1 outliers removed)
   **HashSet**: 35.1 μs (±5.9, 1 outliers removed)
   **ConcurrentDictionary**: 35.6 μs (±4.6, 1 outliers removed)
   **ImmutableHashSet**: 74.3 μs (±5.3, 1 outliers removed)
   **SortedSet**: 98.1 μs (±7.2, 1 outliers removed)
   **SortedDictionary**: 144.6 μs (±11.7, 1 outliers removed)
   **ImmutableList**: 186.4 μs (±52.6, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 26.0% (Unreliable)
- **Array**: CV = 22.3% (Unreliable)
- **Dictionary**: CV = 17.0% (Moderate)

## Scenario: N=10, Lookups=10000

### Total Time Performance Ranking:
🥇 **Array**: 34.7 μs (±1.0, 4 outliers removed)
🥈 **List**: 36.5 μs (±1.5, 1 outliers removed)
🥉 **Dictionary**: 111.3 μs (±1.8, 1 outliers removed)
   **HashSet**: 114.3 μs (±1.7, 2 outliers removed)
   **ConcurrentDictionary**: 145.7 μs (±4.0, 3 outliers removed)
   **SortedSet**: 154.6 μs (±2.8, 2 outliers removed)
   **ImmutableHashSet**: 197.4 μs (±6.1, 4 outliers removed)
   **SortedDictionary**: 303.0 μs (±4.5, 3 outliers removed)
   **ImmutableList**: 1590.7 μs (±30.6, 2 outliers removed)

### Reliability Analysis:
- **Array**: CV = 3.0% (Very Reliable)
- **List**: CV = 4.1% (Very Reliable)
- **Dictionary**: CV = 1.6% (Very Reliable)

## Scenario: N=100, Lookups=10

### Total Time Performance Ranking:
🥇 **List**: 1.1 μs (±0.1, 2 outliers removed)
🥈 **Array**: 1.4 μs (±0.2, 1 outliers removed)
🥉 **HashSet**: 4.2 μs (±0.6, 1 outliers removed)
   **SortedSet**: 8.1 μs (±0.6, 1 outliers removed)
   **Dictionary**: 8.1 μs (±1.1, 1 outliers removed)
   **ConcurrentDictionary**: 19.7 μs (±1.6, 2 outliers removed)
   **ImmutableList**: 26.7 μs (±1.3, 2 outliers removed)
   **SortedDictionary**: 50.5 μs (±3.7, 1 outliers removed)
   **ImmutableHashSet**: 60.0 μs (±5.8, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 8.9% (Reliable)
- **Array**: CV = 14.8% (Moderate)
- **HashSet**: CV = 14.1% (Moderate)

## Scenario: N=100, Lookups=100

### Total Time Performance Ranking:
🥇 **List**: 2.4 μs (±0.3, 2 outliers removed)
🥈 **Array**: 2.9 μs (±0.5, 1 outliers removed)
🥉 **HashSet**: 7.0 μs (±0.8, 2 outliers removed)
   **Dictionary**: 9.7 μs (±0.6, 2 outliers removed)
   **SortedSet**: 14.8 μs (±0.8, 4 outliers removed)
   **ConcurrentDictionary**: 25.0 μs (±1.1, 3 outliers removed)
   **ImmutableHashSet**: 71.5 μs (±3.1, 3 outliers removed)
   **SortedDictionary**: 73.8 μs (±1.9, 2 outliers removed)
   **ImmutableList**: 188.3 μs (±6.5, 3 outliers removed)

### Reliability Analysis:
- **List**: CV = 12.4% (Moderate)
- **Array**: CV = 15.9% (Moderate)
- **HashSet**: CV = 12.0% (Moderate)

## Scenario: N=100, Lookups=1000

### Total Time Performance Ranking:
🥇 **List**: 13.3 μs (±0.9, 4 outliers removed)
🥈 **Array**: 14.7 μs (±1.0, 4 outliers removed)
🥉 **HashSet**: 24.3 μs (±0.9, 3 outliers removed)
   **Dictionary**: 24.8 μs (±0.7, 3 outliers removed)
   **ConcurrentDictionary**: 46.1 μs (±1.3, 2 outliers removed)
   **SortedSet**: 59.3 μs (±1.9, 1 outliers removed)
   **ImmutableHashSet**: 163.9 μs (±4.0, 2 outliers removed)
   **SortedDictionary**: 289.8 μs (±9.4, 0 outliers removed)
   **ImmutableList**: 1697.9 μs (±41.9, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 6.6% (Reliable)
- **Array**: CV = 7.0% (Reliable)
- **HashSet**: CV = 3.8% (Very Reliable)

## Scenario: N=100, Lookups=10000

### Total Time Performance Ranking:
🥇 **List**: 73.8 μs (±5.1, 1 outliers removed)
🥈 **Array**: 84.3 μs (±3.7, 1 outliers removed)
🥉 **ConcurrentDictionary**: 113.0 μs (±37.6, 1 outliers removed)
   **Dictionary**: 123.8 μs (±3.0, 1 outliers removed)
   **HashSet**: 127.1 μs (±4.1, 1 outliers removed)
   **SortedSet**: 328.9 μs (±7.8, 1 outliers removed)
   **ImmutableHashSet**: 392.3 μs (±46.2, 1 outliers removed)
   **SortedDictionary**: 437.9 μs (±75.5, 1 outliers removed)
   **ImmutableList**: 2405.9 μs (±1185.5, 5 outliers removed)

### Reliability Analysis:
- **List**: CV = 6.9% (Reliable)
- **Array**: CV = 4.4% (Very Reliable)
- **ConcurrentDictionary**: CV = 33.3% (Unreliable)

## Scenario: N=1000, Lookups=10

### Total Time Performance Ranking:
🥇 **List**: 2.1 μs (±0.2, 2 outliers removed)
🥈 **Array**: 2.3 μs (±0.3, 3 outliers removed)
🥉 **HashSet**: 32.1 μs (±2.1, 8 outliers removed)
   **Dictionary**: 33.0 μs (±11.5, 0 outliers removed)
   **ImmutableList**: 35.8 μs (±0.8, 3 outliers removed)
   **ConcurrentDictionary**: 93.8 μs (±18.6, 1 outliers removed)
   **SortedSet**: 112.7 μs (±40.3, 1 outliers removed)
   **SortedDictionary**: 152.5 μs (±21.3, 1 outliers removed)
   **ImmutableHashSet**: 184.1 μs (±6.9, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 8.2% (Reliable)
- **Array**: CV = 11.0% (Moderate)
- **HashSet**: CV = 6.5% (Reliable)

## Scenario: N=1000, Lookups=100

### Total Time Performance Ranking:
🥇 **List**: 7.4 μs (±0.6, 3 outliers removed)
🥈 **Array**: 8.1 μs (±0.7, 1 outliers removed)
🥉 **HashSet**: 12.2 μs (±0.2, 2 outliers removed)
   **Dictionary**: 13.3 μs (±0.3, 1 outliers removed)
   **ConcurrentDictionary**: 44.5 μs (±1.3, 3 outliers removed)
   **SortedSet**: 45.5 μs (±1.0, 3 outliers removed)
   **SortedDictionary**: 111.3 μs (±2.8, 1 outliers removed)
   **ImmutableHashSet**: 156.7 μs (±2.2, 1 outliers removed)
   **ImmutableList**: 233.3 μs (±2.5, 1 outliers removed)

### Reliability Analysis:
- **List**: CV = 8.2% (Reliable)
- **Array**: CV = 9.1% (Reliable)
- **HashSet**: CV = 1.8% (Very Reliable)

## Scenario: N=1000, Lookups=1000

### Total Time Performance Ranking:
🥇 **Dictionary**: 22.1 μs (±1.2, 2 outliers removed)
🥈 **HashSet**: 22.3 μs (±1.4, 2 outliers removed)
🥉 **ConcurrentDictionary**: 50.5 μs (±2.6, 0 outliers removed)
   **List**: 61.0 μs (±3.1, 3 outliers removed)
   **Array**: 71.8 μs (±5.4, 1 outliers removed)
   **SortedSet**: 90.6 μs (±5.4, 0 outliers removed)
   **SortedDictionary**: 158.4 μs (±8.6, 1 outliers removed)
   **ImmutableHashSet**: 196.9 μs (±10.0, 1 outliers removed)
   **ImmutableList**: 2156.5 μs (±111.5, 0 outliers removed)

### Reliability Analysis:
- **Dictionary**: CV = 5.2% (Reliable)
- **HashSet**: CV = 6.5% (Reliable)
- **ConcurrentDictionary**: CV = 5.2% (Reliable)

## Scenario: N=1000, Lookups=10000

### Total Time Performance Ranking:
🥇 **ConcurrentDictionary**: 107.4 μs (±6.9, 3 outliers removed)
🥈 **Dictionary**: 108.9 μs (±6.4, 1 outliers removed)
🥉 **HashSet**: 112.9 μs (±6.2, 2 outliers removed)
   **SortedSet**: 490.4 μs (±20.5, 4 outliers removed)
   **List**: 562.2 μs (±17.3, 5 outliers removed)
   **ImmutableHashSet**: 577.7 μs (±12.6, 3 outliers removed)
   **SortedDictionary**: 581.0 μs (±28.9, 1 outliers removed)
   **Array**: 581.4 μs (±17.8, 4 outliers removed)
   **ImmutableList**: 20001.7 μs (±536.4, 3 outliers removed)

### Reliability Analysis:
- **ConcurrentDictionary**: CV = 6.4% (Reliable)
- **Dictionary**: CV = 5.9% (Reliable)
- **HashSet**: CV = 5.5% (Reliable)

## Scenario: N=10000, Lookups=10

### Total Time Performance Ranking:
🥇 **List**: 7.8 μs (±0.4, 4 outliers removed)
🥈 **Array**: 9.5 μs (±0.5, 3 outliers removed)
🥉 **HashSet**: 110.0 μs (±6.5, 0 outliers removed)
   **Dictionary**: 120.3 μs (±7.0, 0 outliers removed)
   **ImmutableList**: 318.3 μs (±16.8, 0 outliers removed)
   **SortedSet**: 462.0 μs (±26.0, 0 outliers removed)
   **ConcurrentDictionary**: 463.4 μs (±24.9, 0 outliers removed)
   **SortedDictionary**: 1257.6 μs (±65.2, 0 outliers removed)
   **ImmutableHashSet**: 1961.5 μs (±96.6, 0 outliers removed)

### Reliability Analysis:
- **List**: CV = 5.3% (Reliable)
- **Array**: CV = 5.0% (Very Reliable)
- **HashSet**: CV = 5.9% (Reliable)

## Scenario: N=10000, Lookups=100

### Total Time Performance Ranking:
🥇 **List**: 51.2 μs (±2.2, 4 outliers removed)
🥈 **Array**: 62.0 μs (±2.3, 5 outliers removed)
🥉 **HashSet**: 115.7 μs (±3.0, 6 outliers removed)
   **Dictionary**: 126.0 μs (±3.3, 6 outliers removed)
   **ConcurrentDictionary**: 478.5 μs (±12.6, 6 outliers removed)
   **SortedSet**: 482.7 μs (±11.1, 6 outliers removed)
   **SortedDictionary**: 1302.1 μs (±22.0, 7 outliers removed)
   **ImmutableHashSet**: 2022.9 μs (±37.7, 6 outliers removed)
   **ImmutableList**: 2346.8 μs (±37.7, 7 outliers removed)

### Reliability Analysis:
- **List**: CV = 4.2% (Very Reliable)
- **Array**: CV = 3.7% (Very Reliable)
- **HashSet**: CV = 2.6% (Very Reliable)

## Scenario: N=10000, Lookups=1000

### Total Time Performance Ranking:
🥇 **HashSet**: 123.0 μs (±8.9, 1 outliers removed)
🥈 **Dictionary**: 131.8 μs (±7.8, 2 outliers removed)
🥉 **List**: 454.1 μs (±23.1, 1 outliers removed)
   **ConcurrentDictionary**: 456.9 μs (±22.6, 0 outliers removed)
   **Array**: 467.2 μs (±17.4, 2 outliers removed)
   **SortedSet**: 514.5 μs (±25.8, 2 outliers removed)
   **SortedDictionary**: 1291.7 μs (±62.5, 1 outliers removed)
   **ImmutableHashSet**: 1907.0 μs (±43.7, 5 outliers removed)
   **ImmutableList**: 20438.8 μs (±495.2, 5 outliers removed)

### Reliability Analysis:
- **HashSet**: CV = 7.2% (Reliable)
- **Dictionary**: CV = 5.9% (Reliable)
- **List**: CV = 5.1% (Reliable)

## Scenario: N=10000, Lookups=10000

### Total Time Performance Ranking:
🥇 **HashSet**: 225.5 μs (±11.5, 1 outliers removed)
🥈 **Dictionary**: 226.2 μs (±10.9, 2 outliers removed)
🥉 **ConcurrentDictionary**: 520.2 μs (±17.1, 3 outliers removed)
   **SortedSet**: 1126.7 μs (±16.0, 5 outliers removed)
   **SortedDictionary**: 1975.8 μs (±43.3, 4 outliers removed)
   **ImmutableHashSet**: 2563.1 μs (±55.4, 3 outliers removed)
   **List**: 4322.2 μs (±71.4, 4 outliers removed)
   **Array**: 4336.7 μs (±69.6, 2 outliers removed)
   **ImmutableList**: 200710.7 μs (±2277.7, 2 outliers removed)

### Reliability Analysis:
- **HashSet**: CV = 5.1% (Reliable)
- **Dictionary**: CV = 4.8% (Very Reliable)
- **ConcurrentDictionary**: CV = 3.3% (Very Reliable)

