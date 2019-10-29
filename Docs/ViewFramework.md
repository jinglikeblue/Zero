# 视图管理

### 目录
- [介绍](#介绍)
    - [AView](#AView)
    - [图层 AViewLayer](#图层)
        - [单视图图层 SingularViewLayer](#单视图图层)
        - [多视图图层 PluraViewLayer](#多视图图层)
- [捕获UI事件](#捕获UI事件)
    - [高效的不可见的响应UI事件的组件： TransparentRaycast]()

# 介绍
Zero提供了了一套视图管理解决方案，在该方案下，开发者可以方便的管理界面上的元素。该视图管理方案是Zero的核心之一，建议所有使用Zero框架的项目都使用视图管理模块来进行开发。

### AView
为了要最大限度的利用代码热更，所以我们的代码都不能继承MonoBehaviour。但是我们又要对GameObject进行管理，于是Zero提供了AView。  

>AView是一个抽象的视图包装器类，每一个AView都有一个关联的GameObject。所有的视图（Prefab）都需要一个对应的AView子类来对其进行管理。

##### 生命周期

1. OnInit(object data)  
**初始化时触发，此时已关联GameObject。如果初始化时没有传入数据，则data为null。**
3. OnEnable  
**GameObject的Active变为true时触发**
4. OnDisable  
**GameObject的Active变为false时触发**
5. OnDestroy  
**GameObject被销毁时触发**

##### 参数

- onDestroyed  
**对象已销毁的事件**
- gameObject  
**关联的GameObject对象**
- transform
**关联对象的Transform**
- isDestroyed  
**是否销毁了**

##### 设置Active

```
.
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
public T CreateChildView<T>(string childName, object data = null) where T:AView
```

每一个AView的子对象都可以用另一个AView对象去管理，通过该API可以将子GameObject通过用AView子类进行包装。

注意：当父AView对象销毁时，其下的所有子AView对象都会被销毁。

# 图层
所有的AView都应该在图层中被管理起来，Zero提供了两种类型的视图层，用来管理AView的显示。

## 单视图图层 

>SingularViewLayer

单一的视图层，该层中的视图，只能存在一个，视图之间的关系是切换。

Demo项目中基于该ViewLayer封装了UIPanelMgr以及StageMgr

## 多视图图层

>PluralViewLayer

复数的视图层，该层中的视图，可以同时存在

Demo项目中基于该ViewLayer封装了UIWinMgr

# 捕获UI事件

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