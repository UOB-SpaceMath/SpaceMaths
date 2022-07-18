using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using IBM.Watson.SpeechToText.V1;
using IBM.Watson.SpeechToText.V1.Model;
using SpaceMath;
using UnityEngine;
using UnityEngine.Serialization;

public class WatsonManager : MonoBehaviour
{
    private const int _recordBufferSec = 1;
    private const string _micDevice = "";

    // final result
    private WatsonOutput _watsonOutput;

    // status
    private bool _isRecording; // flag to start and end recording coroutine, doesn't indicate that audioClip is ready.
    private bool _isHandlingStart;
    private bool _isHandlingStop; // flag indicated that the WatsonManager is fetching result from cloud.

    // results
    private Authenticator _speechToTextIam;
    private Authenticator _AssistantIam;
    private AudioClip _clip;

    // buffer for recording
    private List<float> _recordDataList;

    private static IEnumerator Timer(Action timeOutCallback, float sec)
    {
        // wait for enough sec
        yield return new WaitForSeconds(sec);
        timeOutCallback();
    }

    public void StartRecording()
    {
        ResetAll();
        StartCoroutine(StartHandler());
    }

    public void StopRecording()
    {
        StartCoroutine(StopHandler());
    }

    private void ResetAll()
    {
        StopAllCoroutines();
        _isRecording = false;
        _isHandlingStart = false;
        _isHandlingStop = false;
        _watsonOutput = null;
    }

    private IEnumerator StartHandler()
    {
        _isHandlingStart = true;
        // recording coroutine
        string errorString = null;
        AudioClip clip = null;
        StartCoroutine(RecordingService(
            (resultClip) => clip = resultClip,
            (error) => errorString = error));
        // authentication coroutine
        Authenticator speechToTextIam = null;
        Authenticator assistantIam = null;
        StartCoroutine(GetAuthenticator(
            (result) => speechToTextIam = result,
            (error) => errorString = error,
            _speechToTextKey));
        StartCoroutine(GetAuthenticator(
            (result) => assistantIam = result,
            (error) => errorString = error,
            _assistantKey));

        // when no error and not all result back.
        while (errorString is null &&
               (clip is null || speechToTextIam is null || assistantIam is null))
            yield return null;

        if (errorString is null)
        {
            _speechToTextIam = speechToTextIam;
            _AssistantIam = assistantIam;
            _clip = clip;
        }
        else
        {
            _watsonOutput = new WatsonOutput(errorString);
        }

        Debug.Log("Stop StartHandler.");
        _isHandlingStart = false;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator StopHandler()
    {
        _isHandlingStop = true;
        // wait to stop recording
        var isTimeOut = false;
        StartCoroutine(Timer(() => isTimeOut = true, _stopRecordingSeconds));
        while (!isTimeOut) yield return null;
        // set flag for RecordingService to stop recording.
        Debug.Log("Stop microphone.");
        _isRecording = false;
        // wait until startHandler stop
        while (_isHandlingStart)
            yield return null;
        if (_watsonOutput == null) //if not null, already fail before, no need to access Watson.           
        {
            // timer for stop fetching
            var isFetchingTimeOut = false;
            StartCoroutine(Timer(() => isFetchingTimeOut = true, _connectionTimeOutSecond));
            // speech to text
            string transcript = null;
            StartCoroutine(SpeechToTextService(
                (result) => transcript = result,
                _speechToTextIam,
                _clip));
            // wait for speech to text
            while (transcript is null && !isFetchingTimeOut)
                yield return null;
            if (!isFetchingTimeOut)
            {
                // assistant
                MessageOutput output = null;
                StartCoroutine(AssistantService(
                    (result) => output = result,
                    _AssistantIam,
                    transcript));
                // wait for watson assistant
                while (output is null && !isFetchingTimeOut)
                    yield return null;
                if (!isFetchingTimeOut)
                    _watsonOutput = GetWatsonOutput(output);
            }

            // _watsonOutput is null mean is out of time
            if (_watsonOutput == null)
                _watsonOutput =
                    new WatsonOutput("Sending command time out, you may not be connected to the spaceship.");
        }

        _isHandlingStop = false;
        Debug.Log("Stop StopHandler.");
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator RecordingService(Action<AudioClip> successCallback, Action<string> failureCallback)
    {
        // start timer
        var isTimeOut = false;
        StartCoroutine(Timer(() => isTimeOut = true, _minimalRecordingSeconds));
        // start recording
        Debug.Log("Start Mic");
        _recordDataList = new List<float>();
        var tempClip = Microphone.Start(_micDevice, true, _recordBufferSec, _recordingHZ);
        if (tempClip is null)
        {
            failureCallback("Captain, you don't have an microphone.");
            yield break;
        }

        _isRecording = true;

        var isFirstBlock = true;
        var midPos = tempClip.samples / 2;
        while (_isRecording)
        {
            var currentPos = Microphone.GetPosition(_micDevice);
            if ((isFirstBlock && currentPos >= midPos) || (!isFirstBlock && currentPos < midPos))
            {
                var tempSamples = new float[midPos];
                tempClip.GetData(tempSamples, isFirstBlock ? 0 : midPos);
                _recordDataList.AddRange(tempSamples);
                isFirstBlock = !isFirstBlock;
            }
            else
            {
                var remaining = isFirstBlock ? midPos - currentPos : tempClip.samples - currentPos;
                var timeRemaining = remaining / (float)_recordingHZ;
                yield return new WaitForSeconds(timeRemaining);
            }
        }

        // check recording time long enough
        if (!isTimeOut)
        {
            failureCallback("Your command is to short!");
            yield break;
        }

        var result = AudioClip.Create("result", _recordDataList.Count, 1, _recordingHZ, false);
        result.SetData(_recordDataList.ToArray(), 0);
        successCallback(result);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator GetAuthenticator(Action<IamAuthenticator> successCallback, Action<string> failureCallback,
        string apiKey)
    {
        Debug.Log($"Getting authenticator from apiKey:{apiKey}");
        var isTimeOut = false;
        StartCoroutine(Timer(() => { isTimeOut = true; }, _connectionTimeOutSecond));
        // IBM Cloud authentication
        var authenticator = new IamAuthenticator(apiKey);
        // if not authenticated or not time out, continue to try
        while (!authenticator.CanAuthenticate() && !isTimeOut)
            yield return null;
        if (isTimeOut)
            failureCallback("Authentication time out, you may not connected to your ship");
        else
            successCallback(authenticator);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    // callback string will be null if there is no result
    private IEnumerator SpeechToTextService(
        Action<string> resultCallback,
        Authenticator authenticator,
        AudioClip input)
    {
        Debug.Log("Start S2T");
        var speechToText = new SpeechToTextService(authenticator);
        speechToText.SetServiceUrl(_speechToTextUrl);
        SpeechRecognitionResults recognizeResponse = null;
        // setup timer
        speechToText.Recognize(
            (response, error) => { recognizeResponse = response.Result; },
            new MemoryStream(WavUtility.FromAudioClip(input)),
            "audio/wav",
            _recognizeModel
        );
        Debug.Log("Sent audio to S2T");
        // wait for Speech to text result
        while (recognizeResponse == null)
            yield return null;

        var result = "";
        if (recognizeResponse.Results.Count != 0 &&
            recognizeResponse.Results[0].Alternatives.Count != 0)
            result = recognizeResponse.Results[0].Alternatives[0].Transcript;
        resultCallback(result);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator AssistantService(
        Action<MessageOutput> resultCallback,
        Authenticator authenticator,
        string input)
    {
        Debug.Log("Start Assistant");

        var assistant = new AssistantService("2021-06-14", authenticator);
        assistant.SetServiceUrl(_assistantUrl);

        // sent text to assistant
        MessageResponseStateless messageResponse = null;
        assistant.MessageStateless(
            (response, error) => { messageResponse = response.Result; },
            _assistantId,
            new MessageInputStateless { Text = input }
        );
        Debug.Log($"Sent to assistant: {input}");
        // wait for assistant result
        while (messageResponse == null) yield return null;

        Debug.Log("Assistant result back");
        resultCallback(messageResponse.Output);
    }

    // get final result from Watson Assistant response
    // ReSharper disable Unity.PerformanceAnalysis
    private WatsonOutput GetWatsonOutput(MessageOutput input)
    {
        Debug.Log("Start parsing Watson output.");
        WatsonOutput result;
        // get intent and index
        var intent = GetFinalIntent(input);
        var index = new Vector2Int();
        // no intent
        if (intent == WatsonIntents.Fail)
            result = new WatsonOutput(
                "Your intent is ambitious. Please use keywords such as Attack, Move or Shield.");
        // no index
        else if (!GetFinalIndex(ref index, input) && intent != WatsonIntents.Shield)
            result = new WatsonOutput($"Please specific a target cell to {intent.ToString().ToLower()}.");
        // success
        else
            result = new WatsonOutput(intent, index);
        // log
        Debug.Log(result);
        return result;
    }

    // get intent from Watson assistant result
    private static WatsonIntents GetFinalIntent(MessageOutput input)
    {
        if (input.Intents.Count == 0)
            return WatsonIntents.Fail;
        var intent = input.Intents[0].Intent.ToLower() switch
        {
            "attack" => WatsonIntents.Attack,
            "move" => WatsonIntents.Move,
            "shield" => WatsonIntents.Shield,
            _ => WatsonIntents.Fail
        };
        Debug.Log($"GetFinalIntent(): {intent}");
        return intent;
    }

    // get index from Watson assistant result
    private static bool GetFinalIndex(ref Vector2Int result, MessageOutput input)
    {
        var xList = new List<int>();
        var yList = new List<int>();
        foreach (var entity in input.Entities)
        {
            if (entity.Entity.ToLower().Equals("row")) xList.Add(int.Parse(entity.Value));

            if (entity.Entity.ToLower().Equals("column")) yList.Add(int.Parse(entity.Value));
        }

        Debug.Log($"GetFinalIndex(): x:{string.Join(",", xList)} y:{string.Join(",", yList)}");
        if (xList.Count != 1 || yList.Count != 1) return false;

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
        // todo
        return _isHandlingStart || _isHandlingStop;
    }

    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR

    [Header("Watson Speech To Text")] //
    [SerializeField]
    private string _speechToTextUrl;

    [SerializeField] private string _speechToTextKey;

    [SerializeField] private string _recognizeModel;

    [Header("Watson Assistant")] //
    [SerializeField]
    private string _assistantUrl;

    [SerializeField] private string _assistantKey;

    [SerializeField] private string _assistantId;

    [FormerlySerializedAs("_connectionTimeOut")]
    [Header("Setting")] // 
    [SerializeField]
    private float _connectionTimeOutSecond = 10;

    [SerializeField] private float _minimalRecordingSeconds = 2;
    [SerializeField] private float _stopRecordingSeconds = 0.5f;
    [SerializeField] private int _recordingHZ = 22050;

    #endregion
}

namespace SpaceMath
{
    public enum WatsonIntents
    {
        Attack,
        Move,
        Shield,
        Fail
    }

    public class WatsonOutput
    {
        public WatsonOutput(WatsonIntents intent, Vector2Int selectionIndex)
        {
            Intent = intent;
            SelectionIndex = selectionIndex;
        }

        public WatsonOutput(string failString)
        {
            Intent = WatsonIntents.Fail;
            FailMessage = failString;
        }

        public WatsonIntents Intent { get; }

        public Vector2Int SelectionIndex { get; }

        public string FailMessage { get; }

        public override string ToString()
        {
            var prefix = $"Watson Output:\n{Intent}, ";
            return Intent == WatsonIntents.Fail ? prefix + FailMessage : prefix + SelectionIndex;
        }
    }
}