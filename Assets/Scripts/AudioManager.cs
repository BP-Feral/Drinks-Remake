using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip selectSound;
    public AudioClip dropSound;
    public AudioClip matchSound;

    public AudioMixerGroup selectSoundGroup;
    public AudioMixerGroup dropSoundGroup;
    public AudioMixerGroup matchSoundGroup;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySelectSound()
    {
        if (selectSound != null)
        {
            audioSource.outputAudioMixerGroup = selectSoundGroup;
            audioSource.PlayOneShot(selectSound);
        }
    }

    public void PlayDropSound()
    {
        if (dropSound != null)
        {
            audioSource.outputAudioMixerGroup = dropSoundGroup;
            audioSource.PlayOneShot(dropSound);
        }
    }

    public void PlayMatchSound()
    {
        if (matchSound != null)
        {
            audioSource.outputAudioMixerGroup = matchSoundGroup;
            audioSource.PlayOneShot(matchSound);
        }
    }
}
