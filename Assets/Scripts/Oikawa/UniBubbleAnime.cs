using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public sealed class UniBubbleAnime : MonoBehaviour
{
    [SerializeField] private RectTransform[] _transforms;

    private MotionHandle _handle;
    private float _timer;
    void Start()
    {
        _timer = 0;
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 0.25f)
            return;
        _timer = 0;

        var builder = LSequence.Create();
        foreach (var item in _transforms)
        {
            builder.Join(LMotion.Shake.Create(0f, 20f, 0.25f)
                        .WithFrequency(Random.Range(5, 10))
                        .BindToAnchoredPosition3DX(item))
                    .Join(LMotion.Shake.Create(0f, 20f, 0.25f)
                        .WithFrequency(Random.Range(5, 10))
                        .BindToAnchoredPosition3DY(item));
        }
        _handle = builder.Run();
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }
}
