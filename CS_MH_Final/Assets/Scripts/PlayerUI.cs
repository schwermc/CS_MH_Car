using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI carPositionText;
    public TextMeshProUGUI lapsDown;
    public CarController car;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameOverText;

    void Update()
    {
        carPositionText.text = car.racePosition.ToString() + " / " + GameManager.instance.cars.Count.ToString();
        lapsDown.text = "Laps: " + (car.curLap - 1).ToString() + "/" + GameManager.instance.lapsToWin.ToString();
    }
    public void StartCountdownDisplay()
    {
        StartCoroutine(Countdown());

        IEnumerator Countdown()
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "2";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "1";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "GO!";
            yield return new WaitForSeconds(1.0f);
            countdownText.gameObject.SetActive(false);
        }
    }
    public void GameOver(bool winner)
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.color = winner == true ? Color.green : Color.red;
        gameOverText.text = winner == true ? "You Win" : "You Lost";
    }
}