//  BuildAssetBundles.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/3/2018.
//
using UnityEngine;


public class BuildAssetBundles : MonoBehaviour
{
    public Transform transformCahed
    {
        private set;
        get;
    }	
 
    // Use this for initialization
    void Start()
    {
        transformCahed = this.transform;
    }
     
    // Update is called once per frame
    void Update()
    {
     
    }
}