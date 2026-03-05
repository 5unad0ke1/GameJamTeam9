public static class ResultData
{
    public static bool IsClear { get; private set; }
    public static float Score { get; private set; }
    public static string Rank { get; private set; }
    public static float TapCount { get; private set; }
    public static float TapSpeed { get; private set; }

    public static void Set(bool isClear, float score, string rank, float tapCount, float tapSpeed)
    {
        IsClear = isClear;
        Score = score;
        Rank = rank;
        TapCount = tapCount;
        TapSpeed = tapSpeed;
    }

    public static string CalculateRank(float score)
    {
        if (score >= 10000f) return "S";
        if (score >= 7000f) return "A";
        if (score >= 5000f) return "B";
        if (score >= 3000f) return "C";
        return "D";
    }
}
