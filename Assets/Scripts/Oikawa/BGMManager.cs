using LitMotion;
using LitMotion.Extensions;
using System;
using UnityEngine;

public sealed class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource _cicadaSource;
    [SerializeField] private AudioSource _rastaurantSource;
    [SerializeField] private AudioSource _specialToMeSource;
    [SerializeField] private AudioSource _actionSource;


    [Space]

    [SerializeField] private SimpleScenarioManager _scenarioManager;

    private MotionHandle _handle;

    private void Start()
    {
        if (_scenarioManager == null || _cicadaSource == null || _rastaurantSource == null || _specialToMeSource == null || _actionSource == null)
        {
            Debug.LogError("BGMManager の参照が不足しています。Inspector 設定を確認してください。", this);
            enabled = false;
            return;
        }

        _scenarioManager.OnActionTriggered += BGMUpdate;


        _cicadaSource.volume = 1f;
        _rastaurantSource.volume = 0f;
        _specialToMeSource.volume = 0f;
        _actionSource.volume = 0f;
    }
    private void OnDestroy()
    {
        _scenarioManager.OnActionTriggered -= BGMUpdate;
        _handle.TryCancel();
    }
    private void BGMUpdate(ActionType filter)
    {
        Action action = filter switch
        {
            ActionType.Zoom => Zoom,
            ActionType.ketchup => StopAllBGM,
            ActionType.Potatoes => Fight,
            _ => null,
        };
        action?.Invoke();
    }
    private void Zoom()
    {
        _handle.TryComplete();

        _handle = LSequence.Create()
            .Append(LMotion.Create(1f, 0f, 3f)
                .BindToVolume(_cicadaSource))
            .Insert(1f, LMotion.Create(0f, 0.2f, 3f)
                .WithOnComplete(() => _specialToMeSource.Play())
                .BindToVolume(_rastaurantSource))
            .Insert(4f, LMotion.Create(0f, 1f, 0.2f)
                .BindToVolume(_specialToMeSource))
            .Run();
    }
    private void Fight()
    {
        _handle.TryComplete();

        _actionSource.Play();
        _handle = LMotion.Create(0f, 1f, 0.2f)
            .BindToVolume(_actionSource);
    }
    private void StopAllBGM()
    {
        _cicadaSource.volume = 0f;
        _rastaurantSource.volume = 0f;
        _specialToMeSource.volume = 0f;
        _actionSource.volume = 0f;
    }
}
