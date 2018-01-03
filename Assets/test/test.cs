//  test.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using System;
using System.Collections;

public class test : MonoBehaviour
{
    public Transform transformCahed
    {
        private set;
        get;
    }	
 
    // Use this for initialization
    void Start()
    {
        transformCahed = this.transform;
        LoadTask task = new LoadTaskRemote();
        StartCoroutine(TestLoadTask(task));
    }
     IEnumerator TestLoadTask(LoadTask loadTask)//test hascode
    {
        loadTask.BeginLoad("file:///" + System.IO.Path.Combine(Application.dataPath, @"AssetBundles/AssetBundles"));
        yield return loadTask.asyncOperation;
        if(loadTask.IsDone() && !loadTask.IsError())
        {
            Debug.Log(  "assetbundle hascode: "+ loadTask.GetAssetBundle().GetHashCode());//Object对象的hasCode,并不是AssetBundle对应的hash

            Debug.Log("manifest hascode: " + loadTask.GetAssetBundle().LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAssetBundleHash(("test")));//test AssetBundle对应的hash
        }
    }
    // Update is called once per frame
    void Update()
    {
     
    }
}