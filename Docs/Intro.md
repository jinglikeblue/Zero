![](Imgs/icon.jpg)

### 目录
- [从Demo开始](Demo.md)
- 内容
    - [Preload介绍](Preload.md)
    - [ILContent介绍](ILContent.md)
    - [视图管理](ViewFramework.md)
    - [资源管理](ResManager.md)
    - [其它](Other.md)
    - [扩展](Extend.md)
- Editor工具
    - [Publish](PublishEditor.md)
        - [Setting]
        - [HotRes]
    - [iOS]
        - [ProjectInit]
    - [Assets]
    - [IL2CPP]
    - [ILRuntime]    
- [Q&A](QuestionAnswer.md)


---

## 简介

Zero是在Unity中一套游戏开发框架，为游戏开发核心的问题提供了解决方案。其中包括但不限于：
- 资源管理
    - 可快速切换通过Resources接口加载资源的管理工具
    - 支持开发时通过AssetDataBase接口加载资源调试的管理工具
    - 支持通过AssetBundle加载资源的管理工具
- 视图管理
    - 不用继承MonoBehaviour的视图管理方案，提高视图部分的代码执行效率
- 项目统一使用C#的代码热更新
    - 基于ILRuntime热更框架（支持IL2CPP&Mono）
    - 基于动态Dll（仅支持Mono）
- 工具集
    - 热更资源发布工具
    - XCode项目参数配置工具
    - 资源优化快捷工具
    - IL2CPP工具
    - ILRuntime工具
    
>Zero的宗旨就是为想要快速入门的开发者提供一个可行的稳定的Unity项目开发解决方案。通过尽量简洁的使用方式来方便开发者快速上手，以及模块化的代码易于开发者扩展延伸代码的框架。
