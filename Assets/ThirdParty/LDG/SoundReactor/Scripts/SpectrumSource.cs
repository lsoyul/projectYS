// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

// The following copywrite notice is for the parts of this file that resemble the original work
// created by Keijiro in the project located here:
// https://github.com/keijiro/unity-audio-spectrum
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.IO;

namespace LDG.SoundReactor
{
    public enum NormalizeMode
    {
        Raw,
        PeakBand,
        Peak
    }

    // https://docs.unity3d.com/Manual/class-AudioClip.html
    public enum AudioChannel
    {
        FrontLeft,
        FrontRight,
        FrontCenter,
        Subwoofer,
        RearLeft,
        RearRight,
        AlternativeRearLeft,
        AlternativeRearRight
    }

    [DisallowMultipleComponent]
    public class SpectrumSource : SerializeableObject
    {
        #region Properties
        public int channels
        {
            get
            {
                if (audioSource)
                {
                    return audioSource.clip.channels;
                }

                return 1;
            }
        }

        private float _peakLevel = 0.00001f;
        public float peakLevel
        {
            get
            {
                return _peakLevel;
            }
        }

        public int bands
        {
            get
            {
                if (peaksProfile)
                {
                    return Frequency.FrequencyBands[(int)bandOption].Length - 1;
                }

                return 0;
            }
        }

        bool _recordProfile = false;
        public bool recordProfile
        {
            get
            {
                return _recordProfile;
            }

            set
            {
                _recordProfile = value;
            }
        }
        #endregion

        #region Public variables
        public AudioSource audioSource;
        public PeaksProfile peaksProfile;

        public int audioChannel = (int)AudioChannel.FrontLeft;
        public BandOption bandOption = BandOption.StandardRanges;
        public NormalizeMode normalizeMode = NormalizeMode.PeakBand;

        public float amplitudeScale = 1.0f;
        #endregion

        #region Private variables
        // spectrum index is in Hz [0, 24000] and their values are the amplitude
        float[] spectrumData;

        // these become the size of band count
        float[] levels;
        float[] peakLevels;
        float[] normalizedLevels;
        #endregion

        #region Public Functions

#if UNITY_EDITOR
        float prevTime;

        public void RecordPeaks()
        {
            if (audioSource)
            {
                Refresh();
                audioSource.Play();
                bandOption = BandOption.OneTwelvethOctave;

                prevTime = 0.0f;
                recordProfile = true;
            }
        }

        void AutoSavePeaks()
        {
            if (audioSource && audioSource.clip && recordProfile && peaksProfile)
            {
                if ((audioSource.time - prevTime) < 0.0f)
                {
                    peaksProfile.SetPeaks(peakLevels, peakLevel);

                    EditorUtility.SetDirty(peaksProfile);
                    
                    Debug.Log("Saved peaks for: " + audioSource.clip.name);

                    recordProfile = false;

                    EditorUtility.SetDirty(this);
                }

                prevTime = audioSource.time;
            }
        }
#endif

        public float GetLevel(float linearizedFrequency, InterpolationMode interpolationMode)
        {
            return Spline.Tween(linearizedFrequency, normalizedLevels, false, interpolationMode);
        }

        public void Refresh()
        {
            if (peaksProfile)
            {
                UpdateBuffers();
                ResetPeaks();
            }
        }
        #endregion

        #region Unity overrides
        private void Update()
        {
            if (peaksProfile)
            {
                UpdateSpectrum();
#if UNITY_EDITOR
                AutoSavePeaks();
#endif
            }
        }
        #endregion

        #region Private functions

        /// <summary>
        /// Resets the peaks. If there is a spectrum profile asset, then the peaks will be loaded from there.
        /// </summary>
        void ResetPeaks()
        {
            if (peaksProfile && !peaksProfile.isDirty && peakLevels != null)
            {
                peaksProfile.GetPeaks(ref peakLevels, ref _peakLevel);

                //Debug.Log("Using Profile: " + peaksProfile.name);
            }
            else
            {
                if (peakLevels != null && peakLevels.Length == bands)
                {
                    for (int i = 0; i < bands; i++)
                    {
                        peakLevels[i] = 0.001f;
                    }
                }

                _peakLevel = 0.001f;
            }
        }

        /// <summary>
        /// Allocate buffers dynamically so they can be changed at runtime without crashing the app
        /// </summary>
        void UpdateBuffers()
        {
            if (spectrumData == null || spectrumData.Length != peaksProfile.fftSamples)
            {
                spectrumData = new float[peaksProfile.fftSamples];
            }

            if (levels == null || levels.Length != bands)
            {
                levels = new float[bands];
            }

            if (peakLevels == null || peakLevels.Length != bands)
            {
                peakLevels = new float[bands];

                ResetPeaks();
            }

            if (normalizedLevels == null || normalizedLevels.Length != bands)
            {
                normalizedLevels = new float[bands];
            }
        }

        /// <summary>
        /// Updates spectrum peaks and levels from a given audio channel.
        /// </summary>
        void UpdateSpectrum()
        {
            // leave now if a spectrum profile doesn't exist
            if (!peaksProfile) return;

            float[] bandFreq = Frequency.FrequencyBands[(int)bandOption];

            float lower;
            float upper;

            UpdateBuffers();
            
            // get spectrum data from a particular audio channel
            if (audioSource && audioSource.clip)
            {
                // get filtered audioChannel.
                int audioChannel = Mathf.Clamp(this.audioChannel, 0, (audioSource.clip.channels - 1));

                audioSource.GetSpectrumData(spectrumData, audioChannel, peaksProfile.fftWindow);
            }
            else
            {
                AudioListener.GetSpectrumData(spectrumData, 0, peaksProfile.fftWindow);
            }

            // gather band peaks
            for (var bi = 0; bi < bands; bi++)
            {
                lower = bandFreq[bi];
                upper = bandFreq[bi + 1];

                int minIndex = Frequency.FrequencyToSpectrumIndex(lower, spectrumData.Length, AudioSettings.outputSampleRate);
                int maxIndex = Frequency.FrequencyToSpectrumIndex(upper, spectrumData.Length, AudioSettings.outputSampleRate);

                var bandPeak = 0.0f;

                for (var fi = minIndex; fi <= maxIndex; fi++)
                {
                    bandPeak = Mathf.Max(bandPeak, spectrumData[fi]);
                }

                switch (peaksProfile.amplitudeMode)
                {
                    case AmplitudeMode.Linear:
                        break;

                    case AmplitudeMode.Decibel:
                        bandPeak = Mathf.Max(Frequency.LinearToVelicityDecibel01(bandPeak), 0.0f);
                        break;
                }

                levels[bi] = bandPeak;

                // comenting this out for now until I figure out the math involved in creating sweep peaks
                //if (_recordProfile)
                {
                    peakLevels[bi] = Mathf.Max(bandPeak, peakLevels[bi]);
                    _peakLevel = Mathf.Max(bandPeak, peakLevel);
                }
            }

            // normalize levels
            for (var bi = 0; bi < bands; bi++)
            {
                switch (normalizeMode)
                {
                    case NormalizeMode.Raw:
                        normalizedLevels[bi] = levels[bi];
                        break;
                    case NormalizeMode.PeakBand:
                        normalizedLevels[bi] = levels[bi] / peakLevels[bi];
                        break;
                    case NormalizeMode.Peak:
                        normalizedLevels[bi] = levels[bi] / peakLevel;
                        break;
                }
            }
        }
        #endregion

        #region Developer
#if UNITY_EDITOR
        /// <summary>
        /// Write frequencies to a file. Use this if you'd like to add your own custom frequency range.
        /// </summary>
        void Awake()
        {
            //WriteOctaveBands(2);
            //WritePianoOctave();
        }

        float A440(int n)
        {
            return Mathf.Pow(2, (float)(n - 49) / 12.0f) * 440.0f;
        }

        void WritePianoOctave()
        {
            using (StreamWriter file = new StreamWriter("d:/Piano.txt"))
            {
                for (int i = -8; i < 120; i++)
                {
                    file.Write(A440(i) + "f, ");
                }

                file.Close();
            }
        }

        void WriteOctaveBands(float octaves)
        {
            float centerFreq = Frequency.LowestAudibleFrequency;
            float octave = Mathf.Pow(2, 1f / octaves);

            using (StreamWriter file = new StreamWriter("d:/Octaves.txt"))
            {
                file.Write(centerFreq + "f, ");

                while (centerFreq < 20000)
                {
                    centerFreq *= octave;

                    file.Write(centerFreq + "f, ");
                }

                file.Close();
            }
        }
#endif
        #endregion
    }
}