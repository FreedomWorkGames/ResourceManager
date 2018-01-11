//  BuildAssetBundles.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildAssetBundlesWindow : Editor
{
    const string buildAssetBundlesRoot = "BuildAssetBundles/";
    const string rawResourcesPath = "test/RawResources";
    [MenuItem(buildAssetBundlesRoot + "build")]
    static void BuildAssetBundles()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        string assetBundleDirectoryName = GetAssetBundleDirectoryName(buildTarget);
        string assetBundleFullPath = GetAssetBundleFullPath(assetBundleDirectoryName);
        //if (!Directory.Exists(assetBundleFullPath))
        //{
        //    Directory.CreateDirectory(assetBundleFullPath);
        //}
        //BuildPipeline.BuildAssetBundles(assetBundleDirectoryName, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);
        ////删除所有的多余manifest文件
        //DeleteManifest(assetBundleFullPath);
        CreateAssetList(assetBundleFullPath);
    }
    private static void DeleteManifest(string assetBundleFullPath)
    {
        string[] allManifestPath = Directory.GetFiles(assetBundleFullPath, "*.manifest", SearchOption.AllDirectories);
        //string ingoreManifest = assetBundleDirectoryName + ".manifest";
        for (int i = 0; i < allManifestPath.Length; i++)
        {
            string manifestPath = allManifestPath[i];
            //if (!manifestPath.EndsWith(ingoreManifest))
            //{
            File.Delete(manifestPath);
            //}
        }
    }
    private static void CreateAssetList(string assetBundleFullPath)
    {
        //读取所有文件，并创建md5文件
        HotUpdateAssetsList hotUpdateAssetsList = new HotUpdateAssetsList();
        DirectoryInfo directoryInfo = new DirectoryInfo(assetBundleFullPath);
        FileInfo[] allAsset = directoryInfo.GetFiles();
        for (int i = 0; i < allAsset.Length; i++)
        {
            FileInfo fileInfo = allAsset[i];
            string fullName = fileInfo.FullName.Replace("\\", "/");
            string rootDirectoryPath = assetBundleFullPath.Replace("\\", "/");
            string assetName = fullName.Substring(rootDirectoryPath.Length + 1);
            if (assetName.Split('.').Length > 0)
            {
                assetName = assetName.Split('.')[0];
            }
            FileStream fileStream = fileInfo.Create();
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string md5 = MD5Builder.BuildMD5(data);
            HotUpdateAssetItem hotUpdateAssetItem = new HotUpdateAssetItem(assetName, md5);
            hotUpdateAssetsList.assetList.Add(hotUpdateAssetItem);
        }
        string assetListJsonStr = JsonUtility.ToJson(hotUpdateAssetsList);
        string assetListPath = Path.Combine(assetBundleFullPath, "AssetList");
        File.WriteAllText(assetListPath, assetListJsonStr);
    }
    private static string GetAssetBundleFullPath(string assetBundleDirectoryName)
    {
        return Path.Combine(@Application.dataPath.Substring(0, Application.dataPath.Length - 7), assetBundleDirectoryName);
    }
    private static string GetAssetBundleDirectoryName(BuildTarget buildTarget)
    {
        return buildTarget.ToString(); 
    }
    [MenuItem(buildAssetBundlesRoot + "SetAssetBundlesName")]
    static void SetAssetBundlesName()
    {
        string resourceRootPath = System.IO.Path.Combine(Application.dataPath, @rawResourcesPath);
        if (System.IO.Directory.Exists(resourceRootPath))
        {
            var directoryInfo = new System.IO.DirectoryInfo(resourceRootPath);
            EditorUtility.DisplayProgressBar("设置AssetName", "正在设置AssetName...", 0);
            var allFiles = directoryInfo.GetFiles("*", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                EditorUtility.DisplayProgressBar("设置AssetName", "正在设置AssetName...", 1f * i / allFiles.Length);
                var fileInfo = allFiles[i];
                if (!fileInfo.Name.EndsWith(".meta"))
                {
                    string basePath = fileInfo.FullName.Replace("\\", "/");
                    basePath = basePath.Substring(Application.dataPath.Length - 6);
                    var importer = AssetImporter.GetAtPath(basePath);//以Assets/开头的本地路径
                    if (importer)
                    {
                        string assetBundleName = basePath.Substring(8 + rawResourcesPath.Length);
                        if (assetBundleName.Split('.').Length > 0)
                        {
                            assetBundleName = assetBundleName.Split('.')[0];
                        }
                        importer.assetBundleName = assetBundleName;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
    }


}