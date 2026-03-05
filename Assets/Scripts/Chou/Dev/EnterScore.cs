using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterScore : MonoBehaviour
{
    [SerializeField] private InputField _input;
    public void Doit()
    {
        try
        {
            ScoreRankingManager.Instance.EnterScore(float.Parse(_input.text));
            SceneManager.LoadScene("Dev_Result");
        }
        catch (System.Exception e)
        {
            Debug.Log("スコア登録失敗");
        }
    }
}
