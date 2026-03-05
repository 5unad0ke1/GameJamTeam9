using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScene : MonoBehaviour
{
    public void Doit(string name)
    {
        SceneManager.LoadScene(name);
    }
}
