# 代码扩展

### 目录

- [根据项目需要封装组件](#根据项目需要封装组件)



## 根据项目需要封装组件

> 举例：扩展IL中AView，捕获Collider碰撞事件

就像我们知道的那样，通常我们要捕获碰撞事件例如OnCollisionEnter，我们通常的做法是：

1. 创建一个MonoBehaviour子类，并实现OnCollisionEnter方法
2. 将子类绑定到GameObject上来捕获Collider对象的碰撞

但是我们的AView对象都不是继承MonoBehaviour的，那么要实现碰撞的监听直接写OnCollisionEnter方法显然是不行的。于是我们加入了一套EventListener机制。通过该机制我们将AView捕获GameObject上Collider对象碰撞的流程修改为下：

1. 创建继承于MonoBehaviour的EventListener类（Zero.PhysicsEventListener）
2. 重写实现我们关注的回调方法
3. 定义和回调方法关联的委托，在这些回调方法被调用时，呼叫委托方法

在Demo的Block中，我们就使用了定义好的Zero.PhysicsEventListener类。

```
class Block : AView
{
    protected override void OnEnable()
    {
        //自动获取gameObject上的Physics2DEventListener组件（没有的话会自动添加的），并添加TriggerEnter的委托方法
        Physics2DEventListener.Get(this.gameObject).onTriggerEnter2D += OnTrigger;
    }

    protected override void OnDisable()
    {
        //在不需要时移除委托，提高代码效率
        Physics2DEventListener.Get(this.gameObject).onTriggerEnter2D -= OnTrigger;
    }

    private void OnTrigger(Collider2D obj)
    {
        Destroy();
    }
}
```

其实我们的UI事件也是通过此种方法来实现监听的。在开发的时候，大家可以根据自己的需要，来根据项目的具体情况添加此类EventListener类。