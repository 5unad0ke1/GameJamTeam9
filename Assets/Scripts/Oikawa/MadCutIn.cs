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

    private MotionHandle _handle;

    [ContextMenu("TestPlay")]
    private void TestPlay()
        => PlayAnimation(_actionType).Forget();
    private void Awake()
    {
        _scenarioManager.OnFuncTriggerd += PlayAnimation;
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }

    private async UniTask PlayAnimation(ActionType type)
    {
        if (type != _actionType)
        {
            return;
        }
        _handle.TryCancel();

        var backgroundHandle =
            LSequence.Create()
            .Append(LMotion.Create(0f, 1f, 0.1f)
                .WithEase(Ease.InSine)
                .BindToLocalScaleY(_backgroundT))
            .Append(LMotion.Create(1f, 1.1f, 0.5f)
                .WithLoops(4, LoopType.Flip)
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
                .Join(LMotion.Punch.Create(item.localPosition.x, 0.5f, 0.5f)
                    .WithFrequency(Random.Range(5, 10))
                    .BindToLocalPositionX(item))
                .Join(LMotion.Punch.Create(item.localPosition.y, 0.5f, 0.5f)
                    .WithFrequency(Random.Range(5, 10))
                    .BindToLocalPositionY(item))
                .Insert(2.1f, LMotion.Create(Vector3.one, new Vector3(0f, 0f, 1f), 0.1f)
                    .BindToLocalScale(item));
        }
        var textHandle = builder.Run();

        _handle = LSequence.Create()
            .AppendInterval(0.5f)
            .Append(backgroundHandle)
            .Join(textHandle)
            .Run();
        _handle.Time = 0f;
    }
}
