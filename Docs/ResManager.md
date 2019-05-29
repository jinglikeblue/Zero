# 资源管理

### 目录
- [ResMgr介绍](#ResMgr介绍)
- [初始化管理器](#初始化管理器)
- [获取资源](#获取资源)
    - [资源路径的格式](#资源路径的格式)
- [释放资源](#释放资源)
- [GC](#GC)


## ResMgr介绍
ResMgr提供了统一的本地游戏资源获取接口，它可以对本地AssetBundle文件或者是Resources目录中的资源进行加载/释放。

## 初始化管理器

>资源管理器的初始化工作会在Preload中根据Runtime配置自动完成，这里我们只是了解下就可以了。

```
/// <summary>
/// 初始化管理器
/// </summary>
/// <param name="type">资源管理器类型</param>
/// <param name="manifestFilePath">Manifest文件地址</param>
public void Init(EResMgrType type, string manifestFilePath)
```

目前可以通过资源管理器管理的资源类型有三种

- EResMgrType.ASSET_BUNDLE  
**使用的资源来自于AssetBundle文件**
- EResMgrType.RESOURCES  
**使用的资源来自于Resources目录**
- EResMgrType.ASSET_DATA_BASE  
**使用的资源通过UnityEditor.AssetDatabase类加载。该方式仅支持Editor模式**

Manifest文件

指的并不是后缀名为.manifest的文件，而是打包AB时生成的描述所有AB资源依赖的AB包。

## 获取资源

### 同步方式获取资源

```
/// <summary>
/// 加载资源
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="abName">资源包名称</param>
/// <param name="assetName">资源名称</param>
/// <returns></returns>
public T Load<T>(string abName, string assetName) where T : UnityEngine.Object
```
或者
```
public T Load<T>(string assetPath) where T : UnityEngine.Object
```


### 异步方式获取资源

```
/// <summary>
/// 异步加载一个资源
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="abName">资源包名称</param>
/// <param name="assetName">资源名称</param>
/// <param name="onLoaded">资源加载完成的回调</param>
/// <param name="onProgress">资源加载的进度</param>
public void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
```
或者
```
public void LoadAsync(string assetPath,  Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
```

#### 资源路径的格式

>假设我们打包的ab资源目录下有assetbundle文件ab/hot_res/audios.ab，其中有个声音资源叫click。那么我们通过以下三种格式的写法来获取该资源：  

<font color=#4EB170>

>1.ResMgr.Ins.Load<AudioClip>("hot_res/audios.ab", "click");  

>2.ResMgr.Ins.Load<AudioClip>("hot_res/audios", "click");  

>3.ResMgr.Ins.Load<AudioClip>("hot_res/audios/click");

</font>

### 获取资源的依赖

```
/// <summary>
/// 得到AB资源的依赖
/// </summary>
/// <param name="abName"></param>
/// <returns></returns>
public string[] GetDepends(string abName)
```


## 释放资源
不是用的资源可以释放掉以避免内存泄漏以及回收闲置内存

```
/// <summary>
/// 卸载资源
/// </summary>
/// <param name="abName">资源包名称</param>
/// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
/// <param name="isUnloadDepends">是否卸载关联的资源</param>
public void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
```

```
/// <summary>
/// 卸载所有资源
/// </summary>
/// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
public void UnloadAll(bool isUnloadAllLoaded = false)
```

## GC
为了避免APP占用内存过大，我们在适当的时候可以释放掉APP占用的闲置内存。  
调用的频率不能太快，GC会有一定的CPU开销。

```
/// <summary>
/// 执行一次内存回收(该接口开销大，可能引起卡顿)
/// </summary>
public void DoGC()
{
    //移除没有引用的资源
    Resources.UnloadUnusedAssets();
    GC.Collect();
}
```
