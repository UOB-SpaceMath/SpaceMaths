using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HighScoreTable : MonoBehaviour
{

    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    [SerializeField] private int levelCount;
        

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        //entryContainer = transform.Find("entryContainer");
        //entryTemplate = transform.Find("entryTemplate");
        
                
        entryTemplate.gameObject.SetActive(false);
       

        float templateHeight = 30f;
        for (int i=0; i < levelCount; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(300, -templateHeight * i + 180);
            entryTransform.gameObject.SetActive(true);

            int level = i + 1;
            string levelString = level.ToString();

            string key = "Level" + levelString;
            int score = PlayerPrefs.GetInt(key);

            entryTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = levelString;
            entryTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = score.ToString();
        }
    }
}
