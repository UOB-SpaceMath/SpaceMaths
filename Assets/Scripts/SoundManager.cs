using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Soundtracks")]
    [SerializeField]
    private AudioSource BGM;
    [SerializeField]
    private AudioSource buttonSound;
    [SerializeField]
    private AudioSource nextSound;
    [SerializeField]
    private AudioSource movementSound;
    [SerializeField]
    private AudioSource startWatsonSound;
    [SerializeField]
    private AudioSource stoptWatsonSound;
    [SerializeField]
    private AudioSource exitSound;
    private bool watsonClicked = false;

    public void Awake()
    {
        PlayBGM();
    }

    public void PlayButtonSound()
    {
        buttonSound.Play();
    }

    public void PlayExitSound()
    {
        exitSound.Play();
    }

    public void PlayNextSound()
    {
        nextSound.Play();
    }

    public void PlayMovingSound()
    {
        movementSound.Play();
    }

    public void StopMovingSound()
    {
        movementSound.Stop();
    }

    public void PlayBGM()
    {
        BGM.Play();
    }

    public void StopBGM()
    {
        BGM.Stop();
    }

    public void PlayWatson()
    {
        if (watsonClicked)
        {
            stoptWatsonSound.Play();
            PlayBGM();
            watsonClicked = false;
            return;
        }
        startWatsonSound.Play();
        StopBGM();
        watsonClicked = true;
    }
}
