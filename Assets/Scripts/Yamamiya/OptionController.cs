using UnityEngine;
using UnityEngine.InputSystem;

public class OptionController : MonoBehaviour
{
    [SerializeField] private OptionView _view;
    [SerializeField] private int _initialSelectedIndex = 0;

    private OptionSelector _selector;

    private void Awake()
    {
        if (_view == null)
        {
            Debug.LogError("OptionViewが設定されていません。");
            return;
        }

        _selector = new OptionSelector(_view.OptionCount, _initialSelectedIndex);
        _selector.OnSelectionChanged += OnSelectionChanged;
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        if (_selector != null)
        {
            _selector.OnSelectionChanged -= OnSelectionChanged;
        }
    }

    /// <summary>
    /// 次の選択肢を選択するメソッド。
    /// </summary>
    public void NextSelectOptions(InputAction.CallbackContext context)
    {
        // ボタンが押されたときのみ処理を行う
        if (!context.performed) return;
        _selector.SelectNext();
    }

    /// <summary>
    /// 前の選択肢を選択するメソッド。
    /// </summary>
    public void PreviousSelectOptions(InputAction.CallbackContext context)
    {
        // ボタンが押されたときのみ処理を行う
        if (!context.performed) return;
        _selector.SelectPrevious();
    }

    /// <summary>
    /// 選択肢画面を表示するメソッド。
    /// </summary>
    public void ActivateOptionPanel() => _view.ShowPanel();

    /// <summary>
    /// 選択肢画面を非表示にするメソッド。
    /// </summary>
    public void DeactivateOptionPanel() => _view.HidePanel();

    /// <summary>
    /// 選択肢を表示するメソッド。
    /// </summary>
    public void ActivateOptions() => _view.ShowOptions();

    /// <summary>
    /// 選択肢を非表示にするメソッド。
    /// </summary>
    public void DeactivateOptions() => _view.HideOptions();

    /// <summary>
    /// 選択が変更されたときの処理。
    /// </summary>
    private void OnSelectionChanged(int index)
    {
        _view.SelectButton(index);
    }
}
