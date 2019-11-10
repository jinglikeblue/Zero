using UnityEngine;
using Zero;

public abstract class AEventListener<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// 获取GameObject的事件监听组件，没有则会自动添加
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T Get(GameObject gameObject)
    {
        return ComponentUtil.AutoGet<T>(gameObject);
    }
}
