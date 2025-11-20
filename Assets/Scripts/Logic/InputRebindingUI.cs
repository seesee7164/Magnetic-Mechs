using UnityEngine;
using UnityEngine.UI;

public class InputRebindingUI : MonoBehaviour {

    [SerializeField] private GameObject rebindingOverlay;

    [Header("Buttons")]
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button launchMagnetButton;
    [SerializeField] private Button attractButton;
    [SerializeField] private Button repelButton;
    [SerializeField] private Button chargeButton;

    [Header("Buttons Texts")]
    [SerializeField] private Text moveUpButtonText;
    [SerializeField] private Text moveDownButtonText;
    [SerializeField] private Text moveLeftButtonText;
    [SerializeField] private Text moveRightButtonText;
    [SerializeField] private Text jumpButtonText;
    [SerializeField] private Text fireButtonText;
    [SerializeField] private Text launchMagnetButtonText;
    [SerializeField] private Text attractButtonText;
    [SerializeField] private Text repelButtonText;
    [SerializeField] private Text chargeButtonText;

    private void Start() {
        InputRebinding.Instance.OnInputRebindingCompleted += InputRebinding_OnInputRebindingCompleted;
        InputRebinding.Instance.OnInputRebindingStarted += InputRebinding_OnInputRebindingStarted;
        //rebindingOverlay.SetActive(false);
        UpdateBindingTexts();
    }

    private void OnDestroy() {
        InputRebinding.Instance.OnInputRebindingCompleted -= InputRebinding_OnInputRebindingCompleted;
        InputRebinding.Instance.OnInputRebindingStarted -= InputRebinding_OnInputRebindingStarted;
    }

    private void InputRebinding_OnInputRebindingStarted(object sender, System.EventArgs e) {
        //rebindingOverlay.SetActive(true);
    }

    private void InputRebinding_OnInputRebindingCompleted(object sender, System.EventArgs e) {
        UpdateBindingTexts();
        //rebindingOverlay.SetActive(false);
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
}
