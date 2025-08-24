using System.Collections.Concurrent;

namespace EventBusTest;

public class HandlerCollection
{
    private readonly ConcurrentDictionary<string, List<Delegate>> _handlers = new();
    
    public void AtomicAddHandler(string eventName, Delegate handler)
    {
        while (true)
        {
            if (_handlers.TryGetValue(eventName, out var existingList))
            {
                var updatedList = DeepCopyListAddingHandler(existingList, handler);
                if (_handlers.TryUpdate(eventName, updatedList, existingList))
                    return;
            }
            else
            {
                var newList = new List<Delegate> { handler };
                if (_handlers.TryAdd(eventName, newList))
                    return;
            }
        }
    }

    public void AtomicRemoveHandler(string eventName, Delegate handler)
    {
        while (true)
        {
            if (!_handlers.TryGetValue(eventName, out var existingList))
                return;
            if (existingList.Count == 0)
                return;

            var (updatedList, removed) = DeepCopyListRemovingFirstHandlerMatch(existingList, handler);
            if (!removed)
                return;

            if (updatedList.Count == 0)
            {
                if (_handlers.TryRemove(new KeyValuePair<string, List<Delegate>>(eventName, existingList)))
                    return;
            }
            else
            {
                if (_handlers.TryUpdate(eventName, updatedList, existingList))
                    return;
            }
        }
    }

    public Delegate[] Snapshot(string eventName)
    {
        return !_handlers.TryGetValue(eventName,
            out var existing)
            ? []
            : existing.ToArray();
    }

    private static List<Delegate> DeepCopyListAddingHandler(List<Delegate> existingList, Delegate handler)
    {
        var updatedList = new List<Delegate>(existingList.Count + 1);
        updatedList.AddRange(existingList);
        updatedList.Add(handler);
        return updatedList;
    }

    private static DeepCopyReturn DeepCopyListRemovingFirstHandlerMatch(List<Delegate> existingList,
        Delegate handler)
    {
        var updatedList = new List<Delegate>(existingList.Count);
        var removed = false;
        foreach (var delegate_ in existingList)
        {
            if (!removed && Equals(delegate_, handler))
            {
                removed = true;
                continue;
            }

            updatedList.Add(delegate_);
        }

        return new DeepCopyReturn(updatedList, removed);
    }

    private record DeepCopyReturn(List<Delegate> Updated, bool Removed);
}