using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRandoms
{
    public static int RangeRandom(int min, int max)
    {
        return min + UnityEngine.Random.Range(min, max);
    }
    public static T Roulette<T>(Dictionary<T, int> items)
    {
        int total = 0;
        foreach (var item in items)
        {
            total += item.Value;
        }
        //var random = Random.Range(0, total);
        var random = RangeRandom(0, total);
        foreach (var item in items)
        {
            if (random <= item.Value)
            {
                return item.Key;
            }
            else
            {
                random -= item.Value;
            }
        }
        return default(T);
    }
    public static void Shuffle<T>(T[] items, Action<T, T> onSwap = null)
    {
        for (int i = 0; i < items.Length; i++)
        {
            int random = UnityEngine.Random.Range(i, items.Length);
            if (onSwap != null)
            {
                onSwap(items[i], items[random]);
            }
            T randomItem = items[i];
            items[i] = items[random];
            items[random] = randomItem;
        }
    }
    public static void Shuffle<T>(List<T> items, Action<T, T> onSwap = null)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int random = UnityEngine.Random.Range(i, items.Count);
            if (onSwap != null)
            {
                onSwap(items[i], items[random]);
            }
            T randomItem = items[i];
            items[i] = items[random];
            items[random] = randomItem;
        }
    }
}

