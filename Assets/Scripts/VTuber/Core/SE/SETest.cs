using UnityEngine;

public class SETest : MonoBehaviour
{
    private SoundChannel _curSoundChannel;
    private bool _pause;

    public void PlaySoundTest()
    {
        AudioManager.Instance.PlaySound("shadowtest_1");
        _curSoundChannel = SoundChannel.SFX;
    }

    public void SetVolumeTest(float volume)
    {
        AudioManager.Instance.SetChannelVolume(_curSoundChannel, volume);
    }

    public void StopAllSound()
    {
        AudioManager.Instance.StopAllSounds();
    }

    public void StopSoundByChannel()
    {
        AudioManager.Instance.StopSoundsByChannel(_curSoundChannel);
    }

    public void PauseAllSound()
    {
        _pause = !_pause;
        AudioManager.Instance.PauseAllSounds(_pause);
    }
}