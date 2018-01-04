//  test.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;
using System;
using System.Collections;

//���Խ���������ǳ�������ɣ�isDone��Ϊtrue
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
            //Debug.Log("assetbundle hascode: " + loadTask.GetAssetBundle().GetHashCode());//Object�����hasCode,������AssetBundle��Ӧ��hash

            //Debug.Log("manifest hascode: " + loadTask.GetAssetBundle().LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAssetBundleHash(("tests")));//test AssetBundle��Ӧ��hash
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