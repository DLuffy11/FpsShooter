using UnityEngine;

public static class SaveSystem
{
    private const string WinScoreKey = "WinScore";
    private const string LoseScoreKey = "LoseScore";

    public static void SaveScores(int winScore, int loseScore)
    {
        PlayerPrefs.SetInt(WinScoreKey, winScore);
        PlayerPrefs.SetInt(LoseScoreKey, loseScore);
        PlayerPrefs.Save();
    }

    public static int LoadWinScore()
    {
        return PlayerPrefs.GetInt(WinScoreKey, 0);
    }

    public static int LoadLoseScore()
    {
        return PlayerPrefs.GetInt(LoseScoreKey, 0);
    }
}
