using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour {

        public Text textState;
        public Text textProgress;

        void Start() {
            Preload preload = GetComponent<Preload>();
            preload.onProgress += (float progress) =>
            {
                textProgress.text = string.Format("{0}%", (int)(progress * 100f));
            };

            preload.onStateChange += (state) =>
            {
                textState.text = state.ToString();
            };

        }


        void Update() {

        }
    }
}