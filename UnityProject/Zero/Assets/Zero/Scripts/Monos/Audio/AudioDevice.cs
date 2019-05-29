using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 声音播放驱动
    /// </summary>
    public sealed class AudioDevice
    {
        /// <summary>
        /// 声音驱动工厂
        /// </summary>
        static GameObject _audioDeviceFactory;

        /// <summary>
        /// 自动Id
        /// </summary>
        static int _autoId = 1;

        /// <summary>
        /// 声音驱动字典
        /// </summary>
        static Dictionary<string, AudioDevice> _deviceDic = new Dictionary<string, AudioDevice>();

        /// <summary>
        /// 创建声音设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AudioDevice Create(string id = null)
        {
            if (null == id)
            {
                id = "AudioDeviceAutoId" + _autoId;
                _autoId++;
            }

            if (_deviceDic.ContainsKey(id))
            {
                throw new System.Exception(string.Format("AudioDevice id [{0}] already exists!", id));
            }

            if (null == _audioDeviceFactory)
            {
                _audioDeviceFactory = new GameObject("AudioDeviceFactory");
                GameObject.DontDestroyOnLoad(_audioDeviceFactory);
            }

            var ad = new AudioDevice(id);
            _deviceDic.Add(id, ad);
            return ad;
        }

        /// <summary>
        /// 获取声音设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AudioDevice Get(string id)
        {
            if (_deviceDic.ContainsKey(id))
            {
                return _deviceDic[id];
            }
            return null;
        }

        /// <summary>
        /// 销毁声音设备
        /// </summary>
        /// <param name="id"></param>
        public static void Destroy(string id)
        {
            if (_deviceDic.ContainsKey(id))
            {
                _deviceDic[id].Dispose();
                _deviceDic.Remove(id);
            }
        }

        /// <summary>
        /// 清理AudioDevice里没在使用的资源
        /// </summary>
        public static void GC()
        {
            foreach(var device in _deviceDic.Values)
            {
                device.ClearUnusedAudioSource();
            }
        }

        public static void Destroy(AudioDevice target)
        {
            Destroy(target.Id);
        }

        /// <summary>
        /// 切换AudioListener绑定的对象
        /// </summary>
        /// <param name="go"></param>
        public static void SwitchAudioListener(GameObject go)
        {
            ClearAudioListener();
            go.AddComponent<AudioListener>();
        }

        /// <summary>
        /// 清空AudioListener
        /// </summary>
        public static void ClearAudioListener()
        {
            var listeners = GameObject.FindObjectsOfType<AudioListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                GameObject.Destroy(listeners[i]);
            }
        }

        /// <summary>
        /// 音量
        /// </summary>
        float _volume = 0.5f;        

        public string Id { get; }

        GameObject _go;

        /// <summary>
        /// 通过声音驱动创建的AudioSource列表
        /// </summary>
        HashSet<AudioSource> _createdSourceSet = new HashSet<AudioSource>();

        /// <summary>
        /// 通过声音驱动关联的AudioSource集合
        /// </summary>
        HashSet<AudioSource> _depositedSourceSet = new HashSet<AudioSource>();

        //无法new
        private AudioDevice(string id)
        {
            Id = id;
            GameObject go = new GameObject(Id);
            go.transform.SetParent(_audioDeviceFactory.transform);
            _go = go;
        }

        /// <summary>
        /// 音量，范围为[0,1]，默认为0.5
        /// </summary>
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;                                
                foreach (var source in _depositedSourceSet)
                {
                    source.volume = _volume;
                }
            }
        }        

        /// <summary>
        /// 播放AudioClip
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="loop"></param>
        public int Play(AudioClip ac, bool loop = false)
        {
            return Play(_go, ac, loop).GetInstanceID();
        }

        /// <summary>
        /// 通过指定的对象发声
        /// </summary>
        /// <param name="ac"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public AudioSource Play(GameObject gameObject, AudioClip clip, bool loop = false)
        {            
            var sources = gameObject.GetComponents<AudioSource>();
            AudioSource source = null;
            foreach (var tempSource in sources)
            {
                if (false == tempSource.isPlaying && _createdSourceSet.Contains(tempSource))
                {
                    source = tempSource;
                    break;
                }
            }

            if (source == null)
            {
                source = CreateAudioSource(gameObject);
            }
            
            source.loop = loop;
            source.clip = clip;
            source.volume = _volume;
            source.Play();
            _depositedSourceSet.Add(source);
            return source;
        }

        /// <summary>
        /// 关联一个AudioSource,关联后，调整AudoiDevice的音量以及控制播放暂停都会影响该AudioSource
        /// </summary>
        /// <param name=""></param>
        public void Deposit(AudioSource source)
        {
            source.volume = _volume;
            _depositedSourceSet.Add(source);
        }

        /// <summary>
        /// 取消关联AudioSource
        /// </summary>
        /// <param name="source"></param>
        public void Undeposit(AudioSource source)
        {            
            _depositedSourceSet.Remove(source);
        }

        /// <summary>
        /// 停止播放所有的声音
        /// </summary>
        public void StopAll()
        {
            foreach(var source in _depositedSourceSet)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// 暂停所有声音播放
        /// </summary>
        public void PauseAll()
        {
            foreach (var source in _depositedSourceSet)
            {
                source.Pause();
            }
        }

        /// <summary>
        /// 取消所有声音播放的暂停
        /// </summary>
        public void UnpauseAll()
        {
            foreach (var source in _depositedSourceSet)
            {
                source.UnPause();
            }            
        }

        AudioSource CreateAudioSource(GameObject go)
        {
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _createdSourceSet.Add(source);
            return source;
        }

        /// <summary>
        /// 销毁这个驱动
        /// </summary>
        void Dispose()
        {
            foreach (var source in _createdSourceSet)
            {
                GameObject.Destroy(source);
            }
            _createdSourceSet.Clear();
            _depositedSourceSet.Clear();
        }

        /// <summary>
        /// 清理没有使用的AudioSource(仅清理由自己创建的AudioSource)
        /// </summary>
        void ClearUnusedAudioSource()
        {
            HashSet<AudioSource> _unusedSet = new HashSet<AudioSource>();

            foreach (var source in _createdSourceSet)
            {
                if (false == source.isPlaying)
                {
                    _unusedSet.Add(source);                    
                }
            }

            foreach(var unusedSource in _unusedSet)
            {
                GameObject.Destroy(unusedSource);
                _depositedSourceSet.Remove(unusedSource);
                _createdSourceSet.Remove(unusedSource);
            }
        }
    }
}
