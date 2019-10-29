# 热更架构介绍

### 目录
- [简介](#简介)    
    - 热更内容开始的入口
    - 热更配置文件
    - 热更AssetBundle文件
    - 热更代码文件
- [使用ILBridge捕获Unity引擎事件](#使用ILBridge捕获Unity引擎事件)
- [使用ILBridge执行协程](#使用ILBridge执行协程)
- [使用BindingData给GameObject绑定数据](#BindingData)
- [约定](#约定)

## 简介

> 为了应付项目的频繁更新以及问题修正，资源的热更新（动态更新）是一种在项目中常用的手段。在Unity中，提供了资源的热更方案，就是打包AssetBundle文件。代码部分，Zero通过整合ILRuntime框架实现了代码的热更，并且统一的开发语言（C#）让开发就如同常规开发一样方便。

## 热更内容开始的入口

在Preload预热流程的最后一步，会调用热更代码的Main函数，让程序进入热更代码区域。从这里开始所有的内容，都可以根据需求动态更新。

### 热更配置文件

通常我们需要热更新的配置文件，统一放在```Assets/@Configs```目录中，这样构建热更资源的时候会更新到发布目录

## 热更AssetBundle文件

我们需要打包放到AssetBundle中的资源都统一放在```Assets/@Resources```目录中，构建AB文件时会按照"[文件夹名].ab"的格式将各个文件夹中的资源打包到一个以文件夹名命名的ab包中。*@Resources根目录下的资源会被打包到root_assets.ab中*

配合AssetBundleName生成工具，可以快速创建所有ab包名称的常量

## 热更代码文件
放在```Assets/@Scripts```中的代码，可以通过发布工具构建为dll，这些代码都是可以再Preload中动态更新的。

## 使用ILBridge捕获Unity引擎事件
当我们希望在IL代码中捕获到Unity引擎的OnGUI、Update、FixedUpdate等事件时，可以通过单例ILBridge.Ins提供的委托来捕获。

```
ILBridge.Ins.onUpdate += UpdateHandler;

void UpdateHandler()
{
    Debug.Log("Update")
}
```


## 使用ILBridge执行协程
当我们希望在IL代码中启动一个协程时，可以通过ILBridge.Ins单例来执行协程方法。

- ILBridge.Ins.StartCoroutine(IEnumerator routine)  
**执行协程方法**
- ILBridge.Ins.StopCoroutine(IEnumerator routine)  
**中止协程方法**
- ILBridge.Ins.StopAllCoroutines()  
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
    - 热更框架不支持使用 .NET 4.5 以上的特性
    - 不可开启新的线程。多线程代码请在主工程位置编写。
