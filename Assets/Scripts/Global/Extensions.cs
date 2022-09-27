using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> list) =>
        list is null
        ? true
        : list.Count() == 0;

    public static bool Detected(this RaycastHit2D? hit) =>
        hit is null
        ? false
        : hit.Value;
}