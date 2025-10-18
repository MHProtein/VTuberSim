using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
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
        public bool loop = false;
        public float delay = 0f;
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
        Raising_PlaceEvent
    }

    public enum VBGMType
    {
        MainMenu,
        StreamFailure
    }
    
    public class VAudioPlayer : VSingletonMonobehaviour<VAudioPlayer>
    {
        [SerializeField] private Dictionary<VSFXType, List<VAudioPlayInfo>> sfxs;
        [SerializeField] private Dictionary<VBGMType, List<VAudioPlayInfo>> bgms;
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
            var sfx = sfxs[sfxType].First();
            AudioManager.Instance.PlaySound(sfx.soundName, sfx.channel, sfx.volume, sfx.pitch, sfx.loop, sfx.delay);
            if ((sfxType == VSFXType.Battle_BuffApply))
            {
                VDebug.Log("Battle_BuffApply");
            }
        }

        public void PlayBGM(VBGMType bgmType)
        {
            var bgm = bgms[bgmType].First();
            AudioManager.Instance.PlaySound(bgm.soundName, bgm.channel, bgm.volume, bgm.pitch, bgm.loop, bgm.delay);
        }

        public void StopBGM()
        {
            AudioManager.Instance.StopSoundsByChannel(SoundChannel.Music);
        }
    }
}