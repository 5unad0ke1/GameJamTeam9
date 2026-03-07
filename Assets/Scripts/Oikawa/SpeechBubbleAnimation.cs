using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class SpeechBubbleAnimation : MonoBehaviour
{
    [Tooltip("吹き出し達の")]
    [SerializeField] RectTransform[] _bubbles;

    private MotionHandle _handle;
    private float _timer;

    [ContextMenu("TestPlay")]
    public void TestPlay()
        => Animation(Random.Range(0, _bubbles.Length), true);
    public void PlayAnimation(int index)
        => Animation(index);
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < 2.5f)
            return;
        _timer = 0;
        TestPlay();
    }
    private void OnDestroy()
    {
        _handle.TryCancel();
    }
    private void Animation(int index, bool isTest = false)
    {
        if (_bubbles == null)
        {
            Debug.LogWarning("SpeechBubbleAnimation: `_bubbles` is null.", this);
            return;
        }
        if (index < 0 || index >= _bubbles.Length)
        {
            Debug.LogWarning($"SpeechBubbleAnimation: index {index} is out of range (length {_bubbles.Length}).", this);
            return;
        }
        if (_bubbles[index] == null)
        {
            Debug.LogWarning($"SpeechBubbleAnimation: `_bubbles[{index}]` is null.", this);
            return;
        }

        _handle.TryComplete();

        foreach (var item in _bubbles)
        {
            item.gameObject.SetActive(false);
        }
        _bubbles[index].gameObject.SetActive(true);



        _handle = LSequence.Create()
            .AppendInterval(isTest ? 0.5f : 0f)

            .Append(LMotion.Create(new Vector3(0f, 0f, 1f), Vector3.one, 0.1f)
                .BindToLocalScale(transform))
            .Join(LMotion.Punch.Create(0f, 100f, 1f)
                    .WithEase(Ease.OutQuart)
                    .WithFrequency(Random.Range(10, 20))
                    .BindToLocalPositionX(_bubbles[index]))
            .Join(LMotion.Punch.Create(0f, 100f, 1f)
                .WithEase(Ease.OutQuart)
                .WithFrequency(Random.Range(10, 20))
                .BindToLocalPositionY(_bubbles[index]))
            .Insert(1.6f + (isTest ? 0.5f : 0f), LMotion.Create(Vector3.one, new Vector3(0f, 0f, 1f), 0.1f)
                .BindToLocalScale(transform))
            .Run();
        _handle.Time = 0f;
    }
}
