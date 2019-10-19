![](Docs/Imgs/Zero.png)

---

![](Docs/Imgs/icon.jpg)

# 简介

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

# Zero的特点

- 通常我们在开发的不同时期，对资源的加载方式有不同的需求。而这些你都不用改动一行代码，只需在Inspector中一个设置即可搞定。
    - Zero提供统一的资源管理中心，可以在各种不同的资源加载方式中无缝切换。调试时你可以通过直接加载编辑中的资源。发布后你可以通过情况加载本地AssetBundle文件或来自网络的AssetBundle文件进行调试。
    - 项目代码的热更方案可以快速的切换无热更/反射热更/ILRuntime热更
- 热更资源的打包通过工具整合，不用复杂的管理方案，一次配置好后，代码以及资源一键发布。
    - AssetBundle打包自带依赖自动分析，开发者不用再担心依赖资源打包问题，Zero打包出的AssetBundle之间不会有冗余资源。
    - Zero打包的资源自带版本号文件res.json。配合Zero内部的资源下载工具，可以每次最小化更新资源。
- Zero提供的视图管理框架可以轻松的对游戏中的视图进行管理。

# 快速入门

#### Unity环境
Unity版本： **2018.4.0**  
Scripting Runtime Version: **.NET 4.x Equivalent**    
Api Compatibility Level: **.NET 4.x**    

[中文文档(https://jinglikeblue.github.io/Zero/Docs/Intro)](https://jinglikeblue.github.io/Zero/Docs/Intro)

[2D游戏Demo(https://github.com/jinglikeblue/Zero2DGameDemo)](https://github.com/jinglikeblue/Zero2DGameDemo)

[3D游戏Demo(https://github.com/jinglikeblue/Zero3DGameDemo)](https://github.com/jinglikeblue/Zero3DGameDemo)

# 交流平台

QQ群：695429639

![](Docs/Imgs/QQChatGroups.png)


---
## 更新日志

```
2019.10
Version 2.0
```

>功能调整

- 以下内容直接限定，不再为可配置内容，减少误操作  
    - 启动Prefab
    - 启动类，包括方法
    - DLL文件名
- 增加@Configs目录，放置配置文件，打包时可以输出到打包目录
- 更新只保留打开更新URL，如果需要其它操作，请自行扩展
- 移除Socket库，如需要socket通信请使用第三方库
- SettingUpdate的版本号跳转功能阉割掉，作用不大

>优化
- OnData调整
    - 改为必定触发，没有数据时传null
    - OnData改为在OnInit后，OnEnable前触发，可参考Adam的OnInitData

>Editor
- 完全重构Editor
    - 使用Odin插件重写Editor工具
    - 重新设计Editor结构，配置统一化，清晰、简洁
- HotRes发布修改
    - AssetBundle部分直接采用Adam的打包方案，提高打包效率，减少打包操作
    - 热更资源AB目录限定为@Resources目录
        - 目录限定后，因为可以确定AB包的名称，所以可以通过工具，自动生成该目录的AB文件名称常量
    - 热更代码开发目录限定为@Scripts目录
    - 增加热更配置目录@Configs目录
    - res.json不再配置Manifest文件路径，自动生成
- 不再提供DLL代码排除功能（意义不大）

