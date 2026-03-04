using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardSmashController : MonoBehaviour
{
    [SerializeField, Range(0f, 5f), Tooltip("キーボード叩きの受付時間（秒）")]
    private float _smashWindow = 2f;
    [SerializeField, Tooltip("スコア倍率最大上昇値")]
    private float _scoreScaleMax = 0.5f;
    [SerializeField, Tooltip("最大倍率に必要な押下数")]
    private int _smashCountForMaxScale = 300;

    private SmashKeyboardActions _smashInput;
    private bool _smashFlg = false;
    private int _smashCount = 0;
    private float _smashScoreRate;

    /// <summary>キーボード叩きが発生した瞬間のデリゲート</summary>
    public event Action OnSmashed;
    /// <summary>キーボード叩きが受付終了後のデリゲート</summary>
    public event Action OnSmashFinished;

    /// <summary>キーボード叩きによるスコア倍率</summary>
    public float SmashScoreRate => _smashScoreRate;

    #region ライフサイクル
    private void Awake()
    {
        _smashInput = new SmashKeyboardActions();
        _smashInput.SmashMap.Smash.performed += KeySmashed;
    }
    private void OnDisable()
    {
        _smashInput.SmashMap.Smash.performed -= KeySmashed;
    }
    private void OnDestroy()
    {
        _smashInput.SmashMap.Smash.performed -= KeySmashed;
    }
    #endregion
    public void Init()
    {
        _smashFlg = false;
        _smashCount = 0;
        _smashInput.Enable();
    }

    private void KeySmashed(InputAction.CallbackContext ctx)
    {
        if (!_smashFlg)
        {
            _smashFlg = true;
            // 一定時間内にキーボードの押下を受け付ける
            Invoke(nameof(CloseSmashWindow), _smashWindow);
        }
        _smashCount++;
        OnSmashed?.Invoke();
    }

    #region Privateメソッド
    /// <summary>
    /// キーボード叩きによるスコア倍率を算出する。
    /// </summary>
    private void CalcSmashScoreRate()
    {
        Debug.Log("SmashCount: " + _smashCount.ToString());
        int keyCount = _smashCount > _smashCountForMaxScale ? _smashCountForMaxScale : _smashCount;
        _smashScoreRate = 1 + _scoreScaleMax * keyCount / _smashCountForMaxScale;
    }
    /// <summary>
    /// キーボード叩きの受付を停止する
    /// </summary>
    private void CloseSmashWindow()
    {
        _smashInput.Disable();
        CalcSmashScoreRate();
        OnSmashed?.Invoke();
        OnSmashFinished?.Invoke();
    }
    #endregion
}
