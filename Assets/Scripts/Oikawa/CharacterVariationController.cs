using UnityEngine;

/// <summary>
/// GameObject.SetActiveをもちいて、キャラクターの差分表示を管理するクラス
/// </summary>
public sealed class CharacterVariationController : MonoBehaviour
{
    [Tooltip("差分変更命令を登録する用")]
    [SerializeField] private SimpleScenarioManager _scenarioManager;

    [Tooltip("差分変更を実行するルール管理")]
    [SerializeField] private Rule[] _data;

    [Tooltip("差分表示があるGameObject達(下手に配列の順番変更しないように)")]
    [SerializeField] private GameObject[] _spriteGameObjects;

    [Tooltip("差分変更時のバウンドのアニメーション(null可能 TryGetComponent実装済み)")]
    [SerializeField] private BoundAnimation _animation;

    [ContextMenu("TestVariation")]
    public void TestPlayVariation()
    {
        ChangeVariation(Random.Range(0, _spriteGameObjects.Length));
    }

    void Awake()
    {
        foreach (var item in _spriteGameObjects)
        {
            if (item.TryGetComponent(out SpriteRenderer renderer))
                renderer.enabled = true;
        }

        foreach (var item in _data)
        {
            _scenarioManager.OnActionTriggered += type =>
            {
                if (type != item.Filter)
                    return;
                ChangeVariation(item.Index);
            };
        }


        if (_animation == null)
            if (TryGetComponent(out _animation))
            {
                Debug.Log($"{nameof(CharacterVariationController)}の_animationが自動注入されました", this);
            }
    }

    void Start()
    {

    }

    private void ChangeVariation(int index)
    {
        foreach (var item in _spriteGameObjects)
        {
            item.SetActive(false);
        }
        _spriteGameObjects[index].SetActive(true);

        if (_animation != null)
            _animation.PlayAnimation();
    }

    [System.Serializable]
    private struct Rule
    {
        public ActionType Filter => _filter;

        public int Index => _variationIndex;

        [Tooltip("変更実行するフィルター")]
        [SerializeField] private ActionType _filter;

        [Tooltip("変更する差分番号")]
        [SerializeField] private int _variationIndex;
    }
}
