//  HotUpdateAssetsList.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/8/2018.
//�ȸ�����Դ���б��MD5
using UnityEngine;
using System.Collections.Generic;

[SerializeField]
public class HotUpdateAssetItem
{
    public string assetPath;//��Դ����
    public string md5;
}
public class HotUpdateAssetsList 
{
    //��������

    //������Դ�б�
    public List<HotUpdateAssetItem> assetList;
    public HotUpdateAssetsList()
    {
        assetList = new List<HotUpdateAssetItem>();
    }
}