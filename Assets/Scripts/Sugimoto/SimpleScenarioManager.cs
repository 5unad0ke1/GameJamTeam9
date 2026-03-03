using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleScenarioManager : MonoBehaviour
{
    [SerializeField] private ScenarioLine[] _scenarioLines;

    [SerializeField] private TextMeshProUGUI _lineText;
    [SerializeField] private Image _lineImage;

    [Header("テキストアニメーションの設定")]
    [SerializeField] private float _textAnimationDuration = 1.5f;

    private int _currentLineIndex = 0;
    private string _currentLineText;
    private bool _isAnimatingText = false;
    private MotionHandle _textAnimationHandler;

    private void Start()
    {
        ShowLine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isAnimatingText)
            {
                SkipTextAnimation();
            }
            else
            {
                NextLine();
            }
        }
    }

    // シナリオの現在の行を表示するメソッド
    public void ShowLine()
    {
        if (_currentLineIndex < _scenarioLines.Length)
        {
            _lineImage.sprite = _scenarioLines[_currentLineIndex].lineImage;
            TextAnimation(_scenarioLines[_currentLineIndex].lineText);

        }
    }

    private void TextAnimation(string text)
    {
        // 文字を1文字ずつ表示するアニメーション
        _currentLineText = text;
        _lineText.text = "";
        _isAnimatingText = true;
        _textAnimationHandler = LMotion.String.Create128Bytes("", text, _textAnimationDuration)
            .WithEase(Ease.Linear)
            .BindToText(_lineText)
            .AddTo(this);
    }

    private void NextLine()
    {
        if (_currentLineIndex < _scenarioLines.Length - 1)
        {
            _currentLineIndex++;
            ShowLine();
        }
    }

    private void SkipTextAnimation()
    {
        _textAnimationHandler.TryComplete();
        _isAnimatingText = false;
    }
}
