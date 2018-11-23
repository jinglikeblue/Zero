# ILContent

### 目录
- [Zero中IL的定义](#IL定义)
- [ILContent模板Prefab](#ILContent模板)
- [ILContent.cs](#ILContent.cs)
- [使用ILRuntimeBridge捕获Unity引擎事件](#ILRuntimeBridge)
- [使用CoroutineBridge执行协程](#CoroutineBridge)
- [使用BindingData给GameObject绑定数据](#BindingData)
- [约定](#约定)

## Zero中IL的定义

在Zero中IL指的是包含游戏主体业务逻辑的资源、代码等内容。并且所有IL包括的（遵照「约定」开发的）内容都可以热更新。  

### 代码放置位置

业务逻辑代码统一放在Assets/ZeroIL中。

如果代码需要热更，则将ZeroIL文件夹中的代码统一打包为DLL即可。

建议给自己项目的代码创建一个文件夹来放置所有的代码。

Assets/ZeroIL/Zero目录中代码为Zero框架核心代码，同样参与到热更中。且千万不要删除。

## ILContent模板

Asset/Zero/ILContent是一个基于Zero的标准Prefab模板，结构如下：

- Stage  
Zero中的舞台，由StageMgr管理，同一时间只能有一个舞台在场景中，且舞台必须自带摄像机。用来放置Unity世界中的物体，可以理解为将Scene抽象到Prefab中来理解。
- UICamera  
Zero中用来显示UI的摄像机，由UICanvas绑定。因为该摄像机拍摄的内容和Stage中摄像机拍摄的内容渲染的层级不一样，如果有需求在Stage的物体与物体之间显示UI，那么可以直接在Stage中创建UI对象，只是那些对象需要开发者自行管理。摄像机参数需要根据项目自行调整。
- UICanvas  
Zero框架的UI容器，参数根据项目需求可自行调整。
    - UIPanel   
    Panel表示UI面板，在界面上最多只能存在一个，通过UIPanelMgr管理
    - UIWin  
    Win表示UI中的窗口，在界面上可以根据需要存在多个，通过UIWinMgr管理
        - Blur  
        窗口之间的阻挡，根据打开窗口时的配置，如果需要阻挡窗口与下方的交互，则该图层会出现。可以根据需要自行修改Blur的内容，但是请保留Blur本身。

## ILContent.cs
该组件被ILContent绑定，通过Runtime的配置，其决定了IL代码的环境是使用本地程序集（打包时内嵌的代码），还是外部程序集（热更DLL代码库）。

## 使用ILRuntimeBridge捕获Unity引擎事件
当我们希望在IL代码中捕获到Unity引擎的OnGUI、Update、FixedUpdate等事件时，可以通过单例ILRuntimeBridge.Ins提供的委托来捕获。

```
ILRuntimeBridge.Ins.onUpdate += UpdateHandler;

void UpdateHandler()
{
    Debug.Log("Update")
}
```


## 使用CoroutineBridge执行协程
当我们希望在IL代码中启动一个协程时，可以通过CoroutineBridge.Ins单例来执行协程方法。

- CoroutineBridge.Ins.Run(IEnumerator routine)  
**执行协程方法**
- CoroutineBridge.Ins.Stop(IEnumerator routine)  
**中止协程方法**
- CoroutineBridge.Ins.StopAll()  
**中止所有协程方法**

```
CoroutineBridge.Ins.Run(Update());

IEnumerator Update()
{
    yield return new WaitForEndOfFrame();
}
```

## 使用BindingData给GameObject绑定数据
当我们在Hierarchy里编辑一个视图GameObject的时候，希望能给这个视图绑定数据，这样可以直接在IL的AView中获取。我们可以通过在Inspector中添加数据绑定组件，来附加数据。在AView子类中可通过API直接获取绑定的数据,也可以自己通过GetComplent获得组件自行处理。

绑定的数据是以数据组为单位的，Inspector中的List表示数据组的数量，每一个Element的Key表示数据库的ID，List里则是数据组具体绑定的数据。


目前提供的绑定数据组件有

- DoubleBindingData
- FloatBindingData
- IntBindingData
- ObjectBindingData
- StringBindingData

如有需要可根据项目需求自行添加


## 约定
- Assets/ZeroIL中的代码
    - 均不可继承主工程的类（包括引擎命名空间以及.net命名空间）。
    - 热更框架不支持使用 .NET 3.5 以上的特性
    - 不可开启新的线程。多线程代码请在其它位置编写。
