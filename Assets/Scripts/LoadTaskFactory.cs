//  LoadTaskFactory.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/4/2018.
//
using UnityEngine;

public enum ETaskType
{
    loadRemoteAsset,
    loadLocalAssetBundle,
    loadMainManifest,
}

public class LoadTaskFactory 
{
   static public LoadTask GetLoadTask(string url,ETaskType taskType,string md5="")
    {
        switch (taskType)
        {
            case ETaskType.loadRemoteAsset:
                return new LoadRemoteTask(url, md5);
            case ETaskType.loadLocalAssetBundle:
                return new LoadAssetBundleFromDiskTask(url);
            case ETaskType.loadMainManifest:
                return new LoadManifestTask(url);
        }
        return null;
    }
}