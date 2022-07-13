using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintControl: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HideHint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideHint()
    {
        gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        gameObject.SetActive(true);
    }
}
