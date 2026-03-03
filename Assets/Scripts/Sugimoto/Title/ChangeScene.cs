using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // すべてのキーコードをチェックして、押されたキーをログに出力
            if (Input.anyKeyDown)
            {
                Debug.Log($"押されたキー{Input.inputString}");
                ChangeSceneTo(_sceneName);
            }
        }
    }

    public void ChangeSceneTo(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
