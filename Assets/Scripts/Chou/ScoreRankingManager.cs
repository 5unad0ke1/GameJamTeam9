using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     得点ランキングのデータを管理するクラス。
///     ※必ずタイトルシーンに配置すること。
/// </summary>
public class ScoreRankingManager : MonoBehaviour
{
    public static ScoreRankingManager Instance;

    [SerializeField, Tooltip("最大記録保存数")]
    private int _recordCountMax = 6;
    private List<RankingData> _records;

    /// <summary>得点記録リスト</summary>
    public List<RankingData> Records => _records;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitRecords();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    ///     得点を登録する
    /// </summary>
    /// <param name="score"></param>
    public void EnterScore(float score)
    {
        ClearNewFlg();
        if (score > _records[_recordCountMax - 1].Score)
        {
            _records[_recordCountMax - 1].SetScore(score);
            _records[_recordCountMax - 1].SetNewFlg(true);
            _records.Sort((a, b) => a.Score > b.Score ? -1 : 1);
        }
    }

    private void InitRecords()
    {
        _records = new List<RankingData>();
        for (int i = 0; i < _recordCountMax; i++)
        {
            _records.Add(new RankingData(0f, false));
        }
    }
    private void ClearNewFlg()
    {
        for(int i = 0; i < _recordCountMax; i++)
        {
            _records[i].SetNewFlg(false);
        }
    }
}
