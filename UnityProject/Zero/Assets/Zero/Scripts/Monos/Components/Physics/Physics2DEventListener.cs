using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 2D物理碰撞事件的捕获器
    /// </summary>
    public class Physics2DEventListener : MonoBehaviour
    {
        public Action<Collision2D> onCollisionEnter2D;
        public Action<Collision2D> onCollisionStay2D;
        public Action<Collision2D> onCollisionExit2D;
        public Action<Collider2D> onTriggerEnter2D;
        public Action<Collider2D> onTriggerStay2D;
        public Action<Collider2D> onTriggerExit2D;

        /// <summary>
        /// 获取GameObject的Physics2DEventListener事件组件
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Physics2DEventListener Get(GameObject gameObject)
        {            
            return ComponentUtil.AutoGet<Physics2DEventListener>(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(null != onCollisionEnter2D)
            {
                onCollisionEnter2D.Invoke(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (null != onCollisionStay2D)
            {
                onCollisionStay2D.Invoke(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (null != onCollisionExit2D)
            {
                onCollisionExit2D.Invoke(collision);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (null != onTriggerEnter2D)
            {
                onTriggerEnter2D.Invoke(collision);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (null != onTriggerStay2D)
            {
                onTriggerStay2D.Invoke(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (null != onTriggerExit2D)
            {
                onTriggerExit2D.Invoke(collision);
            }
        }
    }
}