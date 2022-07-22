using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private Text QuestionText;
    private Button AButton;
    private Button BButton;
    private Button CButton;
    private Button DButton;
    private List<Button> btnList;
    private Color basicColor;

    // Record the state of answer.
    public enum AnswerStates { Suspension, Right, Wrong };
    private AnswerStates answerState;

    //Need to change
    private int[] btnArray;


    private int _numberOne;
    private int _numberTwo;
    private int _numberThree;
    private int _trueIndex;




    // Start is called before the first frame update
    void Start()
    {
        answerState = AnswerStates.Suspension;

        //Catch UI, default color is white
        QuestionText = GameObject.Find("Canvas/SF QuestionText/QText").GetComponent<Text>();
        AButton = GameObject.Find("Canvas/SF ButtonA").GetComponent<Button>();
        BButton = GameObject.Find("Canvas/SF ButtonB").GetComponent<Button>();
        CButton = GameObject.Find("Canvas/SF ButtonC").GetComponent<Button>();
        DButton = GameObject.Find("Canvas/SF ButtonD").GetComponent<Button>();
        ColorUtility.TryParseHtmlString("#0772CB", out basicColor);

        btnList = new List<Button>() { AButton, BButton, CButton, DButton };
        btnArray = new int[4];


        GenerateQuestion();
        for (var i = 0; i < btnList.Count; i++)
        {
            int index = i;
            btnList[i].onClick.AddListener(() => StartCoroutine(OnBtnClick(index)));
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAnswerState(AnswerStates s)
    {
        answerState = s;
    }

    public AnswerStates GetAnswerState()
    {
        return answerState;
    }

    private void GenerateQuestion()
    {

        switch (Random.Range(0, 2))
        {
            case 1:
                _numberOne = Random.Range(4, 13);
                _numberTwo = Random.Range(4, 13);
                _numberThree = _numberOne * _numberTwo;
                MultiplicationFunc();
                GetAnswer(_numberThree);
                break;
            default:
                _numberOne = Random.Range(10, 20);
                _numberTwo = Random.Range(1, 10);
                _numberThree = _numberOne * _numberTwo;
                DivisionFunc();
                GetAnswer(_numberOne);
                break;
        }

    }

    private void MultiplicationFunc()
    {
        QuestionText.text = "What is the answer for " + _numberOne.ToString() + " × " + _numberTwo.ToString() + " ?\n";
        ShowRandomAnswer(_numberThree);
    }

    private void DivisionFunc()
    {
        QuestionText.text = "What is the answer for " + _numberThree.ToString() + " ÷ " + _numberTwo.ToString() + " ?\n";
        ShowRandomAnswer(_numberOne);
    }

    private void ShowRandomAnswer(int _result)
    {
        HashSet<int> ResultSet = new HashSet<int>
        {
            _result
        };
        while (ResultSet.Count != 4)
        {
            ResultSet.Add(_result + Random.Range(-4, 5));
        }
        ResultSet.CopyTo(btnArray);
        ShuffleFunc<int>(btnArray);

        //Need to check
        QuestionText.text += "A." + btnArray[0].ToString() + "\n" + "B." + btnArray[1].ToString() + "\n" + "C." + btnArray[2].ToString() + "\n" + "D." + btnArray[3].ToString();

    }

    private void ShuffleFunc<T>(T[] _array)
    {
        int rand;
        T tempValue;
        for (int i = 0; i < _array.Length; i++)
        {
            rand = Random.Range(0, _array.Length - i);
            tempValue = _array[rand];
            _array[rand] = _array[_array.Length - 1 - i];
            _array[_array.Length - 1 - i] = tempValue;
        }
    }

    private void GetAnswer(int _result)
    {

        for (int i = 0; i < btnList.Count; i++)
        {
            if (btnArray[i].Equals(_result))
            {
                _trueIndex = i;
                break;
            }
        }

    }

    private IEnumerator OnBtnClick(int index)
    {
        if (index == _trueIndex)
        {
            ShowCorrect(index);
        }
        else
        {
            ShowIncorrect(index);
        }

        ControlButton(false);
        yield return new WaitForSeconds(1);

        //Need to check this when merge the gameManager
        ResetButton(index);

        GenerateQuestion();
    }

    private void ShowCorrect(int index)
    {
        btnList[index].transform.Find("Background").GetComponent<Image>().color = Color.green;
        btnList[index].transform.Find("Background/Label").GetComponent<Text>().color = Color.green;
        SetAnswerState(AnswerStates.Right);
    }

    private void ShowIncorrect(int index)
    {
        btnList[index].transform.Find("Background").GetComponent<Image>().color = Color.red;
        btnList[index].transform.Find("Background/Label").GetComponent<Text>().color = Color.red;
        SetAnswerState(AnswerStates.Wrong);
    }

    private void ControlButton(bool buttonStatus)
    {
        for (var i = 0; i < btnList.Count; i++)
        {
            btnList[i].GetComponent<Button>().interactable = buttonStatus;
        }
    }

    private void ResetButton(int index)
    {
        btnList[index].transform.Find("Background").GetComponent<Image>().color = basicColor;
        btnList[index].transform.Find("Background/Label").GetComponent<Text>().color = Color.white;
        ControlButton(true);
    }
}
