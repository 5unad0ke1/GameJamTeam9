using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [Header("リザルト時に表示するImage")]
    [SerializeField] Image[] _resultImage;

    [Header("スコア表示のText")]
    [SerializeField] Text _selectText;
    [SerializeField] Text _timeText;
    [SerializeField] Text _tapText;

    [Header("確認用の仮データを入れる所")]
    [SerializeField] bool _useDebugData = true;
    [SerializeField] bool _debugData = true;
    [SerializeField] int _debugSelectData = 2;
    [SerializeField] float _debugTimeData = 0f;
    [SerializeField] float _debugTapData = 10;
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

        if (_useDebugData)
        {
            _isClear = _debugData;
            _selected = _debugSelectData;
            _time = _debugTimeData;
            _tapCount = _debugTapData;
        }
        else
        {
            // 将来ここに本番データ取得を書く
            _isClear = false;
            _selected = 0;
            _time = 0;
            _tapCount = 0;
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
        _tapText.text = "連打数:" + _tapCount;

        Debug.Log($"セレクト: {_selected}");
        Debug.Log($"タイム: {_time}");
        Debug.Log($"連打数: {_tapCount}");
    }
}
