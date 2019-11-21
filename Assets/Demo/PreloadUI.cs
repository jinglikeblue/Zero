using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour
    {

        public Text textState;
        public Text textProgress;

        void Start()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    Screen.SetResolution(640, 960, true);
                    break;
                default:
                    Screen.SetResolution(640, 960, false);
                    break;
            }

            Preload preload = GetComponent<Preload>();
            preload.onProgress += (float progress, long totalSize) =>
            {
                //转换为MB
                float totalMB = totalSize / 1024 / 1024f;
                float loadedMB = totalMB * progress;
                textProgress.text = string.Format("{0}% [{1}MB/{2}MB]", (int)(progress * 100f), loadedMB.ToString("0.00"), totalMB.ToString("0.00"));

                //Log.I("Preload State:{0} Progress:{1} TotalSize:{2}", preload.CurrentState, progress, totalSize);
            };

            preload.onStateChange += (state) =>
            {
                textState.text = state.ToString();
            };

            //从这里启动Ppreload
            GetComponent<Preload>().StartPreload();
        }
    }
}