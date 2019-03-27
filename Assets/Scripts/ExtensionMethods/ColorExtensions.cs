using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorExtensions
{
    public static class ColorExtensions
    {
        public static Color ModifiedAlpha(this Color color, float modification)
        {
            return new Color(color.r, color.g, color.b, color.a + modification);
        }
    }

    public class ColorHelper
    {
        public static Color Lerp(Color start, Color end, float t)
        {
            t = Mathf.Clamp(t, 0, 1);

            float r = Mathf.Lerp(start.r, end.r, t);
            float g = Mathf.Lerp(start.g, end.g, t);
            float b = Mathf.Lerp(start.b, end.b, t);
            float a = Mathf.Lerp(start.a, end.a, t);
            
            return new Color(r, g, b, a);
        }
    }
}
