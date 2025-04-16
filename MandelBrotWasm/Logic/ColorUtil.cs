﻿using SixLabors.ImageSharp.ColorSpaces;

namespace MandelBrotWasm.Logic
{
    public static class ColorUtil
    {
        public static Rgb ToRgb(this Hsv hsv)
        {
            double h = hsv.H;
            double s = hsv.S;
            double v = hsv.V;

            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r, g, b;

            if (h >= 0 && h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return new Rgb((float)r, (float)g, (float)b);
        }
    }
}
