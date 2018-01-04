//  ResourceLoader.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/2/2018.
//
using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// ������Դ���ص��ڴ棬�������У�δ��ʼ���غ�һ��ʼ���أ�������ɵ����������Զ��ͷ�
/// </summary>
public class ResourceLoader
{
    private List<LoadTask> _todoTasks;
    private List<LoadTask> _loadingTasks;
    private Dictionary<string, LoadTask> _taskDictionary;//url��Ϊkey,��ӦloadTask
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
    //ÿ��ƽ̨����Դ��Ŀ¼����ö���ַ���
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
            //�ͻ���Ӧ����һ��������path�����Բ鵽�ڰ��ڻ��ǰ���,�ȸ���֮��ͻ��ڰ�����
            bool isInApp = false;
            stringBuilder.Insert(0, isInApp ? Application.dataPath : Application.persistentDataPath);
        }
        return stringBuilder.ToString();
    }
}