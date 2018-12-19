using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Zero
{
    /// <summary>
    /// 序列帧播放
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MovieClip : MonoBehaviour
    {
        [Header("是否播放")]
        public bool isPlaying = true;
        [Header("是否倒序播放")]
        public bool isInvert = false;
        [Header("是否循环播放")]
        public bool isLoop = true;
        [Header("播放帧率(帧/秒)")]
        public int fps = 10;
        [Header("循环播放的间隔(秒)")]
        public float loopInterval = 0;
        [Header("动画开始播放的延迟(秒)")]
        public float startDelay = 0;
        [Header("序列图(按照播放顺序摆放)")]
        public Sprite[] frames;
        [Header("当前帧")]
        public int currentFrame = 0;
        
        /// <summary>
        /// 播放完成的事件（如果是循环播放，则每次播完都会触发）
        /// </summary>
        public event Action onPlayComplete;

        /// <summary>
        /// 图片显示
        /// </summary>
        Image _img;

        /// <summary>
        /// 缓存的时间
        /// </summary>
        float _cacheTime;
        int _lastFrame;

        /// <summary>
        /// 每帧事件
        /// </summary>
        Action _step;

        private void Awake()
        {
            _img = gameObject.GetComponent<Image>();
            _step = DelayStep;
        }

        private void OnDestroy()
        {
            onPlayComplete = null;
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play()
        {
            isPlaying = true;
        }

        private void Update()        
        {
            if(null != _step)
            {
                _step.Invoke();
            }            
        }

        void DelayStep()
        {
            _cacheTime += Time.deltaTime;
            if (_cacheTime >= startDelay)
            {
                _cacheTime = 0;
                _step = PlayingStep;
            }
        }


        void PlayingStep()
        {
            if (false == isPlaying || fps <= 0 || 0 == frames.Length)
            {
                if (_lastFrame != currentFrame)
                {
                    _img.sprite = frames[currentFrame];
                }
                return;
            }

            float interval = 1f / fps;
            _cacheTime += Time.deltaTime;
            if (_cacheTime < interval)
            {
                return;
            }

            _cacheTime -= interval;

            int newFrame = currentFrame + (isInvert ? -1 : 1);

            if (VerifyFrame(newFrame))
            {
                currentFrame = GetStartFrame();
                if (false == isLoop)
                {
                    isPlaying = false;
                }
                else
                {
                    _cacheTime = 0;
                    _step = LoopStep;
                }

                if (onPlayComplete != null)
                {
                    onPlayComplete();
                }
            }
            else
            {
                if (null != frames[newFrame])
                {
                    ShowFrame(newFrame);
                }
            }
        }

        public void ShowFrame(int frameIdx)
        {
            currentFrame = frameIdx;
            _img.sprite = frames[currentFrame];
            _lastFrame = currentFrame;
        }

        void LoopStep()
        {
            _cacheTime += Time.deltaTime;
            if (_cacheTime >= loopInterval)
            {                
                _cacheTime = 0;
                _step = PlayingStep;
                if(isInvert)
                {
                    ShowFrame(frames.Length - 1);
                }
                else
                {
                    ShowFrame(0);
                }
            }
        }

        /// <summary>
        /// 验证帧是否超出区域
        /// </summary>
        bool VerifyFrame(int frame)
        {
            if (frame < 0)
            {
                return true;
            }
            else if (frame >= frames.Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 得到起始帧
        /// </summary>
        /// <returns></returns>
        int GetStartFrame()
        {
            return isInvert ? frames.Length - 1 : 0;
        }

        /// <summary>
        /// 得到结束帧
        /// </summary>
        /// <returns></returns>
        int GetEndFrame()
        {
            return isInvert ? 0 : frames.Length - 1;
        }
    }


}
