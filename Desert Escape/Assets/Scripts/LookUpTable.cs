using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUpTable<T, U>
{
    Dictionary<T, U> _cache = new Dictionary<T, U>();
    Func<T, U> _operation;
    public LookUpTable(Func<T, U> operation)
    {
        _operation = operation;
    }
    public U Run(T key)
    {
        if (!_cache.ContainsKey(key))
        {
            _cache[key] = _operation(key);
        }
        return _cache[key];
    }
}
