using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite soundIcon;
    private static MusicManager _musicManagerInstance;
    private SceneManager _sceneManager;
    private bool isMuted;
    private Image _buttonImage;



    private void Start()
    {
        isMuted = false;
        _buttonImage = GameObject.Find("SoundButton/Button").GetComponent<Image>();
    }

    private void Update()
    {
        if (isMuted)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    // private void Awake()
    // {
    //     if (_musicManagerInstance == null)
    //     {
    //         _musicManagerInstance = this;
    //         DontDestroyOnLoad(_musicManagerInstance);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    // }
    public void ToggleMusic()
    {
        isMuted = !isMuted;
    }

    public void ChangeSprint()
    {
        if (isMuted)
        {
            _buttonImage.sprite = muteIcon;
        }
        else
        {
            _buttonImage.sprite = soundIcon;
        }
    }

}
