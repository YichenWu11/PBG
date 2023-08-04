using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PBG.Runtime.Util
{
    public class Grabable : MonoBehaviour
    {
        public JointMotionsConfig JointMotionsCfg;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Grabable");
        }
    }
}