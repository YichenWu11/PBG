using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public PBController Controller;

    public Action<bool> OnEndPointTriggered;

    public UIManager uiManager;
    public string DisplayContent;

    public bool HasBeenTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!HasBeenTriggered)
        {
            Controller.IsEnd = true;
            OnEndPointTriggered?.Invoke(true);
            if (uiManager != null)
                uiManager.StartFadeTextInOut(DisplayContent);
        }

        HasBeenTriggered = true;
    }
}