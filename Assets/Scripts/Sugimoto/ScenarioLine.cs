using UnityEngine;

[System.Serializable]
public class ScenarioLine
{
    public string characterName;
    [TextArea(1, 100)]
    public string lineText;
    public Sprite lineImage;
}
