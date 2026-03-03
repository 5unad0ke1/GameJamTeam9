using UnityEngine;
using TMPro;

public class SimpleScenarioManager : MonoBehaviour
{
    [SerializeField] private ScenarioLine[] scenarioLines;

    [SerializeField] private TMPro.TextMeshProUGUI lineText;
    [SerializeField] private UnityEngine.UI.Image lineImage;

    private int currentLineIndex = 0;

    private void Start()
    {
        Sorline();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    public void Sorline() 
    {
        if (currentLineIndex < scenarioLines.Length)
        {
            lineText.text = scenarioLines[currentLineIndex].lineText;
            lineImage.sprite = scenarioLines[currentLineIndex].lineImage;
        }
    }

    public void NextLine()
    {
        if (currentLineIndex < scenarioLines.Length - 1)
        {
            currentLineIndex++;
            Sorline();
        }
    }
}
