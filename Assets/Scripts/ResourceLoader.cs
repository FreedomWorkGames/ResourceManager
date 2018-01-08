//  ResourceLoader.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/2/2018.
//
using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// 管理资源加载到内存，两个队列，未开始下载和一开始下载，下载完成的任务，任务自动释放
/// </summary>
public class ResourceLoader
{
    private int _maxLoadingTaskCount = 10;//如果为0，表示无限制
    private List<LoadTask> _todoTasks;
    private List<LoadTask> _loadingTasks;
    private Dictionary<string, LoadTask> _taskDictionary;//url作为key,对应loadTask
    public ResourceLoader()
    {
        _todoTasks = new List<LoadTask>();
        _loadingTasks = new List<LoadTask>();
        _taskDictionary = new Dictionary<string, LoadTask>();
    }
    public void LoadRemoteAsset(string path, string md5, Action<byte[]> assetHandler)
    {
        BeginTask(path, ETaskType.loadRemoteAsset, (loadTask) =>
        {
            if (assetHandler != null)
            {
                LoadRemoteTask task = loadTask as LoadRemoteTask;
                assetHandler(task.asset);
            }
        }, md5);
    }
    public void LoadLocalAssetBundle(string path, Action<AssetBundle> assetHandler)
    {
        BeginTask(path, ETaskType.loadLocalAssetBundle, (loadTask) =>
        {
            if (assetHandler != null)
            {
                LoadAssetBundleFromDiskTask task = loadTask as LoadAssetBundleFromDiskTask;
                assetHandler(task.asset);
            }
        }
        );
    }
    public void LoadLocalAssetBundle(string[] allPaths,Action<Dictionary<string,AssetBundle>> finishedHandler)
    {
        Dictionary<string, AssetBundle> allAssets = new Dictionary<string, AssetBundle>();
        for(int i=0;i<allPaths.Length;i++)
        {
            string path = allPaths[i];
            int index = i;
            LoadLocalAssetBundle(path, (assetBundle) =>
             {
                 allAssets.Add(path, assetBundle);
                 //check all asset is finished
                 bool isAllFinished = true;
                 for(int k=0;k<allPaths.Length;k++)
                 {
                     string callBackPath = allPaths[k];
                     isAllFinished &= _taskDictionary.ContainsKey(callBackPath) && _taskDictionary[callBackPath].IsDone();
                     if (!isAllFinished)
                         break;
                 }
                 //end check (all asset is finished),do finishedHandler
                 if (isAllFinished && finishedHandler != null)
                 {
                     finishedHandler(allAssets);
                 }
             });
        }
    }
    public void LoadManiFest(string path, Action<AssetBundleManifest> assetHandler)
    {
        string url = UrlCombine.GetLoadRul(path, false, Application.platform);
        AddTask(url, ETaskType.loadMainManifest, (loadTask) =>
        {
            if (assetHandler != null)
            {
                LoadManifestTask task = loadTask as LoadManifestTask;
                assetHandler(task.assetBundleManifest);
            }
        });

    }
    public void RemoveTask(string url)
    {
        LoadTask loadTask = null;
        if (_taskDictionary.TryGetValue(url, out loadTask))
        {
            RemoveTask(loadTask);
        }
    }
    private void BeginTask(string path, ETaskType taskType,Action<LoadTask> finishedHandler,string md5="")
    {
        string url = UrlCombine.GetLoadRul(path, false, Application.platform);
        AddTask(url, taskType, (loadTask) =>
        {
            if (finishedHandler != null)
                finishedHandler(loadTask);
        });
    }
    private bool CombineTask(string url, Action<LoadTask> assetHandler)
    {
        if (_taskDictionary.ContainsKey(url))
        {
            if (assetHandler != null)
                _taskDictionary[url].loadFinishHandler += assetHandler;
            return true;
        }
        return false;
    }
    private void AddTask(string url, ETaskType taskType, Action<LoadTask> assetHandler, string md5 = "")
    {
        LoadTask task = null;
        if (_taskDictionary.TryGetValue(url, out task))
        {
            CombineTask(url, assetHandler);
        }
        else
        {
            task = LoadTaskFactory.GetLoadTask(url, taskType, md5);
            _taskDictionary.Add(url, task);
            _todoTasks.Add(task);
        }
    }
    private void ReloadTask(LoadTask task)
    {
        if (!_taskDictionary.ContainsKey(task.url))
            Debug.LogError("reloadTask is not exist");
        else
        {
            task.SetEloadTaskState(ELoadTaskState.notStart);
            _todoTasks.Add(task);
            _loadingTasks.Remove(task);
        }
    }
    private void RemoveTask(LoadTask loadTask)
    {
        switch (loadTask.loadTaskState)
        {
            case ELoadTaskState.notStart:
                _todoTasks.Remove(loadTask);
                break;
            case ELoadTaskState.loading:
            case ELoadTaskState.finished:
                _loadingTasks.Remove(loadTask);
                break;
        }
        _taskDictionary.Remove(loadTask.url);
        loadTask.Release();
    }

    public void UpdateTask()
    {
        int toLoadCount = 0;
        if (_maxLoadingTaskCount == 0)
            toLoadCount = _todoTasks.Count;
        else if (_loadingTasks.Count < _maxLoadingTaskCount)
        {
            toLoadCount = _maxLoadingTaskCount - _loadingTasks.Count;
        }
        if (toLoadCount > 0)
        {
            var beingTaks = _todoTasks.GetRange(0, toLoadCount);
            for (int i = 0; i < beingTaks.Count; i++)
                beingTaks[i].SetEloadTaskState(ELoadTaskState.loading);
            _loadingTasks.AddRange(beingTaks);
            _todoTasks.RemoveRange(0, toLoadCount);
        }
        for (int i = 0; i < _loadingTasks.Count; i++)
        {
            var task = _loadingTasks[i];
            if (task.IsDone())
            {
                if (task.IsAssetRight())
                {
                    task.OnLoadComplete();
                    RemoveTask(task);
                    i--;
                }
                else
                {
                    ReloadTask(task);
                }
            }
        }
    }
}
public class UrlCombine
{
    //每个平台的资源根目录都是枚举字符串
    static public string GetLoadRul(string path, bool loadFromServer, RuntimePlatform platformType)//
    {
        string serverName = "http:///myServer.com/";
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(platformType.ToString(), 100);
        stringBuilder.Append(@"/");
        stringBuilder.Append(@path);
        if (loadFromServer)
        {
            stringBuilder.Insert(0, serverName);
        }
        else
        {
            //客户端应该有一个表，根据path，可以查到在包内还是包外,热更新之后就会在包外了
            bool isInApp = false;
            stringBuilder.Insert(0, isInApp ? Application.dataPath : Application.persistentDataPath);
        }
        return stringBuilder.ToString();
    }
}