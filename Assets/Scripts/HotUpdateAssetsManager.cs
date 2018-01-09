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
        //����Դ���������أ���Դ�б�
        _resourceLoader.LoadRemoteAsset(Path.Combine(Application.platform.ToString(), assetsListName), "", (data) =>
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            HotUpdateAssetsList serverAssetsList = JsonUtility.FromJson<HotUpdateAssetsList>(jsonStr);
            //��ȡ������Դά����
            string localAssetOutAppFilePath = Path.Combine(Application.persistentDataPath, localAssetOutAppName);
            if (File.Exists(localAssetOutAppFilePath))
                localAssetOutAppList = JsonUtility.FromJson<HotUpdateAssetsList>(File.ReadAllText(localAssetOutAppFilePath));
            else
                localAssetOutAppList = new HotUpdateAssetsList();//��¼�����Ѿ��ȸ��¹�����Դ
            //��ȡ������Դmd5�ļ�
            string localListPath = "";
            if (!File.Exists(Path.Combine(Application.persistentDataPath, assetsListName)))//Ӧ����û���¹���������Դ�б���app����
            {
                // FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
                localListPath = UrlCombine.GetLocalUrl(assetsListName, true, Application.platform);
            }
            else
                localListPath = UrlCombine.GetLocalUrl(assetsListName, false, Application.platform);
            //���ر��ر�������ɺ�Ƚϱ��غͷ��������ر�
            string localAssetListJsonStr = File.ReadAllText(localListPath);
            HotUpdateAssetsList localAssetList = JsonUtility.FromJson<HotUpdateAssetsList>(localAssetListJsonStr);
            List<HotUpdateAssetItem> needUpdateAssetList = CheckDifferent(localAssetList, serverAssetsList);
            //���ڸ�����Դ
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
                          //ȫ����Դ�������,���±��ر��
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