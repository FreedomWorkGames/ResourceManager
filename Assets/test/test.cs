//  test.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using System;
using System.Collections;

//测试结果：无论是出错还是完成，isDone都为true
//经测试：loadAsset，参数为资源名称，资源名称是原资源所在的目录名称，而不是assetbundle的名称，而且填最后名称或者完整路径名称都可以。而assetbundle包名可以跟asset名称不一样，也就是需要知道每个资源所在的包
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
            //Debug.Log("assetbundle hascode: " + loadTask.GetAssetBundle().GetHashCode());//Object对象的hasCode,并不是AssetBundle对应的hash

            //Debug.Log("manifest hascode: " + loadTask.GetAssetBundle().LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAssetBundleHash(("tests")));//test AssetBundle对应的hash
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