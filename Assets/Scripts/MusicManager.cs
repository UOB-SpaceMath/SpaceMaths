using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainMusic; 
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioSource audio;
    private static MusicManager _musicManagerInstance;
    private SceneManager _sceneManager;
    private void Awake()
    {
        if (_musicManagerInstance == null)
        {
            _musicManagerInstance = this;
            DontDestroyOnLoad(_musicManagerInstance);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void ToggleMusic(bool muted)
    {
        if (muted)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

}
