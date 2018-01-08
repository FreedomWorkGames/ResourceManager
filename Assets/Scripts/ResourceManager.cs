//  ResourceManager.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//runtime resource load and cached manager
using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceManager 
{
    class AssetInfo
    {
        public AssetBundle assetBundle;
#if USE_ASSETBUNDLE_REFRENCE_COUNT
        public uint referenceCount;
#endif
        public AssetInfo(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }
    }
    private ResourceLoader _resourceLoader;
    private Dictionary<string, AssetInfo> _resourceDictionary;
    private AssetBundleManifest _manifest;
    public bool canWork
    {
        get { return _manifest != null; }
    }
    public ResourceManager()
    {
        _resourceLoader = new ResourceLoader();
        _resourceDictionary = new Dictionary<string, AssetInfo>();
        LoadManifest();
    }
    public void GetAssetSync(string path,Action<AssetBundle> handler)
    {
        if(!canWork)
        {
            Debug.Log("resourceManager is not Working");
            return;
        }
        if(_resourceDictionary.ContainsKey(path))
        {
#if USE_ASSETBUNDLE_REFRENCE_COUNT
            AddReferenceCount(path);
#endif
            if (handler != null)
                handler(_resourceDictionary[path].assetBundle);
        }
        else
        {
            string[] dependencies = _manifest.GetAllDependencies(path);
            if(dependencies != null && dependencies.Length>0)
            {
                int finishedCount = 0;
                int totalCount = dependencies.Length;
                for (int i = 0; i < totalCount; i++)
                {
                    string dependentPath = dependencies[i];
                    GetAssetSync(dependentPath, (dependentAssetBundle) =>
                     {
                         LoadFinishHandler(dependentPath, dependentAssetBundle,null);
                         finishedCount++;
                         if (finishedCount == totalCount)
                             _resourceLoader.LoadLocalAssetBundle(path, (assetBundle)=>
                             {
                                 LoadFinishHandler(path, assetBundle, handler);
                             });
                     });
                }
            }
            else
            {
                _resourceLoader.LoadLocalAssetBundle(path, (assetBundle) =>
                {
                    LoadFinishHandler(path, assetBundle,handler);
                });
            }
        }
    }
    private void LoadFinishHandler(string path,AssetBundle assetBundle,Action<AssetBundle> callBack)
    {
        _resourceDictionary.Add(path, new AssetInfo(assetBundle));
 #if USE_ASSETBUNDLE_REFRENCE_COUNT
        AddReferenceCount(path);
 #endif
        if (callBack != null)
            callBack(assetBundle);
    }
 #if USE_ASSETBUNDLE_REFRENCE_COUNT
    //写一个最底层脚本 ，所有脚本继承自这个，有一个实例化函数，自动调用引用增加，同时，脚本自行记录使用了多少次，脚本销毁时，自动减去所有引用
    public void AddReferenceCount(string path,uint addCount=1)
    {
        _resourceDictionary[path].referenceCount += addCount;
    }
    public void ReleaseAssetBundle(string path,uint releaseCount =1)
    {
        _resourceDictionary[path].referenceCount -= releaseCount;
        if (_resourceDictionary[path].referenceCount < 0)
            Debug.LogError("assetBundle referenceCount is error,should  not be less 0");
    }
#endif
    private void LoadManifest()
    {
        _resourceLoader.LoadManiFest(Application.platform.ToString(), (manifest) =>
         {
             _manifest = manifest as AssetBundleManifest;
         });
    }
}