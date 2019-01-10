using UnityEngine;

namespace Zero
{
    public class AudioPlayer : ASingletonMonoBehaviour<AudioPlayer>
    {
        [Header("音效轨道数量")]
        [Range(1,32)]
        public int effectTrackCount = 1;

        /// <summary>
        /// 背景音播放源
        /// </summary>
        AudioSource _bgmTrack = null;

        /// <summary>
        /// 音效播放源
        /// </summary>
        AudioSource[] _effectTracks = null;

        /// <summary>
        /// 背景音音量
        /// </summary>
        public float BgmVolume
        {
            get { return _bgmTrack.volume; }
            set
            {
                _bgmTrack.volume = value;
            }
        }

        /// <summary>
        /// 音效音量
        /// </summary>
        public float EffectVolume
        {
            get
            {
                return _effectTracks[0].volume;
            }

            set
            {
                foreach (var obj in _effectTracks)
                {
                    obj.volume = value;
                }
            }
        }

        private void Awake()
        {
            _bgmTrack = gameObject.AddComponent<AudioSource>();
            _bgmTrack.loop = true;
            _bgmTrack.playOnAwake = false;

            _effectTracks = new AudioSource[effectTrackCount];
            for (int i = 0; i < effectTrackCount; i++)
            {
                _effectTracks[i] = gameObject.AddComponent<AudioSource>();
                _effectTracks[i].loop = false;
                _effectTracks[i].playOnAwake = false;
            }
        }

        public void PlayBGM(AudioClip ac)
        {
            _bgmTrack.Stop();
            if(null == ac)
            {
                return;
            }
            _bgmTrack.clip = ac;
            _bgmTrack.Play();
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        /// <param name="smooth">是否平滑淡出</param>
        public void StopBGM(bool smooth = false)
        {
            _bgmTrack.Stop();
        }

        public void PlayEffect(AudioClip ac)
        {
            if(null == ac)
            {
                return;
            }            

            AudioSource useSource = null;            
            foreach (var source in _effectTracks)
            {
                if (false == source.isPlaying)
                {
                    useSource = source;
                }
            }

            if (null == useSource)
            {
                useSource = _effectTracks[0];
            }

            useSource.clip = ac;
            useSource.Play();
        }

        public void PlayEffect(AudioClip ac, int track)
        {
            AudioSource useSource = _effectTracks[track];            
            useSource.clip = ac;
            useSource.Play();
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="track">默认停止所有音效</param>
        public void StopEffect(int track = -1)
        {
            if(track < 0)
            {
                foreach(var source in  _effectTracks)
                {
                    source.Stop();
                }
            }
            else if(track < _effectTracks.Length)
            {
                _effectTracks[track].Stop();
            }
        }

        /// <summary>
        /// 停止正在播放特定音频的音效
        /// </summary>
        public void StopEffectByAudio(AudioClip ac)
        {
            if(null == ac)
            {
                return;
            }
            
            foreach (var source in _effectTracks)
            {
                if (source.isPlaying && source.clip == ac)
                {
                    source.Stop();
                    break;
                }
            }
        }
        

    }
}
