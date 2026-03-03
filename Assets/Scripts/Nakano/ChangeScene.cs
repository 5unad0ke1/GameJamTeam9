using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]string _titleScene;
    [SerializeField] float _countdownTime = 3f;
    float _timer;
    bool _isCounting = false;
    private void Update()
    {
        if (Input.anyKeyDown && !_isCounting)
        {
            _isCounting = true;
            _timer = _countdownTime;
        }
        if (_isCounting)
        {
            _timer -= Time.deltaTime;
            Debug.Log(_timer);
            if(_timer <= 0)
            {
                if (!string.IsNullOrEmpty(_titleScene))
                {
                    _isCounting = false;
                    SceneManager.LoadScene(_titleScene);
                }
                else
                {
                    Debug.LogError("titleScene が設定されていません！");
                }
                
            }
        }
    }
}
