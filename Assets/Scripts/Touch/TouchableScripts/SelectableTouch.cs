using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableTouch : Touchable, ISelectable
{

    public Transform ThisTransform { get; set; }

    [SerializeField]
    private SelectableType _selectableType = SelectableType.Box;

    public SelectableType SelectableType { get => _selectableType; }

    private void Awake()
    {
        ThisTransform = transform;
    }

    /// <summary> Need to add selectable to the selectable list of TouchManager when instantiated or activated in the scene.</summary>
    private void Start()
    {
        TouchManager.Add(this);
    }

    /// <summary> Need to remove from the selectable list of TouchManager when destroyed or disabled in the scene.</summary>
    private void OnDisable()
    {
        TouchManager?.Remove(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        TouchManager.ChangeTouchedObject(this);
    }


    public void SetSelected(bool select) => TouchManager.AddTouchedObject(this);

}
