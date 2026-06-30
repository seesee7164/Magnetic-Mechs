using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableCanvasGroupWhileRebinding : MonoBehaviour {


    [SerializeField] private CanvasGroup group;


    private void Start() {
        InputRebinding.Instance.OnInputRebindingStarted += InputRebinding_OnInputRebindingStarted;
        InputRebinding.Instance.OnInputRebindingCompleted += InputRebinding_OnInputRebindingCompleted;
    }

    private void OnDestroy() {
        InputRebinding.Instance.OnInputRebindingStarted -= InputRebinding_OnInputRebindingStarted;
        InputRebinding.Instance.OnInputRebindingCompleted -= InputRebinding_OnInputRebindingCompleted;
    }

    private void InputRebinding_OnInputRebindingStarted(object sender, System.EventArgs e) {
        group.interactable = false;
    }

    private void InputRebinding_OnInputRebindingCompleted(object sender, System.EventArgs e) {
        StartCoroutine(EnableGroupAfterDelay());
    }

    private IEnumerator EnableGroupAfterDelay() {
        yield return new WaitForSecondsRealtime(0.1f);
        group.interactable = true;
    }

}
