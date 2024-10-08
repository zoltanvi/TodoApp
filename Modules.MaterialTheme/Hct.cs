﻿// See: https://github.com/albi005/MaterialColorUtilities

namespace Modules.MaterialTheme;

/// <summary>
/// HCT: hue, chroma, and tone. A color system that provides a perceptually
/// accurate color measurement system that can also accurately render what
/// colors will appear as in different lighting environments.
/// </summary>
public class Hct
{
    private double hue;
    private double chroma;
    private double tone;
    private uint argb;

    /// <summary>
    /// Create an HCT color from hue, chroma, and tone.
    /// </summary>
    /// <param name="hue">0 ≤ hue &lt; 360; invalid values are corrected.</param>
    /// <param name="chroma">
    /// 0 ≤ chroma &lt; ?; Informally, colorfulness. The color returned may be lower than
    /// the requested chroma. Chroma has a different maximum for any given hue and tone.
    /// </param>
    /// <param name="tone">0 ≤ tone ≤ 100; invalid values are corrected.</param>
    /// <returns>HCT representation of a color in default viewing conditions.</returns>
    public static Hct From(double hue, double chroma, double tone)
    {
        uint argb = CamSolver.SolveToInt(hue, chroma, tone);
        return new Hct(argb);
    }

    /// <summary>
    /// Create an HCT color from a color.
    /// </summary>
    /// <param name="argb">ARGB representation of a color</param>
    /// <returns>HCT representation of a color in default viewing conditions</returns>
    public static Hct FromInt(uint argb) => new Hct(argb);

    private Hct(uint argb)
    {
        SetInternalState(argb);
    }

    /// <summary>
    /// The hue of this color.
    /// </summary>
    /// <remarks>
    /// 0 ≤ Hue &lt; 360; invalid values are corrected.<br/>
    /// After setting hue, the color is mapped from HCT to the more
    /// limited sRGB gamut for display. This will change its ARGB/integer
    /// representation. If the HCT color is outside of the sRGB gamut, chroma
    /// will decrease until it is inside the gamut.
    /// </remarks>
    public double Hue
    {
        get => hue;
        set => SetInternalState(CamSolver.SolveToInt(value, chroma, tone));
    }

    /// <summary>
    /// The chroma of this color.
    /// </summary>
    /// <remarks>
    /// 0 ≤ chroma &lt; ?<br/>
    /// After setting chroma, the color is mapped from HCT to the more
    /// limited sRGB gamut for display. This will change its ARGB/integer
    /// representation. If the HCT color is outside of the sRGB gamut, chroma
    /// will decrease until it is inside the gamut.
    /// </remarks>
    public double Chroma
    {
        get => chroma;
        set => SetInternalState(CamSolver.SolveToInt(hue, value, tone));
    }

    /// <summary>
    /// Lightness. Ranges from 0 to 100.
    /// </summary>
    /// <remarks>
    /// 0 ≤ tone ≤ 100; invalid values are corrected.<br/>
    /// After setting tone, the color is mapped from HCT to the more
    /// limited sRGB gamut for display. This will change its ARGB/integer
    /// representation. If the HCT color is outside of the sRGB gamut, chroma
    /// will decrease until it is inside the gamut.
    /// </remarks>
    public double Tone
    {
        get => tone;
        set => SetInternalState(CamSolver.SolveToInt(hue, chroma, value));
    }

    public uint ToInt() => argb;

    private void SetInternalState(uint argb)
    {
        this.argb = argb;
        Cam16 cam = Cam16.FromInt(argb);
        hue = cam.Hue;
        chroma = cam.Chroma;
        tone = ColorUtils.LStarFromArgb(argb);
    }
}