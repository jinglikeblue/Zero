# Preload

### 目录
- [简介](#简介)
- [Zero的四种资源资源模式](#Zero的四种资源资源模式)
- [预热](#预热)
    1. [解压Package.zip化](#1.解压Package.zip)
    2. [检查/更新Setting.json](#2.检查/更新setting.json)
    3. [检查/更新客户端](#3.检查/更新客户端)
    4. [检查/更新启动资源](#4.检查/更新启动资源)
    5. [启动ILContents](#5.启动ILContents)
- [Preload配置Runtime参数详解](#Preload配置Runtime参数详解)

![](Imgs/preload_inspector.jpg)

---

## 简介

> Zero中的Preload.cs脚本是整个程序预热的核心组件。

Hierarchy中的Preload是一个简单的游戏启动的加载视图，该视图可以由使用者自己安排，用来表示游戏的启动画面和加载进度。重要的是该GameObject上我们绑定了组件「Preload.cs」，该组件上有一个「Runtime」配置详细描述了程序在启动时运行环境的参数。

整个程序的预热过程都在该类中完成，其中依次包括：

1. [解压Package.zip化](#1.解压Package.zip)
2. [检查/更新Setting.json](#2.检查/更新setting.json)
3. [检查/更新客户端](#3.检查/更新客户端)
4. [检查/更新启动资源](#4.检查/更新启动资源)
5. [启动ILContents](#5.启动ILContents)

>PS:如果项目不使用热更，则Preload会直接进入[步骤5]

---

## Zero的四种资源资源模式

考虑到不同项目对于资源的需求，Zero提供了以下四种资源的使用方式

- ***从Resources加载资源***
 
当Preload的「使用热更」未勾选时，表示项目为单机项目，这样通过资源管理器获取的资源都将来自于Resources目录

- ***使用AssetDataBase加载资源***

当项目为一个热更资源项目时，开发者可以在开发阶段使用这种资源加载方式来进行调试开发。

- ***从资源发布目录加载资源***

当项目为一个热更资源项目时，开发者在打包出AssetBundle后，可以先通过本地的资源加载来测试资源是否正确

- ***从网络资源目录加载资源***

当项目为一个热更资源项目时，开发者在打包出AssetBundle并更新到资源服务器后，通过该加载方式来加载资源。该方式也是热更项目的正式安装包的资源加载方式。

---

## 预热
所谓预热，就是指在我们真正的逻辑代码开始执行前，Zero框架会把所有启动游戏必须的热更资源都提前准备好，再启动游戏业务逻辑内容。而我们的Preload.cs脚本就是负责Zero预热的。

预热的步骤如下：

### 1.解压Package.zip
>在我们打包发布APP的时候，有些资源希望内嵌在APP中，那么我们可以选择将这些资源压缩为「Package.zip」（**注意名字、大小写必须一致**），并放到StreamingAssets目录下，这样打包的时候便会和APP一起发布。

程序启动的时候，会检测如果是第一次安装程序，且存在Package.zip，便会将其解压出来。解压后的资源使用方式和热更资源的使用方式一致，参考 **「资源管理解决方案」**

### 2.检查/更新setting.json
>setting.json可以理解为网络资源的入口文件，所有网络资源的加载都从这个配置文件开始。该文件可通过Editor菜单项[[Zero/Publish/Setting]](PublishEditor.md)进行配置/发布。

### 3.检查/更新客户端
>当客户端版本低于服务器配置时，会根据配置更新客户端。

### 4.资源更新检查
>加载热更资源的res.json(通过[[Zero/Publish/HotRes]](PublishEditor.md)发布)文件，并根据配置的启动资源组，比较并更新资源为最新版本。

### 5.启动ILContents
>当Zero完成了预热以后，则会创建[[ILContent]](ILContent.md)，并销毁Preload。

至此，整个游戏进入中间层(热更区域)阶段。

#### 预热的状态以及进度获取

Preload.cs提供了以下两个委托，用来获取当前Preload的情况：
- onStateChange  
当前预热状态切换时触发，参数表示进入的状态
- onProgress  
当前预热状态的进度[0 - 1]

---

## Preload配置Runtime参数详解
Inspector中参数解释：

- 是否打印日志<br>如果关闭该选项，则通过Zero.Log以及UnityEngine.Debug打印的日志会自动屏蔽。<br>注意：警告以及错误仍然会打印。建议正式版本关闭打印，可以提高性能

- 启动Prefab<br>该Prefab指的是Preload预热完后启动的对象。如果是热更项目，则该Prefab为热更资源。通常Zero中使用[「ILContent」](ILContent.md)即可

- 启动类(完全限定)<br>该Prefab指的是Preload预热完后会调用的第一个类对象。如果是热更项目，则该类为热更DLL中的类。

- 使用热更<br>如果没有勾选，则项目为本地项目，不依赖网络资源  

    - 资源来源

        - 从网络资源目录加载<br>将通过配置的网络目录获取setting文件，并下载热更资源

            - 网络资源根目录<br>该位置填写web服务器上放置资源的目录,格式通常为*http://wepieces.cn/unity/zero/demo/Res*这种

        - 从本地资源目录加载<br>将从*Zero/Publish/HotRes*中配置的Res发布目录下获取资源

            - 本地的资源根目录

        - 使用AssetDataBase加载(推荐开发阶段使用)

            - Asset中热更资源的目录<br>将从*Zero/Publish/HotRes*中配置的Res发布目录下获取资源

    - 使用DLL *如果不勾选，则会通过安装包的代码执行程序。勾选后，将根据选择的方式执行HotRes打包出的DLL*

        - DLL执行方式
            - 选择ILRuntime解释执行DLL可以兼容MONO和IL2CPP
            - 选择反射执行，则只能兼容MONO，但是因为JIT的原因执行性能高于ILRuntime

        - 文件目录

        - DLL文件名<br>打包出的DLL文件的名称，不需要扩展名

        - 启动方法(必须为Static)<br>启动类中的该方法将在Preload预热后被调用

        - 调试功能<br>开启后可以配合ILRuntime的调试工具，在真机环境调试DLL代码      

        - 加载Pdb文件<br>开启后可以在DLL中代码执行出错时打印错误堆栈信息
        
    
    
    