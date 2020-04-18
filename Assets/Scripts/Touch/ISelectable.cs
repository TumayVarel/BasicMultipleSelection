using UnityEngine;
/// <summary>
/// General selectable interface.
/// </summary>
public interface ISelectable
{
    SelectableType SelectableType { get; }

    Transform ThisTransform { get; set; }

    void SetSelected(bool select);
}

/// <summary>
/// Touchable interface for terrain, buildings or other assets that interacts with a touch without being selected.
/// </summary>
public interface INonSelectable
{
    void SetTouch(Vector3 touchPosition);
}

/// <summary>
/// Selectable types to identify their ISelectables.
/// </summary>
public enum SelectableType{ Box, Cylinder };