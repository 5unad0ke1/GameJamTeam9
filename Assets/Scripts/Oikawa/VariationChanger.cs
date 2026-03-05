using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class VariationChanger : MonoBehaviour
{
    [SerializeField] private Transform _shakeParent;
    [SerializeField] private GameObject[] _imageGameObjects;
    [SerializeField] private bool _isShake = true;
    [SerializeField] private float _updateFrequency = 0.5f;

    private int _index;
    private float _timer;
    private MotionHandle _handle;
    private void Awake()
    {
        _timer = 0;
        _index = 0;
    }
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _updateFrequency)
            return;

        _timer = 0;
        _index = (_index + 1) % _imageGameObjects.Length;

        foreach (var item in _imageGameObjects)
        {
            item.SetActive(false);
        }
        _imageGameObjects[_index].SetActive(true);

        if (_isShake)
            Shake();
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }
    void Shake()
    {
        _handle = LSequence.Create()
            .Join(LMotion.Punch.Create(0f, 0.2f, 0.2f)
                .WithEase(Ease.OutQuart)
                .WithFrequency(Random.Range(5, 10))
                .BindToLocalPositionX(_shakeParent))
            .Join(LMotion.Punch.Create(0f, 0.2f, 0.2f)
                .WithEase(Ease.OutQuart)
                .WithFrequency(Random.Range(5, 10))
                .BindToLocalPositionY(_shakeParent))
            .Run();
    }
}
