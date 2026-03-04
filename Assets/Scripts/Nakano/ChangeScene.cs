using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]string _titleScene;
    [SerializeField] float _countdownTime = 3f;
    float _timer;
    private void Update()
    {
        if (Input.anyKey)
        {
            _timer += Time.deltaTime;
            Debug.Log(_timer);
            if (_timer >= _countdownTime)
            {
                if (!string.IsNullOrEmpty(_titleScene))
                {
                    SceneManager.LoadScene(_titleScene);
                }
                else
                {
                    Debug.LogError("titleScene が設定されていません！");
                }
            }
        }
        else
        {
            _timer = 0;
        }
    }
}
