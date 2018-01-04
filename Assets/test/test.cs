//  test.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using System;
using System.Collections;

//测试结果：无论是出错还是完成，isDone都为true
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
        _loadTask = new LoadRemteTask(System.IO.Path.Combine(Application.dataPath, @"AssetBundles/AssetBundless"),"");
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
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(_loadTask != null)
         CheckLoadTask(_loadTask);
    }
}