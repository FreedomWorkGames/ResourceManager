//  HotUpdateAssetsManager.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/8/2018.
// ����Ƿ���Ҫ�ȸ�����Դ
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HotUpdateAssetsManager 
{
    class UpdateAssetItemInfo
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
    public void CheckNeedUpdate(System.Action<bool> checkResultHandler)
    {
        //����Դ���������أ���Դ�б�
        _resourceLoader.LoadRemoteAsset(Path.Combine( Application.platform.ToString() , assetsListName),"",(data)=>
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            HotUpdateAssetsList serverAssetsList = JsonUtility.FromJson<HotUpdateAssetsList>(jsonStr);
            //��ȡ����
            if (!File.Exists(System.IO.Path.Combine(Application.persistentDataPath, assetsListName)))//Ӧ����û���¹���������Դ�б���app����
            {
               // FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
            }
           
        });
    }
    public void BeginUpdate()
    {

    }
}