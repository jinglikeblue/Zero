# Preload

### 目录
- [简介](#简介)
- [Zero的四种资源资源模式](#Zero的四种资源资源模式)
- [预热](#预热)
    1. [解压Package.zip化](#解压Package.zip)
    2. [检查/更新Setting.json](#检查/更新setting.json)
    3. [检查/更新客户端](#检查/更新客户端)
    4. [检查/更新启动资源](#检查/更新启动资源)
    5. [启动ILContents](#启动ILContents)
- [通过Runtime配置运行环境](#通过Runtime配置运行环境)

## 简介 

> Zero中的Preload.cs脚本是整个程序预热的核心组件。

Hierarchy中的Preload是一个简单的游戏启动的加载视图，该视图可以由使用者自己安排，用来表示游戏的启动画面和加载进度。重要的是该GameObject上我们绑定了组件「Preload.cs」，该组件上有一个「Runtime」配置详细描述了程序在启动时运行环境的参数。

整个程序的预热过程都在该类中完成，其中依次包括：
1. 内嵌资源初始化
2. Setting文件更新检查
3. 客户端更新检查
4. 资源更新检查
5. 启动ILContent对象

## Zero的四种资源资源模式
考虑到不同项目对于资源的需求，Zero提供了以下四种资源的使用方式

- INLINE_RELEASE（内嵌资源项目-发布模式）
 
该模式下所有的资源不依赖网络，用以开发纯单机的游戏

- NET_RELEASE（网络资源项目-发布模式）  

该模式下部分资源依赖于网络进行更新，用以开发不需要重装APP也能更新游戏内容的游戏

- NET_LOCAL_DEBUG（网络资源项目-开发模式）  

该模式是网络资源项目的开发阶段的可选模式。  
在开发阶段加载本地资源来替代网络资源的开发模式，用以提高开发调试的效率

- NET_LOCAL_AND_RESOURCES_DEBUG（网络资源项目-开发模式）

该模式是网络资源项目的开发阶段的可选模式。    
当需要使用资源的时候，资源管理器会自动从Resources目录下进行读取，这样修改资源可以立刻调试不用重新Build，提高开发效率。  
<font color=#FF0000>限制条件：需要打包的资源必须放在Resources目录下该模式才可正常使用</font>

>当使用「INLINE_RELEASE」资源模式时预热阶段会跳过前面步骤直接从[启动ILContents](#启动ILContents)开始


## 预热
所谓预热，就是指在我们真正的逻辑代码开始执行前，Zero框架会把所有启动游戏必须的资源都提前准备好，再启动游戏业务逻辑内容。而我们的Preload.cs脚本就是负责Zero预热的。
预热的内容包括：

### 解压Package.zip
>在我们打包发布APP的时候，有些资源希望第一次安装的用户不用从网上去下载启动必须的资源，那么我们可以选择将这些资源压缩为「Package.zip」（**注意名字必须一致**），并放到StreamingAssets目录下，这样打包的时候便会和APP一起发布。

程序启动的时候，会检测如果是第一次安装程序，且存在Package.zip，便会将其解压出来。解压后的资源使用方式和热更资源的使用方式一致，参考 **「资源管理解决方案」**

### 检查/更新setting.json
>setting.json可以理解为网络资源的入口文件，所有网络资源的加载都从这个配置文件开始。该文件可通过Editor中的菜单项「Zero/Setting」进行配置/发布。

. 客户端版本号
描述了客户端的版本号(匹配*Application.version*)以及客户端需要更新时的下载地址。在[检查/更新客户端](#检查/更新客户端)中使用。
客户端版本号跳转功能，可以让指定版本号的客户端，使用指定地址的setting.json文件来初始化项目。

. 资源配置
描述了使用的网络资源的根目录（Zero基于APP平台会加上对应的子目录），启动需要的资源组(在[检查/更新启动资源](#检查/更新启动资源)中使用)。

. 附加参数
用户可以在附加参数里自定义一些程序初始化需要的参数。

### 检查/更新客户端
>当客户端版本和服务器配置不一致时，会在浏览器中访问配置的更新地址URL。

### 资源更新检查
>加载配置的资源目录里的res.json文件，并根据配置的启动需要资源组，找出网络上和本地版本不一致的资源并更新。

res.json文件描述了网络资源的存储路径以及MD5码，可以通过Editor中的菜单项「Zero/Res」进行配置/发布。

### 启动ILContents
>当Zero完成了预热以后，则会生成ILContents这个Prefab，并销毁Preload。至此整个游戏进入中间层阶段。

#### 预热的状态以及进度获取

Preload.cs提供了以下两个委托，用来获取当前Preload的情况：
- onStateChange  
当前预热状态切换时触发，参数表示进入的状态
- onProgress  
当前预热状态的进度[0 - 1]

## 使用Preload.cs来启动游戏
我们只需要创建一个启动界面的GameObject，并绑定脚本「Preload.cs」，并配置好Runtime参数即可。


## 通过Runtime配置运行环境
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
    
    
    