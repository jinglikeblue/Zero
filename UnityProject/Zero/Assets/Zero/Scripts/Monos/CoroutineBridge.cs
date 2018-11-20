using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 协程中心
    /// </summary>
    public class CoroutineBridge : MonoBehaviour
    {

        const string NAME = "CoroutineBridge";

        static CoroutineBridge _ins;

        public static CoroutineBridge Ins
        {
            get
            {

                if (null == _ins)
                {
                    GameObject ins = GameObject.Find(NAME);
                    if (null == ins)
                    {
                        ins = new GameObject();
                    }
                    ins.name = NAME;
                    _ins = ins.AddComponent<CoroutineBridge>();
                    GameObject.DontDestroyOnLoad(ins);
                }

                return _ins;
            }
        }

        public void Run(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

        public void Stop(IEnumerator routine)
        {
            StopCoroutine(routine);
        }

        public void StopAll()
        {
            StopAllCoroutines();
        }
    }
}