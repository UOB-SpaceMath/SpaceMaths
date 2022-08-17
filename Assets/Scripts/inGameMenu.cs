using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class inGameMenu : MonoBehaviour
{
    // Drag these screens into the inspector
    [Header("Menus")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _wonMenu;
    [SerializeField] private GameObject _LostMenu;

    [SerializeField] private GameObject _menuButton;
    [SerializeField] private GameObject _instructions;
    [SerializeField] private GameObject _background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMenu()
    {
        DisableMenuButton();
        _menu.SetActive(true);
    }

    public void HideMenu()
    {
        _menu.SetActive(false);
    }

    private void DisableMenuButton()
    {
        _menuButton.SetActive(false);
        _background.SetActive(false);
    }

    private void EnableMenuButton()
    {
        _menuButton.SetActive(true);
    }

    public void ShowInstructions()
    {
        _instructions.SetActive(true);

    }   

    public void Resume()
    {
        _menu.SetActive(false);
        _background.SetActive(false);
        EnableMenuButton();
    }

    public void ShowLoseScreen()
    {
        _LostMenu.SetActive(true);
        _background.SetActive(true);
    }

    public void ShowWinScreen()
    {
        _wonMenu.SetActive(true);
        _background.SetActive(true);
    }

    public void DisableContinueScreen()
    {
        _wonMenu.SetActive(false);
        _LostMenu.SetActive(false);
        _background.SetActive(false);
    }

    public void ShowBackground()
    {
        _background.SetActive(true);
    }
}
