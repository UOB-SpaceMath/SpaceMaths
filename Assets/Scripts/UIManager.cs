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
    private GameObject AChoise;
    private GameObject BChoise;
    private GameObject CChoise;
    private GameObject DChoise;
    public Material MatWhite;
    public Material MatGreen;
    public Material MatRed;
    private List<Button> btnList;
    private List<GameObject> cubeList;

    private int _numberOne;
    private int _numberTwo;
    private int _numberThree;
    private int _trueIndex;




    // Start is called before the first frame update
    void Start()
    {
        //Catch UI, default color is white
        QuestionText = GameObject.Find("Canvas/QuestionText").GetComponent<Text>();
        AButton = GameObject.Find("Canvas/AButton").GetComponent<Button>();
        BButton = GameObject.Find("Canvas/BButton").GetComponent<Button>();
        CButton = GameObject.Find("Canvas/CButton").GetComponent<Button>();
        DButton = GameObject.Find("Canvas/DButton").GetComponent<Button>();
        AChoise = GameObject.Find("ChoiseA");
        BChoise = GameObject.Find("ChoiseB");
        CChoise = GameObject.Find("ChoiseC");
        DChoise = GameObject.Find("ChoiseD");
        AChoise.transform.GetComponent<Renderer>().material = MatWhite;
        BChoise.transform.GetComponent<Renderer>().material = MatWhite;
        CChoise.transform.GetComponent<Renderer>().material = MatWhite;
        DChoise.transform.GetComponent<Renderer>().material = MatWhite;
        btnList = new List<Button>() { AButton, BButton, CButton, DButton };
        cubeList = new List<GameObject>() { AChoise, BChoise, CChoise, DChoise };

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

    private void GenerateQuestion()
    {
        

         switch (Random.Range(0, 2))
         {
             case 1:
                _numberOne = Random.Range(4, 10);
                _numberTwo = Random.Range(4, 10);
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
        QuestionText.text = "What is the answer for " + _numberOne.ToString() + " ¡Á " + _numberTwo.ToString() + " ?";
        ShowRandomAnswer(_numberThree);
    }

    private void DivisionFunc()
    {
        QuestionText.text = "What is the answer for " + _numberThree.ToString() + " ¡Â " + _numberTwo.ToString() + " ?";
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
        int[] array = new int[4];
        ResultSet.CopyTo(array);
        ShuffleFunc<int>(array);
        AButton.transform.Find("Text").GetComponent<Text>().text = (array[0]).ToString();
        BButton.transform.Find("Text").GetComponent<Text>().text = (array[1]).ToString();
        CButton.transform.Find("Text").GetComponent<Text>().text = (array[2]).ToString();
        DButton.transform.Find("Text").GetComponent<Text>().text = (array[3]).ToString();
    }

    private void ShuffleFunc<T>(T[] _array)
    {
        int rand;
        T tempValue;
        for(int i = 0; i < _array.Length; i++)
        {
            rand = Random.Range(0, _array.Length - i);
            tempValue = _array[rand];
            _array[rand] = _array[_array.Length - 1 - i];
            _array[_array.Length - 1 - i] = tempValue;
        }
    }

    private void GetAnswer(int _result)
    {
        string numString = string.Format("{0}",_result);
        for (int i = 0; i < btnList.Count; i++)
        {
            if (btnList[i].transform.Find("Text").GetComponent<Text>().text.Equals(numString))
            {
                _trueIndex = i;
                break;
            }
        }
        
    }

    private IEnumerator OnBtnClick(int index)
    {   
        if(index == _trueIndex)
        {
            ShowCorrect(index);
        }
        else
        {
            ShowIncorrect(index);
        }
        
        yield return new WaitForSeconds(1);

        Reset(index);

        GenerateQuestion();
    }

    private void ShowCorrect(int index)
    {
        btnList[index].GetComponent<Image>().color = Color.green;
        cubeList[index].GetComponent<MeshRenderer>().material = MatGreen;
    }

    private void ShowIncorrect(int index)
    {
        btnList[index].GetComponent<Image>().color = Color.red;
        cubeList[index].GetComponent<MeshRenderer>().material = MatRed;
    }

    private void Reset(int index)
    {
        btnList[index].GetComponent<Image>().color = Color.white;
        cubeList[index].GetComponent<MeshRenderer>().material = MatWhite;
    }
}
