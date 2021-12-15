using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(AudioSource))]

public class Volume : MonoBehaviour
{
    public int m_Frequency = 44100;
    public int m_Lenght = 1;
    public bool m_IsLoop = true;
    public Microphone microphone;
    public string[] device; // 麥克風設備名稱
    public int devicePos = 0; // 設備位置
    public int minFreq = int.MaxValue, maxFreq = int.MinValue; // 最小頻率, 最大頻率

    [SerializeField] public AudioSource m_AudioSource;
    // [SerializeField] public Pitch pitch;
    public float[] microphoneSamles;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        device = Microphone.devices; // get device name

        // To warn if have no microphone detected
        if (device.Length == 0) { Debug.LogWarning ("No microphone input."); }
        StartCaptureVoice();
    }

    private void Update () {
        // float volume = Volume;
        // Debug.Log("Microphone Volume: " + volume.ToString());
        // Debug.Log("Microphone Frequency: " + pitch.pitchVal.ToString());
        // Debug.Log("Microphone Frequency: " + test().ToString());
    }

    private void OnDestroy () {
        StopCaptureVoice ();
    }

    public float frequency {
        get { return m_AudioSource.clip.frequency; }
        // set { m_Frequency = value; }
    }

    public float volume {
        get {
            // if (Microphone.isRecording (device[devicePos])) {
                // 取得的樣本數量
                int sampleSize = 128;
                float[] samples = new float[sampleSize];
                int startPosition = Microphone.GetPosition (device[devicePos]) - (sampleSize + 1);
                // 得到資料
                this.m_AudioSource.clip.GetData (samples, startPosition);
                microphoneSamles = samples;

                // Getting a peak on the last 128 samples
                float levelMax = 0;
                for (int i = 0; i < sampleSize; i++) {
                    float wavePeek = samples[i];
                    if (levelMax < wavePeek) {
                        levelMax = wavePeek;
                    }
                }
                return levelMax * 99;
            // }
            // return 0;
        }

    }

    public float test() {
        int amountSamples = 128;
            float Threshold = 0.02f;
            float[] dataSpectrum = new float[amountSamples];
            m_AudioSource.GetSpectrumData(dataSpectrum, 0, FFTWindow.BlackmanHarris); //Rectangular
            float maxV = 0;
            var maxN = 0;
            for (int i = 0; i < amountSamples; i++) {
                if (!(dataSpectrum[i] > maxV) || !(dataSpectrum[i] > Threshold)) {
                    continue;
                }
            Debug.Log("dataSpectrum: " + dataSpectrum[i].ToString());

                maxV = dataSpectrum[i];
                maxN = i; // maxN is the index of max
            }

            float freqN = maxN; // pass the index to a float variable
            Debug.Log("maxN: " + maxN.ToString());
            if (maxN > 0 && maxN < amountSamples - 1) { // interpolate index using neighbours
                var dL = dataSpectrum[maxN - 1] / dataSpectrum[maxN];
                var dR = dataSpectrum[maxN + 1] / dataSpectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
                Debug.Log("freqN: " + freqN.ToString());
            }

            return freqN * (AudioSettings.outputSampleRate / 2) / amountSamples; // convert index to frequency
        }

    public void StartCaptureVoice()
    {
        // get microphone frequency
        Microphone.GetDeviceCaps (device[devicePos], out minFreq, out maxFreq);
        // set audio source
        m_AudioSource.clip = Microphone.Start (device[devicePos], true, 3599, maxFreq);
        m_AudioSource.loop = true;
        m_AudioSource.timeSamples = Microphone.GetPosition (device[devicePos]);
        m_AudioSource.Play();
    }

    public void StopCaptureVoice()
    {
        // if (Microphone.IsRecording(null) == false)
        //     return;

        Microphone.End(null);
        m_AudioSource.Stop();
    }
}