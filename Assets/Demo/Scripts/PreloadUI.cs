using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour
    {

        public Text text;

        void Start()
        {    
            SetProgress(0, 1);
            Preload preload = GetComponent<Preload>();
            preload.onProgress += SetProgress;

            preload.onStateChange += (state) =>
            {
                Debug.Log("Preload State Change: " + state);                
            };

            //从这里启动Ppreload
            GetComponent<Preload>().StartPreload();
        }

        void SetProgress(float progress, long totalSize)
        {
            //转换为MB
            text.text = $"{(int)(progress * 100)}%";
        }
    }
}