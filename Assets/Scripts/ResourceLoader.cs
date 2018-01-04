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
    private List<LoadTask> _todoTasks;
    private List<LoadTask> _loadingTasks;
    private Dictionary<string, LoadTask> _taskDictionary;//url作为key,对应loadTask
    public ResourceLoader()
    {
        _todoTasks = new List<LoadTask>();
        _loadingTasks = new List<LoadTask>();
        _taskDictionary = new Dictionary<string, LoadTask>();
    }
    public void LoadRemoteAsset(string path, string md5, Action<UnityEngine.Object> assetHandler)
    {
        string url = UrlCombine.GetLoadRul(path, false,Application.platform);
        AddTask(url, ETaskType.loadRemoteAsset, assetHandler, md5);
    }
    public void LoadLocalAssetBundle(string path, Action<UnityEngine.Object> assetHandler)
    {
        string url = UrlCombine.GetLoadRul(path, true, Application.platform);
        AddTask(url, ETaskType.loadLocalAssetBundle, assetHandler);
    }
    public void LoadManiFest(string path, Action<UnityEngine.Object> assetHandler)
    {
        string url = UrlCombine.GetLoadRul(path, true,Application.platform);
        AddTask(url, ETaskType.loadMainManifest, assetHandler);
    }
    public void RemoveTask(string url)
    {
        LoadTask loadTask = null;
        if (_taskDictionary.TryGetValue(url, out loadTask))
        {
            RemoveTask(loadTask);
        }
    }
    private bool CombineTask(string url, Action<UnityEngine.Object> assetHandler)
    {
        if (_taskDictionary.ContainsKey(url))
        {
            _taskDictionary[url].loadFinishHandler += assetHandler;
            return true;
        }
        return false;
    }
    private void AddTask(string url, ETaskType taskType, Action<UnityEngine.Object> assetHandler, string md5 = "")
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
}
public class UrlCombine
{
    //每个平台的资源根目录都是枚举字符串
    static public string GetLoadRul(string path, bool loadFromServer, RuntimePlatform platformType )//
    {
        string serverName = "http:///myServer.com/";
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(platformType.ToString(),100);
        stringBuilder.Append(@"/");
        stringBuilder.Append(@path);
         if(loadFromServer)
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