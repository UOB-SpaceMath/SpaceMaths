using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceMath;
using UnityEngine.SceneManagement;
using System;

public class PopulateGrid : MonoBehaviour
{
    [SerializeField]
    private Button prefab;

    [SerializeField]
    private int NumberOfButtons;

    void Start()
    {
        populate();
    }

    private void populate()
    {
        Button newButton;

        for(int i = 0; i < NumberOfButtons; i++)
        {
            newButton = Instantiate(prefab, transform);
            newButton.GetComponentInChildren<Text>().text = "Level " + (i + 1);
            int level = i;
            newButton.onClick.AddListener(() =>
            {
                GlobalInformation.CurrentLevelIndex = level;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            });
        }
    }
}
