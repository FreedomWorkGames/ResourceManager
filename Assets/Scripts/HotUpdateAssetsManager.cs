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
        //����Դ���������أ���Դ�б�
        _resourceLoader.LoadRemoteAsset(Path.Combine( Application.platform.ToString() , assetsListName),"",(data)=>
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            HotUpdateAssetsList serverAssetsList = JsonUtility.FromJson<HotUpdateAssetsList>(jsonStr);
            HotUpdateAssetsList localAssetOutApp = new HotUpdateAssetsList();//��¼�����Ѿ��ȸ��¹�����Դ
            //��ȡ����
            string localListPath = "";
            if (!File.Exists(Path.Combine(Application.persistentDataPath, assetsListName)))//Ӧ����û���¹���������Դ�б���app����
            {
                // FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
                localListPath = UrlCombine.GetLocalUrl(assetsListName, true, Application.platform);
            }
            else
                localListPath = UrlCombine.GetLocalUrl(assetsListName, false, Application.platform);
            //���ر��ر�������ɺ�Ƚϱ��غͷ��������ر�
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