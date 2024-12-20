using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject[] leaderboardEntries;

    public static Leaderboard instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnLoggedIn ()
    {
        leaderboardCanvas.SetActive(true);
        DisplayLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "FastestTime",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest,
            result => UpdateLeaderboardUI(result.Leaderboard),
            error => Debug.Log(error.ErrorMessage)
        );
    }

    void UpdateLeaderboardUI (List<PlayerLeaderboardEntry> leaderboard)
    {
        for (int x = 0; x < leaderboardEntries.Length; x++)
        {
            leaderboardEntries[x].SetActive(x < leaderboard.Count);
            if (x >= leaderboard.Count) continue;

            leaderboardEntries[x].transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderboardEntries[x].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = (-(float)leaderboard[x].StatValue * 0.001f).ToString("F2");
        }
    }

    public void SetLeaderboardEntry (int newScore)
    {
        bool useLegacyMethod = false;

        if (useLegacyMethod)
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpdateHighscore",
                FunctionParameter = new { score = newScore }
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    Debug.Log(result);
                    DisplayLeaderboard();
                    Debug.Log(result.ToJson());
                },
                error =>
                {
                    Debug.Log(error.ErrorMessage);
                    Debug.Log("ERROR");
                }
            );
        }
        else
        {
            bool hasFastest = false;
            PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                result =>
                {
                    foreach (var eachStat in result.Statistics)
                    {
                        if (eachStat.StatisticName == "FastestTime")
                        {
                            hasFastest = true;
                            Debug.Log("old: " + eachStat.Value + "\nNew: " + newScore);
                            if (eachStat.Value < newScore)
                            {
                                Debug.Log("update");
                                UpdatePlayerStatisics(newScore);
                            }
                        }
                    }
                    if (!hasFastest)
                    {
                        Debug.Log("initialize");
                        UpdatePlayerStatisics(newScore);
                    }
                },
                error => { Debug.Log(error.ErrorMessage); }
            );
        }
    }

    private void UpdatePlayerStatisics(int newScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate {StatisticName = "FastestTime", Value = newScore},
                    }
        },
        result =>
        {
            Debug.Log("User statistics update");
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }
}
