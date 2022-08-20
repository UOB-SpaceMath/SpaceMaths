using System;
using UnityEngine;
using UnityEngine.UI;

public class WatsonButtonController : MonoBehaviour
{
    [SerializeField] private Sprite _normalIcon;

    [SerializeField] private Sprite _recordingIcon;

    [SerializeField] private Sprite _fetchingIcon;

    private Image _icon;

    private Animator _animator;

    private int _recordingId;

    private int _fetchingId;

    private int _normalId;

    // Start is called before the first frame update
    private void Start()
    {
        _icon = GetComponentsInChildren<Image>()[1];
        _animator = GetComponent<Animator>();
        _recordingId = Animator.StringToHash("Recording");
        _fetchingId = Animator.StringToHash("Fetching");
        _normalId = Animator.StringToHash("Normal");

        _icon.sprite = _normalIcon;
    }

    public void SetButtonNormal()
    {
        _icon.sprite = _normalIcon;
        _animator.Play(_normalId);
    }

    public void SetButtonRecording()
    {
        _icon.sprite = _recordingIcon;
        _animator.Play(_recordingId);
    }

    public void SetButtonFetching()
    {
        _icon.sprite = _fetchingIcon;
        _animator.Play(_fetchingId);
    }
}