using Cysharp.Threading.Tasks;
using UnityEngine;

public class RendaGameManager : MonoBehaviour
{
    [Header("各種参照")]
    [SerializeField, Tooltip("シナリオマネージャー(Action/Funcの登録)")]
    private SimpleScenarioManager _scenarioManager;

    [SerializeField, Tooltip("連打システム")]
    private RendaController _rendaController;
    [SerializeField, Tooltip("キーボード叩きシステム")]
    private KeyboardSmashController _smashController;
    [SerializeField, Tooltip("連打ゲームのUI")]
    private RendaUIManager _uiManager;

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

    [Header("切り替え時間")]
    [SerializeField, Tooltip("キーボード叩きから連打への切り替え時間（秒）")]
    private float _switchTimeSmashToRenda = 1;

    [SerializeField] private ActionType _smashFilter;

    [Header("シーン遷移先")]
    [SerializeField] private string _resultSceneName = "ResultScene";

    // 選択肢で選択した物の名前
    private string _selectedName;

    private bool _started = false;
    private bool _smashFlg = false;

    private void Start()
    {
        _smashController.OnSmashed += OnSmashed;
        _smashController.OnSmashFinished += OnKeyboardSmashFinished;
        _rendaController.OnRendaEnd += OnRendaEnd;
        _scenarioManager.OnFuncTriggerd.Add(StartSmashSequence);
    }

    private void Update()
    {
        // スタートしていないときは更新処理を行わない
        if (!_started) return;
        _uiManager.UpdateImageScale();
        _uiManager.UpdateUiTimer();
    }

    private void OnDestroy()
    {
        _scenarioManager.OnFuncTriggerd.Remove(StartSmashSequence);
    }

    /// <summary>
    /// 選択肢から難易度パラメータを設定する。
    /// </summary>
    /// <param name="oppPower">相手の押付力</param>
    /// <param name="diffcultyScoreScale">難易度によるスコア倍率</param>
    public void SetDifficultyParams(float oppPower, float diffcultyScoreScale)
    {
        _oppPower = oppPower;
        _diffcultyScoreScale = diffcultyScoreScale;
    }

    public void SetSelectedName(string name)
    {
        _selectedName = name;
    }

    /// <summary>
    /// キーボード叩きシステムを開始するシーケンス。
    /// </summary>
    /// <returns></returns>
    public async UniTask StartSmashSequence(ActionType type)
    {
        if (type != _smashFilter) return;

        _uiManager.ShowImgSmashKeyboard();
        _smashController.Init();

        await UniTask.WaitUntil(() => _smashFlg, cancellationToken: destroyCancellationToken);

        // メッセージウィンドウを消す
        _uiManager.HideImgSmashKeyboard();

        // 「パァン！」倍率表示
        _uiManager.ShowBangPanel();
        _smashScoreScale = _smashController.SmashScoreRate;
        _uiManager.UpdateSmashRate(_smashScoreScale);

        await UniTask.Delay(System.TimeSpan.FromSeconds(_switchTimeSmashToRenda)
            , cancellationToken: destroyCancellationToken);
        _uiManager.HideBangPanel();

        await _uiManager.ShowPlayerAssertion();

        await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: destroyCancellationToken);

        await StartRendaGameSequence();
    }

    /// <summary>
    /// 連打ゲームを開始するシーケンス。
    /// </summary>
    /// <returns></returns>
    public async UniTask StartRendaGameSequence()
    {
        // 画像の表示
        _uiManager.ShowRendaGameStartEffect();
        Debug.Log("何かのキーを押して連打ゲーム開始");
        await UniTask.WaitUntil(() => Input.anyKeyDown, cancellationToken: destroyCancellationToken);
        Debug.Log("連打ゲーム開始");
        _uiManager.HideRendaGameStartEffect();
        _uiManager.ShowRendaPanel();

        RendaInitParams initParams = new RendaInitParams(_timerMax, _turnTime, _playerPower, _oppPower, _smashScoreScale, _diffcultyScoreScale);
        _rendaController.Init(initParams);
        _rendaController.RendaStart();
        OnRendaGameStart();
        _started = true;
    }

    private void OnTurnSwitch()
    {
        if (_rendaController.Turn == EnumTurn.Player)
        {
            // プレイヤーのターンになったときの処理
            // 画像の切り替え
        }
        else
        {
            // 相手のターンになったときの処理
            // 画像の切り替え
            // 反論の台詞を表示
        }

    }

    private void OnRendaEnd()
    {
        // リザルト画面に遷移する処理
        RendaEnd().Forget(); ;
    }

    private async UniTask RendaEnd()
    {
        bool isClear = _rendaController.VsValue > _rendaController.VsValueInit;
        float score = _rendaController.Score;
        float tapCount = _rendaController.PressCount;
        float tapSpeed = _timerMax > 0f ? tapCount / _timerMax : 0f;
        string rank = ResultData.CalculateRank(score);

        ResultData.Set(isClear, score, rank, tapCount, tapSpeed);

        await _uiManager.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene(_resultSceneName);
    }

    private void OnSmashed()
    {
        _uiManager.UpdateImgSmashKeyBoardScale();
    }

    private void OnKeyboardSmashFinished()
    {
        _smashFlg = true;
    }

    private void OnRendaGameStart()
    {
        _started = true;
    }
}
