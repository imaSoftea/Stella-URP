using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartingArea : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        TurnOff();
        SingletonHolder.GetManager().StartGame();
    }

    private void TurnOff()
    {

    }
}
