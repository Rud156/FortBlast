using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Extras
{
    public static class ExtensionFunctions
    {
        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static Color ConvertAndClampColor(float r = 0, float g = 0, float b = 0, float a = 0)
        {
            return new Color(Mathf.Clamp(r, 0, 255) / 255, Mathf.Clamp(g, 0, 255) / 255, Mathf.Clamp(b, 0, 255) / 255,
                Mathf.Clamp(a, 0, 255) / 255);
        }

        public static float To360Angle(float angle)
        {
            while (angle < 0.0f)
                angle += 360.0f;
            while (angle >= 360.0f)
                angle -= 360.0f;

            return angle;
        }

        public static string FormatSecondsToMinutes(int seconds)
        {
            var minutes = seconds / 60;
            var remainingSeconds = seconds - minutes * 60;

            if (minutes <= 0 && remainingSeconds <= 0)
                return "0 s";

            return remainingSeconds <= 0 ? $"{minutes} M" : $"{minutes} M {remainingSeconds} S";
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            var random = new System.Random();
            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }

            return list;
        }
    }
}