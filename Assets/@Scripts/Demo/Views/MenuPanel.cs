using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ILDemo
{
    public class MenuPanel : AView
    {
        GameObject buttonPrefab;
        Transform content;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            buttonPrefab.SetActive(false);

            AddBtn("Roushan", RoushanTest);
            AddBtn("TestAndroidBridge", TestAndroidBridge);
            AddBtn("TestCrossDepend", TestCrossDepend);
            AddBtn("TestLoadAllAB", TestLoadAllAB);
        }

        private void TestCrossDepend()
        {
            var a = ResMgr.Ins.Load<GameObject>(AB.CROSS_DEPEND_TEST_A.A_assetPath);
            if (null != a)
            {
                Debug.Log($"资源读取成功：{AB.CROSS_DEPEND_TEST_A.NAME}");
            }
            else
            {
                Debug.Log($"资源读取失败：{AB.CROSS_DEPEND_TEST_A.NAME}");
            }
            var b = ResMgr.Ins.Load<GameObject>(AB.CROSS_DEPEND_TEST_B.B_assetPath);
            if (null != b)
            {
                Debug.Log($"资源读取成功：{AB.CROSS_DEPEND_TEST_B.NAME}");
            }
            else
            {
                Debug.Log($"资源读取失败：{AB.CROSS_DEPEND_TEST_B.NAME}");
            }
        }

        private void TestAndroidBridge()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.Log(Log.Zero1("当前环境并不是Android实机！"));
                return;
            }
            var bridge = new AndroidJavaClass("pieces.jing.zerolib.UnityBridge");
            bridge.CallStatic<bool>("showToast", "Bridge Test!!!");
        }

        void RoushanTest()
        {
            UIPanelMgr.Ins.Switch<StartupPanel>();
        }

        void AddBtn(string label, Action action)
        {
            var go = GameObject.Instantiate(buttonPrefab, content);
            go.name = label;
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = label;
            go.GetComponent<Button>().onClick.AddListener(() => { action.Invoke(); });
        }

        /// <summary>
        /// 测试加载AB中所有的资源
        /// </summary>
        private void TestLoadAllAB()
        {
            var abName = AB.ROOT_ASSETS.NAME;

            Debug.Log(Log.Zero1($"获取AB中的资源名称列表:{abName}"));
            var names = ResMgr.Ins.GetAllAsssetsNames(abName);
            for(int i = 0; i < names.Length; i++)
            {
                Debug.Log(Log.Zero1($"{i}:{names[i]}"));
                var temp = ResMgr.Ins.Load(abName,names[i]);
            }

            Debug.Log(Log.Zero1($"开始同步加载:{abName}"));
            var objs = ResMgr.Ins.LoadAll(abName);
            Debug.Log(Log.Zero1($"加载完成，资源数:{objs?.Length}"));

            Debug.Log(Log.Zero1($"开始异步加载:{abName}"));
            ResMgr.Ins.LoadAllAsync(abName, (assets) =>
            {
                Debug.Log(Log.Zero1($"加载完成，资源数:{assets?.Length}"));
            },
            (progress) =>
            {
                Debug.Log(Log.Zero1($"加载进度:{progress}"));
            });

        }
    }
}
