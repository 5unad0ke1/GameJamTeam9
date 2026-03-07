using System;
using UnityEngine;

public class ScenarioItem : MonoBehaviour
{
    [SerializeField] private SimpleScenarioManager _scenarioManager;
    [SerializeField] private ActionType _actionType;

    private Action<ActionType> _onActionTriggered;

    private void Awake()
    {
        if (_scenarioManager == null)
        {
            enabled = false;
            Debug.LogError($"{nameof(ScenarioItem)} の {nameof(_scenarioManager)} が未設定です", this);
            return;
        }

        _onActionTriggered = type =>
        {
            if (type == _actionType)
                gameObject.SetActive(true);
        };

        _scenarioManager.OnActionTriggered += _onActionTriggered;
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_scenarioManager == null || _onActionTriggered is null)
            return;
        _scenarioManager.OnActionTriggered -= _onActionTriggered;
        _onActionTriggered = null;
    }
}
