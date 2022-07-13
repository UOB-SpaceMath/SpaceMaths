using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class InstructionImage : MonoBehaviour
{

    private int currentPage = 0;
    
    public RawImage _displayImage;
    public TextMeshProUGUI _displayText;
    public TextMeshProUGUI _displayHeaderText;
   
    public Texture[] instructionSlide = new Texture[5];
    public TextMeshProUGUI[] instructionText = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] headerText = new TextMeshProUGUI[5];
  
    // Start is called before the first frame update
    void Start()
    {
        _displayImage.texture = instructionSlide[currentPage];
        _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);

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
        _displayImage.texture = instructionSlide[currentPage];
         _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);
    

    }

    public void PreviousButton()
    {
        currentPage--;
        if(currentPage < 0)
        {
            currentPage = 4;
        }
        _displayImage.texture = instructionSlide[currentPage];
        _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);
    }

}
