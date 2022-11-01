using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;

public class StringValue : Attribute
{
    public string Value { get; protected set; }

    public StringValue(string value)
    {
        this.Value = value;
    }

}
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

    // https://weblogs.asp.net/stefansedich/enum-with-string-values-in-c
    public static string GetStringValue(this Enum value)
    {
        Type type = value.GetType();

        FieldInfo fieldInfo = type.GetField(value.ToString());

        StringValue[] attributes = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];

        return attributes.Length > 0 ? attributes[0].Value : value.ToString();
    }

    public static Vector2 PointDirection(this GameObject from, GameObject to) =>
        (to.transform.position - from.transform.position).normalized;

    public static IEnumerable<GameObject> SortByClosest(this IEnumerable<GameObject> list, GameObject target)
    {
        var temp = list.ToList();
        temp.Sort(delegate (GameObject x, GameObject y)
        {
            return Vector2.Distance(x.transform.position, target.transform.position).CompareTo(Vector2.Distance(y.transform.position, target.transform.position));
        });

        return temp;
    }

    public static void TryRemoveEffect(this List<EnemyEffect> list, Func<EnemyEffect, bool> predicate)
    {
        foreach(var effect in list)
        {
            if (predicate(effect))
            {
                list.Remove(effect);
            }
        }
    }

    /// <summary>
    /// This method enumerates through a list of GameObjects and
    /// returns the closest one to the target GameObject
    /// </summary>
    /// <param name="list">The list of gameobject to get the closest from</param>
    /// <param name="target">The GameObject we compare the list to</param>
    /// <returns>The closest GameObject</returns>
    public static GameObject GetClosest(this IEnumerable<GameObject> list, GameObject target)
    {
        GameObject closest = list.First();

        foreach(var obj in list)
        {

            // this is faster than taking the actual magnitude since we dont take the square root
            var v1 = obj.transform.position - target.transform.position;
            float v1Dist = v1.x * v1.x + v1.y * v1.y;

            var v2 = closest.transform.position - target.transform.position;
            float v2Dist = v2.x * v2.x + v2.y * v2.y;

            if(v1Dist < v2Dist)
            {
                closest = obj;
            }
        }

        return closest;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector3 Rotate2D(this Vector3 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float Distance2D(this GameObject self, GameObject target) =>
        Vector2.Distance(self.transform.position, target.transform.position);
}