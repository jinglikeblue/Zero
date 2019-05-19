![](Imgs/icon.jpg)

### 目录
- [从Demo开始](#从Demo开始)
- [内容](#教程)
    - [Preload介绍](Preload.md)
    - [ILContent介绍](ILContent.md)
    - [视图管理](ViewFramework.md)
    - [资源管理](ResManager.md)
    - [其它](Other.md)
    - [扩展](Extend.md)
- [Editor工具](#Editor工具)
    - [Publish]
        - [Setting]
        - [HotRes]
    - [iOS]
        - [ProjectInit]
    - [Assets]
    - [IL2CPP]
    - [ILRuntime]    
- [Q&A](QuestionAnswer.md)
 

## 从Demo开始

>Zero框架自带Demo，简单描述了整个框架的运作流程以及部分功能的使用

为了让Demo能够正常的启动，请确保导入项目时选择的Platform为“PC,Mac&Linux Standalone”。

在unity项目文件夹中的Asset/Demo/Demo场景，即可进入Zero框架自带的示例程序。
这里仅有两个GameObject在Hierarchy中，其中一个是EventSystem，另一个是Preload。

#### Preload

> Zero中的Preload.cs脚本是整个程序预热的核心组件。

Hierarchy中的Preload是一个简单的游戏启动的加载视图，该视图可以由使用者自己安排，用来表示游戏的启动画面和加载进度。重要的是该GameObject上我们绑定了组件「Preload.cs」，该组件上有一个「Runtime」配置详细描述了程序在启动时运行环境的参数。

整个程序的预热过程都在该类中完成，其中依次包括：
1. 内嵌资源初始化
2. Setting文件更新检查
3. 客户端更新检查
4. 资源更新检查
5. 启动ILContent对象

从「Preload介绍」了解更多

#### ILContent

> ILContent是游戏业务逻辑内容的起点，根据需求可以放到热更环境中。IL前缀表示这是一个中间件。在Zero中，所有带有IL标识符的内容都表示可以进行热更。

在Preload完成预热后，会自动销毁绑定自己的GameObject，并拉起ILContent。而我们的游戏的整个逻辑则在ILContent中完成。

Asset/Zero/ILContent组件是Zero框架中视图框架的很好的模板。根据Demo代码可以理解到整个Zero的视图管理机制可以通过ILContent组件很好的运作起来。

- ILContent.cs 组件  
该组件被ILContent绑定，通过Runtime的配置，其决定了IL代码的环境是使用本地程序集（打包时内嵌的代码），还是外部程序集（热更DLL代码库）

从「ILContent介绍」了解更多

## 教程

>通过Demo只能大概的了解Zero框架的轮廓。其具体的细节以及如何利用请参考以下针对各个功能的详解

- [Preload介绍](Preload.md)
- [ILContent介绍](ILContent.md)
- [视图管理](ViewFramework.md)
- [资源管理](ResManager.md)
- [其它](Other.md)
- [扩展](Extend.md)
