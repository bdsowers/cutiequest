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
}
