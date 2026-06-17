using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRebindingUI : MonoBehaviour {

    [SerializeField] private GameObject rebindingOverlay;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI moveUpButtonText;
    [SerializeField] private TextMeshProUGUI moveDownButtonText;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private TextMeshProUGUI jumpButtonText;
    [SerializeField] private TextMeshProUGUI fireButtonText;
    [SerializeField] private TextMeshProUGUI launchMagnetButtonText;
    [SerializeField] private TextMeshProUGUI attractButtonText;
    [SerializeField] private TextMeshProUGUI repelButtonText;
    [SerializeField] private TextMeshProUGUI chargeButtonText;
    [SerializeField] private Toggle holdToAttractToggle;

    private void Start() {
        InputRebinding.Instance.OnInputRebindingCompleted += InputRebinding_OnInputRebindingCompleted;
        InputRebinding.Instance.OnInputRebindingStarted += InputRebinding_OnInputRebindingStarted;
        InputRebinding.Instance.OnHoldToAttractChanged += InputRebinding_OnHoldToAttractChanged;
        //rebindingOverlay.SetActive(false);
        UpdateBindingTexts();
        UpdateHoldToAttract();
    }

    private void OnDestroy() {
        InputRebinding.Instance.OnInputRebindingCompleted -= InputRebinding_OnInputRebindingCompleted;
        InputRebinding.Instance.OnInputRebindingStarted -= InputRebinding_OnInputRebindingStarted;
        InputRebinding.Instance.OnHoldToAttractChanged -= InputRebinding_OnHoldToAttractChanged;
    }

    private void InputRebinding_OnInputRebindingStarted(object sender, System.EventArgs e) {
        //rebindingOverlay.SetActive(true);
    }

    private void InputRebinding_OnInputRebindingCompleted(object sender, System.EventArgs e) {
        UpdateBindingTexts();
        //rebindingOverlay.SetActive(false);
    }

    private void InputRebinding_OnHoldToAttractChanged(object sender, System.EventArgs e) {
        UpdateHoldToAttract();
    }

    public void RebindMoveUp() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_UP);
    }

    public void RebindMoveDown() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_DOWN);
    }

    public void RebindMoveLeft() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_LEFT);
    }

    public void RebindMoveRight() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_RIGHT);
    }

    public void RebindJump() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.JUMP);
    }

    public void RebindFire() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.FIRE);
    }

    public void RebindLaunchMagnet() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.LAUNCH_MAGNET);
    }

    public void RebindAttract() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.ATTRACT);
    }

    public void RebindRepel() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.REPEL);
    }

    public void RebindCharge() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.CHARGE);
    }

    public void ResetBindings() => InputRebinding.Instance.ResetAllBindings();

    public void ToggleHoldToAttract() {
        InputRebinding.Instance.SetHoldToAttract(!InputRebinding.Instance.GetHoldToAttract());
    }

    public void UpdateBindingTexts() {
        moveUpButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_UP);
        moveDownButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_DOWN);
        moveLeftButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_LEFT);
        moveRightButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_RIGHT);
        jumpButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.JUMP);
        fireButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.FIRE);
        launchMagnetButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.LAUNCH_MAGNET);
        attractButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.ATTRACT);
        repelButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.REPEL);
        chargeButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.CHARGE);
    }

    public void UpdateHoldToAttract() {
        holdToAttractToggle.SetIsOnWithoutNotify(InputRebinding.Instance.GetHoldToAttract());
    }
}
