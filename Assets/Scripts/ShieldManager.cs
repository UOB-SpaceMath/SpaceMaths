using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceMath;

public class ShieldManager : MonoBehaviour
{
    private bool _isClicked = false;
    [SerializeField] GameObject _button1;
    [SerializeField] GameObject _button2;

    public bool IsClicked { get => _isClicked; set => _isClicked = value; }

    public void SetClicked()
    {
        _isClicked = true;
    }

    public void SwitchShield(Ships player)
    {
        if (player.ShieldsEnabled)
        {
            player.CloseShields();
            _button2.SetActive(true);
            _button1.SetActive(false);
            
        }
        else
        {
            player.OpenShields();
            StartCoroutine(RaiseShieldSound());
            _button2.SetActive(false);
            _button1.SetActive(true);
        }
    }

    private IEnumerator RaiseShieldSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
    }
}
