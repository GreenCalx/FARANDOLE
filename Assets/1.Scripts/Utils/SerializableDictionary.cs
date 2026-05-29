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

    public SerializableKeyValuePair()
    {
        Key = default;
        Value = default;
    }

    public SerializableKeyValuePair(TKey iKey, TValue iValue)
    {
        Key = iKey;
        Value = iValue;
    }
}


[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableKeyValuePair<TKey, TValue>> items = new();

    private Dictionary<TKey, TValue> dictionary = new();

    public Dictionary<TKey, TValue> Dictionary => dictionary;

    public void OnBeforeSerialize()
    {
        items.Clear();
        foreach (var kvp in dictionary)
        {
            items.Add(new SerializableKeyValuePair<TKey, TValue>
            {
                Key = kvp.Key,
                Value = kvp.Value
            });
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        foreach (var kvp in items)
        {
            if (kvp.Key == null)
            {
                // no serializable data found
                Debug.Log("null key skipped"); 
                continue;
            }
            dictionary[kvp.Key] = kvp.Value;
        }
    }

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }
}
