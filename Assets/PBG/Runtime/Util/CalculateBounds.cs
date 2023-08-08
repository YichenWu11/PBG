using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PBG.Runtime.Util
{
    public class CalculateBounds : MonoBehaviour
    {
        public GameObject GameObject;

        public void Start()
        {
            if (GameObject != null)
                Debug.Log(Calculate(GameObject).size);
        }

        public static Bounds Calculate(GameObject gameObject, bool includeInactive = false)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactive);

            if (renderers.Length == 0)
                return new Bounds();

            var bounds = renderers[0].bounds;

            for (var i = 1; i < renderers.Length; ++i)
                bounds.Encapsulate(renderers[i].bounds);

            return bounds;
        }
    }
}