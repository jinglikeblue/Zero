using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class GUIDeviceInfo : MonoBehaviour
    {
        public static GUIDeviceInfo _ins;

        public static void Show()
        {
            if (null == _ins)
            {
                const string NAME = "GUIDeviceInfo";
                GameObject go = new GameObject();
                go.name = NAME;
                _ins = go.AddComponent<GUIDeviceInfo>();
                DontDestroyOnLoad(go);
            }
        }

        public static void Close()
        {
            if (null != _ins)
            {
                GameObject.Destroy(_ins.gameObject);
                _ins = null;
            }
        }

        int _frameCount = 0;
        float _cd = 1f;
        int _avgFps = 0;

        private void Update()
        {
            _frameCount++;
            _cd -= Time.deltaTime;
            if (_cd <= 0f)
            {
                _avgFps = _frameCount;
                _frameCount = 0;
                _cd = 1f;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(string.Format("FPS:{0}", _avgFps));
        }
    }
}