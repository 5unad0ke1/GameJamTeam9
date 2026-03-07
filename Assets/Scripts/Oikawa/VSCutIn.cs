using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public sealed class VSCutIn : MonoBehaviour
{
    [SerializeField] private SimpleScenarioManager _scenarioManager;
    [SerializeField] private ActionType _filter;


    [Tooltip("画面全体揺らすためのTransform")]
    [SerializeField] private Transform _shakeParentT;

    [Tooltip("中心線を出すためのSpriteRenderer")]
    [SerializeField] private SpriteRenderer _lighting;


    [Space]

    [Tooltip("背景の左側のTransform")]
    [SerializeField] private Transform _leftBackground;
    [Tooltip("背景の右側のTransform")]
    [SerializeField] private Transform _rightBackground;

    [Space]

    [SerializeField] private Transform _leftCharactor;
    [SerializeField] private Transform _rightCharactor;

    [Space]

    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _slideClip;
    [SerializeField] private AudioClip _ironSmashClip;

    private bool _isRegistered;

    public async UniTask PlayAnimationAsync()
    {
        await PlayAnimation(false);
    }
    public void PlayAnimation()
        => PlayAnimation(false).Forget();

    [ContextMenu("PlayTest")]
    private void PlayTest()
        => PlayAnimation(true).Forget();

    [ContextMenu("Init")]
    private void Initialize()
    {
        _shakeParentT.gameObject.SetActive(false);

        if (_scenarioManager == null)
        {
            Debug.LogError("シナリオマネージャーが未登録です。", this);
            return;
        }
        if (_isRegistered)
            return;

        _scenarioManager.OnFuncTriggerd.Add(PlayAnimation);
        _isRegistered = true;
    }
    private void Awake()
    {
        Initialize();
    }
    private void OnDestroy()
    {
        if (_isRegistered && _scenarioManager != null)
            _scenarioManager.OnFuncTriggerd.Remove(PlayAnimation);
    }
    private async UniTask PlayAnimation(ActionType type)
    {
        if (type != _filter)
            return;
        await PlayAnimation(false);
    }
    private async UniTask PlayAnimation(bool isTest = false)
    {
        if (isTest)
        {
            _shakeParentT.gameObject.SetActive(false);
            await UniTask.Delay(500);
        }


        _shakeParentT.gameObject.SetActive(true);
        _lighting.enabled = false;
        _leftCharactor.gameObject.SetActive(false);
        _rightCharactor.gameObject.SetActive(false);


        //_source.PlayOneShot(_slideClip);
        var handle = LSequence.Create()
            .Append(LMotion.Create(-17f, -7f, 0.3f)
                .WithEase(Ease.InCirc)
                .BindToLocalPositionX(_leftBackground))
            .Join(LMotion.Create(17f, 7f, 0.3f)
                .WithEase(Ease.InCirc)
                .BindToLocalPositionX(_rightBackground))
            .Run();
        handle.Time = 0;
        await handle;
        _source.Stop();

        _source.PlayOneShot(_ironSmashClip);
        _lighting.enabled = true;

        _leftCharactor.gameObject.SetActive(true);
        _rightCharactor.gameObject.SetActive(true);
        handle = LSequence.Create()
             .Join(LMotion.Punch.Create(0f, 0.5f, 1f)
                .WithEase(Ease.OutQuart)
                .WithFrequency(Random.Range(5, 10))
                .BindToLocalPositionX(_shakeParentT))
            .Join(LMotion.Punch.Create(0f, 0.5f, 1f)
                .WithEase(Ease.OutQuart)
                .WithFrequency(Random.Range(5, 10))
                .BindToLocalPositionY(_shakeParentT))

            .Join(LMotion.Create(-14f, -6f, 0.5f)
                .WithEase(Ease.OutCirc)
                .BindToLocalPositionX(_leftCharactor))
            .Join(LMotion.Create(14f, 6f, 0.5f)
                .WithEase(Ease.OutCirc)
                .BindToLocalPositionX(_rightCharactor))
            .Run();
        await handle;
    }
}
