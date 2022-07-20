using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceMath;

public class ShieldManager : MonoBehaviour
{
    private bool _isClicked = false;

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
        }
        else
        {
            player.OpenShields();
        }
    }
}
