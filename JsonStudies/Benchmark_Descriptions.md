# Benchmark Descriptions

This document explains the different benchmarks run in `MassiveHierarchyBenchmarks.cs`.

## Overview

The goal is to compare the performance of modifying a large object hierarchy (~600 fields) using three different strategies:

1.  **JSON DOM (`System.Text.Json.Nodes`)**:
    *   Parses JSON into a `JsonNode` tree.
    *   Modifies the tree directly.
    *   Serializes the tree back to string.
    *   *Pros*: No strong typing overhead, flexible.
    *   *Cons*: Traversal can be slow without an index; weak typing.

2.  **POCO Tree (No Index)**:
    *   Deserializes JSON into strongly typed C# objects (`Page`, `Section`, `Card`, `Field`).
    *   Modifies the object graph by traversing lists to find the target ID.
    *   Serializes the object graph back to string.
    *   *Pros*: Standard OOP, low memory overhead compared to indexing.
    *   *Cons*: Finding an item by ID requires O(N) traversal.

3.  **POCO Tree (Fully Indexed)**:
    *   Deserializes JSON into strongly typed C# objects.
    *   **Builds a Dictionary Index** mapping IDs to objects (`Dictionary<Guid, Field>`, etc.).
    *   Modifies the object graph by looking up the target ID in the dictionary (O(1)).
    *   Serializes the object graph back to string.
    *   *Pros*: O(1) lookups for modifications.
    *   *Cons*: High setup cost (building the index) and higher memory usage.

## Benchmark Groups

### Group 1: Full Workflow
Simulates a complete API request lifecycle: **Parse JSON -> Modify Field -> Serialize JSON**.
*   **Workflow_JsonDOM**: Uses `JsonNode`.
*   **Workflow_Poco_NoIndex**: Uses `Page` model and traversal.
*   **Workflow_Poco_Indexed**: Uses `IndexedPage` (builds index on every request).

### Group 2: Parsing
Measures the time to go from a JSON string to the in-memory representation.
*   **Parse_JsonDOM**: `JsonNode.Parse()`.
*   **Parse_Poco_NoIndex**: `JsonSerializer.Deserialize<Page>()`.
*   **Parse_Poco_Indexed**: Deserialize + `new IndexedPage()` (Index construction).

### Group 3: Field Replacement (In-Memory)
Measures the time to find and update a single field's value when the object is **already in memory**.
*   **ModifyField_JsonDOM**: Traverses the `JsonNode` tree to find the ID.
*   **ModifyField_Poco_NoIndex**: Traverses the `Page` object tree.
*   **ModifyField_Poco_Indexed**: Uses Dictionary lookup (O(1)).

### Group 4: Section Replacement (In-Memory)
Measures the time to find and replace an entire `Section` object when the object is **already in memory**.
*   **ModifySection_JsonDOM**: Finds the section in the `JsonArray` and replaces it with a new `JsonNode`.
*   **ModifySection_Poco_NoIndex**: Finds the section in the `List<Section>` and replaces it.
*   **ModifySection_Poco_Indexed**: Updates the `List<Section>` AND updates the Dictionary Index (removing old keys, adding new keys).

### Group 5: Serialization
Measures the time to convert the in-memory representation back to a JSON string.
*   **Serialize_JsonDOM**: `JsonNode.ToJsonString()`.
*   **Serialize_Poco_NoIndex**: `JsonSerializer.Serialize(Page)`.
*   **Serialize_Poco_Indexed**: `JsonSerializer.Serialize(Page)` (same as NoIndex).

## Hypothesis

*   **Single Request**: The "No Index" approach (C01) should be faster than "With Index" (C02) because the cost of building the index (D01) is likely higher than the cost of a single traversal (D02).
*   **Multiple Requests**: If the object stays in memory, the "With Index" approach (D03) is orders of magnitude faster than Traversal (D02).
