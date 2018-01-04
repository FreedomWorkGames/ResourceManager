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
        string url = UrlCombine.GetRul(path, false);
    }
    public void LoadLocalAsset(string path, Action<UnityEngine.Object> assetHandler)
    {
        string url = UrlCombine.GetRul(path, true);

    }
    public void LoadManiFest(string path, Action<UnityEngine.Object> assetHandler)
    {
        string url = UrlCombine.GetRul(path, true);
    }
    public void RemoveTask(string url)
    {
        LoadTask loadTask = null;
        if (_taskDictionary.TryGetValue(url, out loadTask))
        {
            RemoveTask(loadTask);
        }
    }
    protected void CreateNewTask(string url, Action<UnityEngine.Object> assetHandler)
    {
    }
    private void AddNewTask(LoadTask task, Action<AssetBundle> finishedHandler)
    {
        if (!_taskDictionary.ContainsKey(task.url))
            _taskDictionary.Add(task.url, task);
        _todoTasks.Add(task);
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
    static public string GetRul(string path, bool loadFromServer)//
    {
        return path;
    }
}