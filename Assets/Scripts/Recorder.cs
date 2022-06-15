using UnityEngine;

public class Recorder : MonoBehaviour
{
    AudioSource m_Source;

    void Start()
    {
        m_Source = GetComponent<AudioSource>();
    }

    public void StartRecording()
    {
        m_Source.clip = Microphone.Start("", false, 30, 44100);

    }

    public void EndRecording()
    {
        if (Microphone.IsRecording(""))
        {
            Microphone.End(null);
        }
    }

    public void PlayRecord()
    {
        m_Source.Play();
    }
}

