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
public abstract class LoadTask 
{
    public string url
    {
        get;
        private set;
    }
    public AsyncOperation asyncOperation
    {
        protected set;
        get;
    }
    public ELoadTaskState loadTaskState
    {
        protected set;
        get;
    }
    public Action<UnityEngine.Object> loadFinishHandler;

    public LoadTask(string url)
    {
        this.url = url;
        SetEloadTaskState(ELoadTaskState.notStart);
    }
    public bool IsDone() { return asyncOperation.isDone; }
    public float GetProgress() { return asyncOperation.progress; }
    public virtual void BeginLoad()
    {
    }
    public void SetEloadTaskState(ELoadTaskState state)
    {
        if (loadTaskState == state)
            return;
        loadTaskState = state;
    }
    public virtual void OnLoadComplete()
    {
        SetEloadTaskState(ELoadTaskState.finished);
        if (IsError())
        {
            Debug.Log("LoadTask Error: " + GetError());
            return;
        }
    }
    public abstract void Release();
    public abstract string GetError();
    public abstract bool IsError();
}
public abstract class LoadAssetBundleTask : LoadTask
{
    public AssetBundle assetBundle
    {
        get;
        private set;
    }
    //public Action<AssetBundle> loadFinishHandler;
    public LoadAssetBundleTask(string url) : base(url)
    { }
    public abstract AssetBundle GetAssetBndle();
    public override void OnLoadComplete()
    {
        base.OnLoadComplete();

        if (IsDone() && !IsError())
        {
            assetBundle = GetAssetBndle();
            loadFinishHandler(assetBundle);
        }
    }
}
public class LoadRemteTask : LoadTask
{
    UnityWebRequest _unityWebRequest;
    public byte[] data
    {
        get;
        protected set;
    }
    public string md5
    {
        get;//用于下载完成后，检测完整性使用
        protected set;
    }
    public LoadRemteTask(string url, string md5) : base(url)
    {
        //System.Security.Cryptography.MD5.Create();
        this.md5 = md5;
    }
    //public override AssetBundle GetData()
    //{
    //    return AssetBundle.LoadFromMemory(_unityWebRequest.downloadHandler.data); //DownloadHandlerAssetBundle.GetContent(_unityWebRequest);
    //}
    public override void BeginLoad()
    {
        base.BeginLoad();
        _unityWebRequest = UnityWebRequest.Get(url);
#if UNITY_2017_3_OR_NEWER
        asyncOperation = _unityWebRequest.SendWebRequest();
#else
        asyncOperation = _unityWebRequest.Send();
#endif
    }
    public override bool IsError()
    {
        return _unityWebRequest.isNetworkError;
    }
    public override string GetError()
    {
        return _unityWebRequest.error;
    }
    public override void OnLoadComplete()
    {
        base.OnLoadComplete();
        if (!IsError() && IsDone())
        {
            data = _unityWebRequest.downloadHandler.data;
        }
    }
    public override void Release()
    {
        if(IsDone())//测试证明，error后isOone为true
        {
            _unityWebRequest.Dispose();
        }
        else
        {
            _unityWebRequest.Abort();
        }
    }
}
public class LoadAssetBundleFromDiskTask : LoadAssetBundleTask
{
    public LoadAssetBundleFromDiskTask(string url) : base(url)
    {

    }
    protected AssetBundleCreateRequest _request
    {
        get { return asyncOperation as AssetBundleCreateRequest; }
    }
    public override void BeginLoad()
    {
        base.BeginLoad();
        asyncOperation = AssetBundle.LoadFromFileAsync(url);
    }
    public override AssetBundle GetAssetBndle()
    {
        return _request.assetBundle;
    }
    public override bool IsError()
    {
        return IsDone() && GetAssetBndle() == null;
    }
    public override string GetError()
    {
        return url + " is not exist in local disk";
    }
    public override void OnLoadComplete()
    {
        base.OnLoadComplete();
    }
    public override void Release()
    {
       
    }
}
public class LoadManifestTask : LoadAssetBundleFromDiskTask
{
    public LoadManifestTask(string url) : base(url)
    {

    }
    public AssetBundleManifest assetBundleManifest
    {
        get;
        private set;
    }
    public override void OnLoadComplete()
    {
        base.OnLoadComplete();
        if (!IsError() && IsDone())
        {
            AssetBundle assetBundle = GetAssetBndle();
            assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }
}

