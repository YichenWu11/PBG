using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public WorldManager WorldMngr;
    public bool HasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!HasBeenTriggered)
            WorldMngr.CurStartPoint = this;

        HasBeenTriggered = true;
    }
}