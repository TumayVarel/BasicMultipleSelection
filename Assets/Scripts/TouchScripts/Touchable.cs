using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Touchable : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private static TouchManager _touchManager;

    protected TouchManager TouchManager
    {
        get
        {
            if (_touchManager == null)
                _touchManager = FindObjectOfType<TouchManager>();
            return _touchManager;
        }
    }

    public abstract void OnPointerClick(PointerEventData eventData);

    public virtual void OnBeginDrag(PointerEventData eventData) => TouchManager.OnDragStart(eventData.position);

    public virtual void OnDrag(PointerEventData eventData) => TouchManager.DragSelection(eventData.position);

    public virtual void OnEndDrag(PointerEventData eventData) => TouchManager.OnDragEnd();
}
