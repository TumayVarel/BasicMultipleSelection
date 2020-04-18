using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainTouch : Touchable, INonSelectable
{

    /// <summary>
    /// If dragging then no need to invoke SetTouch. If dragging and already some selectables have been selected then clickEvent delegates the SetTouch.
    /// If dragging and no selectable has been selected then clickEvent clear everything.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (TouchManager.IsDragging)
        {
            // If there will be too many different delegates with touch objects, command pattern can be used to keep track of them.
            if (TouchManager.IsTouchedAlready)
                TouchManager.clickEvent = () => SetTouch(eventData.pointerCurrentRaycast.worldPosition);
            else
                TouchManager.clickEvent = () => TouchManager.ClearAllTouchedOnesAndCursors();
            return;
        }
        SetTouch(eventData.pointerCurrentRaycast.worldPosition);
    }

    /// <summary>
    /// If some selectable have been selected then do something with them. If no selectable has been selected then clear.
    /// </summary>
    /// <param name="touchPosition"></param>
    public void SetTouch(Vector3 touchPosition)
    {
        if (TouchManager.IsTouchedAlready)
            Debug.Log("Do something with already touched selectables."); // Do something
        else
            TouchManager.ClearAllTouchedOnesAndCursors();
    }


}
