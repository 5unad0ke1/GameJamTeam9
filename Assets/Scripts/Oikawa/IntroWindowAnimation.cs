using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

/// <summary>
/// Start時に外から部屋の窓を映すようにフェードインし、
/// SimpleScenarioManagerのFunc実行によって外観から内装へのアニメーションを再生
/// </summary>
public sealed class IntroWindowAnimation : MonoBehaviour
{
    [Tooltip("シナリオマネージャー(Action/Funcの登録 enabled管理)")]
    [SerializeField] private SimpleScenarioManager _scenarioManager;

    [Tooltip("レストラン外観のTransform (アニメーション用)")]
    [SerializeField] private Transform _exteriorT;

    [Tooltip("レストラン外観のSpriteRenderer (フェード用)")]
    [SerializeField] private SpriteRenderer _exterior;

    [Tooltip("レストラン外観ズーム版のSpriteRenderer (フェード用)")]
    [SerializeField] private SpriteRenderer _exteriorZoom;

    [Tooltip("フェード用SpriteRenderer")]
    [SerializeField] private SpriteRenderer _fade;

    [Tooltip("外観から内装へアニメーション実行するActionType")]
    [SerializeField] private ActionType _filterType;

    // GameTitle
    // A4sizeのGame説明
    // TitleのSnapshot

    [ContextMenu("TestPlayIntro")]
    public void TestPlayIntro()
    {
        PlayIntro().Forget();
    }
    [ContextMenu("TestPlayExteriorToInterior")]
    public void TestPlayExteriorToInterior()
    {
        PlayExteriorToInterior(_filterType).Forget();
    }

    private void Awake()
    {
        _scenarioManager.enabled = false;
        _scenarioManager.OnFuncTriggerd.Add(PlayExteriorToInterior);
    }
    private void Start()
    {
        PlayIntro().Forget();
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
        _scenarioManager.OnFuncTriggerd.Remove(PlayExteriorToInterior);
    }

    private async UniTask PlayIntro()
    {
        _handle.TryComplete();
        _handle = LMotion.Create(1f, 0f, 0.7f)
            .WithDelay(0.5f)
            .WithEase(Ease.OutCirc)
            .BindToColorA(_fade);


        _exteriorT.localScale = Vector3.one / 1.4f;
        _exterior.color = _exterior.color.WithAlpha(1f);
        _exteriorZoom.color = _exteriorZoom.color.WithAlpha(1f);

        await _handle;
        _scenarioManager.enabled = true;
    }
    private async UniTask PlayExteriorToInterior(ActionType type)
    {
        if (type != _filterType)
        {
            return;
        }
        _handle.TryComplete();
        _handle = LSequence.Create()

            .Append(LMotion.Create(Vector3.one / 1.4f, Vector3.one * 1.4f, 4f)
                .WithEase(Ease.Linear)
                .BindToLocalScale(_exteriorT))

            .Join(LMotion.Create(1f, 0f, 1f)
                .WithDelay(1.5f)
                .WithEase(Ease.Linear)
                .BindToColorA(_exterior))

            .Join(LMotion.Create(1f, 0f, 1f)
                .WithDelay(3f)
                .WithEase(Ease.Linear)
                .BindToColorA(_exteriorZoom))

            .Run();
        _handle.Time = 0f;
        await _handle;
    }
    private MotionHandle _handle;
}
