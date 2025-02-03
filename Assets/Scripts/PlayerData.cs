using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public string PlayerColor;  // Stored as hex string
    public string PlayerPerks;
    public float PoliticalX;
    public float PoliticalY;
    public float LikeabilityLiberals;
    public float LikeabilityConservatives;
    public float LikeabilityLibertarians;
    public float LikeabilityAuthoritarians;

    public PlayerData()
    {
        PlayerName = "New Player";
        PlayerColor = "#FFFFFF"; // Default white
        PlayerPerks = "None";
        PoliticalX = 0f;
        PoliticalY = 0f;
        LikeabilityLiberals = 50;
        LikeabilityConservatives = 50;
        LikeabilityLibertarians = 50;
        LikeabilityAuthoritarians = 50;
    }

    public Color GetColor()
    {
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + PlayerColor, out color))
            return color;
        return Color.white;
    }

    public void SetColor(Color color)
    {
        PlayerColor = ColorUtility.ToHtmlStringRGB(color);
    }
}
