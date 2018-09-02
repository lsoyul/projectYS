using System;
using UnityEngine;

namespace LDG.SoundReactor
{
    public enum AmplitudeMode
    {
        Linear,
        Decibel
    }

    //[CreateAssetMenu(fileName = "PeakProfile.peaks.asset", menuName = "Peaks Profile", order = 300)]
    public class PeaksProfile : ScriptableObject
    {
        public FFTWindow fftWindow = FFTWindow.Hamming;
        public int fftSamples = 2048;
        public AmplitudeMode amplitudeMode = AmplitudeMode.Linear;

        public float peak;
        public float[] peaks;

        [SerializeField]
        private int hash = 0;

        public bool isDirty
        {
            get
            {
                return hash != GetHashCode();
            }
        }

        public PeaksProfile()
        {
            ResetPeaks();
        }

        public void SetPeaks(float[] peaks, float peak)
        {
            int bands = 30;

            this.peaks = new float[bands];

            for (int i = 0; i < this.peaks.Length; i++)
            {
                this.peaks[i] = Spline.Tween((float)i / (float)(this.peaks.Length - 1), peaks, false, InterpolationMode.Curve);
            }

            this.peak = peak;

            hash = GetHashCode();
        }

        public void GetPeaks(ref float[] peaks, ref float peak)
        {
            if(peaks.Length == 1)
            {
                peak = this.peak;
                return;
            }
            for (int i = 0; i < peaks.Length; i++)
            {
                peaks[i] = Spline.Tween((float)i / (float)(peaks.Length - 1), this.peaks, false, InterpolationMode.Curve);
            }

            peak = this.peak;
        }

        public void ResetPeaks()
        {
            peaks = new float[30];

            for (int i = 0; i < this.peaks.Length; i++)
            {
                peaks[i] = 0.001f;
            }

            peak = 0.001f;

            hash = 0;
        }

        private int StringToHashCode(string s)
        {
            int result = 0;

            foreach(char c in s)
            {
                result += c;
            }

            return result * s.Length;
        }

        // https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
        override public int GetHashCode()
        {
            int hash = 17;

            unchecked
            {
                // DARN YOU GetHashCode FOR NOT RETURNING CONSISTENT VALUES! So I will hash using modified string representations then :P
                hash = hash * 31 + StringToHashCode(fftWindow.ToString());
                hash = hash * 31 + StringToHashCode(fftSamples.ToString());
                hash = hash * 31 + StringToHashCode(amplitudeMode.ToString());
            }

            return hash;
        }
    }
}