using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableTypeToogles : MonoBehaviour
{
    private TouchManager touchManager;

    private void Awake()
    {
        touchManager = FindObjectOfType<TouchManager>();
    }

    public void BoxToggle(Toggle toggle)
    {
        if(toggle.isOn)
            touchManager.AddSelectableType(SelectableType.Box);
        else
            touchManager.RemoveSelectableType(SelectableType.Box);
    }

    public void CylinderToggle(Toggle toggle)
    {
        if (toggle.isOn)
            touchManager.AddSelectableType(SelectableType.Cylinder);
        else
            touchManager.RemoveSelectableType(SelectableType.Cylinder);
    }
}
