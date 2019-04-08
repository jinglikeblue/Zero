目标版本：**alpha 0.2**

#### 测试环境
Unity版本： **2018.2.20**  
Scripting Runtime Version: **.NET 3.5 Equivalent**    
Api Compatibility Level: **.NET Standard 2.0**    


#### 修改
- 调整了一些代码和提示的细节
- 协程的使用从此改为使用ILBridge.Ins，CoroutineBridge已移除
- 目录调整，Res目录以及ILProject目录放到Asset中(方便一个项目迁出多个平台的项目)
- Demo直接支持所有4种资源模式的运行
- 增加了MonoBehaviour的单例模式组件SingletonMonoBehaviour。实现参考AudioPlayer。
- 状态机类修改为泛型委托
- 修改了DLL热更代码执行的方式
    - MONO编译模式时为直接反射执行DLL代码
    - IL2CPP编译模式时为使用ILRuntime框架执行DLL代码
    - 因为IL代码并不是一定运行在ILRuntime中，所以ILRuntimeBridge改为ILBridge。并且反射执行DLL和ILRuntime执行DLL两种方式拆分为ILWorker类。
    - ILRuntime需要的适配器，由玩家继承BaseILRuntimeGenerics并实现。
- 调试
    - Log.Msg改为使用GUI实现，移除旧的UGUI实现
- 新的图层管理机制Layer，可以根据项目需要创建任意多个图层
- 每个AView绑定的GameObject都添加一个ZeroView脚本，用来获取enable destroy等事件。这样GameObject被销毁的时候，AView也能捕获到并处理。
- Preload增加了错误捕获接口
- 升级了ILRuntime框架，可以支持2018.3
- 热更资源的发布功能整合到「Zero -> Publish -> HotRes」菜单中
    - 资源模式统一为所有资源放到Resources目录下用以兼容开发和正式环境的资源调用方式切换 

#### 新增
- 增加AudioPlayer组件，可以自定义音效轨道数量。管理播放声音
- 客户端更新增加一个选择项，可以指定下载优先为下载安装包还是跳转更新网址
- 新增一个Zero的Android库项目，提供Android下支持客户端直接下载APK更新。（详见文档介绍）
- 增加了IOS发布XCode项目时自动添加参数的Editor窗口
- 资源
    - res.json中增加文件大小（字节）
- UI
    - 可以在UGUI中播放序列帧动画的组件MovieClip
    - 用于扩展UGUI ScrollView的高效列表组件ZList
- 调试
    - 增加GUIDeviceInfo，可以显示游戏的帧数等信息
- Editor
    - PackingTag管理器   
    可以列举出项目中所有的Packing Tag标记。选择删除指定的Packing Tag标记。
    - Find Useless  
    可以查找选定目录中的无效资源（没有被其它任何资源引用，也没有AssetBundle标记）
    - link.xml生成器  
    扫描指定目录中的DLL，并生成Linker.xml，让IL2CPP排除对这些类的裁剪
    - ILRuntime管理器  
    通过管理器可以对热更DLL生成自动绑定代码
