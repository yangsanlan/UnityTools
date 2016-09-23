//
//  ExtensionHelper.cs
//  UnityTools
//
//  Created by JYMain on 9/23/2016.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class ExtensionHelper
{
    /// <summary>
    /// 得到字符串中的数字，返回数字字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isbreak">得到第一个非数字是是否断掉</param>
    /// <returns></returns>
    public static string GetNumeric(this string str, bool isbreak)
    {
        Regex r = new Regex("\\d+");
        var ms = r.Matches(str);
        if (ms.Count > 0)
        {
            Match[] match = ms.OfType<Match>().ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (var item in match)
            {
                sb.Append(item.Value);
                if (isbreak)
                    return sb.ToString();
            }
            return sb.ToString();
        }
        else
            return str;
    }

    /// <summary>
    /// 字符串转化为List int
    /// </summary>
    /// <param name="src"></param>
    /// <param name="split"></param>
    /// <returns></returns>
    public static List<int> ToListInt(this string src, char split = '_')
    {
        List<int> result = new List<int>();
        if (string.IsNullOrEmpty(src)) return result;
        string[] splits = src.Split(split);
        for (int i = 0; i < splits.Length; i++)
        {
            result.Add(int.Parse(splits[i]));
        }
        return result;
    }

    /// <summary>
    ///     分割字符串为字典int_int
    /// </summary>
    /// <param name="source">格式a_b|c_d</param>
    /// <returns></returns>
    public static Dictionary<int, int> ToDictionaryIntInt(this string source)
    {
        var result = new Dictionary<int, int>();
        var temps = source.Split('|');

        for (var i = 0; i < temps.Length; i++)
        {
            var reTemp = temps[i].Split('_');
            var key = int.Parse(reTemp[0]);
            var value = int.Parse(reTemp[1]);
            result[key] = value;
        }

        return result;
    }
}
