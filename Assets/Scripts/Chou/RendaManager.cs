using System;
using UnityEngine;

public class RendaManager : MonoBehaviour
{
    [SerializeField] private RendaController _rendaController;

    [SerializeField] private float _playerScaleMin = 0.3f;
    [SerializeField] private float _playerScaleMax = 1.3f;
    [SerializeField] private float _oppScaleMin = 0.3f;
    [SerializeField] private float _oppScaleMax = 1.3f;
    [SerializeField] private float _playerPower = 0.5f;
    [SerializeField] private float _oppPower = 1f;
    [SerializeField] private float _counterPowerScale = 1.25f;
    [SerializeField] private float _timerInit = 0;
    [SerializeField] private float _timerMax = 10;
    [SerializeField] private float _turnTime = 3;

    private float _timer;
    private float _turnTimer;
    private bool _timerFlg; // タイマー有効フラグ
    private bool _turnFlg; // 押しつけ／反論フラグ
    private int _pressCount = 0; // ボタン押下の総回数
    private float _vsValue; // 押しつけ値
    private float _vsValueInit = 100; // 押しつけ初期値
    private float _vsValueMin = 0; // 押しつけ最小値
    private float _vsValueMax = 200; // 押しつけ最大値

    public float Timer => _timer;
    public bool TurnFlg => _turnFlg;
    public float PressCount => _pressCount;
    public float VsValue => _vsValue;
    public float VsValueInit => _vsValueInit;
    public float VsValueMin => _vsValueMin;
    public float VsValueMax => _vsValueMax;

    /// <summary>ボタン押下時のデリゲート</summary>
    public event Action OnKeyPressed;
    /// <summary>連打フェーズ終了時のデリゲート</summary>
    public event Action OnRendaEnd;
    /// <summary>反論発生／終了時のデリゲート</summary>
    public event Action OnTurnSwitch;

    /// <summary>
    ///     連打管理を初期化する
    /// </summary>
    /// <param name="values"></param>
    public void Init(RendaInitParams values)
    {
        _timerMax = values.timerMax;
        _playerPower = values.playerPower;
        _oppPower = values.oppPower;
    }
    /// <summary>
    ///     連打フェーズを開始する
    /// </summary>
    public void RendaStart()
    {
        _rendaController.RendaStart();
        _timer = _timerInit;
        _turnTimer = _timerInit;
        _timerFlg = true;
    }
    /// <summary>
    ///     連打フェースを終了する
    /// </summary>
    public void RendaEnd()
    {
        _timerFlg = false;
        OnRendaEnd.Invoke();
    }

    private void Update()
    {
        if (!_timerFlg) return;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if(_timer >= _timerMax)
        {
            RendaEnd();
            return;
        }
        // 反論時間が来た場合
        if(_turnTimer > _turnTime)
        {
            _turnTimer = 0;
            SwitchTurn();
        }
        else
        {
            _turnTimer += Time.deltaTime;
        }
        _timer += Time.deltaTime;
    }

    private void SwitchTurn()
    {
        if (_turnFlg)
        {
            _oppPower *= _oppPower;
        }
        else
        {
            _oppPower /= _oppPower;
        }
        OnTurnSwitch?.Invoke();
    }
}

public readonly struct RendaInitParams
{
    public readonly float timerMax;
    public readonly float playerPower;
    public readonly float oppPower;
}
