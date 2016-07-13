//
//  AssetBundleHelper..cs
//  UnityTools
//
//  Created by Arcadegame on 7/13/2016.
//

using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetBundleHelper : MonoBehaviour
{

    [MenuItem("Tools/AssetBundle/Android")]
    static void BuildAssetBundleAndroid()
    {
        PackAssetBundle(BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundle/WSA")]
    static void BuildAssetBundleWSA()
    {
        PackAssetBundle(BuildTarget.WSAPlayer);
    }

    [MenuItem("Tools/AssetBundle/iOS")]
    static void BuildAssetBundleiOS()
    {
        PackAssetBundle(BuildTarget.iOS);
    }

    /// <summary>
    /// Build AssetBundle
    /// </summary>
    /// <param name="target"></param>
    static void PackAssetBundle(BuildTarget target)
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (objs.Length <= 0) return;

        string resourcePath = Application.dataPath + "/Resources";
        if (!Directory.Exists(resourcePath))
            Directory.CreateDirectory(resourcePath);

        string savePath = EditorUtility.SaveFolderPanel("Save Folder", resourcePath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        AssetBundleBuild[] assetBundle = new AssetBundleBuild[1];
        string[] assetNames = new string[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            assetNames[i] = AssetDatabase.GetAssetPath(objs[i]);
        }
        string fileName = objs[0].name + "_" + target.ToString() + ".unity3d";
        assetBundle[0] = new AssetBundleBuild()
        {
            assetBundleName = fileName,
            assetNames = assetNames
        };

        BuildPipeline.BuildAssetBundles(savePath, assetBundle,
                    BuildAssetBundleOptions.UncompressedAssetBundle, target);

        #region Î¢Ð¡¹¤×÷
        if (File.Exists(savePath + "/" + fileName + ".manifest"))
            File.Delete(savePath + "/" + fileName + ".manifest");
        string[] filepathInfo = savePath.Split('/');
        string flodName = filepathInfo[filepathInfo.Length - 1];
        if (File.Exists(savePath + "/" + flodName))
            File.Delete(savePath + "/" + flodName);
        if (File.Exists(savePath + "/" + flodName + ".manifest"))
            File.Delete(savePath + "/" + flodName + ".manifest");
        #endregion

        if (File.Exists(savePath + "/" + fileName))
            EditorUtility.OpenWithDefaultApp(savePath);
    }

    [MenuItem("Tools/CheckAssetBundle")]
    static void CheckAssetBundle()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Asset Bundle", Application.dataPath + "/Resources/", "*.*");
        if (string.IsNullOrEmpty(filePath)) return;
        string assetBundleInfo = filePath + "\n Include:";

        AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
        var infos = assetBundle.GetAllAssetNames();
        for (int i = 0; i < infos.Length; i++)
        {
            assetBundleInfo += infos[i] + "\n";
        }
        //Editor.Instantiate(assetBundle.LoadAsset(infos[0]));
        assetBundle.Unload(true);
        Debug.Log(assetBundleInfo);
    }
}
