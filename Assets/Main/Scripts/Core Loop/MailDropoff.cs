using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailDropoff : MonoBehaviour
{
    public int id;
    private bool mailObtained = false;
    public GameObject indicator;

    void Start()
    {
        SingletonHolder.AddMailDropoff(id, this);
    }

    void OnTriggerEnter()
    {
        if(mailObtained == true)
        {
            SingletonHolder.GetManager().DeliveredMail(id);
            indicator.SetActive(false);
        }
    }

    public void TurnOn()
    {
        indicator.SetActive(true);
        mailObtained = true;
    }
}
