using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        HideHint();
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