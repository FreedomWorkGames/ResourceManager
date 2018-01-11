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
        testJsonClass tom = new testJsonClass();
        tom.name = "tom";
        tom.age = "11";
        testJsonClassList list = new testJsonClassList();
        list.list = new List<testJsonClass>();
        list.list.Add(tom);
        list.list.Add(tom);
        string jsonStr = JsonUtility.ToJson(list);

        TestFromJson(jsonStr);
        Debug.Log("testToJson: " + jsonStr);
    }
}
