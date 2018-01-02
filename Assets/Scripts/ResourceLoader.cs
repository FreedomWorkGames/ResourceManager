//  ResourceLoader.cs
//  ResourceManager
//
//  Created by ZhangRuiHao on 1/2/2018.
//
using UnityEngine;


public class ResourceLoader : MonoBehaviour
{
    public Transform transformCahed
    {
        private set;
        get;
  
    }
    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start()
    {
        this.transformCahed = this.transform;
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}