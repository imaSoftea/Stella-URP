using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  SingletonHolder : MonoBehaviour
{
    // Start Variations
    public DeliveryManager manager;
    public Dictionary<int, MailPoint> mailPoints;
    public Dictionary<int, MailDropoff> mailDropoffs;

    // Static Variations
    private static SingletonHolder instance;

    void Awake()
    {
        instance = this;
        mailPoints = new Dictionary<int, MailPoint>();
        mailDropoffs = new Dictionary<int, MailDropoff>();
    }

    public static DeliveryManager GetManager()
    {
        return instance.manager;
    }

    public static MailPoint GetMailPoint(int id)
    {
        return instance.mailPoints[id];
    }

    public static MailDropoff GetMailDropoff(int id)
    {
        return instance.mailDropoffs[id];
    }

    public static void AddMailPoint(int id, MailPoint point)
    {
        instance.mailPoints[id] = point;
    }

    public static void AddMailDropoff(int id, MailDropoff dropoff)
    {
        instance.mailDropoffs[id] = dropoff;
    }   
}
