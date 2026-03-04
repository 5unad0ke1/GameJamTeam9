using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public sealed class BoundAnimation : MonoBehaviour
{
    [Tooltip("アニメーション再生時最初のサイズ")]
    [SerializeField] private Vector3 _boundSize;

    [Tooltip("アニメーション再生時最後のサイズ")]
    [SerializeField] private Vector3 _defalutSize;

    private MotionHandle _handle;

    [ContextMenu("TestPlay")]
    public void PlayAnimation()
    {
        _handle.TryComplete();
        _handle = LMotion.Create(_boundSize, _defalutSize, 0.7f)
            .WithEase(Ease.OutCirc)
            .BindToLocalScale(transform);
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }
}
