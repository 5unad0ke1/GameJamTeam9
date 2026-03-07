using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleScenarioManager : MonoBehaviour
{
    [SerializeField] private ScenarioLine[] _scenarioLines;

    [SerializeField, Tooltip("表示名")] private TextMeshProUGUI _NameText;
    [SerializeField, Tooltip("表示テキスト")] private TextMeshProUGUI _lineText;
    [SerializeField, Tooltip("背景イメージ")] private Image _lineImage;

    [Header("テキストアニメーションの設定")]
    [SerializeField, Tooltip("セリフを言い終わる時間")] private float _textAnimationDuration = 1.5f;
    [SerializeField, Tooltip("大文字にする文字のキーワード")] private string[] _upperCaseWords;
    [SerializeField, Tooltip("揺らす文字を入れる配列")] private string[] _ShakeWords;

    [Header("アクションの設定")]
    [Min(1)]
    [SerializeField] private int WaitingTime;

    private int _currentLineIndex = 0;
    private bool _isAnimatingText = false;
    private bool _isCanSkip = false;
    private bool _isNeedInput = false;
    private MotionHandle _textAnimationHandler;
    private MotionHandle _shakeHandle;
    private Vector3[][] _originalVertices;
    private TMP_TextInfo _cachedTextInfo;
    private List<int> _shakeCharIndices = new();

    //セリフのアクション
    public Action<ActionType> OnActionTriggered;
    public List<Func<ActionType, UniTask>> OnFuncTriggerd = new();


    private void Start()
    {
        ShowLine();

    }

    private void Update()
    {
        if (_isAnimatingText && !_textAnimationHandler.IsPlaying())
        {
            _isAnimatingText = false;
            StartShake();
        }

        if (!_isNeedInput)
        {
            if (!_isAnimatingText && _isCanSkip)
                NextLine();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _isCanSkip)
        {
            if (_isAnimatingText)
                SkipTextAnimation();
            else
                NextLine();
        }
    }

    public void ShowLine()
    {
        if (_currentLineIndex >= _scenarioLines.Length) return;

        _NameText.text = _scenarioLines[_currentLineIndex].characterName;

        string text = _scenarioLines[_currentLineIndex].lineText;

        foreach (var word in _upperCaseWords)
            text = text.Replace(word, word.ToUpper());

        _isNeedInput = _scenarioLines[_currentLineIndex].isNeedPush;

        TextAnimation(text, _scenarioLines[_currentLineIndex].isActionExecuted, _scenarioLines[_currentLineIndex].isFuncExecuted).Forget();
    }

    public async UniTask PlayEffect(ActionType type)
    {
        if (type != ActionType.none)
            return;

        await UniTask.Delay(WaitingTime);
    }

    // テキストを1文字ずつ表示するアニメーション
    private async UniTask TextAnimation(string text, bool action, bool func)
    {
        _lineText.text = "";
        _isAnimatingText = true;

        // テキストアニメーションを作成して、テキストにバインドする
        _textAnimationHandler = LMotion.String.Create128Bytes(_lineText.text, text, _textAnimationDuration)
            .WithRichText()
            .WithEase(Ease.Linear)
            .BindToText(_lineText)
            .AddTo(this);

        if (action)
        {
            OnActionTriggered?.Invoke(_scenarioLines[_currentLineIndex].actionType);
        }

        if (func)
        {
            _isCanSkip = false;

            foreach (var item in OnFuncTriggerd)
            {
                if (item == null)
                    continue;
                await item(_scenarioLines[_currentLineIndex].actionType);
            }

        }

        _isCanSkip = true;
    }

    // 次の行に進む
    private void NextLine()
    {


        _shakeHandle.TryComplete();
        _shakeCharIndices.Clear();

        if (_currentLineIndex < _scenarioLines.Length - 1)
        {
            _currentLineIndex++;
            ShowLine();
        }
    }

    // テキストアニメーションをスキップして、すぐに全テキストを表示する
    private void SkipTextAnimation()
    {
        _textAnimationHandler.TryComplete();
        _isAnimatingText = false;
        StartShake();
    }

    // 揺らす文字の頂点を更新するアニメーション
    private void StartShake()
    {
        PrepareShakeData();
        CollectShakeCharacters();

        if (_shakeCharIndices.Count == 0) return;

        //5fの範囲で上下に揺らすアニメーションを作成して、揺らす文字の頂点にバインドする
        _shakeHandle = LMotion.Create(-5f, 5f, 0.5f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(offsetY => ApplyShake(offsetY))
            .AddTo(this);
    }

    // テキストの頂点情報をキャッシュして、揺らす文字の頂点を保存する
    private void PrepareShakeData()
    {
        _lineText.ForceMeshUpdate();
        _cachedTextInfo = _lineText.textInfo;

        _originalVertices = new Vector3[_cachedTextInfo.meshInfo.Length][];

        for (int i = 0; i < _cachedTextInfo.meshInfo.Length; i++)
        {
            _originalVertices[i] =
                _cachedTextInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }
    }

    // 揺らす文字のインデックスを収集する
    private void CollectShakeCharacters()
    {
        _shakeCharIndices.Clear();

        string fullText = _lineText.text;

        foreach (var word in _ShakeWords)
        {
            int startIndex = 0;

            while ((startIndex = fullText.IndexOf(word, startIndex, System.StringComparison.OrdinalIgnoreCase)) != -1)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    _shakeCharIndices.Add(startIndex + i);
                }

                startIndex += word.Length;
            }
        }
    }

    // 指定したオフセットで揺らす文字の頂点を更新する
    private void ApplyShake(float offsetY)
    {
        // 揺らす文字のインデックスをループして処理する
        foreach (int charIndex in _shakeCharIndices)
        {
            if (charIndex >= _cachedTextInfo.characterCount) continue;

            var charInfo = _cachedTextInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            // 文字の頂点インデックスとマテリアルインデックスを取得
            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            var vertices = _cachedTextInfo.meshInfo[materialIndex].vertices;

            // 文字の4つの頂点をオフセットで更新する
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] =
                    _originalVertices[materialIndex][vertexIndex + j] +
                    new Vector3(0, offsetY, 0);
            }

            _cachedTextInfo.meshInfo[materialIndex].mesh.vertices =
                _cachedTextInfo.meshInfo[materialIndex].vertices;

            // メッシュを更新して変更を反映する
            _lineText.UpdateGeometry(
                _cachedTextInfo.meshInfo[materialIndex].mesh,
                materialIndex);
        }
    }
}
