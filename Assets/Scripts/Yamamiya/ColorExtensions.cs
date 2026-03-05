using UnityEngine;

public static class ColorExtensions
{
    /// <summary>
    /// 色のアルファ値を変更する拡張メソッド。
    /// </summary>
    /// <param name="color">元の色</param>
    /// <param name="alpha">新しいアルファ値（0から1の範囲）</param>
    /// <returns>アルファ値が変更された新しい色</returns>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
