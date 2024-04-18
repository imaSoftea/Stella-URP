using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

public class DeliveryManager : MonoBehaviour
{

    // Timer
    public float maxTimeLeft = 240;
    private float currentTimeLeft = 0;
    private bool gameInProgress = false;

    // Mail Collected
    private int countLeft = 0;
    private List<int> collectedMailIds;

    void Start()
    {
        currentTimeLeft = maxTimeLeft;
        collectedMailIds = new List<int>();
    }

    void Update()
    {
        if(gameInProgress)
        {
            currentTimeLeft = Time.deltaTime;
        }
        if(currentTimeLeft < 0)
        {
            GameLost();
        }
    }

    public void AddMail()
    {
        countLeft += 1;
    }

    public void ObtainedMail(int id)
    {
        collectedMailIds.Add(id);
    }

    public void DeliveredMail(int id)
    {
        collectedMailIds.Remove(id);

        countLeft -= 1;
        if(countLeft <= 0)
        {
            GameWon();
        }
    }

    public void StartGame()
    {
        gameInProgress = true;
    }

    private void GameWon()
    {
        gameInProgress = false;
    }

    private void GameLost()
    {
        gameInProgress = false;
    }

    public int GetCurrentTime()
    {
        return (int) currentTimeLeft;
    }
}
