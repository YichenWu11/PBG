using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PBG.Runtime.Util;
using UnityEngine;

public class NButton : MonoBehaviour
{
    public List<ButtonTrigger> AllTriggers;
    public List<bool> DownLists;

    public Action<bool> OnNButtonTriggered;

    public GameObject Box;
    private bool m_HasBox = false;

    private void Start()
    {
        OnNButtonTriggered += ProduceBox;
        foreach (var button in AllTriggers)
            button.OnButtonTriggered += CheckAndInvoke;
    }

    private void CheckAndInvoke(bool value)
    {
        var ok =
            !AllTriggers.Where((t, i) => t.IsDown != DownLists[i]).Any();
        if (ok)
            OnNButtonTriggered?.Invoke(true);
    }

    private void ProduceBox(bool value)
    {
        if (!m_HasBox)
        {
            var box = Instantiate(Box);
            box.transform.position = transform.position;
            box.AddComponent<RecycleObject>();
        }

        m_HasBox = true;
    }
}