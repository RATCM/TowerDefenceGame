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

    public static Vector2 PointDircetion(this GameObject from, GameObject to) =>
        to.transform.position - from.transform.position;

    public static IEnumerable<GameObject> SortByClosest(this IEnumerable<GameObject> list, GameObject target)
    {
        var temp = list.ToList();
        temp.Sort(delegate (GameObject x, GameObject y)
        {
            return Vector2.Distance(x.transform.position, target.transform.position).CompareTo(Vector2.Distance(y.transform.position, target.transform.position));
        });

        return temp;
    }
}