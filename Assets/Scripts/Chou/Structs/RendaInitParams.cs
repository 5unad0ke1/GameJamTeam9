/// <summary>
///     連打システム初期化用のstruct
/// </summary>
public readonly struct RendaInitParams
{
    /// <summary>時間制限</summary>
    public readonly float timerMax;
    /// <summary>反論切替の時間間隔</summary>
    public readonly float turnTime;
    /// <summary>プレイヤーの押付力</summary>
    public readonly float playerPower;
    /// <summary>相手の押付力</summary>
    public readonly float oppPower;
    /// <summary>キーボード叩きによるスコア倍率</summary>
    public readonly float smashScoreScale;
    /// <summary>難易度によるスコア倍率</summary>
    public readonly float difficultyScoreScale;
    public RendaInitParams(float timerMax, float turnTime, float playerPower, float oppPower, float smashScoreScale, float difficultyScoreScale)
    {
        this.timerMax = timerMax;
        this.turnTime = turnTime;
        this.playerPower = playerPower;
        this.oppPower = oppPower;
        this.smashScoreScale = smashScoreScale;
        this.difficultyScoreScale = difficultyScoreScale;
    }
}
