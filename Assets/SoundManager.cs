using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource countDownAudioSource;
    [SerializeField] AudioClip foodEaten;
    [SerializeField] AudioClip peeing;
    [SerializeField] AudioClip finishPeeing;
    [SerializeField] AudioClip levelUp;
    [SerializeField] AudioClip CountDown;
    [SerializeField] AudioClip barking;
    [SerializeField] AudioClip barking2;
    [SerializeField] AudioClip Win;
    [SerializeField] AudioClip Lose;
    [SerializeField] AudioClip CarStart;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlayFoodEatenSoundEffect()
    {
        audioSource.PlayOneShot(foodEaten, 1.0f);
    }

    public void PlayPeeingSoundEffect()
    {
        audioSource.clip = peeing;
        audioSource.Play();
    }

    public void StopPeeingSoundEffect()
    {
        if (audioSource.clip == peeing) audioSource.Stop();
    }

    public void PlayFinishPeeingSoundEffect()
    {
        audioSource.PlayOneShot(finishPeeing, 2.0f);
    }

    public void PlayLevelUpSoundEffect()
    {
        audioSource.PlayOneShot(levelUp, 1.0f);
    }

    public void PlayCountDownSoundEffect()
    {
        countDownAudioSource.clip = CountDown;
        countDownAudioSource.PlayDelayed(2.0f);
    }

    public void PlayBarkSoundEffect()
    {
        audioSource.PlayOneShot(barking, 1.0f);
    }
    public void PlayBarkSound2Effect()
    {
        audioSource.PlayOneShot(barking2, 1.0f);
    }

    public void PlayWinSoundEffect()
    {
        audioSource.PlayOneShot(Win, 1.0f);
    }

    public void PlayGameOverSoundEffect()
    {
        audioSource.PlayOneShot(Lose, 1.0f);
    }

    public void PlayCarStartSoundEffect()
    {
        audioSource.PlayOneShot(CarStart, 1.0f);
    }

}
