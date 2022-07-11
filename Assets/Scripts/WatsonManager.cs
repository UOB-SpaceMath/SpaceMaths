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
    #endregion

    readonly string _micDevice;
    readonly int _recordingHZ = 22050;
    readonly int _recordBufferSec = 1;

    // results
    AudioClip _resultClip;
    string _speechToTextResult;
    MessageOutput _assistantResult;
    string _failMessage;
    // final result
    WatsonOutput _watsonOutput;


    // status
    bool _isRecording = false;  // flag to start and end recording coroutine, false doesn't indicate that audioClip is ready.
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
        StartCoroutine(WaitSecThenStopRecording(0.5f));
    }

    IEnumerator WaitSecThenStopRecording(float sec)
    {
        yield return new WaitForSeconds(sec);
        Debug.Log("Stop microphone.");
        _isRecording = false;
        StartCoroutine(GetWatsonOutput());
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
        _failMessage = null;

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
        else
        {
            _failMessage = "Recording time too short.";
        }
        _isAssemblingClip = false;
    }

    IEnumerator SpeechToTextService()
    {
        Debug.Log("Start S2T");
        _isSpeechToTextRunning = true;
        // IBM Cloud authentication
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
        // already fail in recording.
        if (_failMessage != null)
        {
            _isSpeechToTextRunning = false;
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
        // wait for Speech to text result
        while (recognizeResponse == null)
        {
            yield return null;
        }
        if (recognizeResponse.Results.Count != 0 &&
            recognizeResponse.Results[0].Alternatives.Count != 0)
        {
            _speechToTextResult = recognizeResponse.Results[0].Alternatives[0].Transcript;
        }
        else
        {
            _failMessage = "Please try to record clearly.";
        }
        _isSpeechToTextRunning = false;
    }

    IEnumerator AssistantService()
    {
        Debug.Log("Start Assistant");
        _isAssistantRunning = true;
        // IBM Cloud authentication
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
        // already fail in other service;
        if (_failMessage != null)
        {
            _isAssistantRunning = false;
            yield break;
        };
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

    // get final result from Watson Assistant response
    IEnumerator GetWatsonOutput()
    {
        // wait for assistant finish
        while (_isAssistantRunning)
        {
            yield return null;
        }
        Debug.Log(string.Format("GetWatsonOutput(): start parsing."));
        if (_failMessage == null)
        {
            // get intent and index
            var intent = GetFinalIntent();
            var index = new Vector2Int();
            if (intent == WatsonIntents.Fail)
                _watsonOutput = new WatsonOutput("Watson cannot identify your intent. Please try keywords such as Attack, Navigate or Shield.");
            else if (!GetFinalIndex(ref index) && intent != WatsonIntents.Sheild)
                _watsonOutput = new WatsonOutput(string.Format("Please specific a target cell to {0}.", intent.ToString().ToLower()));
            else
                _watsonOutput = new WatsonOutput(intent, index);
            // log
            if (intent == WatsonIntents.Fail)
                Debug.Log(string.Format("GetFinalResult(): Fail {0}", _watsonOutput.FailMessage));
            else
                Debug.Log(string.Format("GetFinalResult(): {0} {1}", _watsonOutput.Intent, _watsonOutput.SelectionIndex));
        }
        else
        {
            _watsonOutput = new WatsonOutput(_failMessage);
        }
    }

    // get intent from Watson assistant result
    WatsonIntents GetFinalIntent()
    {
        if (_assistantResult.Intents.Count == 0)
            return WatsonIntents.Fail;
        var intent = _assistantResult.Intents[0].Intent.ToLower() switch
        {
            "attack" => WatsonIntents.Attack,
            "move" => WatsonIntents.Move,
            "shield" => WatsonIntents.Sheild,
            _ => WatsonIntents.Fail
        };
        Debug.Log(string.Format("GetFinalIntent(): {0}", intent));
        return intent;
    }

    // get index from Watson assistant result
    bool GetFinalIndex(ref Vector2Int result)
    {
        var xList = new List<int>();
        var yList = new List<int>();
        foreach (var entity in _assistantResult.Entities)
        {
            if (entity.Entity.ToLower().Equals("row"))
            {
                xList.Add(int.Parse(entity.Value));
            }
            if (entity.Entity.ToLower().Equals("column"))
            {
                yList.Add(int.Parse(entity.Value));
            }
        }
        Debug.Log(string.Format("GetFinalIndex(): x:{0} y:{1}", string.Join(",", xList), string.Join(",", yList)));
        if (xList.Count != 1 || yList.Count != 1)
        {
            return false;
        }
        result.x = xList[0];
        result.y = yList[0];
        return true;
    }

    public WatsonOutput GetFinalResult()
    {
        return _watsonOutput;
    }

    public void ResetFinalResult()
    {
        _watsonOutput = null;
    }

    public bool IsWatsonRunning()
    {
        return _isAssemblingClip || _isSpeechToTextRunning || _isAssistantRunning;
    }
}

namespace SpaceMath
{
    public enum WatsonIntents { Attack, Move, Sheild, Fail }

    public class WatsonOutput
    {
        readonly WatsonIntents _intent;
        Vector2Int _selectionIndex;
        readonly string _failMessage;

        public WatsonOutput(WatsonIntents intent, Vector2Int selectionIndex)
        {
            _intent = intent;
            _selectionIndex = selectionIndex;
        }

        public WatsonOutput(string failString)
        {
            _intent = WatsonIntents.Fail;
            _failMessage = failString;
        }

        public WatsonIntents Intent { get => _intent; }
        public Vector2Int SelectionIndex { get => _selectionIndex; }
        public string FailMessage { get => _failMessage; }
    }
}


