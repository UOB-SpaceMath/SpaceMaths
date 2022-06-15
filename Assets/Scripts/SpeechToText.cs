using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechToText : MonoBehaviour
{
    [SerializeField]
    AudioSource _audioSource;
    readonly string _micDevice;
    readonly int _recordingHZ = 22050;
    readonly int _recordBufferSec = 1;

    AudioClip _ResultClip;
    bool _isRecording = false;

    List<float> recordDataList = new List<float>();

    public void StartRecording()
    {
        // clear clip
        if (_ResultClip)
        {
            Destroy(_ResultClip);
            _ResultClip = null;
        }
        // null is default microphone
        StopCoroutine(RecordingHandler());
        StartCoroutine(RecordingHandler());
    }


    IEnumerator RecordingHandler()
    {
        Debug.Log("Start Mic");
        recordDataList = new List<float>();
        AudioClip tempClip = Microphone.Start(_micDevice, true, _recordBufferSec, _recordingHZ);
        _isRecording = true;
        yield return null; // leave to next frame


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
        _ResultClip = AudioClip.Create("result", recordDataList.Count, 1, _recordingHZ, false);
        _ResultClip.SetData(recordDataList.ToArray(), 0);
        yield break;
    }






    public void StopRecording()
    {
        Debug.Log("Stop microphone.");
        _isRecording = false;
    }

    public void PlayAudioRecord()
    {
        if (_ResultClip)
        {
            _audioSource.clip = _ResultClip;
            _audioSource.Play();
        }
    }
}
