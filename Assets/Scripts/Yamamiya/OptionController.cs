using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private OptionView _view;
    [SerializeField] private int _initialSelectedIndex = 0;
    [SerializeField, Tooltip("選択肢画面の制限時間")] private float _timeLimit = 5f;


    private OptionSelector _selector;
    private CancellationTokenSource _timeOutCts;

    private void Awake()
    {
        if (_view == null)
        {
            Debug.LogError("OptionViewが設定されていません。");
            return;
        }

        _selector = new OptionSelector(_view.OptionCount, _initialSelectedIndex);
        _selector.OnSelectionChanged += OnSelectionChanged;
        _timeOutCts = new CancellationTokenSource();
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
        _timeOutCts?.Cancel();
        _timeOutCts?.Dispose();
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
    public void ActivateOptions()
    {
        // タイムアウトのキャンセルと新しいタイムアウトの開始
        _timeOutCts?.Cancel();
        _timeOutCts?.Dispose();

        _timeOutCts = new CancellationTokenSource();

        _view.ShowOptions();
        _view.SelectButton(_selector.SelectedIndex);
        StartTimeLimit(_timeOutCts.Token).Forget();
    }

    /// <summary>
    /// 選択肢を非表示にするメソッド。
    /// </summary>
    public void DeactivateOptions() => _view.HideOptions();

    /// <summary>
    /// 選択が変更されたときの処理。
    /// </summary>
    private void OnSelectionChanged(int index, int previousIndex)
    {
        _view.SelectButton(index);
        _view.DeselectButton(previousIndex);
    }

    /// <summary>
    /// 選択肢画面の制限時間を開始するメソッド。
    /// </summary>
    private async UniTask StartTimeLimit(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_timeLimit), cancellationToken: token);

            OnTimeout();
        }
        catch (System.OperationCanceledException)
        {
            // タイムアウトがキャンセルされた場合は何もしない
        }
    }

    /// <summary>
    /// タイムアウトしたときにランダムなボタンのクリックイベントを発火させる。
    /// </summary>
    private void OnTimeout()
    {
        int randamIndex = Random.Range(0, _view.OptionCount);
        Button randomButton = _view.GetButton(randamIndex);
        Debug.Log(randomButton.name);

        if(randomButton != null)
        {
            randomButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// プレイヤーが選択したときにタイムアウトをキャンセルする。
    /// </summary>
    public void CancelTimeout()
    {
        _timeOutCts?.Cancel();
        Debug.Log("タイムアウトがキャンセルされました。");
    }
}
