using UnityEngine;

public class ScenarioItem : MonoBehaviour
{
    [SerializeField] private SimpleScenarioManager _scenarioManager;
    [SerializeField] private ActionType _actionType;

    private void Awake()
    {
        _scenarioManager.OnActionTriggered += type =>
        {
            if (type == _actionType)
                gameObject.SetActive(true);
        };
        gameObject.SetActive(false);
    }
}
