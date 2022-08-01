using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{

    [SerializeField] GameObject _button1;
    [SerializeField] GameObject _button2;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RevokeButtonColor()
    {
        _button1.SetActive(false);
        _button2.SetActive(true);
    }

    public void ActivateButtonColor()
    {
        _button2.SetActive(false);
        _button1.SetActive(true);
    }
}
