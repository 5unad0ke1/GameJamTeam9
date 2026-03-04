using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RendaController : MonoBehaviour
{
    [Header("レベルデザイン調整用")]
    [SerializeField, Tooltip("反論時の押付力減少倍率")]
    private float _counterPowerScale = 1.25f;

    [Header("監視用。値は初期化処理によって設定される")]
    [SerializeField, Tooltip("プレイヤーの押付力")]
    private float _playerPower = 0.5f;
    [SerializeField, Tooltip("相手の押付力基礎値")]
    private float _oppPowerBase = 1f;
    [SerializeField, Tooltip("時間制限（秒）")]
    private float _timerMax = 10;
    [SerializeField, Tooltip("反論の切替間隔（秒）")]
    private float _turnTime = 3;
    [SerializeField, Tooltip("キーボード叩きによるスコア倍率")]
    private float _smashScoreScale;
    [SerializeField, Tooltip("難易度によるスコア倍率")]
    private float _diffcultyScoreScale;

    private RendaActions _input;
    private float _timer; // タイマー
    private float _turnTimer; // 反論切替タイマー
    private bool _timerFlg; // タイマー有効フラグ
    private EnumTurn _turn; // 押しつけ／反論
    private int _pressCount = 0; // ボタン押下の総回数
    private float _oppPower; // 相手の押付力
    private float _vsValue; // 押付度
    private float _vsValueInit = 100; // 押付度の初期値
    private float _vsValueMin = 0; // 押付度の最小値
    private float _vsValueMax = 200; // 押付度の最大値
    private float _score = 0; // スコア
    private float _scoreScale = 1; // スコア倍率


    /// <summary>タイマー</summary>
    public float Timer => _timer;
    /// <summary>押しつけ／反論の切替を表す</summary>
    public EnumTurn Turn => _turn;
    /// <summary>ボタン押下合計数</summary>
    public float PressCount => _pressCount;
    /// <summary>押付度</summary>
    public float VsValue => _vsValue;
    /// <summary>押付度の初期値</summary>
    public float VsValueInit => _vsValueInit;
    /// <summary>押付度の最小値</summary>
    public float VsValueMin => _vsValueMin;
    /// <summary>押付度の最大値</summary>
    public float VsValueMax => _vsValueMax;
    /// <summary>スコア</summary>
    public float Score => _score;

    /// <summary>ボタン押下時のデリゲート</summary>
    public event Action OnKeyPressed;
    /// <summary>連打フェーズ終了時のデリゲート</summary>
    public event Action OnRendaEnd;
    /// <summary>反論切替時のデリゲート</summary>
    public event Action OnTurnSwitch;

    #region ライフサイクル
    private void Awake()
    {
        _input = new RendaActions();
        _input.RendaMap.RendaPress1.performed += KeyPressed;
        _input.RendaMap.RendaPress2.performed += KeyPressed;
    }
    private void OnDisable()
    {
        _input.RendaMap.RendaPress1.performed -= KeyPressed;
        _input.RendaMap.RendaPress2.performed -= KeyPressed;
    }
    private void OnDestroy()
    {
        _input.RendaMap.RendaPress1.performed -= KeyPressed;
        _input.RendaMap.RendaPress2.performed -= KeyPressed;
    }
    #endregion
    /// <summary>
    ///     連打管理を初期化する
    /// </summary>
    /// <param name="values"></param>
    public void Init(RendaInitParams values)
    {
        _timerMax = values.timerMax;
        _turnTime = values.turnTime;
        _playerPower = values.playerPower;
        _oppPowerBase = values.oppPower;
        _oppPower = _oppPowerBase;
        _smashScoreScale = values.smashScoreScale;
        _diffcultyScoreScale = values.difficultyScoreScale;
        _scoreScale = _smashScoreScale * _diffcultyScoreScale;
        _vsValue = 100;
    }
    /// <summary>
    ///     連打フェーズを開始する
    /// </summary>
    public void RendaStart()
    {
        _timer = _timerMax;
        _turnTimer = 0;
        _timerFlg = true;
        _input.Enable();
    }
    /// <summary>
    ///     連打フェースを終了する
    /// </summary>
    public void RendaEnd()
    {
        _input.Disable();
        _timerFlg = false;
        OnRendaEnd.Invoke();
    }

    private void Update()
    {
        if (!_timerFlg) return;
        UpdateTimer();
        UpdateVsValue();
    }
    private void UpdateTimer()
    {
        if(_timer <= 0)
        {
            RendaEnd();
            return;
        }
        // 反論時間が来た場合
        if(_turnTimer > _turnTime)
        {
            SwitchTurn();
            _turnTimer = 0;
        }
        else
        {
            _turnTimer += Time.deltaTime;
        }
        _timer -= Time.deltaTime;
    }

    private void KeyPressed(InputAction.CallbackContext ctx)
    {
        _pressCount++;
        _vsValue += _playerPower;
        _score += _vsValue * _scoreScale;
        OnKeyPressed?.Invoke();
    }

    private void UpdateVsValue()
    {
        _vsValue -= _oppPower * Time.deltaTime;
        if (_vsValue < _vsValueMin) _vsValue = _vsValueMin;
        if (_vsValue > _vsValueMax) _vsValue = _vsValueMax;
    }
    private void SwitchTurn()
    {
        if (_turn == EnumTurn.Player)
        {
            _turn = EnumTurn.Opponent;
            _oppPower = _oppPowerBase * _counterPowerScale;
        }
        else
        {
            _turn = EnumTurn.Player;
            _oppPower = _oppPowerBase;
        }
        OnTurnSwitch?.Invoke();
    }
}
