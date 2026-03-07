using System;
using UnityEngine;

public sealed class RapidFireChallenge : MonoBehaviour
{
    public float Range
    {
        get => _range;
        set => _range = value;
    }


    [Range(-1f, 1f)]
    [SerializeField] private float _range;

    [Space]
    [SerializeField] private Transform _backgroundT;

    [SerializeField] private Transform _targetT;

    [SerializeField] private RectTransform _bubbleT;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _backgroundT.localPosition = Vector3.right * _range * 5;

        float targetT = Mathf.Clamp01(_range) * 4;
        _targetT.localPosition = Vector3.right * targetT;

        float bubbleValue = (_range + 1f) / 2f;
        _bubbleT.anchoredPosition3D = Vector3.right * Mathf.Lerp(-500f, 500f, bubbleValue);
        _bubbleT.localScale = Vector3.one * Mathf.Lerp(1f, 3f, bubbleValue);
    }
}
