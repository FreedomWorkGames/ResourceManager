//  ResourceGetFaced.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/8/2018.
// ��ȡһ����Դ����������һ��ʵ��
using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceGetFaced//�����඼������һ�����������ר�õĽ��������Դʵ���ͻ�ȡ��Դ
{
#if USE_ASSETBUNDLE_REFRENCE_COUNT
    private Dictionary<string, uint> _referenceDicionary;
#endif

    public ResourceGetFaced()
    {
#if USE_ASSETBUNDLE_REFRENCE_COUNT
        _referenceDicionary = new Dictionary<string, uint>();
#endif
    }

    public void GetAssetSync(string path, Action<AssetBundle> handler)
    {
        ResourceManager.instance.GetAssetSync(path, handler);
#if USE_ASSETBUNDLE_REFRENCE_COUNT
        CachedReference(path);
#endif
    }

    public void OnDestroy()
    {
#if USE_ASSETBUNDLE_REFRENCE_COUNT
        var itr = _referenceDicionary.GetEnumerator();
        while(itr.MoveNext())
        {
            ResourceManager.instance.ReleaseAssetBundle(itr.Current.Key, itr.Current.Value);
        }
#endif
    }
#if USE_ASSETBUNDLE_REFRENCE_COUNT
    private void CachedReference(string path,uint count=1)
    {
        if (_referenceDicionary.ContainsKey(path))
            _referenceDicionary[path] = _referenceDicionary[path] + count;
        else
            _referenceDicionary.Add(path, count);

    }
#endif
}