using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [Tooltip("シナリオマネージャー(Action/Funcの登録)")]
    [SerializeField] private SimpleScenarioManager _scenarioManager;

    [SerializeField] private OptionView _view;
    [SerializeField] private int _initialSelectedIndex = 0;
    [SerializeField, Tooltip("選択肢画面の制限時間")] private float _timeLimit = 5f;

    [SerializeField] private ActionType _filterType;


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
        _scenarioManager.OnFuncTriggerd.Add(EndSelectedOptions);

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

        _scenarioManager.OnFuncTriggerd.Remove(EndSelectedOptions);
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
    public void ShowOptionPanel() => _view.ShowPanel();

    /// <summary>
    /// 選択肢画面を非表示にするメソッド。
    /// </summary>
    public void HideOptionPanel() => _view.HidePanel();

    /// <summary>
    /// 選択肢を表示するメソッド。
    /// </summary>
    public void ShowOptions()
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
    public void HideOptions() => _view.HideOptions();

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
            float elapsedTime = 0f;
            _view.UpdateTimerPrrogress(_timeLimit);

            while(elapsedTime < _timeLimit)
            {
                elapsedTime += Time.deltaTime;
                _view.UpdateTimerPrrogress(_timeLimit - elapsedTime);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }

            _view.UpdateTimerPrrogress(0f);
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

        if (randomButton != null)
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

    /// <summary>
    /// 選択肢が選ばれるまで待機する処理。
    /// フィルタリングされているActionTypeと一致するFuncが呼び出されたときのみ待機する。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private async UniTask EndSelectedOptions(ActionType type)
    {
        if (type != _filterType)
        {
            return;
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        ShowOptionPanel();
        ShowOptions();

        await UniTask.WaitUntil(() => OptionClick._selected, cancellationToken: destroyCancellationToken);
    }
}
