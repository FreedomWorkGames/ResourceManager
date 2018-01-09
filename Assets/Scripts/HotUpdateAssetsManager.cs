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
    public class UpdateAssetItemInfo
    {
        public string path;
        public string md5;
        public UpdateAssetItemInfo(string path,string md5)
        {
            this.path = path;
            this.md5 = md5;
        }
    }
    ResourceLoader _resourceLoader;
    private const string serverName = @"http:///myServer.com/";
    private const string assetsListName = "AssetsList";
    private List<string> _needUpdateList;
    public HotUpdateAssetsManager()
    {
        _resourceLoader = new ResourceLoader(serverName);
        _needUpdateList = new List<string>();
    }
    public void CheckNeedUpdate()
    {
        //从资源服务器下载，资源列表
        _resourceLoader.LoadRemoteAsset(Path.Combine( Application.platform.ToString() , assetsListName),"",(data)=>
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            HotUpdateAssetsList serverAssetsList = JsonUtility.FromJson<HotUpdateAssetsList>(jsonStr);
            HotUpdateAssetsList localAssetOutApp = new HotUpdateAssetsList();//记录本地已经热更新过得资源
            //读取本地
            string localListPath = "";
            if (!File.Exists(Path.Combine(Application.persistentDataPath, assetsListName)))//应该是没更新过，本地资源列表还在app包内
            {
                // FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
                localListPath = UrlCombine.GetLocalUrl(assetsListName, true, Application.platform);
            }
            else
                localListPath = UrlCombine.GetLocalUrl(assetsListName, false, Application.platform);
            //加载本地表，加载完成后比较本地和服务器返回表
            string localAssetListJsonStr =  File.ReadAllText(localListPath);
            HotUpdateAssetsList localAssetList = JsonUtility.FromJson<HotUpdateAssetsList>(localAssetListJsonStr);

        });
    }
    public List<UpdateAssetItemInfo> CheckDifferent(HotUpdateAssetsList localList,HotUpdateAssetsList serverList)
    {
        var  differentList = new List<UpdateAssetItemInfo>();

        return differentList;
    }
    public void BeginUpdate()
    {

    }
}