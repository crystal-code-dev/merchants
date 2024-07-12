
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class KeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public KeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}