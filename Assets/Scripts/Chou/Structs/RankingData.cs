using System.Linq.Expressions;
using UnityEngine;

public class RankingData
{
    private float _score;
    private bool _isNew;

    public float Score => _score;
    public bool IsNew => _isNew;
    public RankingData(float score, bool isNew)
    {
        _isNew = isNew;
        _score = score;
    }
    public void SetNewFlg(bool value)
    {
        _isNew = value;
    }
    public void SetScore(float value)
    {
        _score = value;
    }
}
