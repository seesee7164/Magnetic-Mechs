using UnityEngine;

public class ButtonSelectionParent : MonoBehaviour
{
    public int currentSelection = 0;

    public void SetCurrentButton(int newButton)
    {
        currentSelection = newButton;
        SetButtonSize(currentSelection);
    }
    public virtual void SetButtonSize(int currentSelection)
    {
        Debug.Log("ButtonSelection no Override");
    }
}
