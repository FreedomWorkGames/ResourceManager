using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class testJsonClass
{
    public string name;
    public string age;
    public testJsonClass()
    {

    }
    public testJsonClass(string name,string age)
    {
        this.name = name;
        this.age = age;
    }
}
public class testJsonClassList
{
    public List<testJsonClass> list;
    public testJsonClassList()
    {
        list = new List<testJsonClass>();
    }
}
public class testJson : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        TestToJson();
    }
    void TestFromJson(string jsonStr)
    {
        //string jsonStr = "{\"name\": \"tom\",\"age\": 11}";
        testJsonClassList testClass = JsonUtility.FromJson<testJsonClassList>(jsonStr);


        Debug.Log("");
    }
    void TestToJson()
    {
        HotUpdateAssetItem item = new HotUpdateAssetItem("123","456");
        HotUpdateAssetsList hotUpdateAssetsList = new HotUpdateAssetsList();
        hotUpdateAssetsList.assetList.Add(item);
        hotUpdateAssetsList.assetList.Add(item);
        string jsonStr = JsonUtility.ToJson(hotUpdateAssetsList);

        TestFromJson(jsonStr);
        Debug.Log("testToJson: " + jsonStr);
    }
}
