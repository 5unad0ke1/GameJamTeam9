using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 選択肢UIの表示制御を担当するクラス。
/// </summary>
public class OptionView : MonoBehaviour
{
    [SerializeField] private GameObject _optionPanel;
    [SerializeField] private Button[] _options;

    public int OptionCount => _options?.Length ?? 0;

    private void Awake()
    {
        ValidateComponents();
    }

    /// <summary>
    /// コンポーネントの検証を行います。
    /// </summary>
    private void ValidateComponents()
    {
        if (_options == null || _options.Length == 0)
        {
            Debug.LogError("選択肢のボタンが設定されていません。");
        }

        if (_optionPanel == null)
        {
            Debug.LogError("選択肢のパネルが設定されていません。");
        }
    }

    /// <summary>
    /// パネルを表示します。
    /// </summary>
    public void ShowPanel() => _optionPanel.SetActive(true);

    /// <summary>
    /// パネルを非表示にします。
    /// </summary>
    public void HidePanel() => _optionPanel.SetActive(false);

    /// <summary>
    /// すべての選択肢を表示します。
    /// </summary>
    public void ShowOptions()
    {
        if (_options == null) return;

        foreach (var option in _options)
        {
            option.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// すべての選択肢を非表示にします。
    /// </summary>
    public void HideOptions()
    {
        if (_options == null) return;

        foreach (var option in _options)
        {
            option.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 指定されたインデックスのボタンを選択状態にします。
    /// </summary>
    public void SelectButton(int index)
    {
        if (_options == null || index < 0 || index >= _options.Length)
        {
            return;
        }

        _options[index].Select();
    }
}
