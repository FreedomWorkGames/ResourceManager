//  ResourceLoader.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/2/2018.
//
using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceLoader
{
    private List<LoadTask> _todoTasks;
    private List<LoadTask> _loadingTasks;
    private Dictionary<string, LoadTask> _taskDictionary;
    public ResourceLoader()
    {
        _todoTasks = new List<LoadTask>();
        _loadingTasks = new List<LoadTask>();
        _taskDictionary = new Dictionary<string, LoadTask>();
    }

    public void AddNewTask(string path,Action<AssetBundle> finishedHandler)
    {
       

    }
}
public class UrlCombine
{
    static public string GetRul(string path, bool loadFromServer)//
    {
        return path;
    }
}