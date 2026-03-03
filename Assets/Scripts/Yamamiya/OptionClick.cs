using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 選択肢をクリックしたときの処理を担当するクラス。
/// </summary>
public class OptionClick : MonoBehaviour
{
    private RendaController _rendaController;
    private OptionView _optionView;

    [SerializeField] private GameObject _rendaPanel;

    [Header("選択肢のテキスト")]
    [SerializeField] private Text _optionText;

    [Header("選択肢の名前")]
    [SerializeField] private string _optionName;
    [Header("連打の難易度を変更する値")]
    [SerializeField, Min(0f)] private float _value;

    private void Awake()
    {
        if(_rendaController == null)
        {
            _rendaController = FindAnyObjectByType<RendaController>();
        }
        if(_optionView == null)
        {
            _optionView = FindAnyObjectByType<OptionView>();
        }

        _optionText.text = _optionName;
    }

    /// <summary>
    /// 連打ゲームに移行する。
    /// </summary>
    public void OnClick()
    {
        _optionView.HideOptions();
        _optionView.HidePanel();
        _rendaPanel.SetActive(true);
        _rendaController.RendaStart();
    }
}
