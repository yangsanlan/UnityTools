using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameHelper
{
    /// <summary>
    ///     分割字符串为字典int_int
    /// </summary>
    /// <param name="source">格式a_b|c_d</param>
    /// <param name="blockchar">块与块间隔</param>
    /// <param name="valuechar">key.value间隔</param>
    /// <returns></returns>
    public static Dictionary<int, int> StringToDicIntInt(string source, char blockchar = '|', char valuechar = '_')
    {
        var result = new Dictionary<int, int>();
        var temps = source.Split(blockchar);

        for (var i = 0; i < temps.Length; i++)
        {
            var reTemp = temps[i].Split(valuechar);
            var key = int.Parse(reTemp[0]);
            var value = int.Parse(reTemp[1]);
            result[key] = value;
        }

        return result;
    }

    public static string ListIntToString(List<int> source)
    {
        var result = "";
        if (source != null && source.Count >= 0)
        {
            for (var i = 0; i < source.Count; i++)
            {
                result += source[i] + "_";
            }
            result = result.Substring(0, result.Length - 1);
        }

        return result;
    }

    /// <summary>
    ///     随机一个最大值为max最小值min且长度为max-min的数组
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int[] RandomArray(int min, int max)
    {
        return RandomArray(min, max, max - min);
    }

    /// <summary>
    ///     随机一个最大值为max最小值min且长度为length的数组
    /// </summary>
    public static int[] RandomArray(int min, int max, int length)
    {
        if (length <= 0 || max < min || (max - min) < length) return null;
        var temp = new int[length];
        var array = new List<int>();
        for (var i = min; i < max; i++)
        {
            array.Add(i);
        }

        for (var i = 0; i < length; i++)
        {
            var index = Random.Range(0, array.Count);
            temp[i] = array[index];
            array.RemoveAt(index);
        }
        return temp;
    }

    /// <summary>
    ///     随机一个最大值为max最小值min且长度为length,且包含num的数组
    /// </summary>
    public static int[] RandomArray(int min, int max, int length, int num)
    {
        if (length <= 0 || max < min || (max - min) < length) return null;
        var temp = new int[length];
        var array = new List<int>();
        for (var i = min; i < max; i++)
        {
            if (i != num)
                array.Add(i);
        }

        for (var i = 0; i < length - 1; i++)
        {
            var index = Random.Range(0, array.Count);
            temp[i] = array[index];
            array.RemoveAt(index);
        }
        temp[length - 1] = num;
        return temp;
    }

    /// <summary>
    ///     调用java代码
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    public static void JavaCall(string methodName, params object[] args)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        using (var ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var jo = ajc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (jo == null)
                {
                    Debugger.LogError("java对象未获取到");
                    return;
                }
                jo.Call(methodName, args);
            }
        }
    }

    /// <summary>
    ///     调用返回值为string的java代码
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static string JavaCall(string methodName)
    {
        using (var ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var jo = ajc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (jo == null)
                {
                    Debugger.LogError("java对象未获取到");
                    return "";
                }
                return jo.Call<string>(methodName);
            }
        }
    }

    public static byte[] Encrypt(byte[] buffer, string key)
    {
        using (AesManaged aes = new AesManaged())
        {
            byte[] code = Encoding.UTF8.GetBytes(key);
            var encryptor = aes.CreateEncryptor(code, code);
            var temp = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return temp;
        }
    }

    public static byte[] Decrypt(byte[] input, string key)
    {
        using (AesManaged aes = new AesManaged())
        {
            byte[] code = Encoding.UTF8.GetBytes(key);
            try
            {
                var decryptor = aes.CreateDecryptor(code, code);
                var temp = decryptor.TransformFinalBlock(input, 0, input.Length);
                return temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public IEnumerator WaitForRealSeconds(float time)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < time)
        {
            yield return null;
        }
    }
}