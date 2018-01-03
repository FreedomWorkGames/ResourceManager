//  BuildAssetBundles.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using UnityEditor;

public class BuildAssetBundlesWindow: Editor
{
    const string buildAssetBundlesRoot = "BuildAssetBundles/";
   [MenuItem(buildAssetBundlesRoot+"build")]
   static void BuildAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(@"Assets/AssetBundles/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
    [MenuItem(buildAssetBundlesRoot+"testCrc")]
    static void TestCrc()
    {
        uint crc = 0;
        Debug.Log(BuildPipeline.GetCRCForAssetBundle(@"Assets/AssetBundles/test", out crc) + "---crc: " + crc);
    }
    [MenuItem(buildAssetBundlesRoot + "testHash")]
    static void TestHash()
    {
        Hash128 hash;
        Debug.Log(BuildPipeline.GetHashForAssetBundle(@"Assets/AssetBundles/test", out hash) + "---hash: " + hash.ToString());
    }
}