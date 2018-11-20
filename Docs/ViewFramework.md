# 视图管理

### 目录
- [介绍](#介绍)
    - [AView](#AView)
    - [StageMgr](#StageMgr)
    - [UIPanelMgr](#UIPanelMgr)
    - [UIWinMgr](#UIWinMgr)
- [初始化管理器](#初始化管理器)
- [注册视图](#注册视图)
- [捕获UI事件](#捕获UI事件)
    - [高效的不可见的响应UI事件的组件： TransparentRaycast]()

## 介绍
Zero提供了了一套视图管理解决方案，在该方案下，开发者可以方便的管理界面上的元素。该视图管理方案是Zero的核心之一，建议所有使用Zero框架的项目都使用视图管理模块来进行开发。

### AView
AView是一个抽象的视图包装器类，每一个AView都有一个关联的GameObject。所有的视图（Prefab）都需要一个对应的AView子类来对其进行管理。

##### 生命周期

1. OnInit  
**初始化时触发，此时已关联GameObject**
2. OnData(object data)  
**初始化后如果有数据传入，该方法被触发**
3. OnEnable  
**GameObject的Active变为true时触发**
4. OnDisable  
**GameObject的Active变为false时触发**
5. OnDestroy  
**被销毁时触发**

##### 参数

- onDestroyHandler  
**销毁委托事件**
- gameObject  
**关联的GameObject对象**
- IsDestroyed  
**是否销毁了**
- Name  
**对象(GameObject)名称**
- ViewName  
**对象对应的视图（Prefab）名称**

##### 设置Active

```
public void SetActive(bool isActive)
```


##### 获取组件

```
public T GetComplent<T>()
```

##### 获取子对象

```
public Transform GetChild(string childName)
public GameObject GetChildGameObject(string childName)
public T GetChildComplent<T>(string childName)
```

##### 子视图创建

```
public T CreateViewChlid<T>(string childName, object data = null) where T:AView
```

每一个AView的子对象都可以用另一个AView对象去管理，通过该API可以将子GameObject通过用AView子类进行包装。

注意：当父AView对象销毁时，其下的所有子AView对象都会被销毁。

##### 获取绑定数据

```
AView
{
    public UnityEngine.Object[] GetBindingObject(string key);
    public double[] GetBindingDouble(string key);
    public float[] GetBindingFloat(string key);
    public int[] GetBindingInt(string key);
    public string[] GetBindingString(string key);
}
```

### StageMgr接口

```
/// <summary>
/// 切换Stage
/// </summary>
/// <param name="viewName">视图名称</param>
/// <param name="data">传递的数据</param>
/// <param name="isClearPanel">是否清理UIPanel</param>
/// <param name="isCloseWindows">是否清理UIWin</param>
/// <returns></returns>
public AView Switch(string viewName, object data = null, bool isClearPanel = true, bool isCloseWindows = true)
```

```
/// <summary>
/// 切换Stage
/// </summary>
/// <typeparam name="T">对应Stage的AView对象</typeparam>
/// <param name="data">传递的数据</param>
/// <param name="isClearPanel">是否清理UIPanel</param>
/// <param name="isCloseWindows">是否清理UIWin</param>
/// <returns></returns>
public T Switch<T>(object data = null, bool isClearPanel = true, bool isCloseWindows = true) where T : AView
```

```
/// <summary>
/// 异步切换场景
/// </summary>
/// <param name="viewName">视图名称</param>
/// <param name="data">传递的数据</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
/// <param name="isClearPanel">是否清理UIPanel</param>
/// <param name="isCloseWindows">是否清理UIWin</param>
public void SwitchASync(string viewName, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, bool isClearPanel = true, bool isCloseWindows = true)
```

```
/// <summary>
/// 异步切换场景
/// </summary>
/// <typeparam name="T">对应Stage的AView对象</typeparam>
/// <param name="data">传递的数据</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
/// <param name="isClearPanel">是否清理UIPanel</param>
/// <param name="isCloseWindows">是否清理UIWin</param>
public void SwitchASync<T>(object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, bool isClearPanel = true, bool isCloseWindows = true)
 
```

```
/// <summary>
/// 清理当前的舞台
/// </summary>
/// <param name="isClearPanel">是否清理UIPanel</param>
/// <param name="isCloseWindows">是否清理UIWin</param>
public void ClearNowStage(bool isClearPanel = true, bool isCloseWindows = true)
```

### UIPanelMgr接口

```
/// <summary>
/// 切换UIPanel
/// </summary>
/// <param name="viewName">视图名称</param>
/// <param name="data">传递的数据</param>
/// <returns></returns>
public AView Switch(string viewName, object data = null)
```

```
/// <summary>
/// 切换UIPanel
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data">传递的数据</param>
/// <returns></returns>
public T Switch<T>(object data = null) where T : AView
```

```
/// <summary>
/// 异步切换UIPanel
/// </summary>
/// <param name="viewName"></param>
/// <param name="data">传递的数据</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
public void SwitchASync(string viewName, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)

```

```
/// <summary>
/// 异步切换UIPanel
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data">传递的数据</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
public void SwitchASync<T>(object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
```

```
/// <summary>
/// 清理当前的面板
/// </summary>
public void ClearNowPanel()
```

### UIWinMgr接口

```
/// <summary>
/// 打开窗口
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data">传递的数据</param>
/// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
/// <param name="isCloseOthers">是否关闭其它窗口</param>
/// <returns></returns>
public T Open<T>(object data = null, bool isBlur = true, bool isCloseOthers = true) where T : AView
```

```
/// <summary>
/// 打开窗口
/// </summary>
/// <param name="viewName">视图名称</param>
/// <param name="data">传递的数据</param>
/// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
/// <param name="isCloseOthers">是否关闭其它窗口</param>
/// <returns></returns>
public AView Open(string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true)
```

```
/// <summary>
/// 异步打开窗口
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data">传递的数据</param>
/// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
/// <param name="isCloseOthers">是否关闭其它窗口</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
public void OpenAsync<T>(object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null)
```

```
/// <summary>
/// 异步打开窗口
/// </summary>
/// <param name="viewName">视图名称</param>
/// <param name="data">传递的数据</param>
/// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
/// <param name="isCloseOthers">是否关闭其它窗口</param>
/// <param name="onCreated">创建完成回调方法</param>
/// <param name="onProgress">创建进度回调方法</param>
public void OpenAsync(string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null)
```

```
/// <summary>
/// 关闭窗口
/// </summary>
/// <param name="target">关闭对象</param>
public void Close(AView target)
```

```
/// <summary>
/// 关闭(当前打开的)所有窗口
/// </summary>
public void CloseAll()
```

## 初始化管理器

在IL代码的入口，我们要先初始化视图管理工具，指定各类视图的根容器。

```
//这是标准ILContent模板下初始化的方法

var ILContent = GameObject.Find(Runtime.Ins.VO.mainPrefab.assetName);
var stageRoot = ILContent.transform.Find("Stage");
var uiPanelRoot = ILContent.transform.Find("UICanvas/UIPanel");
var uiWinRoot = ILContent.transform.Find("UICanvas/UIWin");

StageMgr.Ins.Init(stageRoot);
UIPanelMgr.Ins.Init(uiPanelRoot);
UIWinMgr.Ins.Init(uiWinRoot);
```

## 注册视图
只有注册过的界面，才可以通过视图管理器来管理。

```
/// <summary>
/// 注册一个界面
/// </summary>
/// <param name="viewName">Prefab的名称</param>
/// <param name="abName">Prefab所在AssetBundle的名称</param>
/// <param name="type">Prefab的Type</param>
static public void Regist(string viewName, string abName, Type type)
```

## 捕获UI事件

#### UIEventListener

通过UIEventListener实现简单方便的UI事件捕获，其包括了大多数的UI事件，只需要注册对应的委托即可使用。

比如我们要对一个按钮图片进行点击事件捕获

```
UIEventListener.Get(btnTap).onClick += OnClickGo;
UIEventListener.Get(btnTap).onClick -= OnClickGo;
```

#### 事件捕获优化

UIEventListener直接对对象捕获了多数UI事件，但是我们只想关注对象的单个事件时，可以用高效的更具体的事件捕获类。他们的使用方法和UIEventListener一致，只是事件类型单一。目前已有的有：

- PointerBeginDragEvent
- PointerClickEvent
- PointerDownEvent
- PointerDragEvent
- PointerEndDragEvent
- PointerEnterEvent
- PointerExitEvent
- PointerMoveEvent
- PointerUpEvent

#### 高效的不可见的响应UI事件的组件： TransparentRaycast 

当我们需要在GameObject上放置一个透明的组件，用来响应UI事件时，可以使用TransparentRaycast.cs，他是一个透明的且优化过的UI组件。