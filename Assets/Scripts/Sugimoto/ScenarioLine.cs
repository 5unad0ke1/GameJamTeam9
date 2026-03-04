using UnityEngine;
using static SimpleScenarioManager;

[System.Serializable]
public class ScenarioLine
{
    public string characterName;
    [TextArea(1, 100)]
    public string lineText;
    public Sprite lineImage;
    public Sprite Face;
    public ActionType actionType;
    public bool isActionExecuted;
    public bool isFuncExecuted;
}

public enum ActionType
{
    //何もないとき
    none,
    //ナレーションの後
    Zoom,
    //ポテトか来た時
    Potatoes,
    //ケチャップをつけるとき
    ketchup,
}

