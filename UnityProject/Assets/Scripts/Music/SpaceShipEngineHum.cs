using UnityEngine;
using Random = System.Random;

public class SpaceShipEngineHum : SfxSource {
    public float baseFrequency = 25f;     // much deeper fundamental
    public float modFrequency = 1.2f;     // slow sub-rumble
    public float modAmplitude = 3f;       // very subtle pitch wobble
    public float noiseAmplitude = 0.02f;  // almost no noise

    private Random random = new();
    private double time;
    private int sampleRate;
    private AudioClip clip;

    void Start() {
        sampleRate = AudioSettings.outputSampleRate;
        source.clip = AudioClip.Create("EngineHum", sampleRate, 1, sampleRate, true, OnAudioRead, OnAudioSetPosition);
        base.Start();
    }
    
    public override void PlaySfx() { source.Play(); }

    void OnAudioRead(float[] data) {
        double increment = 1.0 / sampleRate;

        for (int i = 0; i < data.Length; i++) {
            double currentFreq = baseFrequency;
            currentFreq += Mathf.Sin((float)(2.0 * Mathf.PI * modFrequency * time)) * modAmplitude;

            // Fundamental sine
            double sine1 = Mathf.Sin((float)(2.0 * Mathf.PI * currentFreq * time));
            // Sub octave (half frequency, super deep)
            double sineSub = Mathf.Sin((float)(2.0 * Mathf.PI * (currentFreq * 0.5) * time));
            // Harmonic (double frequency, adds body)
            double sine2 = Mathf.Sin((float)(2.0 * Mathf.PI * (currentFreq * 2.0) * time));

            // Noise (tiny amount just for texture)
            double noise = (random.NextDouble() * 2.0 - 1.0) * noiseAmplitude;

            // Weighted mix
            float sample = (float)(
                (sine1 * 0.6) +    // main body
                (sineSub * 0.3) +  // deep sub rumble
                (sine2 * 0.1) +    // slight higher harmonic
                noise
            );

            data[i] = sample;
            time += increment;
        }
    }

    void OnAudioSetPosition(int newPosition) { /* Nothing special needed here for continuous loop */ }
}