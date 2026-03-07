using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RendaUIManager : MonoBehaviour
{
    [SerializeField] private RapidFireChallenge _rapidFireChallenge;



    [SerializeField, Tooltip("連打システム")]
    private RendaController _rendaController;


    [SerializeField, Tooltip("メッセージウィンドウ")]
    private Image _imgSmashKeyboard;
    [SerializeField, Tooltip("フェードイン用")]
    private Image _fade;

    [SerializeField, Tooltip("連打パネル")]
    private GameObject _uiRendaPanel;
    [SerializeField, Tooltip("パァン！")]
    private GameObject _uiBang;
    [SerializeField, Tooltip("押し付け")]
    private GameObject _turnPlayer;
    [SerializeField, Tooltip("反論")]
    private GameObject _turnOpp;
    [SerializeField, Tooltip("プレイヤーの主張")]
    private GameObject _playerAssertion;
    [SerializeField, Tooltip("連打ゲーム開始のUI")]
    private GameObject _rendaGameStartUI;

    [SerializeField, Tooltip("得点")]
    private TextMeshProUGUI _scoreText;
    [SerializeField, Tooltip("残り時間")]
    private TextMeshProUGUI _timerText;
    [SerializeField, Tooltip("キーボード叩き倍率")]
    private TextMeshProUGUI _uiSmashRateText;
    [SerializeField, Tooltip("選択肢で選択した物")]
    private TextMeshProUGUI[] _uiSelectedNameText;
    [SerializeField, Tooltip("連打ゲーム開始のテキスト")]
    private TextMeshProUGUI _gameStartEffectText;

    [Header("UI更新用")]
    [SerializeField, Tooltip("プレイヤー画像最小拡大倍率")]
    private float _playerScaleMin = 0.3f;
    [SerializeField, Tooltip("プレイヤー画像最大拡大倍率")]
    private float _playerScaleMax = 1.3f;
    [SerializeField, Tooltip("相手画像最小拡大倍率")]
    private float _oppScaleMin = 0.3f;
    [SerializeField, Tooltip("相手画像最大拡大倍率")]
    private float _oppScaleMax = 1.3f;
    [SerializeField, Tooltip("キーボードチャタリング指示Pulse倍率")]
    private float _imgSmashPulseScale = 1.3f;
    [SerializeField, Tooltip("キーボードチャタリング指示縮小倍率")]
    private float _imgSmashZoomOutScale = 0.95f;
    [SerializeField, Tooltip("連打ゲーム開始のテキスト")]
    private string _gameStartEffect = "押・付・開・始";

    [Header("各LitMotionの設定")]
    [SerializeField, Tooltip("プレイヤーの主張の拡大アニメーションの時間")]
    private float _playerAssertionDuration = 0.5f;
    [SerializeField, Tooltip("プレイヤーの主張の拡大アニメーションのイージング")]
    private Ease _playerAssertionEase;
    [SerializeField, Tooltip("連打ゲーム開始のテキストアニメーションの時間")]
    private float _textAnimationDuration = 1.5f;
    [SerializeField, Tooltip("連打ゲーム開始のテキストアニメーションのイージング")]
    private Ease _textAnimationEase;

    private MotionHandle _playerAssertionHandle;
    private MotionHandle _textAnimationHandle;
    private MotionHandle _fadeHandle;

    private void Start()
    {
        _uiRendaPanel.SetActive(false);
    }
    private void Update()
    {
        if (_imgSmashKeyboard.gameObject.activeInHierarchy)
        {
            ZoomOutImgSmashKeyboard();
        }


        _rapidFireChallenge.Range = (_rendaController.VsValue / _rendaController.VsValueMax) * 2 - 1;
    }

    private void OnDestroy()
    {
        _playerAssertionHandle.TryCancel();
        _textAnimationHandle.TryCancel();
        _fadeHandle.TryCancel();
    }

    public void ShowBangPanel() => _uiBang.SetActive(true);
    public void HideBangPanel() => _uiBang.SetActive(false);
    public void ShowImgSmashKeyboard() => _imgSmashKeyboard.gameObject.SetActive(true);
    public void HideImgSmashKeyboard() => _imgSmashKeyboard.gameObject.SetActive(false);

    public async UniTask ShowPlayerAssertion()
    {
        _playerAssertionHandle.TryCancel();
        _playerAssertion.SetActive(true);
        _playerAssertion.transform.localScale = Vector3.zero;

        Vector3 playerAssertionScale = _playerAssertion.gameObject.transform.localScale;
        var handle = LMotion.Create(playerAssertionScale, Vector3.one, _playerAssertionDuration)
            .WithEase(_playerAssertionEase)
            .BindToLocalScale(_playerAssertion.gameObject.transform);

        _playerAssertionHandle = handle;
        await handle;
    }

    /// <summary>
    /// 連打ゲームが開始するときのエフェクトを表示
    /// </summary>
    public void ShowRendaGameStartEffect()
    {
        _textAnimationHandle.TryCancel();

        _rendaGameStartUI.SetActive(true);
        _gameStartEffectText.text = "";

        int pairCount = (_gameStartEffect.Length + 1) / 2;
        int maxShownPairs = 0;

        _textAnimationHandle = LMotion.Create(0, pairCount, _textAnimationDuration)
            .WithEase(_textAnimationEase)
            .Bind(value =>
            {
                maxShownPairs = Mathf.Max(maxShownPairs, value);
                int charCount = Mathf.Min(maxShownPairs * 2, _gameStartEffect.Length);
                _gameStartEffectText.text = _gameStartEffect[..charCount];
            })
            .AddTo(this);
    }
    public void HideBubbleEffect() => _playerAssertion.SetActive(false);

    public void HideRendaGameStartEffect() => _rendaGameStartUI.SetActive(false);

    public void ShowRendaPanel() => _uiRendaPanel.SetActive(true);

    /// <summary>
    /// 黒背景をフェードインさせる。
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeIn()
    {
        _fadeHandle.TryCancel();
        _fade.gameObject.SetActive(true);
        _fadeHandle = LMotion.Create(0f, 1f, 0.7f)
            .WithEase(Ease.OutCirc)
            .BindToColorA(_fade);

        await _fadeHandle;
    }

    /// <summary>
    /// タイマーテキストの更新
    /// </summary>
    public void UpdateUiTimer()
    {
        _timerText.text = _rendaController.Timer.ToString("0");
    }

    /// <summary>
    /// スコアテキストの更新
    /// </summary>
    public void UpdateScore()
    {
        _scoreText.text = _rendaController.Score.ToString("0");
    }

    /// <summary>
    /// 叩き倍率テキストの更新
    /// </summary>
    /// <param name="value"></param>
    public void UpdateSmashRate(float value)
    {
        _uiSmashRateText.text = $"スコア倍率\nx{value.ToString("0.00")}！！";
    }

    /// <summary>
    /// 選択した名前テキストの更新
    /// </summary>
    /// <param name="name"></param>
    public void UpdateSelectedName(string name)
    {
        if (_uiSelectedNameText == null || _uiSelectedNameText.Length == 0)
        {
            return;
        }

        foreach (var item in _uiSelectedNameText)
        {
            if (item == null)
                continue;
            item.text = $" どう考えても\n{name} だろうがーー！";
        }
    }

    /// <summary>
    /// キーボードを叩けというポップアップの拡大率を更新
    /// </summary>
    public void UpdateImgSmashKeyBoardScale()
    {
        _imgSmashKeyboard.rectTransform.localScale = Vector2.one * _imgSmashPulseScale;
    }

    /// <summary>
    /// キーボードを叩けというポップアップを徐々に縮小していく
    /// </summary>
    public void ZoomOutImgSmashKeyboard()
    {
        Vector2 imgScale = _imgSmashKeyboard.rectTransform.localScale;
        if (imgScale.x > 1f)
        {
            _imgSmashKeyboard.rectTransform.localScale *= _imgSmashZoomOutScale;
        }
    }
}

