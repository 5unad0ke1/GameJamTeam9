using LitMotion;
using LitMotion.Adapters;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [Header("リザルト時に表示するImage")]
    [SerializeField] Image[] _resultImage;
    [SerializeField] private TMP_Text _tmpText;
    [SerializeField] float _clearMessageSpeed = 1f;
    [SerializeField] float _gameOverMessageSpeed = 1f;
    [SerializeField] Text _resultText;

    [Header("スコア表示のText")]
    [SerializeField] Text _selectText;
    [SerializeField] Text _timeText;
    [SerializeField] Text _tapCountText;
    [SerializeField] Text _tapSpeedText;

    [Header("確認用の仮データを入れる所")]
    [SerializeField] bool _useDebugData = true;
    [SerializeField] bool _debugData = true;
    [SerializeField] int _debugSelectData = 2;
    [SerializeField] float _debugTimeData = 0f;
    [SerializeField] float _debugTapData = 10;
    [SerializeField] float _debugTapSpeedData = 100;

   

    void Start()
    {
        ShowResult();
    }

    void ShowResult()
    {

        bool _isClear; //クリアしたかのフラグ
        int _selected; //選択したもの
        float _time; //経過時間
        float _tapCount; //連打数
        float _tapSpeed; //連打速度

        if (_useDebugData)
        {
            _isClear = _debugData;
            _selected = _debugSelectData;
            _time = _debugTimeData;
            _tapCount = _debugTapData;
            _tapSpeed = _debugTapSpeedData;
        }
        else
        {
            // 将来ここに本番データ取得を書く
            _isClear = false;
            _selected = 0;
            _time = 0;
            _tapCount = 0;
            _tapSpeed = 0;
        }

       
        string message = _isClear ? "完全押付" : "妥協";

        //アニメーション再生
        if (_isClear)
        {
            ClearAnimText(message);
        }
        else
        {
            GameOverAnimText(message);
        }


        if (_resultImage != null && _resultImage.Length > 0)
        {
            foreach (var image in _resultImage)
            {
                if (image != null)
                    image.gameObject.SetActive(false);
            }

            if (_isClear)
            {
                if (_resultImage.Length > 0 && _resultImage[0] != null)
                    _resultImage[0].gameObject.SetActive(true);
            }
            else
            {
                if (_resultImage.Length > 1 && _resultImage[1] != null)
                    _resultImage[1].gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Result Image 未設定（デバッグモード動作中）");
        }

        _selectText.text = "セレクト:" + _selected;
        _timeText.text = "タイム:" + _time;
        _tapCountText.text = "連打数:" + _tapCount;
        _tapSpeedText.text ="連打速度" + _tapSpeed;


        Debug.Log($"セレクト: {_selected}");
        Debug.Log($"タイム: {_time}");
        Debug.Log($"連打数: {_tapCount}");
    }

    void ClearAnimText(string message)
    {
        _resultText.text = "";
        _resultText.alignment = TextAnchor.MiddleLeft;
        _tmpText.alignment = TextAlignmentOptions.Left;
        int _length = message.Length;

        LMotion.String.Create64Bytes(string.Empty, message, _clearMessageSpeed)
            .BindToText(_tmpText);
        return;
    }

    void GameOverAnimText(string message)
    {
        _resultText.text = "";
        _tmpText.alignment = TextAlignmentOptions.Center;
        int _length = message.Length;

        LMotion.String.Create64Bytes(string.Empty, message, _gameOverMessageSpeed)
            .BindToText(_tmpText);
        return;
    }
}
