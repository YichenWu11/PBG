using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime
{
    public class StartPoint : MonoBehaviour
    {
        public WorldManager WorldMngr;
        public UIManager uiManager;
        public string DisplayContent;

        public bool HasBeenTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!HasBeenTriggered)
            {
                WorldMngr.CurStartPoint = this;
                if (uiManager != null)
                    uiManager.StartFadeTextInOut(DisplayContent);
            }

            HasBeenTriggered = true;
        }
    }
}