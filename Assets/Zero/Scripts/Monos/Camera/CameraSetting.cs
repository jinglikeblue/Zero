using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    [RequireComponent(typeof(Camera))]
    public class CameraSetting : MonoBehaviour
    {
        [Header("是否强制设置摄像机宽高比")]
        public bool isResetAspect = false;
        public float aspectW;
        public float aspectH;

        private void Awake()
        {
            if (isResetAspect)
            {
                GetComponent<Camera>().aspect = aspectW / aspectH;
            }
        }
    }
}