using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using VTuber.Core.SE;

public enum SoundChannel
{
    Master,
    SFX,
    UI,
    Ambient,
    Voice,
    Music
}

[Serializable]
public class ChannelMixerGroup
{
    public SoundChannel channel;
    public AudioMixerGroup mixerGroup;
}

public class AudioManager : MonoBehaviour
{
    [Header("Configuration")] [SerializeField]
    private string soundConfigPath = "Configurations/AudioConfig/SoundConfig";

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private int maxConcurrentSounds = 20;
    [SerializeField] private float spatialBlend = 0.8f;

    [Header("Mixer Groups")] [SerializeField]
    private List<ChannelMixerGroup> channelMixerGroups = new();

    private readonly List<PooledAudioSource> _audioSourcePool = new();
    private readonly Dictionary<SoundChannel, float> _channelVolumes = new();
    private readonly Dictionary<SoundChannel, AudioMixerGroup> _mixerGroupDictionary = new();
    private readonly Dictionary<string, int> _playingSounds = new();
    private readonly Dictionary<string, AudioClip> _soundDictionary = new();
    private Transform _listenerTransform;
    private SoundConfig _soundConfig;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        //ToDo��������Ϊ������
        foreach (SoundChannel channel in Enum.GetValues(typeof(SoundChannel))) _channelVolumes[channel] = 1f;

        foreach (var group in channelMixerGroups)
            if (!_mixerGroupDictionary.ContainsKey(group.channel))
                _mixerGroupDictionary.Add(group.channel, group.mixerGroup);

        for (var i = 0; i < maxConcurrentSounds; i++) CreateAudioSource();

        _listenerTransform = FindObjectOfType<AudioListener>()?.transform;
        if (_listenerTransform == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} AudioListener�������볡����");
            _listenerTransform = transform;
        }

        LoadSoundConfig();


        //�����߼�
        //SetChannelVolume(SoundChannel.Master, 0.8f);
        //SetChannelVolume(SoundChannel.SFX, 1f);
        //SetChannelVolume(SoundChannel.Music, 0.7f);
    }

    private AudioSource CreateAudioSource()
    {
        var sourceObj = new GameObject($"AudioSource_{_audioSourcePool.Count}");
        sourceObj.transform.SetParent(transform);
        var source = sourceObj.AddComponent<AudioSource>();
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
            Debug.LogError($"SoundEffectSystem:{{Error}} δ�ҵ���Ч������Դ·��:  {soundConfigPath}");
            return;
        }

        foreach (var clip in _soundConfig.SoundEffects)
            if (clip != null)
            {
                var clipName = clip.name;

                if (!_soundDictionary.ContainsKey(clipName))
                    _soundDictionary.Add(clipName, clip);
                else
                    Debug.LogWarning($"SoundEffectSystem:{{Warning}} �ظ���Ч����: {clipName}");
            }
    }

    private class PooledAudioSource
    {
        public SoundChannel channel;
        public bool isPaused;
        public bool isPlaying;
        public float pausedTime;
        public string soundName;
        public AudioSource source;
    }

    #region ͨ������

    public void SetChannelVolume(SoundChannel channel, float volume)
    {
        _channelVolumes[channel] = Mathf.Clamp01(volume);

        if (audioMixer != null)
        {
            var dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat($"{channel}Volume", dB);
        }
    }

    public float GetChannelVolume(SoundChannel channel)
    {
        if (audioMixer != null)
        {
            float dB;
            if (audioMixer.GetFloat($"{channel}Volume", out dB)) return Mathf.Pow(10f, dB / 20f);
        }

        return _channelVolumes[channel];
    }

    #endregion

    #region �߼����Žӿ�

    public AudioSource PlaySound(string soundName, SoundChannel channel = SoundChannel.SFX,
        float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f)
    {
        if (!_soundDictionary.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"SoundEffectSystem:{{Warning}} δ�ҵ�����Ч: {soundName}");
            return null;
        }

        if (_playingSounds.TryGetValue(soundName, out var count) && count > 3)
        {
            Debug.Log($"SoundEffectSystem:{{Log}} �����ظ���Ч���� {soundName}, ����.");
            return null;
        }

        var pooledSource = GetAvailableAudioSource();
        if (pooledSource == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} û�п��õ�AudioSource");
            return null;
        }

        _playingSounds[soundName] = count + 1;

        var source = pooledSource.source;
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
            source.PlayDelayed(delay);
        else
            source.Play();

        if (!loop) StartCoroutine(ReturnToPoolWhenFinished(pooledSource));

        return source;
    }

    private IEnumerator ReturnToPoolWhenFinished(PooledAudioSource pooledSource)
    {
        yield return new WaitWhile(() => pooledSource.source.isPlaying || pooledSource.isPaused);

        if (_playingSounds.ContainsKey(pooledSource.soundName))
            _playingSounds[pooledSource.soundName] =
                Mathf.Max(0, _playingSounds[pooledSource.soundName] - 1);

        pooledSource.isPlaying = false;
        pooledSource.isPaused = false;
        pooledSource.soundName = "";
        pooledSource.source.clip = null;
    }

    //ToDO: ������
    public void PlaySoundAtPosition(string soundName, Vector3 position,
        SoundChannel channel = SoundChannel.SFX, float volume = 1f,
        float spatialBlend = 1f)
    {
        if (!_soundDictionary.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"SoundEffectSystem:{{Warning}} δ�ҵ�����Ч: {soundName}");
            return;
        }

        var pooledSource = GetAvailableAudioSource();
        if (pooledSource == null)
        {
            Debug.LogWarning("SoundEffectSystem:{Warning} û�п��õ�AudioSource");
            return;
        }

        var source = pooledSource.source;
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
            if (!pooledSource.isPlaying)
                return pooledSource;

        Debug.LogWarning("SoundEffectSystem:{Warning} AudioSource����غľ�. �������µ�AudioSource����.");
        CreateAudioSource();
        return _audioSourcePool[_audioSourcePool.Count - 1];
    }

    private AudioMixerGroup GetMixerGroupForChannel(SoundChannel channel)
    {
        AudioMixerGroup group;
        if (_mixerGroupDictionary.TryGetValue(channel, out group)) return group;
        return null;
    }

    #endregion

    #region �߼�����

    public void StopAllSounds()
    {
        foreach (var pooledSource in _audioSourcePool)
            if (pooledSource.isPlaying || pooledSource.isPaused)
            {
                pooledSource.source.Stop();
                pooledSource.isPlaying = false;
                pooledSource.isPaused = false;
                pooledSource.pausedTime = 0;
            }

        _playingSounds.Clear();
    }

    public void StopSoundsByChannel(SoundChannel channel)
    {
        foreach (var pooledSource in _audioSourcePool)
            if ((pooledSource.isPlaying || pooledSource.isPaused) &&
                pooledSource.channel == channel)
            {
                pooledSource.source.Stop();
                pooledSource.isPlaying = false;
                pooledSource.isPaused = false;
                pooledSource.pausedTime = 0;

                if (_playingSounds.ContainsKey(pooledSource.soundName))
                    _playingSounds[pooledSource.soundName] =
                        Mathf.Max(0, _playingSounds[pooledSource.soundName] - 1);
            }
    }

    public void PauseAllSounds(bool pause)
    {
        foreach (var pooledSource in _audioSourcePool)
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


    //ToDO: ������
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

    #region ��Դ����

    //ToDO: ������

    #endregion
}