//  LoadTask.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public enum ELoadTaskState
{
    notStart,
    loading,
    finished,
}
public class LoadTaskBase
{
    protected string url;
    public AsyncOperation asyncOperation
    {
        protected set;
        get;
    }
    protected ELoadTaskState _loadTaskState = ELoadTaskState.notStart;
    public bool IsDone() { return asyncOperation.isDone; }
    public float GetProgress() { return asyncOperation.progress; }
    public virtual void BeginLoad(string url)
    {
        this.url = url;
    }
    public virtual void SetEloadTaskState(ELoadTaskState state)
    {
        if (_loadTaskState == state)
            return;
        _loadTaskState = state;
    }
    public Action<AssetBundle> loadFinishHandler;
}

public abstract class LoadTask : LoadTaskBase
{
    public abstract string GetError();
    public abstract bool IsError();
    public abstract AssetBundle GetAssetBundle();
}
public class LoadTaskRemote : LoadTask
{
    UnityWebRequest _unityWebRequest;
    public override AssetBundle GetAssetBundle()
    {
        return DownloadHandlerAssetBundle.GetContent(_unityWebRequest);
    }
    public override void BeginLoad(string path)
    {
        base.BeginLoad(path);
        _unityWebRequest = UnityWebRequest.GetAssetBundle(url);
        asyncOperation = _unityWebRequest.SendWebRequest();
    }
    public override bool IsError()
    {
        return _unityWebRequest.isNetworkError;
    }
    public override string GetError()
    {
        return _unityWebRequest.error;
    }
}
public class LoadTaskFromLocalDisk : LoadTask
{
    protected AssetBundleCreateRequest _request
    {
        get { return asyncOperation as AssetBundleCreateRequest; }
    }
    public override void BeginLoad(string path)
    {
        base.BeginLoad(path);
        asyncOperation = AssetBundle.LoadFromFileAsync(url);
    }
    public override AssetBundle GetAssetBundle()
    {
        return _request.assetBundle;
    }
    public override bool IsError()
    {
        return IsDone() && GetAssetBundle() == null;
    }
    public override string GetError()
    {
        return url + " is not exist in local disk";
    }
}
public class LoadTaskLoadManifest : LoadTaskFromLocalDisk
{
    public AssetBundleManifest assetBundleManifest
    {
        get;
        private set;
    }
    public override void SetEloadTaskState(ELoadTaskState state)
    {
        base.SetEloadTaskState(state);
        if (_loadTaskState == state)
            return;
        if (state == ELoadTaskState.finished && !IsError() && IsDone())
        {
            assetBundleManifest = GetAssetBundle().LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }
}

