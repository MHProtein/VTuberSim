using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using VTuber.Core.SE;
using System.Collections;

public enum SoundChannel
{
    Master,
    SFX,
    UI,
    Ambient,
    Voice,
    Music
}

[System.Serializable]
public class ChannelMixerGroup
{
    public SoundChannel channel;
    public AudioMixerGroup mixerGroup;
}

public class AudioManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string soundConfigPath = "Configurations/AudioConfig/SoundConfig";
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private int maxConcurrentSounds = 20;
    [SerializeField] private float spatialBlend = 0.8f;

    [Header("Mixer Groups")]
    [SerializeField] private List<ChannelMixerGroup> channelMixerGroups = new List<ChannelMixerGroup>();

    private static AudioManager _instance;
    private Dictionary<string, AudioClip> _soundDictionary = new Dictionary<string, AudioClip>();
    private SoundConfig _soundConfig;
    private Dictionary<SoundChannel, float> _channelVolumes = new Dictionary<SoundChannel, float>();
    private Dictionary<string, int> _playingSounds = new Dictionary<string, int>();
    private Transform _listenerTransform;
    private Dictionary<SoundChannel, AudioMixerGroup> _mixerGroupDictionary = new Dictionary<SoundChannel, AudioMixerGroup>();

    private class PooledAudioSource
    {
        public AudioSource source;
        public SoundChannel channel;
        public string soundName;
        public bool isPlaying;
        public bool isPaused;
        public float pausedTime;
    }
    private List<PooledAudioSource> _audioSourcePool = new List<PooledAudioSource>();

    public static AudioManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        //ToDo：待调整为走配置
        foreach (SoundChannel channel in System.Enum.GetValues(typeof(SoundChannel)))
        {
            _channelVolumes[channel] = 1f;
        }

        foreach (var group in channelMixerGroups)
        {
            if (!_mixerGroupDictionary.ContainsKey(group.channel))
            {
                _mixerGroupDictionary.Add(group.channel, group.mixerGroup);
            }
        }

        for (int i = 0; i < maxConcurrentSounds; i++)
        {
            CreateAudioSource();
        }

        _listenerTransform = FindObjectOfType<AudioListener>()?.transform;
        if (_listenerTransform == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} AudioListener不存在与场景中");
            _listenerTransform = transform;
        }

        LoadSoundConfig();


        //测试逻辑
        //SetChannelVolume(SoundChannel.Master, 0.8f);
        //SetChannelVolume(SoundChannel.SFX, 1f);
        //SetChannelVolume(SoundChannel.Music, 0.7f);
    }

    private AudioSource CreateAudioSource()
    {
        GameObject sourceObj = new GameObject($"AudioSource_{_audioSourcePool.Count}");
        sourceObj.transform.SetParent(transform);
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        source.spatialBlend = spatialBlend;
        source.playOnAwake = false;
        _audioSourcePool.Add(new PooledAudioSource
        {
            source = source,
            channel = SoundChannel.SFX,
            soundName = "",
            isPlaying = false,
            isPaused = false,
            pausedTime = 0
        });
        return source;
    }

    private void LoadSoundConfig()
    {
        _soundConfig = Resources.Load<SoundConfig>(soundConfigPath);

        if (_soundConfig == null)
        {
            Debug.LogError($"SoundEffectSystem:{{Error}} 未找到音效配置资源路径:  {soundConfigPath}");
            return;
        }

        foreach (AudioClip clip in _soundConfig.SoundEffects)
        {
            if (clip != null)
            {
                string clipName = clip.name;

                if (!_soundDictionary.ContainsKey(clipName))
                {
                    _soundDictionary.Add(clipName, clip);
                }
                else
                {
                    Debug.LogWarning($"SoundEffectSystem:{{Warning}} 重复音效名称: {clipName}");
                }
            }
        }
    }

    #region 通道管理

    public void SetChannelVolume(SoundChannel channel, float volume)
    {
        _channelVolumes[channel] = Mathf.Clamp01(volume);

        if (audioMixer != null)
        {
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat($"{channel}Volume", dB);
        }
    }

    public float GetChannelVolume(SoundChannel channel)
    {
        if (audioMixer != null)
        {
            float dB;
            if (audioMixer.GetFloat($"{channel}Volume", out dB))
            {
                return Mathf.Pow(10f, dB / 20f);
            }
        }
        return _channelVolumes[channel];
    }

    #endregion

    #region 高级播放接口

    public AudioSource PlaySound(string soundName, SoundChannel channel = SoundChannel.SFX,
        float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f)
    {
        if (!_soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"SoundEffectSystem:{{Warning}} 未找到该音效: {soundName}");
            return null;
        }

        if (_playingSounds.TryGetValue(soundName, out int count) && count > 3)
        {
            Debug.Log($"SoundEffectSystem:{{Log}} 过多重复音效播放 {soundName}, 跳过.");
            return null;
        }

        var pooledSource = GetAvailableAudioSource();
        if (pooledSource == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} 没有可用的AudioSource");
            return null;
        }

        _playingSounds[soundName] = count + 1;

        AudioSource source = pooledSource.source;
        source.clip = clip;
        source.volume = volume * _channelVolumes[channel] * _channelVolumes[SoundChannel.Master];
        source.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
        source.loop = loop;
        source.outputAudioMixerGroup = GetMixerGroupForChannel(channel);

        source.transform.position = _listenerTransform.position;

        pooledSource.channel = channel;
        pooledSource.soundName = soundName;
        pooledSource.isPlaying = true;
        pooledSource.isPaused = false;
        pooledSource.pausedTime = 0;

        if (delay > 0)
        {
            source.PlayDelayed(delay);
        }
        else
        {
            source.Play();
        }

        if (!loop)
        {
            StartCoroutine(ReturnToPoolWhenFinished(pooledSource));
        }

        return source;
    }

    private IEnumerator ReturnToPoolWhenFinished(PooledAudioSource pooledSource)
    {
        yield return new WaitWhile(() => pooledSource.source.isPlaying || pooledSource.isPaused);

        if (_playingSounds.ContainsKey(pooledSource.soundName))
        {
            _playingSounds[pooledSource.soundName] =
                Mathf.Max(0, _playingSounds[pooledSource.soundName] - 1);
        }

        pooledSource.isPlaying = false;
        pooledSource.isPaused = false;
        pooledSource.soundName = "";
        pooledSource.source.clip = null;
    }

    //ToDO: 待调整
    public void PlaySoundAtPosition(string soundName, Vector3 position,
        SoundChannel channel = SoundChannel.SFX, float volume = 1f,
        float spatialBlend = 1f)
    {
        if (!_soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"SoundEffectSystem:{{Warning}} 未找到该音效: {soundName}");
            return;
        }

        var pooledSource = GetAvailableAudioSource();
        if (pooledSource == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} 没有可用的AudioSource");
            return;
        }

        AudioSource source = pooledSource.source;
        source.spatialBlend = spatialBlend;
        source.transform.position = position;
        source.volume = volume * _channelVolumes[channel] * _channelVolumes[SoundChannel.Master];
        source.clip = clip;
        source.outputAudioMixerGroup = GetMixerGroupForChannel(channel);

        pooledSource.channel = channel;
        pooledSource.soundName = soundName;
        pooledSource.isPlaying = true;
        pooledSource.isPaused = false;
        pooledSource.pausedTime = 0;

        source.Play();
        StartCoroutine(ReturnToPoolWhenFinished(pooledSource));
    }

    private PooledAudioSource GetAvailableAudioSource()
    {
        foreach (var pooledSource in _audioSourcePool)
        {
            if (!pooledSource.isPlaying)
            {
                return pooledSource;
            }
        }

        Debug.LogWarning("SoundEffectSystem:{Warning} AudioSource对象池耗尽. 将创建新的AudioSource对象.");
        CreateAudioSource();
        return _audioSourcePool[_audioSourcePool.Count - 1];
    }

    private AudioMixerGroup GetMixerGroupForChannel(SoundChannel channel)
    {
        AudioMixerGroup group;
        if (_mixerGroupDictionary.TryGetValue(channel, out group))
        {
            return group;
        }
        return null;
    }

    #endregion

    #region 高级控制

    public void StopAllSounds()
    {
        foreach (var pooledSource in _audioSourcePool)
        {
            if (pooledSource.isPlaying || pooledSource.isPaused)
            {
                pooledSource.source.Stop();
                pooledSource.isPlaying = false;
                pooledSource.isPaused = false;
                pooledSource.pausedTime = 0;
            }
        }
        _playingSounds.Clear();
    }

    public void StopSoundsByChannel(SoundChannel channel)
    {
        foreach (var pooledSource in _audioSourcePool)
        {
            if ((pooledSource.isPlaying || pooledSource.isPaused) &&
                pooledSource.channel == channel)
            {
                pooledSource.source.Stop();
                pooledSource.isPlaying = false;
                pooledSource.isPaused = false;
                pooledSource.pausedTime = 0;

                if (_playingSounds.ContainsKey(pooledSource.soundName))
                {
                    _playingSounds[pooledSource.soundName] =
                        Mathf.Max(0, _playingSounds[pooledSource.soundName] - 1);
                }
            }
        }
    }

    public void PauseAllSounds(bool pause)
    {
        foreach (var pooledSource in _audioSourcePool)
        {
            if (pooledSource.isPlaying)
            {
                if (pause)
                {
                    pooledSource.pausedTime = pooledSource.source.time;
                    pooledSource.source.Pause();
                    pooledSource.isPaused = true;
                }
                else if (pooledSource.isPaused)
                {
                    pooledSource.source.time = pooledSource.pausedTime;
                    pooledSource.source.UnPause();
                    pooledSource.isPaused = false;
                }
            }
        }
    }


    //ToDO: 待补充
    //public void FadeOut(AudioSource source, float duration)
    //{
    //    StartCoroutine(FadeOutCoroutine(source, duration));
    //}

    //private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    //{
    //    float startVolume = source.volume;
    //    float timer = 0f;

    //    while (timer < duration)
    //    {
    //        timer += Time.deltaTime;
    //        source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
    //        yield return null;
    //    }

    //    source.Stop();
    //    source.volume = startVolume;
    //}

    #endregion

    #region 资源管理
    //ToDO: 待补充
    #endregion
}