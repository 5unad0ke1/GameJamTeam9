using UnityEngine;

public class ScenarioAudioManager : MonoBehaviour
{
    [Tooltip("登録用")]
    [SerializeField] private SimpleScenarioManager _scenarioManager;

    [Tooltip("BGMの再生、停止、Sourceの管理　(1BGM 1AudioSource　という考え)")]
    [SerializeField] private BGMSourceData[] _bgmSources;

    [SerializeField] private AudioSource _seSource;
    [Tooltip("SEの再生、Clipの管理　(1ActionType 1Clip　という考え)")]
    [SerializeField] private SEClipData[] _seClips;
    void Start()
    {
        foreach (var item in _bgmSources)
        {
            _scenarioManager.OnActionTriggered += type =>
            {
                if (type == item.PlayType)
                {
                    item.Source.Play();
                }
                if (type == item.StopType)
                {
                    item.Source.Stop();
                }
            };
        }
        foreach (var item in _seClips)
        {
            _scenarioManager.OnActionTriggered += type =>
            {
                if (item.PlayType != type)
                    return;
                _seSource.PlayOneShot(item.Clip);
            };
        }
    }


    [System.Serializable]
    private struct BGMSourceData
    {
        public readonly AudioSource Source => _source;
        public readonly ActionType PlayType => _playType;
        public readonly ActionType StopType => _stopType;

        [Tooltip("再生実行するActionType")]
        [SerializeField] private ActionType _playType;

        [Tooltip("停止実行するActionType")]
        [SerializeField] private ActionType _stopType;

        [SerializeField] private AudioSource _source;
    }
    [System.Serializable]
    private struct SEClipData
    {
        public readonly AudioClip Clip => _clip;
        public readonly ActionType PlayType => _playType;

        [Tooltip("再生実行するActionType")]
        [SerializeField] private ActionType _playType;

        [SerializeField] private AudioClip _clip;
    }
}
