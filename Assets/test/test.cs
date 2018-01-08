//  test.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using System;
using System.Collections;

//���Խ���������ǳ�������ɣ�isDone��Ϊtrue
//�����ԣ�loadAsset������Ϊ��Դ���ƣ���Դ������ԭ��Դ���ڵ�Ŀ¼���ƣ�������assetbundle�����ƣ�������������ƻ�������·�����ƶ����ԡ���assetbundle�������Ը�asset���Ʋ�һ����Ҳ������Ҫ֪��ÿ����Դ���ڵİ�
public class test : MonoBehaviour
{
    public Transform transformCahed
    {
        private set;
        get;
    }
    LoadTask _loadTask=null;
    // Use this for initialization
    void Start()
    {
        transformCahed = this.transform;
        _loadTask = new LoadAssetBundleFromDiskTask(System.IO.Path.Combine(Application.dataPath, @"AssetBundles/aaa/test1"));
        _loadTask.BeginLoad();
    }
     void  CheckLoadTask(LoadTask loadTask)//test hascode
    {
        if(loadTask.IsDone() || loadTask.IsError())
        {
            Debug.Log("idDone: "+loadTask.IsDone() +"--isError: "+loadTask.IsError());
        }
        if(loadTask.IsDone() && !loadTask.IsError())
        {
            //Debug.Log("assetbundle hascode: " + loadTask.GetAssetBundle().GetHashCode());//Object�����hasCode,������AssetBundle��Ӧ��hash

            //Debug.Log("manifest hascode: " + loadTask.GetAssetBundle().LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAssetBundleHash(("tests")));//test AssetBundle��Ӧ��hash
            loadTask.OnLoadComplete();
            var allAsset = (loadTask as LoadAssetBundleFromDiskTask).asset.GetAllAssetNames();
            var mainAsset = (loadTask as LoadAssetBundleFromDiskTask).asset.LoadAsset("test1");//
            int i = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_loadTask != null)
        {
            CheckLoadTask(_loadTask);
            if (_loadTask.IsDone())
                _loadTask = null;
        }
    }
}