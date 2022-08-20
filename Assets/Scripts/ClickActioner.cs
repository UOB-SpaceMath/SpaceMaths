using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickActioner : MonoBehaviour
{
  private AudioSource _audio ;
  [SerializeField] private AudioClip _inputClip;
  [SerializeField] private AudioSource _audioSource;
  private static ClickActioner _instance;
  private void Awake()
  {
    if (_instance == null)
    {
      _instance = this;
      DontDestroyOnLoad(_instance);
    }
    else
    {
      Destroy(gameObject);
    }
  }
  
  public void PlayOneShot()
  {
    _audioSource.PlayOneShot(_inputClip);
    StartCoroutine(HoldSound());
  }
 
  private IEnumerator HoldSound()
  {
    yield return new WaitForSeconds(_inputClip.length + 3.0f);
  }
}
