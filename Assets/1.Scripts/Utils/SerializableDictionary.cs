using System;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine;

[Serializable]
public class SerializableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}



[Serializable]
public class SerializableDictionary<TKey, TValue>
    : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableKeyValuePair<TKey, TValue>> items = new();

    private Dictionary<TKey, TValue> dictionary = new();

    public Dictionary<TKey, TValue> Dictionary => dictionary;

    // ──────────────────────────────────────────────
    // Inspector → Runtime
    // ──────────────────────────────────────────────
    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        foreach (var kvp in items)
        {
            if (kvp.Key == null)
                continue;

            dictionary[kvp.Key] = kvp.Value;
        }
    }

    // ──────────────────────────────────────────────
    // Runtime → Inspector (ONLY when needed)
    // ──────────────────────────────────────────────
    public void OnBeforeSerialize()
    {
        // Do nothing here.
        // Let the inspector control `items`.
    }

    // ──────────────────────────────────────────────
    // Runtime API
    // ──────────────────────────────────────────────
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set
        {
            dictionary[key] = value;
            SyncToItems();
        }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        SyncToItems();
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    private void SyncToItems()
    {
        items.Clear();
        foreach (var kv in dictionary)
        {
            items.Add(new SerializableKeyValuePair<TKey, TValue>
            {
                Key = kv.Key,
                Value = kv.Value
            });
        }
    }
}

