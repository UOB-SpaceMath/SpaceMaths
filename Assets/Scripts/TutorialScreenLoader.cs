using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScreenLoader : MonoBehaviour
{


    private int currentPage = 0;
    
    public RawImage _displayImage;
    public TextMeshProUGUI _displayText;
   
    public Texture[] tutorialSlide = new Texture[5];
    public TextMeshProUGUI[] tutorialText = new TextMeshProUGUI[5];

  
    // Start is called before the first frame update
    void Start()
    {
        _displayImage.texture = tutorialSlide[currentPage];
        _displayText.SetText(tutorialText[currentPage].text);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextButton()
    {
        currentPage++;

        if(currentPage > 4)
        {
            currentPage = 0;
        }
        _displayImage.texture = tutorialSlide[currentPage];
         _displayText.SetText(tutorialText[currentPage].text);

    }

    public void PreviousButton()
    {
        currentPage--;
        if(currentPage < 0)
        {
            currentPage = 4;
        }
        _displayImage.texture = tutorialSlide[currentPage];
        _displayText.SetText(tutorialText[currentPage].text);

    }

}
