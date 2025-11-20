using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.Core.SE
{
    [Serializable]
    public class VAudioPlayInfo
    {
        public string soundName;
        public SoundChannel channel = SoundChannel.SFX;
        public float volume = 1f;
        public float pitch = 1f;
        public bool loop;
        public float delay;
    }

    public enum VSFXType
    {
        ButtonClick,
        Battle_CardPlayed,
        Selection,
        Battle_PopularityIncrease,
        Battle_EffectApply,
        Raising_AttributeChange,
        Battle_BuffApply,
        Raising_PlaceEvent,
        Raising_ZoomInOut,
        Raising_EnterEvent,
        Loading,
    }

    public enum VBGMType
    {
        MainMenu,
        StreamFailure,
        StreamSuccess,
        StreamHugeSuccess,
        Dialog,
        ScheduleCreation,
        Store,
        NonDialogEvent,
        Stream,
        Pause,
        None
    }

    public class VAudioPlayer : VSingletonMonobehaviour<VAudioPlayer>
    {
        [SerializeField] private Dictionary<VSFXType, List<VAudioPlayInfo>> sfxs;
        [SerializeField] private Dictionary<VBGMType, List<VAudioPlayInfo>> bgms;
        private VBGMType _currentBGM = VBGMType.None;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public void PlayButtonSound()
        {
            PlayStaticSFX(VSFXType.ButtonClick);
        }

        public void PlayStaticSFX(VSFXType sfxType)
        {
            if (sfxs.TryGetValue(sfxType, out var sfxList))
            {
                var sfx = sfxList.First();
                AudioManager.Instance.PlaySound(sfx.soundName, sfx.channel, sfx.volume, sfx.pitch, sfx.loop, sfx.delay);
            }
        }

        public void PlayBGM(VBGMType bgmType)
        {
            if (_currentBGM == bgmType)
                return;
            if (bgms.TryGetValue(bgmType, out var bgmList))
            {
                StopBGM();
                VDebug.Log($"PlayBGM: {bgmType}");
                _currentBGM = bgmType;
                var bgm = bgmList.First();
                AudioManager.Instance.PlaySound(bgm.soundName, bgm.channel, bgm.volume, bgm.pitch, bgm.loop, bgm.delay);
            }
        }

        public void StopBGM()
        {
            VDebug.Log($"StopBGM: {_currentBGM}");
            _currentBGM = VBGMType.None;
            AudioManager.Instance.StopSoundsByChannel(SoundChannel.Music);
        }

        public void PlaySFX(VAudioPlayInfo sfx)
        {
            AudioManager.Instance.PlaySound(sfx.soundName, sfx.channel, sfx.volume, sfx.pitch, sfx.loop, sfx.delay);
        }
    }
}