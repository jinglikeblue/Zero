using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// XCODE项目初始化参数
/// </summary>
[Serializable]
public class IOSProjectInitData
{
    /// <summary>
    /// 是否需要支付宝SDK
    /// </summary>
    public bool needAliSDK = true;

    /// <summary>
    /// 是否需要微信SDK
    /// </summary>
    public bool needWXSDK = true;

    /// <summary>
    /// 程序支持被拉起的URLScheme
    /// </summary>
    public string[] urlSchemes;

    /// <summary>
    /// 程序信任的URLScheme
    /// </summary>
    public string[] whiteSchemeList;
    static public void Save(IOSProjectInitData data)
    {
        string jsonStr = JsonUtility.ToJson(data);

        string dir = Path.GetDirectoryName(GetFilePath());        
        if (false == Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(GetFilePath(), jsonStr);
    }

    static public IOSProjectInitData Load()
    {
        IOSProjectInitData data = null;
        try
        {
            string jsonStr = File.ReadAllText(GetFilePath());
            data = JsonUtility.FromJson<IOSProjectInitData>(jsonStr);
        }
        catch
        {

        }
        if(null == data)
        {
            data = new IOSProjectInitData();
        }
        return data;
    }

    static public string GetFilePath()
    {                 
        DirectoryInfo dir = Directory.GetParent(Application.dataPath);
        return Path.Combine(dir.ToString(), "IOSProjectInit/data.json");
    }
    
}
