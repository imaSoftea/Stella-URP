using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailPoint : MonoBehaviour
{
    public int id;

    void Start()
    {
        SingletonHolder.AddMailPoint(id, this);
    }

    void OnTriggerEnter()
    {
        SingletonHolder.GetManager().ObtainedMail(id);
        SingletonHolder.GetMailDropoff(id).TurnOn();
        Destroy(this.gameObject);
    }
}
