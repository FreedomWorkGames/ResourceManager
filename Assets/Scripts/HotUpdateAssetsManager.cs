//  HotUpdateAssetsManager.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/8/2018.
// 检查是否需要热更新资源
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HotUpdateAssetsManager
{
    ResourceLoader _resourceLoader;
    private const string serverName = @"http:///myServer.com/";
    private const string assetsListName = "AssetsList";
    private const string localAssetOutAppName = "localAssetOutApp";
    private HotUpdateAssetsList localAssetOutAppList;
    public HotUpdateAssetsManager()
    {
        _resourceLoader = new ResourceLoader(serverName);
    }
    public void CheckNeedUpdate()
    {
        //从资源服务器下载，资源列表
        _resourceLoader.LoadRemoteAsset(Path.Combine(Application.platform.ToString(), assetsListName), "", (data) =>
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            HotUpdateAssetsList serverAssetsList = JsonUtility.FromJson<HotUpdateAssetsList>(jsonStr);
            //读取本地资源维护表
            string localAssetOutAppFilePath = Path.Combine(Application.persistentDataPath, localAssetOutAppName);
            if (File.Exists(localAssetOutAppFilePath))
                localAssetOutAppList = JsonUtility.FromJson<HotUpdateAssetsList>(File.ReadAllText(localAssetOutAppFilePath));
            else
                localAssetOutAppList = new HotUpdateAssetsList();//记录本地已经热更新过得资源
            //读取本地资源md5文件
            string localListPath = "";
            if (!File.Exists(Path.Combine(Application.persistentDataPath, assetsListName)))//应该是没更新过，本地资源列表还在app包内
            {
                // FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
                localListPath = UrlCombine.GetLocalUrl(assetsListName, true, Application.platform);
            }
            else
                localListPath = UrlCombine.GetLocalUrl(assetsListName, false, Application.platform);
            //加载本地表，加载完成后比较本地和服务器返回表
            string localAssetListJsonStr = File.ReadAllText(localListPath);
            HotUpdateAssetsList localAssetList = JsonUtility.FromJson<HotUpdateAssetsList>(localAssetListJsonStr);
            List<HotUpdateAssetItem> needUpdateAssetList = CheckDifferent(localAssetList, serverAssetsList);
            //加在更新资源
            int loadedAssetCouint = 0;
            for (int i = 0; i < needUpdateAssetList.Count; i++)
            {
                var assetInfo = needUpdateAssetList[i];
                _resourceLoader.LoadRemoteAsset(assetInfo.assetPath, assetInfo.md5, (assetBytes) =>
                  {
                      loadedAssetCouint++;
                      UpdateOutAppAssetPath(assetInfo);
                      string assetPath = Path.Combine(Application.persistentDataPath, assetInfo.assetPath);
                      WriteFile(assetPath, assetBytes);
                      if (loadedAssetCouint == needUpdateAssetList.Count)
                      {
                          //全部资源更新完成,更新本地表格
                          string assetListJsonString = JsonUtility.ToJson(serverAssetsList);
                          WriteFile(Path.Combine(Application.persistentDataPath, assetsListName), assetListJsonString);
                          string assetOutAppJsonString = JsonUtility.ToJson(localAssetOutAppList);
                          WriteFile(localAssetOutAppFilePath, assetOutAppJsonString);
                      }
                  });
            }
        });
    }
    private List<HotUpdateAssetItem> CheckDifferent(HotUpdateAssetsList localList, HotUpdateAssetsList serverList)
    {
        var differentList = new List<HotUpdateAssetItem>();
        Dictionary<string, HotUpdateAssetItem> dictionary = new Dictionary<string, HotUpdateAssetItem>();
        for (int i = 0; i < localList.assetList.Count; i++)
        {
            var item = localList.assetList[i];
            dictionary.Add(item.assetPath, item);
        }
        for(int i = 0;i<serverList.assetList.Count;i++)
        {
            var item = serverList.assetList[i];
            if(dictionary.ContainsKey(item.assetPath))
            {
                if (dictionary[item.assetPath].md5 != item.md5)
                    differentList.Add(item);
            }
            else
            {
                differentList.Add(item);
            }
        }
        return differentList;
    }
    private void UpdateOutAppAssetPath(HotUpdateAssetItem info)
    {
        int i = 0;
        for (i = 0; i < localAssetOutAppList.assetList.Count; i++)
        {
            var assetInfo = localAssetOutAppList.assetList[i];
            if (assetInfo.assetPath == info.assetPath)
            {
                assetInfo.md5 = info.md5;
                break;
            }
        }
        if (i >= localAssetOutAppList.assetList.Count)
            localAssetOutAppList.assetList.Add(info);
    }
    private void WriteFile(string path, byte[] data)
    {
        if (File.Exists(path))
            File.Delete(path);
        FileStream assetFileStream = File.Create(path);
        assetFileStream.Write(data, 0, data.Length);
        assetFileStream.Flush();
        assetFileStream.Close();
    }
    private void WriteFile(string path, string text)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
        WriteFile(path, data);
    }
}