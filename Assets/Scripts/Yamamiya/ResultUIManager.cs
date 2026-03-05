using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField, Tooltip("リザルトのレシート")]
    private RectTransform _resultRect;

    [Header("リザルト背景Image")]
    [Tooltip("クリア時の背景Image")]
    [SerializeField] private Image _clearImage;
    [Tooltip("ゲームオーバー時の背景Image")]
    [SerializeField] private Image _gameOverImage;

    [Header("テキスト")]
    [Tooltip("クリア/ゲームオーバーメッセージText")]
    [SerializeField] private TMP_Text _messageText;
    [Tooltip("スコア表示Text")]
    [SerializeField] private TMP_Text _scoreTmpText;
    [Tooltip("ランク表示Text")]
    [SerializeField] private TMP_Text _rankTmpText;
    [Tooltip("連打数表示Text")]
    [SerializeField] private TMP_Text _tapCountTmpText;
    [Tooltip("連打速度表示Text")]
    [SerializeField] private TMP_Text _tapSpeedTmpText;

    [Header("テキストアニメーション設定")]
    [Tooltip("クリアメッセージのアニメーション時間")]
    [SerializeField] private float _clearMessageSpeed = 1f;
    [Tooltip("ゲームオーバーメッセージのアニメーション時間")]
    [SerializeField] private float _gameOverMessageSpeed = 1f;

    [Header("SE")]
    [Tooltip("メッセージ表示時の効果音")]
    [SerializeField] private AudioSource _messageAudioSource;
    [SerializeField] private AudioClip _messageSE;

    [Header("フェード")]
    [Tooltip("フェード用のImage")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.7f;

    [SerializeField, Tooltip("リザルトレシートの表示にかかる時間")]
    private float _resultDisplayTime = 2f;

    private MotionHandle _messageHandle;
    private MotionHandle _fadeHandle;
    private MotionHandle _resultHandle;

    private void Awake()
    {
        ValidateComponents();
    }

    private void OnDestroy()
    {
        _messageHandle.TryCancel();
        _fadeHandle.TryCancel();
    }

    /// <summary>
    /// コンポーネントの検証を行います。
    /// </summary>
    private void ValidateComponents()
    {
        if (_clearImage == null) Debug.LogError("[ResultView] ClearImage が未設定です。");
        if (_gameOverImage == null) Debug.LogError("[ResultView] GameOverImage が未設定です。");
        if (_messageText == null) Debug.LogError("[ResultView] MessageText が未設定です。");
        if (_fadeImage == null) Debug.LogWarning("[ResultView] FadeImage が未設定です。フェードはスキップされます。");
    }

    /// <summary>
    /// フェードアウト（黒幕 → 透明）を再生します。
    /// </summary>
    public async UniTask PlayFadeOutAsync()
    {
        if (_fadeImage == null) return;

        _fadeHandle = LMotion.Create(1f, 0f, _fadeDuration)
            .WithDelay(0.5f)
            .WithEase(Ease.OutCirc)
            .BindToColorA(_fadeImage);

        await _fadeHandle;
    }

    /// <summary>
    /// クリア / ゲームオーバーに応じた背景Imageを表示します。
    /// </summary>
    public void ShowResultImage(bool isClear)
    {
        _clearImage.gameObject.SetActive(isClear);
        _gameOverImage.gameObject.SetActive(!isClear);
    }

    /// <summary>
    /// リザルトレシートの表示アニメーションを再生します。
    /// </summary>
    public void PlayReultAnim()
    {
        _resultHandle.TryCancel();
        _resultRect.gameObject.SetActive(true);

        _resultHandle = LMotion.Create(_resultRect.anchoredPosition.y, 0f, _resultDisplayTime)
            .WithEase(Ease.OutQuad)
            .BindToAnchoredPositionY(_resultRect);
    }

    /// <summary>
    /// スコア情報をUIに反映します。
    /// </summary>
    public void SetScoreTexts(float score, string rank, float tapCount, float tapSpeed)
    {
        _scoreTmpText.text = "スコア:" + ((int)score).ToString("0");
        _rankTmpText.text = "ランク:" + rank;
        _tapCountTmpText.text = "連打数:" + ((int)tapCount).ToString("0");
        _tapSpeedTmpText.text = "連打速度" + tapSpeed;
    }

    /// <summary>
    /// クリア時のメッセージアニメーションを再生します。
    /// </summary>
    public void PlayClearMessageAnim(string message)
    {
        _messageHandle.TryCancel();
        _messageText.alignment = TextAlignmentOptions.Left;

        int previousLength = 0;
        _messageHandle = LMotion.String.Create64Bytes(string.Empty, message, _clearMessageSpeed)
            .Bind(text =>
            {
                int currentLength = text.Length;
                if (currentLength > previousLength)
                {
                    _messageAudioSource.PlayOneShot(_messageSE);
                    previousLength = currentLength;
                }
                _messageText.text = text.ToString();
            });
    }

    /// <summary>
    /// ゲームオーバー時のメッセージアニメーションを再生します。
    /// </summary>
    public void PlayGameOverMessageAnim(string message)
    {
        _messageHandle.TryCancel();
        _messageText.alignment = TextAlignmentOptions.Center;

        int previousLength = 0;
        _messageHandle = LMotion.String.Create64Bytes(string.Empty, message, _gameOverMessageSpeed)
            .Bind(text =>
            {
                int currentLength = text.Length;
                if (currentLength > previousLength)
                {
                    _messageAudioSource.PlayOneShot(_messageSE);
                    previousLength = currentLength;
                }
                _messageText.text = text.ToString();
            });
    }
}
