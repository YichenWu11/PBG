using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public PBController Controller;

    public Action<bool> OnEndPointTriggered;

    private void OnTriggerEnter(Collider other)
    {
        Controller.IsEnd = true;
        OnEndPointTriggered?.Invoke(true);
    }
}