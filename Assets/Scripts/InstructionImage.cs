using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class InstructionImage : MonoBehaviour
{

    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;

    private int currentPage = 0;

    [SerializeField] private  RawImage _displayImage;
    [SerializeField] private  TextMeshProUGUI _displayText;
    [SerializeField] private  TextMeshProUGUI _displayHeaderText;

    [SerializeField] private  List<Texture> instructionSlide = new List<Texture>();
    [SerializeField] private  List<TextMeshProUGUI> instructionText = new List<TextMeshProUGUI>();
    [SerializeField] private  List<TextMeshProUGUI> headerText = new List<TextMeshProUGUI>();


    // Start is called before the first frame update
    private void Start()
    {
        previousButton.SetActive(false);
        _displayImage.texture = instructionSlide[currentPage];
        _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);

    }

    // Update is called once per frame
    private void Update()
    {
        if(currentPage > 0)
        {
            previousButton.SetActive(true);
        }

        nextButton.SetActive(currentPage != instructionSlide.Count - 1);
    }

    public void NextButton()
    {
        if (currentPage >= instructionSlide.Count - 1) return;
        currentPage++;
        _displayImage.texture = instructionSlide[currentPage];
        _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);

    }

    public void PreviousButton()
    {
        if (currentPage <= 0) return;
        currentPage--;
        _displayImage.texture = instructionSlide[currentPage];
        _displayText.SetText(instructionText[currentPage].text);
        _displayHeaderText.SetText(headerText[currentPage].text);

    }

}