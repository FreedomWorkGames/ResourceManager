//  HotUpdateAssetsList.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/8/2018.
//热更新资源的列表和MD5
using UnityEngine;
using System.Collections.Generic;

[SerializeField]
public class HotUpdateAssetItem
{
    public string assetPath;//资源名称
    public string md5;
}
public class HotUpdateAssetsList 
{
    //其他数据

    //所有资源列表
    public List<HotUpdateAssetItem> assetList;
    public HotUpdateAssetsList()
    {
        assetList = new List<HotUpdateAssetItem>();
    }
}