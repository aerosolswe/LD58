using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class GenericExtensions
{
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Light2D target, Color endValue, float duration)
    {
        TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }

    public static string FormatPercent(this float amount, int maxDecimals = 0)
    {
        float percent = amount * 100f;

        // If it's effectively an integer, just show whole number
        if (Mathf.Approximately(percent, Mathf.Round(percent)))
            return Mathf.RoundToInt(percent) + "%";

        // Otherwise format with decimals, trimming trailing zeros
        string formatted = percent.ToString($"F{maxDecimals}").TrimEnd('0');
        if (formatted.EndsWith(".")) // avoid dangling dot
            formatted = formatted.TrimEnd('.');

        return formatted + "%";
    }

    public static string FormatCurrency(this string s, long amount)
    {
        FormattableString message = $"{amount:N0}";
        s = FormattableString.Invariant(message);
        return s;
    }

    public static string FormatCurrency(this long amount)
    {
        FormattableString message = $"{amount:N0}";
        string s = FormattableString.Invariant(message);
        return s;
    }

    public static string FormatCurrency(this string s, int amount)
    {
        FormattableString message = $"{amount:N0}";
        s = FormattableString.Invariant(message);
        return s;
    }

    public static string FormatCurrency(this int amount)
    {
        FormattableString message = $"{amount:N0}";
        string s = FormattableString.Invariant(message);
        return s;
    }

    public static string FormatTimeLeft(this TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0)
            timeSpan = TimeSpan.Zero;

        string result = "";

        if (timeSpan.TotalHours >= 1)
        {
            result += (int)timeSpan.TotalHours + "h";
        }

        if (timeSpan.TotalMinutes >= 1)
        {
            if (result.Length > 0)
                result += " ";
            result += timeSpan.Minutes + "m";
        }

        if (timeSpan.TotalHours < 1 && timeSpan.TotalSeconds >= 1)
        {
            if (result.Length > 0)
                result += " ";
            result += timeSpan.Seconds + "s";
        }

        return result;
    }

    public static T ToEnum<T>(this string str)
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T)));
        }

        var enumString = str.Replace("-", "_");
        T value;

        try
        {
            value = (T)Enum.Parse(typeof(T), enumString, true);
        } catch
        {
            Debug.LogErrorFormat("Could not convert '{0}' to enum type '{1}'.", enumString, typeof(T).Name);
            value = default(T);
        }

        return value;
    }

    public static float ToFloat(this string str)
    {
        float result;
        if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
        {
            Debug.LogErrorFormat("Could not convert '{0}' to float.", str);
        }

        return result;
    }

    public static int ToInt(this string str)
    {
        int result;
        if (!int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
        {
            Debug.LogErrorFormat("Could not convert '{0}' to int.", str);
        }

        return result;
    }

    public static bool ToBool(this string str)
    {
        string lowerCaseStr = str.ToLower();
        if (lowerCaseStr.Equals("1") || lowerCaseStr.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        } else if (lowerCaseStr.Equals("0") || lowerCaseStr.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        Debug.LogErrorFormat("Could not convert '{0}' to bool.", lowerCaseStr);
        return false;
    }

    public static void AddOrChange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = val;
        } else
        {
            dict.Add(key, val);
        }
    }

    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, val);
        }

        return dict[key];
    }

    // seconds
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    // seconds utc
    public static long ToUTCUnixTimestamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    public static DateTime ToUtcDateTime(this long unixDate)
    {
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return start.AddSeconds(unixDate);
    }

    public static DateTime ToLocalDateTime(this long unixDate)
    {
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        return start.AddSeconds(unixDate);
    }

    public static List<T> ToList<T>(this T[] array)
    {
        var list = new List<T>(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
        return list;
    }
}