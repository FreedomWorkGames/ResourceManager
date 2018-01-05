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
    private ResourceLoader _resourceLoader;
    private Dictionary<string, UnityEngine.Object> _resourceDictionary;
    private AssetBundleManifest _manifest;
    public bool canWork
    {
        get { return _manifest != null; }
    }
    public ResourceManager()
    {
        _resourceLoader = new ResourceLoader();
        _resourceDictionary = new Dictionary<string, UnityEngine.Object>();
        LoadManifest();
    }
    public void GetAssetSync(string path,Action<UnityEngine.Object> handler)
    {
        if(!canWork)
        {
            Debug.Log("resourceManager is not Working");
            return;
        }
        if(_resourceDictionary.ContainsKey(path))
        {
            if (handler != null)
                handler(_resourceDictionary[path]);
        }
        else
        {
            _resourceLoader.LoadLocalAssetBundle()
        }
    }
    private void LoadResource(string path, Action<UnityEngine.Object> finishHandler)
    {
        if (!canWork)
        {
            Debug.Log("resourceManager is not Working");
            return;
        }

    }
    private void LoadManifest()
    {
        _resourceLoader.LoadManiFest(Application.platform.ToString(), (manifest) =>
         {
             _manifest = manifest as AssetBundleManifest;
         });
    }
}