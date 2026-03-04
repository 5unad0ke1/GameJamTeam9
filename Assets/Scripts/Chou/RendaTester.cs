using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RendaTester : MonoBehaviour
{
    [Header("各種参照")]
    [SerializeField, Tooltip("連打システム")]
    private RendaController _rendaController;
    [SerializeField, Tooltip("キーボード叩きシステム")]
    private KeyboardSmashController _smashController;
    [SerializeField, Tooltip("プレイヤーのSprite")]
    private RectTransform _playerImageTransform;
    [SerializeField, Tooltip("相手のSprite")]
    private RectTransform _oppImageTransform;
    [SerializeField, Tooltip("UI文字：連打回数")]
    private Text _pressedCountValueText;
    [SerializeField, Tooltip("UI文字：押付度")]
    private Text _vsValueText;
    [SerializeField, Tooltip("UI：開始パネル")]
    private GameObject _uiStartPanel;
    [SerializeField, Tooltip("UI：パァン！")]
    private GameObject _uiBang;
    [SerializeField, Tooltip("UI文字：キーボード叩き倍率")]
    private Text _uiSmashRateText;
    [SerializeField, Tooltip("UI：終了パネル")]
    private GameObject _uiEndPanel;
    [SerializeField, Tooltip("UI文字：得点")]
    private Text _scoreText;
    [SerializeField, Tooltip("UI文字：結果パネル得点")]
    private Text _resultScoreText;
    [SerializeField, Tooltip("UI文字：残り時間")]
    private Text _timerText;
    [SerializeField, Tooltip("UI：押しつけ")]
    private GameObject _turnTextPlayer;
    [SerializeField, Tooltip("UI：反論")]
    private GameObject _turnTextOpp;
    [SerializeField, Tooltip("画像：「ポテトはなぁ…」")]
    private Image _imgPotatoMustBe;
    [SerializeField, Tooltip("画像：叩け")]
    private Image _imgSmashKeyboard;
    [SerializeField, Tooltip("画像：「石油だろうがーー！」")]
    private Image _imgWithOil;

    [Header("連打システム初期化用")]
    [SerializeField, Tooltip("プレイヤーの押付力")]
    private float _playerPower = 0.5f;
    [SerializeField, Tooltip("相手の押付力")]
    private float _oppPower = 1f;
    [SerializeField, Tooltip("時間制限（秒）")]
    private float _timerMax = 10;
    [SerializeField, Tooltip("反論の切替間隔（秒）")]
    private float _turnTime = 3;
    [SerializeField, Tooltip("キーボード叩きによるスコア倍率")]
    private float _smashScoreScale;
    [SerializeField, Tooltip("難易度によるスコア倍率")]
    private float _diffcultyScoreScale;

    [Header("UI更新用")]
    [SerializeField, Tooltip("プレイヤー画像最小拡大倍率")]
    private float _playerScaleMin = 0.3f;
    [SerializeField, Tooltip("プレイヤー画像最大拡大倍率")]
    private float _playerScaleMax = 1.3f;
    [SerializeField, Tooltip("相手画像最小拡大倍率")]
    private float _oppScaleMin = 0.3f;
    [SerializeField, Tooltip("相手画像最大拡大倍率")]
    private float _oppScaleMax = 1.3f;

    private bool _started = false;
    private bool _smashFlg = false;

    #region ライフサイクル


    private void Start()
    {
        // 反論発生する時の処理を連打システムのデリゲートに登録する。例えばUI変化や演出など
        _rendaController.OnTurnSwitch += OnTurnSwitch;
        // 連打フェーズ終了時の処理を連打システムのデリゲートに登録する。例えばUI変化や演出など
        _rendaController.OnRendaEnd += OnRendaEnd;

        // キーボード叩きの受付が終わった時の処理をキーボード叩きシステムのデリゲートに登録する
        _smashController.OnSmashFinished += OnKeyboardSmashFinished;

        StartSequenceTask().Forget();
    }

    private void FixedUpdate()
    {
        if (!_started) return;
        UpdateUi();
        UpdateImageScale();
        UpdateUiTimer();
        UpdateScore();
    }
    #endregion

    #region Privateメソッド
    private void UpdateUi()
    {
        // このように、連打システムから値を取得して、UI更新などに使う。以下同様。
        _pressedCountValueText.text = _rendaController.PressCount.ToString();
        _vsValueText.text = _rendaController.VsValue.ToString();
    }
    private void UpdateUiTimer()
    {
        _timerText.text = _rendaController.Timer.ToString("0.0");
    }

    private void UpdateScore()
    {
        _scoreText.text = _rendaController.Score.ToString("0");
    }
    private void UpdateImageScale()
    {
        _playerImageTransform.localScale = Vector3.one * GetPlayerImageScale();
        _oppImageTransform.localScale = Vector3.one * GetOppImageScale();
    }

    private float GetPlayerImageScale()
    {
        float scaleValue = _rendaController.VsValue / _rendaController.VsValueMax;
        return _playerScaleMin + (_playerScaleMax - _playerScaleMin) * scaleValue;
    }

    private float GetOppImageScale()
    {
        float scaleValue = _rendaController.VsValue / _rendaController.VsValueMax;
        return _oppScaleMax - (_oppScaleMax - _oppScaleMin) * scaleValue;
    }

    private void OnTurnSwitch()
    {
        if (_rendaController.Turn == EnumTurn.Player)
        {
            _turnTextPlayer.SetActive(true);
            _turnTextOpp.SetActive(false);
        }
        else
        {
            _turnTextPlayer.SetActive(false);
            _turnTextOpp.SetActive(true);
        }
    }

    private void OnRendaEnd()
    {
        _uiEndPanel.SetActive(true);
        _resultScoreText.text = _rendaController.Score.ToString("0");
    }

    private void OnKeyboardSmashFinished()
    {
        _smashFlg = true;
    }

    private async UniTask StartSequenceTask()
    {
        // 「ポテトはーー」
        _imgPotatoMustBe.gameObject.SetActive(true);
        await UniTask.Delay(1500);
        _imgPotatoMustBe.gameObject.SetActive(false);
        
        // 「キーボードを叩け」
        _imgSmashKeyboard.gameObject.SetActive(true);
        // キーボード叩きシステムの初期化。キーボード叩き受付開始
        _smashController.Init();
        
        // 入力を待つ
        await UniTask.WaitUntil(() => _smashFlg);
        
        // 叩い後
        _imgSmashKeyboard.gameObject.SetActive(false);
        
        // 「パァン！」倍率表示
        _uiBang.SetActive(true);
        _smashScoreScale = _smashController.SmashScoreRate;
        _uiSmashRateText.text = "スコア倍率\n×" + _smashScoreScale.ToString("0.00") + "！！";
        await UniTask.Delay(3000);
        _uiBang.SetActive(false);
        
        // 「石油だろうがーー！」
        _imgWithOil.gameObject.SetActive(true);
        await UniTask.Delay(1500);
        _uiStartPanel.gameObject.SetActive(false);

        // 連打システムを初期化する処理、連打フェーズ開始
        RendaInitParams initParams = new RendaInitParams(_timerMax, _turnTime, _playerPower, _oppPower, _smashScoreScale, _diffcultyScoreScale);
        _rendaController.Init(initParams);
        _rendaController.RendaStart();
        _started = true;
    }
    #endregion
}
