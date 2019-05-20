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

- ResMode  
资源模式

- NetRoot  
网路资源放置的根目录（不含平台路径）

- DevelopResRoot  
本地存放开发资源的目录，网络资源-开发模式时会使用

- LogEnable  
是否允许Zero的Log系统打印日志

- ILCfg  
针对IL（可热更）代码的配置

    - IsOnlyDll 
    true：则强制加载DLL并执行IL代码  false：有项目本地代码的情况下，优先使用本地IL代码，如果没有，则加载DLL代码
    
    - FileDir  
    在资源目录中DLL文件所在的文件夹的（相对）路径
    
    - FileName  
    DLL文件的文件名，不需要加后缀
    
    - ClassName  
    IL代码启动的类的完全限定名称
    
    - MethodName  
    启动类中的启动方法的名称
    
    - IsDebugIL  
    是否开启IL代码调试，需要配合ILRuntime的VS插件，详见ILRuntime介绍
    
    - IsLoadPdb  
    是否加载DLL匹配的PDB文件，如果需要IL代码出错时打印出错代码位置，则需要开启，且确保PDB文件和DLL文件放在同一目录。正式发布时建议关闭该选项。

 - MainPrefab    
 预热完以后启动业务逻辑中间件的配置

    - ABName
    中间件所在资源包的名称
    
    - AssetName
    中间价Prefab的名称
    
    
    