### 目录
- [下载网络文件](#下载网络文件)
- [Zip文件解压](#Zip文件解压)
- [Log打印](#Log打印)

## 下载网络文件

unity自带的WWW类下载文件的时候，只能将文件整个缓存到内存。如果下载的文件过大，会对内存造成很大的开销。为了节省内存，我们对更底层的System.Net.WebRequest进行了封装用来直接将文件从网络下载到硬盘上，并且只需要占用很少的内存。

#### 单个文件的下载

通过使用Jing.Downloader类，可以将网络文件下载到本地。使用方法可以参考Zero.SettingUpdate类。使用方法和WWW类几乎一致，都通过isDone、progress、error等属性来判断下载的状况。只是初始化的方法略有不同。

```
/// <summary>
/// 初始化下载类
/// </summary>
/// <param name="url">下载文件的URL地址</param>
/// <param name="savePath">保存文件的本地地址</param>
/// <param name="version">URL对应文件的版本号</param>
public Downloader(string url, string savePath, string version = null)
```

#### 多个文件的队列下载

通过使用Jing.GroupDownloader，我们可以将一组资源按照添加的顺序依次下载。使用方法可以参考Zero.ResUpdate类。

```
//实例化一个资源组下载器
GroupDownloader groupLoader = new GroupDownloader();
foreach (var itemName in itemSet)
{
    string localVer = Runtime.Ins.localResVer.GetVer(itemName);
    var netItem = Runtime.Ins.netResVer.Get(itemName);

    if (localVer != netItem.version)
    {
        //将要下载的文件依次添加入下载器
        groupLoader.AddLoad(Runtime.Ins.netResDir + itemName, Runtime.Ins.localResDir + itemName, netItem.version, OnItemLoaded, netItem);
    }
}
//启动下载器开始下载
groupLoader.StartLoad();

//判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
do
{
    _onProgress.Invoke(groupLoader.progress);
    yield return new WaitForEndOfFrame();
}
while (false == groupLoader.isDone);

//判断下载是否返回错误
if (null != groupLoader.error)
{
    Log.E("下载出错：{0}", groupLoader.error);
    yield break;
}
```

## Zip文件解压

当我们在本地存在一个Zip文件，需要将其中的内容解压出来使用的时候，可以使用Zero.ZipHelper类对文件进行解压。

你可以直接解压压缩文件数据

```
/// <summary>
/// 解压文件
/// </summary>
/// <param name="bytes">二进制数据</param>
/// <param name="targetDir">解压目录</param>
public void UnZip(byte[] bytes, string targetDir)
```


也可以通过指定文件的地址进行解压

```
/// <summary>
/// 解压文件
/// </summary>
/// <param name="zipFile">压缩文件路径</param>
/// <param name="targetDir">解压目录</param>
public void UnZip(string zipFile, string targetDir)
```

具体使用方法可以参考Zero.PackageUpdate类

```
ZipHelper zh = new ZipHelper();
zh.UnZip(www.bytes, Runtime.Ins.localResDir);
while (false == zh.isDone)
{
    onProgress((zh.progress * 0.5f) + 0.5f);
    yield return new WaitForEndOfFrame();
}
```

## Log打印

Zero为了方便自己封装了Zero.Log类来打印内容。并且可通过Preload配置Runtime的LogEnable属性来控制打印的开关。

你也可以方便的合成含有颜色的文字内容。