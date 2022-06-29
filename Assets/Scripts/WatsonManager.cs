using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using IBM.Watson.SpeechToText.V1;
using IBM.Watson.SpeechToText.V1.Model;
using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WatsonManager : MonoBehaviour
{

    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Header("Watson Speech To Text")]
    [SerializeField]
    private string _speechToTextUrl;
    [SerializeField]
    private string _speechToTextKey;
    [SerializeField]
    private string _recognizeModel;

    [Header("Watson Assistant")]
    [SerializeField]
    private string _assistantUrl;
    [SerializeField]
    private string _assistantKey;
    [SerializeField]
    private string _assistantId;

    [Header("Debug")]
    public Text _displayField;
    #endregion

    readonly string _micDevice;
    readonly int _recordingHZ = 22050;
    readonly int _recordBufferSec = 1;

    // results
    AudioClip _resultClip;
    string _speechToTextResult;
    MessageOutput _assistantResult;
    // final result
    WatsonIntents _action;
    Vector2Int _targetIndex;


    // status
    bool _isRecording = false;  // is recording (true to false doesn't mean recording finish)
    bool _isLongEnough = false; // recording long enough
    bool _isAssemblingClip = false; // _resultClip is Assembling. (true to false means finish recording)
    bool _isSpeechToTextRunning = false;
    bool _isAssistantRunning = false;

    // buffer for recording
    List<float> recordDataList = new List<float>();

    public void StartRecording()
    {
        ResetAll();
        // start recording and Watson
        StartCoroutine(RecordingHandler());
        StartCoroutine(SpeechToTextService());
        StartCoroutine(AssistantService());
    }

    public void StopRecording()
    {
        Debug.Log("Stop microphone.");
        _isRecording = false;

        StartCoroutine(GetFinalResult());
    }


    void ResetAll()
    {
        // stop all
        StopAllCoroutines();
        // reset status
        _isRecording = false;
        _isLongEnough = false;
        _isAssemblingClip = false;
        _isSpeechToTextRunning = false;
        _isAssistantRunning = false;
        // reset result
        _speechToTextResult = null;
        _assistantResult = null;
        _action = WatsonIntents.Fail;

        // clear clip
        if (_resultClip)
        {
            Destroy(_resultClip);
            _resultClip = null;
        }
    }

    IEnumerator EnsureRecordLength(int sec)
    {
        _isLongEnough = false;
        // wait for enough sec
        yield return new WaitForSeconds(sec);
        _isLongEnough = true;
    }

    IEnumerator RecordingHandler()
    {
        var timer = StartCoroutine(EnsureRecordLength(2));

        Debug.Log("Start Mic");
        recordDataList = new List<float>();
        AudioClip tempClip = Microphone.Start(_micDevice, true, _recordBufferSec, _recordingHZ);
        _isRecording = true;
        _isAssemblingClip = true;

        bool isFirstBlock = true;
        int midPos = tempClip.samples / 2;
        while (_isRecording)
        {
            int currentPos = Microphone.GetPosition(_micDevice);
            if ((isFirstBlock && currentPos >= midPos) || (!isFirstBlock && currentPos < midPos))
            {
                float[] tempSamples = new float[midPos];
                tempClip.GetData(tempSamples, isFirstBlock ? 0 : midPos);
                recordDataList.AddRange(tempSamples);
                isFirstBlock = !isFirstBlock;
            }
            else
            {
                int remaining = isFirstBlock ? (midPos - currentPos) : (tempClip.samples - currentPos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }
        }
        StopCoroutine(timer);
        // check recording time long enough
        if (_isLongEnough)
        {
            _resultClip = AudioClip.Create("result", recordDataList.Count, 1, _recordingHZ, false);
            _resultClip.SetData(recordDataList.ToArray(), 0);
        }
        _isAssemblingClip = false;
    }


    IEnumerator AssistantService()
    {
        Debug.Log("Start Assistant");
        _isAssistantRunning = true;
        var authenticator = new IamAuthenticator(apikey: _assistantKey);
        while (!authenticator.CanAuthenticate())
            yield return null;

        var assistant = new AssistantService("2021-06-14", authenticator);
        assistant.SetServiceUrl(_assistantUrl);
        // wait for SpeechToTextFinish
        while (_isSpeechToTextRunning)
        {
            yield return null;
        }
        if (_speechToTextResult is null)
        {
            // todo
            Debug.Log("fail to get text from speech.");
            yield break;
        }
        // sent text to assistant
        MessageResponseStateless messageResponse = null;
        assistant.MessageStateless(
            callback: (DetailedResponse<MessageResponseStateless> response, IBMError error) =>
            {
                messageResponse = response.Result;
            },
            assistantId: _assistantId,
            input: new MessageInputStateless()
            {
                Text = _speechToTextResult
            }
        );
        Debug.Log(string.Format("Sent to assistant: {0}", _speechToTextResult));
        // wait for assistant result
        while (messageResponse == null)
        {
            yield return null;
        }
        Debug.Log("Assistant result back");

        _assistantResult = messageResponse.Output;
        _isAssistantRunning = false;
    }

    IEnumerator SpeechToTextService()
    {
        Debug.Log("Start S2T");
        _isSpeechToTextRunning = true;
        var authenticator = new IamAuthenticator(apikey: _speechToTextKey);
        while (!authenticator.CanAuthenticate())
            yield return null;

        var speechToText = new SpeechToTextService(authenticator);
        speechToText.SetServiceUrl(_speechToTextUrl);
        SpeechRecognitionResults recognizeResponse = null;

        // wait for assembling audioCip
        while (_isAssemblingClip)
        {
            yield return null;
        }
        if (_resultClip is null)
        {
            // todo 
            Debug.Log("Recording fail. Might be to short.");
            yield break;
        }
        speechToText.Recognize(
            callback: (DetailedResponse<SpeechRecognitionResults> response, IBMError error) =>
            {
                recognizeResponse = response.Result;
            },
            audio: new MemoryStream(WavUtility.FromAudioClip(_resultClip)),
            contentType: "audio/wav"
        );
        Debug.Log("Sent audio to S2T");
        // wait for Speech to text
        while (recognizeResponse == null)
        {
            yield return null;
        }
        _speechToTextResult = recognizeResponse.Results[0].Alternatives[0].Transcript;
        _isSpeechToTextRunning = false;
    }

    private void Update()
    {

    }

    // todo 
    IEnumerator GetFinalResult()
    {
        // wait for assistant finish
        while (_isAssistantRunning)
        {
            yield return null;
        }
        // get intent
        if (_assistantResult.Intents.Count != 0)
        {
            _action = _assistantResult.Intents[0].Intent.ToLower() switch
            {
                "attack" => WatsonIntents.Attack,
                "move" => WatsonIntents.Move,
                "shield" => WatsonIntents.Sheild,
                _ => WatsonIntents.Fail
            };
            Debug.Log(_action);
        }
        bool hasRow = false;
        bool hasCol = false;
        foreach (var entity in _assistantResult.Entities)
        {
            if (entity.Entity.Equals("row") && !hasRow)
            {
                hasRow = true;
                _targetIndex.x = int.Parse(entity.Value);
                Debug.Log(string.Format("X: {0}", _targetIndex.x));
            }
            if (entity.Entity.Equals("column") && !hasCol)
            {
                hasCol = true;
                _targetIndex.y = int.Parse(entity.Value);
                Debug.Log(string.Format("Y: {0}", _targetIndex.y));

            }
        }
        if (!hasCol || !hasRow)
        {
            _action = WatsonIntents.Fail;
        }
        Debug.Log(string.Format("{0}: {1}", _action.ToString(), _targetIndex.ToString()));
    }

    enum WatsonIntents { Attack, Move, Sheild, Fail }
}


