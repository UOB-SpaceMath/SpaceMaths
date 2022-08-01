using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class InstructionImage : MonoBehaviour
{

    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;

    private int currentPage = 0;

    public RawImage _displayImage;
    public TextMeshProUGUI _displayText;
    public TextMeshProUGUI _displayHeaderText;

    public List<Texture> instructionSlide = new List<Texture>();
    public List<TextMeshProUGUI> instructionText = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> headerText = new List<TextMeshProUGUI>();


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
        if (currentPage == 0)
        {
            previousButton.SetActive(false);
        }
        else
        {
            previousButton.SetActive(true);
        }

        if (currentPage == instructionSlide.Count - 1)
        {
            nextButton.SetActive(false);
        }
        else
        {
            nextButton.SetActive(true);
        }
    }

    public void NextButton()
    {
        if (currentPage < instructionSlide.Count - 1)
        {
            currentPage++;
            _displayImage.texture = instructionSlide[currentPage];
            _displayText.SetText(instructionText[currentPage].text);
            _displayHeaderText.SetText(headerText[currentPage].text);
        }

    }

    public void PreviousButton()
    {
        if (currentPage > 0)
        {
            currentPage--;
            _displayImage.texture = instructionSlide[currentPage];
            _displayText.SetText(instructionText[currentPage].text);
            _displayHeaderText.SetText(headerText[currentPage].text);
        }

    }

}
