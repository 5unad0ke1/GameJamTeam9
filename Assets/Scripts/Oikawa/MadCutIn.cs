using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class MadCutIn : MonoBehaviour
{
    [Tooltip("登録用")]
    [SerializeField] private SimpleScenarioManager _scenarioManager;
    [Tooltip("実行するフィルター")]
    [SerializeField] private ActionType _actionType;

    [Tooltip("カットインの電撃の背景")]
    [SerializeField] private Transform _backgroundT;

    [Tooltip("カットインの文字")]
    [SerializeField] private Transform[] _textT;

    [Tooltip("プレイヤーSpriteRendererのTransform")]
    [SerializeField] private Transform _playerT;

    [Tooltip("Shakiin効果音を再生するAudioSource")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _shakiinClip;

    private MotionHandle _handle;

    [ContextMenu("TestPlay")]
    private void TestPlay()
        => PlayAnimation(_actionType, true).Forget();
    private void Awake()
    {
        _scenarioManager.OnFuncTriggerd.Add(type => PlayAnimation(type));

        var scale = _backgroundT.localScale;
        scale.y = 0;
        _backgroundT.localScale = scale;
        foreach (var item in _textT)
        {
            item.localScale = Vector3.zero;
        }
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }

    private async UniTask PlayAnimation(ActionType type, bool isTest = false)
    {
        if (type != _actionType)
        {
            return;
        }
        if (isTest)
        {
            await UniTask.Delay(500);
        }

        _source.PlayOneShot(_shakiinClip);

        _handle.TryCancel();

        var backgroundHandle =
            LSequence.Create()
            .Append(LMotion.Create(0f, 1f, 0.1f)
                .WithEase(Ease.InSine)
                .BindToLocalScaleY(_backgroundT))
            .Append(LMotion.Create(1f, 1.1f, 0.5f)
                .WithLoops(3, LoopType.Flip)
                .WithEase(Ease.InOutSine)
                .BindToLocalScaleY(_backgroundT))
            .Append(LMotion.Create(1f, 0f, 0.1f)
                .WithEase(Ease.InSine)
                .BindToLocalScaleY(_backgroundT))
            .Run();

        var builder = LSequence.Create();
        foreach (var item in _textT)
        {
            builder.Append(LMotion.Create(new Vector3(0f, 0f, 1f), Vector3.one, 0.1f)
                    .BindToLocalScale(item))

                .Join(LMotion.Punch.Create(item.localPosition.x, 0.5f, 1.8f)
                    .WithEase(Ease.OutQuart)
                    .WithFrequency(Random.Range(15, 30))
                    .BindToLocalPositionX(item))
                .Join(LMotion.Punch.Create(item.localPosition.y, 0.5f, 1.8f)
                    .WithEase(Ease.OutQuart)
                    .WithFrequency(Random.Range(15, 30))
                    .BindToLocalPositionY(item))

                .Insert(1.6f, LMotion.Create(Vector3.one, new Vector3(0f, 0f, 1f), 0.1f)
                    .BindToLocalScale(item));
        }
        var textHandle = builder.Run();


        var playerHandle = LSequence.Create()
            .Append(LMotion.Create(-1f, 0f, 0.2f)
                .WithEase(Ease.OutCirc)
                .BindToLocalPositionY(_playerT))
            .Append(LMotion.Create(0f, 0.25f, 1.5f)
                .WithEase(Ease.Linear)
                .BindToLocalPositionY(_playerT))
            .Run();

        _handle = LSequence.Create()
            .Append(backgroundHandle)
            .Join(playerHandle)
            .Join(textHandle)
            .Run();
        _handle.Time = 0f;
        await _handle;
    }
}
