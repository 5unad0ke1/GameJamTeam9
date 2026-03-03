using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShinobiExecution : MonoBehaviour
{
    void Start()
    {
        _background.gameObject.SetActive(false);
    }

    [ContextMenu("Play")]
    private void Play()
    {
        StartExecution(true).Forget();
    }
    private async UniTask StartExecution(bool IsDelayExecution = false)
    {
        if (IsDelayExecution)
        {
            _parentObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }

        _parentObject.SetActive(true);
        var builder = LSequence.Create()
            .Append(LMotion.Create(0f, 0.5f, 1f)
                .WithEase(Ease.OutQuad)
                .BindToColorA(_background));
        foreach (var item in _graphics)
        {
            builder.Join(LMotion.Create(0f, 1f, 1f)
                .WithEase(Ease.OutQuad)
                .BindToColorA(item));
        }
        var handle = builder.Run(x => x.AddTo(destroyCancellationToken));
        handle.Time = 0;
        await handle;

        await UniTask.Delay(TimeSpan.FromSeconds(_displayTime), cancellationToken: destroyCancellationToken);

        builder = LSequence.Create()
            .Append(LMotion.Create(0.5f, 0f, 1f)
                .WithEase(Ease.OutQuad)
                .BindToColorA(_background));
        foreach (var item in _graphics)
        {
            builder.Join(LMotion.Create(1f, 0f, 1f)
                .WithEase(Ease.OutQuad)
                .BindToColorA(item));
        }
        handle = builder
            .Run(x => x.AddTo(destroyCancellationToken));
        handle.Time = 0;
        await handle;

        _parentObject.SetActive(false);
    }

    [Tooltip("演出の親オブジェクト(SetActive用による表示/非表示管理)")]
    [SerializeField] private GameObject _parentObject;

    [Tooltip("背景のImage")]
    [SerializeField] private Image _background;
    [Tooltip("演出に使用するGraphicの配列 (当然 Image,Textなどアタッチ可能)")]
    [SerializeField] private Graphic[] _graphics;

    [Tooltip("演出の表示時間")]
    [SerializeField] private float _displayTime;
}
