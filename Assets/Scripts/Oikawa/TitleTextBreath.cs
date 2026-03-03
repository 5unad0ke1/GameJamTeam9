using LitMotion;
using LitMotion.Adapters;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TitleTextBreath : MonoBehaviour
{

    void Start()
    {
        bool binded = false;

        if (_image != null)
        {
            _handleImage = GetBuilder().BindToColorA(_image);
            binded = true;
        }
        if (_text != null)
        {
            _handleText = GetBuilder().BindToColorA(_text);
            binded = true;
        }
        if (_tmpText != null)
        {
            _handleTMP = GetBuilder().BindToColorA(_tmpText);
            binded = true;
        }
        if (!binded)
        {
            Debug.LogWarning("TitleTextBreath: 適用するオブジェクトがありません");
        }
    }

    private MotionBuilder<float, NoOptions, FloatMotionAdapter> GetBuilder()
        => LMotion.Create(1f, 0f, 1f / _breathRate)
        .WithEase(Ease.InOutQuad)
        .WithLoops(-1, LoopType.Flip);
    private void OnDestroy()
    {
        _handleImage.TryCancel();
        _handleText.TryCancel();
        _handleTMP.TryCancel();
    }


    [Tooltip("適応するImage(null可能)")]
    [SerializeField]
    private Image _image;

    [Tooltip("適応するText(null可能)")]
    [SerializeField]
    private Text _text;

    [Tooltip("適応するTextMeshPro(null可能)")]
    [SerializeField]
    private TMP_Text _tmpText;

    [Tooltip("呼吸アニメーションの頻度")]
    [SerializeField]
    private float _breathRate;


    private MotionHandle _handleImage;
    private MotionHandle _handleText;
    private MotionHandle _handleTMP;
}
