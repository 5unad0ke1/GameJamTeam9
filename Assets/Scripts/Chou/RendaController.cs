using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class RendaController : MonoBehaviour
{
    
    [SerializeField] private Image _playerImage;
    [SerializeField] private RectTransform _playerImageTransform;
    [SerializeField] private Image _oppImage;
    [SerializeField] private RectTransform _oppImageTransform;
    [SerializeField] private Text _pushedValueText;
    [SerializeField] private Text _vsValueText;
    [SerializeField] private float _playerScaleMin = 0.3f;
    [SerializeField] private float _playerScaleMax = 1.3f;
    [SerializeField] private float _oppScaleMin = 0.3f;
    [SerializeField] private float _oppScaleMax = 1.3f;
    [SerializeField] private float _vsIncreaseAmount = 0.5f;
    [SerializeField] private float _vsDecreaseAmount = 1f;

    private RendaActions _input;
    private bool _rendaFlg = false;
    private int _pushedCount = 0;
    private float _vsValue;
    private float _vsValueInit = 100;
    private float _vsValueMin = 0;
    private float _vsValueMax = 200;

    private void Awake()
    {
        _input = new RendaActions();
        _input.RendaMap.RendaPress1.performed += KeyPressed;
        _input.RendaMap.RendaPress2.performed += KeyPressed;
        _input.RendaMap.Debug_StartRenda.performed += StartRenda;
        //_anyKeyAction = new InputAction(
        //    type: InputActionType.PassThrough,
        //    binding: "<Keyboard>/*"
        //);
        //_anyKeyAction.performed += OnKeyPressed;
    }

    private void Start()
    {
        _vsValue = _vsValueInit;
    }

    void OnEnable()
    {
        //_anyKeyAction.Enable();
        _input.Enable();
    }

    void OnDisable()
    {
        //_anyKeyAction.Disable();
        _input.Disable();
    }

    public void RendaStart()
    {
        _rendaFlg = true;
    }
    private void KeyPressed(InputAction.CallbackContext ctx)
    {
        if (!_rendaFlg) return;
        if(ctx.control is KeyControl)
        {
            _pushedCount++;
            _vsValue += _vsIncreaseAmount;
        }
    }
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _rendaFlg = true;
        }
    }
    private void StartRenda(InputAction.CallbackContext ctx)
    {
        RendaStart();
    }
    private void FixedUpdate()
    {
        if (!_rendaFlg) return;
        UpdateVsValue();
        _pushedValueText.text = _pushedCount.ToString();
        _vsValueText.text = _vsValue.ToString();
        UpdateImageScale();
    }
    private void UpdateVsValue()
    {
        _vsValue -= _vsDecreaseAmount * Time.deltaTime;
        if (_vsValue < _vsValueMin) _vsValue = _vsValueMin;
        if (_vsValue > _vsValueMax) _vsValue = _vsValueMax;
    }

    private void UpdateImageScale()
    {
        _playerImageTransform.localScale = Vector3.one * GetPlayerImageScale();
        _oppImageTransform.localScale = Vector3.one * GetOppImageScale();
    }

    private float GetPlayerImageScale()
    {
        float scaleValue = _vsValue / _vsValueMax;
        return _playerScaleMin + (_playerScaleMax - _playerScaleMin) * scaleValue;
    }

    private float GetOppImageScale()
    {
        float scaleValue = _vsValue / _vsValueMax;
        return _oppScaleMax - (_oppScaleMax - _oppScaleMin) * scaleValue;
    }
}
