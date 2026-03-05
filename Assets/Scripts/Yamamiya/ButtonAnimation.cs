using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private float _animationDuration = 0.2f;
    [SerializeField] private float _selectScale = 1.5f;
    [SerializeField] private Vector3 _defaultScale = Vector3.one;
    [SerializeField] private Ease _ease = Ease.Linear;

    private MotionHandle _currentMotion;

    private void Awake()
    {
        if (_rect == null)
        {
            _rect = GetComponent<RectTransform>();
        }

        _rect.localScale = _defaultScale;
    }

    private void OnDestroy()
    {
        _currentMotion.TryCancel();
    }

    /// <summary>
    /// ボタンが選択されたときのアニメーションを行う。
    /// </summary>
    public void OnSelect()
    {
        _currentMotion.TryCancel();

        var selectScale = _rect.localScale * _selectScale;
        _currentMotion = LMotion.Create(_rect.localScale, selectScale, _animationDuration)
            .WithEase(_ease)
            .BindToLocalScale(_rect);
    }

    /// <summary>
    /// ボタンが選択解除されたときのアニメーションを行う。
    /// </summary>
    public void OnDeselect()
    {
        _currentMotion.TryCancel();

        _currentMotion = LMotion.Create(_rect.localScale, _defaultScale, _animationDuration)
            .WithEase(_ease)
            .BindToLocalScale(_rect);
    }
}
