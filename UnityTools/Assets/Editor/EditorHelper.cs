using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class EditorHelper : MonoBehaviour
{

    /// <summary>
    /// 只打包选择的物体
    /// </summary>
    [MenuItem("Tools/加密文件")]
    [MenuItem("Assets/加密文件")]
    static void EncryptFile()
    {
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        string SavePath = EditorUtility.SaveFolderPanel("Save Resource", Application.dataPath + "/Resources/DataConfig", "");//, "bytes"
        if (SavePath.Length != 0 && selection.Length > 0)
        {
            string path = AssetDatabase.GetAssetPath(selection[0]);
            TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            byte[] buffEncrypt = Encrypt(Encoding.UTF8.GetBytes(ta.text), key);
            string path1 = SavePath + "/" + selection[0].name + ".bytes";
            FileStream cfs = new FileStream(path1, FileMode.Create);
            cfs.Write(buffEncrypt, 0, buffEncrypt.Length);

            cfs.Close();
            if (File.Exists(path1))
                EditorUtility.RevealInFinder(path1);
        }
    }

    [MenuItem("Assets/解密文件")]
    static void DecryptFile()
    {
        if (Selection.objects.Length < 1)
        {
            Debug.Log("# 未选中任何文件");
            return;
        }
        try
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            byte[] decyptBytes = Decrypt(ta.bytes, key);
            string decyptString = Encoding.UTF8.GetString(decyptBytes, 0, decyptBytes.Length);
            Debug.Log("# Success");
        }
        catch (Exception e)
        {
            Debug.Log("# Error:" + e.Message.ToString());
        }
    }

    static string key = "%f2BEd4g(dsE9G$D";

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="key"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] Decrypt(byte[] buffer, string key)
    {
        using (AesManaged aes = new AesManaged())
        {
            byte[] code = Encoding.UTF8.GetBytes(key);
            var encryptor = aes.CreateDecryptor(code, code);
            var temp = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return temp;
        }
    }
}
