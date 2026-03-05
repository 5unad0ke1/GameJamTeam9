using TMPro;
using UnityEngine;

/// <summary>
/// 選択肢をクリックしたときの処理を担当するクラス。
/// </summary>
public class OptionClick : MonoBehaviour
{
    [SerializeField] private RendaGameManager _rendaGameManager;
    [SerializeField] private RendaUIManager _rendaUIManager;
    [SerializeField] private OptionController _optionController;

    [Header("選択肢のテキスト")]
    [SerializeField] private TextMeshProUGUI _optionText;

    [Header("選択肢の名前")]
    [SerializeField] private string _optionName;

    [Header("連打の難易度を変更する値")]
    [SerializeField, Tooltip("相手の押付力")]
    private float _oppPower;
    [SerializeField, Tooltip("難易度によるスコア倍率")]
    private float _diffcultyScoreScale;

    public static bool Selected { get; private set; } = false;

    private void Awake()
    {
        if (_rendaGameManager == null)
        {
            Debug.LogError("RendaGameManagerが設定されていません。");
        }

        if (_rendaUIManager == null)
        {
            Debug.LogError("RendaUIManagerが設定されていません。");
        }

        if (_optionController == null)
        {
            Debug.LogError("OptionControllerが設定されていません。");
        }

        if (_optionText != null)
        {
            _optionText.text = _optionName;
        }
    }

    /// <summary>
    /// 選択肢画面を終了する。
    /// </summary>
    public void OnClick()
    {
        _optionController.CancelTimeout();
        _optionController.HideOptions();

        _rendaGameManager.SetDifficultyParams(_oppPower, _diffcultyScoreScale);
        _rendaUIManager.UpdateSelectedName(_optionName);
        Selected = true;
    }

    public static void ResetSelection()
    {
        Selected = false;
    }
}
