using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 物理碰撞事件的捕获器
    /// </summary>
    public class PhysicsEventListener : MonoBehaviour
    {
        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionStay;
        public Action<Collision> onCollisionExit;
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        /// <summary>
        /// 获取GameObject的PhysicsEventListener事件组件
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static PhysicsEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PhysicsEventListener>(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (null != onCollisionEnter)
            {
                onCollisionEnter.Invoke(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (null != onCollisionStay)
            {
                onCollisionStay.Invoke(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (null != onCollisionExit)
            {
                onCollisionExit.Invoke(collision);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (null != onTriggerEnter)
            {
                onTriggerEnter.Invoke(collision);
            }
        }

        private void OnTriggerStay(Collider collision)
        {
            if (null != onTriggerStay)
            {
                onTriggerStay.Invoke(collision);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (null != onTriggerExit)
            {
                onTriggerExit.Invoke(collision);
            }
        }

    }
}