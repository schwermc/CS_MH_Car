using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<CarController> cars = new List<CarController>();
    public Transform[] spawnPoints;
    public CarController[] players;
    int playersInGame;
    public string playerPrefabLocation;

    public float positionUpdateRate = 0.05f;
    private float lastPositionUpdateTime;

    public bool gameStarted = false;

    public int playersToBegin = 2;
    public int lapsToWin = 3;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length);
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        // Instantiate the player across the nertwork
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // Get the player script
        CarController playerScript = playerObj.GetComponent<CarController>();

        // Initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    void Update()
    {
        // update the car race positions
        if (Time.time - lastPositionUpdateTime > positionUpdateRate)
        {
            lastPositionUpdateTime = Time.time;
            UpdateCarRacePositions();
        }

        // start the countdown when all cars are ready
        if (!gameStarted && cars.Count == playersToBegin)
        {
            gameStarted = true;
            StartCountdown();
        }
    }

    // called when all players in in-game and ready to begin
    void StartCountdown()
    {
        PlayerUI[] uis = FindObjectsOfType<PlayerUI>();

        for (int x = 0; x < uis.Length; ++x)
            uis[x].StartCountdownDisplay();

        Invoke("BeginGame", 3.0f);
    }

    // called after the countdown has ended and players can now race
    void BeginGame()
    {
        for (int x = 0; x < cars.Count; ++x)
        {
            cars[x].canControl = true;
        }
    }

    [PunRPC]
    // updates which car is coming first, second, etc
    void UpdateCarRacePositions()
    {
        if (cars.Count > 1)
        {
            cars.Sort(SortPosition);
        }

        for (int x = 0; x < cars.Count; x++)
        {
            cars[x].racePosition = cars.Count - x;
        }
    }
    int SortPosition(CarController a, CarController b)
    {
        if (a.zonesPassed > b.zonesPassed)
            return 1;
        else if (b.zonesPassed > a.zonesPassed)
            return -1;

        if (a.curTrackZone != null && b.curTrackZone != null)
        {
            float aDist = Vector3.Distance(a.transform.position, a.curTrackZone.transform.position);
            float bDist = Vector3.Distance(b.transform.position, b.curTrackZone.transform.position);

            return aDist > bDist ? 1 : -1;
        }
        return 0;
    }

    [PunRPC]
    // called when a car has crossed the finish line
    public void CheckIsWinner(CarController car)
    {
        if (car.curLap == lapsToWin + 1)
        {
            for (int x = 0; x < cars.Count; ++x)
            {
                cars[x].canControl = false;
            }

            PlayerUI[] uis = FindObjectsOfType<PlayerUI>();

            for (int x = 0; x < uis.Length; ++x)
                uis[x].GameOver(uis[x].car == car);

            Invoke("GoBackToMenu", 3.0f);
        }
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}