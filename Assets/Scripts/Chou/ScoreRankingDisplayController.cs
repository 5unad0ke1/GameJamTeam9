using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRankingDisplayController : MonoBehaviour
{
    [SerializeField] private Text[] _scores;
    [SerializeField] private Text[] _newFlgs;
    [SerializeField] private GameObject _rankingUi;
    private bool _displayFlg = false;

    private void Start()
    {
        _displayFlg = _rankingUi.activeInHierarchy;
    }
    /// <summary>
    ///     ランキングの表示切替。ボタンの押下Eventに割り当てる
    /// </summary>
    public void ToggleRankingDisplay()
    {
        _displayFlg = !_displayFlg;
        if(_displayFlg)
        {
            GetRankingData();
        }
        _rankingUi.SetActive(_displayFlg);
    }

    private void GetRankingData()
    {
        List<RankingData> records = ScoreRankingManager.Instance.Records;
        for (int i = 0; i < records.Count; i++)
        {
            _scores[i].text = records[i].Score.ToString("0");
            _newFlgs[i].text = records[i].IsNew ? "New!" : "";
        }
    }
}
