using LitMotion;
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
    [SerializeField] Text _scoreText;
    [SerializeField] Text _rankText;
    [SerializeField] Text _tapCountText;
    [SerializeField] Text _tapSpeedText;

    [Header("リザルトメッセージで流す効果音")]
    [SerializeField] AudioSource _MessageSourco;
    [SerializeField] AudioClip _MessageSE;

    [Header("確認用の仮データを入れる所")]
    [SerializeField] bool _useDebugData = true;
    [SerializeField] bool _debugData = true;
    [SerializeField] float _debugScoreData = 2;
    [SerializeField] string _debugRankData = "A";
    [SerializeField] float _debugTapData = 10;
    [SerializeField] float _debugTapSpeedData = 100;

   

    void Start()
    {
        ShowResult();
    }

    void ShowResult()
    {

        bool _isClear; //クリアしたかのフラグ
        float _score; //スコア
        string _rank; //スコアのランク
        float _tapCount; //連打数
        float _tapSpeed; //連打速度

        if (_useDebugData)
        {
            _isClear = _debugData;
            _score = _debugScoreData;
            _rank = _debugRankData;
            _tapCount = _debugTapData;
            _tapSpeed = _debugTapSpeedData;
        }
        else
        {
            // 将来ここに本番データ取得を書く
            _isClear = false;
            _score = 0;
            _rank = "A";
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

        _scoreText.text = "スコア:" + _score;
        _rankText.text = "ランク:" + _rank;
        _tapCountText.text = "連打数:" + _tapCount;
        _tapSpeedText.text ="連打速度" + _tapSpeed;


        Debug.Log($"スコア: {_score}");
        Debug.Log($"ランク: {_rank}");
        Debug.Log($"連打数: {_tapCount}");
    }

    void ClearAnimText(string message)
    {
        _resultText.text = "";
        _resultText.alignment = TextAnchor.MiddleLeft;
        _tmpText.alignment = TextAlignmentOptions.Left;
        int _length = message.Length;
        int previousLength = 0;
        //LMotion.String.Create64Bytes(string.Empty, message, _clearMessageSpeed)
        //    .BindToText(_tmpText);
        LMotion.String.Create64Bytes(string.Empty, message, _clearMessageSpeed)
       .Bind(text =>
       {
           string currentText = text.ToString();
           // 文字表示
           _tmpText.text = currentText;

           // 文字が増えた瞬間だけ音を鳴らす
           if (text.Length > previousLength)
           {
               if (_MessageSourco != null && _MessageSE != null)
               {
                   Debug.Log("音を鳴らすタイミング！");
                   _MessageSourco.PlayOneShot(_MessageSE);
               }

               previousLength = text.Length;
           }
       });
        return;
    }

    void GameOverAnimText(string message)
    {
        _resultText.text = "";
        _tmpText.alignment = TextAlignmentOptions.Center;
        int _length = message.Length;
        int previousLength = 0;

        //LMotion.String.Create64Bytes(string.Empty, message, _gameOverMessageSpeed)
        //    .BindToText(_tmpText);
        LMotion.String.Create64Bytes(string.Empty, message, _gameOverMessageSpeed)
        .Bind(text =>
        {
            string currentText = text.ToString();
            // 文字表示
            _tmpText.text = currentText;

            // 文字が増えた瞬間だけ音を鳴らす
            if (text.Length > previousLength)
            {
                if (_MessageSourco != null && _MessageSE != null)
                {
                    Debug.Log("音を鳴らすタイミング！");
                    _MessageSourco.PlayOneShot(_MessageSE);
                }

                previousLength = text.Length;
            }
        });
        return;
    }
}
