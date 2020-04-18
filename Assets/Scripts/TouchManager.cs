using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cursorPrefab = null; // Cursor gameo object prefab

    private bool touchedAlready = false;
    private List<ISelectable> touchedObjects = new List<ISelectable>(); // Alrady tocuhed objects.
    private Dictionary<ISelectable, GameObject> characterCursorDictionary = new Dictionary<ISelectable, GameObject>(); // Already selected selectables and their cursor game objects.
    private Queue<GameObject> cursorPool = new Queue<GameObject>(); // Cursor game object pool.
    private List<SelectableType> selectableTypes = new List<SelectableType>(); // Selectable type
    private List<ISelectable> dragSelectables = new List<ISelectable>(); // All selectables in the scene 
    private bool canTouch = true;

    public bool IsDragging { get => dragging; }
    public bool IsTouchedAlready { get => touchedAlready; }

    private void Awake()
    {
        selectableTypes.Add(SelectableType.Box); // Default selectable type, it can be changed.
        SetDragOptions(); // Set drag options.
    }

    /// <summary>
    /// Clears everything and add a selectable to the touched object list and activate a cursor for it. Need to seperate this method from AddTouchedObject since this method is
    /// invoked with a click or with one selectable.
    /// </summary>
    /// <param name="dragSelectable"></param>
    /// <param name="isGeneral"></param>
    public void ChangeTouchedObject(ISelectable dragSelectable, bool isGeneral = false)
    {
        if (!canTouch)
            return;
        if (!selectableTypes.Contains(dragSelectable.SelectableType))
            return;
        ClearAllTouchedOnesAndCursors();
        CheckAndMakeActive(dragSelectable);
        touchedAlready = true;
        touchedObjects.Add(dragSelectable);
    }

    /// <summary>
    /// Add selectable to the touched obkect list and activate a cursor for it.
    /// </summary>
    /// <param name="dragSelectable"></param>
    public void AddTouchedObject(ISelectable dragSelectable)
    {
        if (!selectableTypes.Contains(dragSelectable.SelectableType))
            return;
        CheckAndMakeActive(dragSelectable);
        touchedAlready = true;
        touchedObjects.Add(dragSelectable);
    }

    /// <summary>Clear all previously touched selectables and their added cursors.</summary>
    public void ClearAllTouchedOnesAndCursors()
    {
        foreach (ISelectable selectable in characterCursorDictionary.Keys)
            RemoveCursorInstance(selectable);
        touchedAlready = false;
        touchedObjects.Clear();
        characterCursorDictionary.Clear();
    }

    /// <summary>Remove cursor instance from the previous touched selectable gameobject.</summary><param name="selectable"></param>
    private void RemoveCursorInstance(ISelectable selectable)
    {
        characterCursorDictionary[selectable].SetActive(false);
        characterCursorDictionary[selectable].transform.parent = null;
        cursorPool.Enqueue(characterCursorDictionary[selectable]);
    }

    #region Selectable Types
    /// <summary>Add selectable type. </summary><param name="selectableType"></param>
    public void AddSelectableType(SelectableType selectableType)
    {
        if (!selectableTypes.Contains(selectableType))
            selectableTypes.Add(selectableType);
    }

    /// <summary>Remove selectable type.</summary><param name="selectableType"></param>
    public void RemoveSelectableType(SelectableType selectableType) => selectableTypes.Remove(selectableType);

    #endregion

    #region Selection Functions

    /// <summary>Add selectables to the list of possible selectables.</summary><param name="selectable"></param>
    public void Add(ISelectable selectable) => dragSelectables.Add(selectable);

    /// <summary>Remove selectables from the list of possible selectables.</summary><param name="selectable"></param>
    public void Remove(ISelectable selectable) => dragSelectables.Remove(selectable);

    #endregion

    /// <summary>
    /// Activate a cursor on the selectable.
    /// </summary>
    /// <param name="selectable"></param>
    public void CheckAndMakeActive(ISelectable selectable)
    {
        if (characterCursorDictionary.ContainsKey(selectable))
            return;
        GameObject cursor = AddCursorInstance();
        cursor.SetActive(true);
        cursor.transform.SetParent(selectable.ThisTransform, false);
        characterCursorDictionary.Add(selectable, cursor);
    }

    /// <summary>Instantiate a cursor instance or select it from cursor pool. </summary><returns></returns>
    public GameObject AddCursorInstance()
    {
        GameObject cursorGO = null;
        if (cursorPool.Count > 0)
            cursorGO = cursorPool.Dequeue();
        else
            cursorGO = Instantiate(cursorPrefab);
        cursorGO.transform.position = Vector3.up * 2.3f;
        return cursorGO;
    }


    #region Drag Section

    #region Variables

    private Vector3 clickStart = Vector3.zero;
    private bool dragging = false;

    public delegate void ClickEvent();
    public ClickEvent clickEvent = new ClickEvent(() => { });


    private bool dragSelectedAtLeastOne = false; // Indicates if a selectable is selected while dragging
    private Camera cameraMain = null;

    // GUI
    [SerializeField]
    private Color dragColor = new Color(0.25f, 1, 0.25f, 0.3f); // Drag texture color on the canvas
    private Texture2D dragTexture; // Drag texture will be seen on the canvas
    private Rect guiRect = new Rect(0, 0, 0, 0);

    #endregion

    /// <summary>Set drag texture options that will be drawn with any dragging.</summary>
    public void SetDragOptions()
    {

        dragTexture = new Texture2D(1, 1);
        dragTexture.SetPixel(0, 0, dragColor);
        dragTexture.Apply();
        cameraMain = Camera.main; // There can be more than one camera. The camera where the selection will be done has to be chosen.
    }


    /// <summary>Start dragging with click position.</summary><param name="position"></param>
    public void OnDragStart(Vector3 position)
    {
        if (!canTouch)
            return;
        clickStart = position;
        dragging = true;
        dragSelectedAtLeastOne = false;
    }

    /// <summary>
    /// End of the dragging. If no selectable has been selected, we need to calculate the drag area to deduce the players intent.
    /// The player drags a little bit while he was only trying to click especially with mobile devices on touch screens.
    /// If the dragging area is so small, we can deduce the players intent as a click so we invoke the click event which was delegated
    /// by the touch script invoked the drag event in the first place. If the dragging is big enough, we can clear the previous cursors and touched selectables.
    /// </summary>
    public void OnDragEnd()
    {
        if (!canTouch)
            return;
        if (!dragSelectedAtLeastOne)
        {
            if (guiRect.size.x * guiRect.size.y > 1000) // The offset value can be changed, according to the target platform or screen resolution of the device.
            {
                ClearAllTouchedOnesAndCursors();
                Debug.Log("Big dragging clears the touched selectables.");
            }
            else
            {
                clickEvent?.Invoke();
                clickEvent = () => { };
                Debug.Log("Small dragging is handled like a click.");
            }
        }
        dragging = false;
    }

    /// <summary>
    /// With click start position and current touch position creates a rectangle and add the selectables in that rectangle to the touched objects.
    /// </summary>
    /// <param name="end">Current event data posiiton when dragging</param>
    public void DragSelection(Vector3 end)
    {
        if (!canTouch)
            return;
        Vector3 start = clickStart;
        start.y = Screen.height - start.y; // To set the corret Y value sof the guiRect for GUI.DrawTexture since it takes y resolution reverse.
        end.y = Screen.height - end.y; // To set the corret Y value sof the guiRect for GUI.DrawTexture since it takes y resolution reverse.
        guiRect = new Rect(
                Mathf.Min(start.x, end.x),
                Mathf.Min(start.y, end.y),
                Mathf.Abs(end.x - start.x),
                Mathf.Abs(end.y - start.y));
        for (int i = 0; i < dragSelectables.Count; i++)
        {
            Vector3 pos = cameraMain.WorldToScreenPoint(dragSelectables[i].ThisTransform.position);
            pos.y = Screen.height - pos.y; // To set the corret Y value sof the guiRect for GUI.DrawTexture since it takes y resolution reverse.

            if (guiRect.Contains(pos))
            {
                if (!dragSelectedAtLeastOne) // If no selectables has been selected yet, clear previous touched objects and cursors
                {
                    dragSelectedAtLeastOne = true;
                    ClearAllTouchedOnesAndCursors();
                }
                dragSelectables[i].SetSelected(true);
            }
        }
    }

    /// <summary>Draw the drag texture onto the canvas.</summary>
    protected void OnGUI()
    {
        if (dragging)
        {
            GUI.DrawTexture(guiRect, dragTexture);
        }
    }

    #endregion

}
