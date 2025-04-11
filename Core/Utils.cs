namespace Dimworld;

using System;
using Godot;

public class Utils
{

    public static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static Color Lerp(Color a, Color b, float t)
    {
        return new Color(
            Lerp(a.R, b.R, t),
            Lerp(a.G, b.G, t),
            Lerp(a.B, b.B, t),
            Lerp(a.A, b.A, t)
        );
    }

}