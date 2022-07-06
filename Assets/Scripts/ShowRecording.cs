using UnityEngine;
using UnityEngine.UI;


//Temp script
public class ShowRecording : MonoBehaviour
{
    private Text _recordingText;
    // Start is called before the first frame update
    void Start()
    {
        _recordingText = GetComponent<Text>();
    }

    public void ShowRecordText()
    {

        _recordingText.text = "Recording";
    }
}
