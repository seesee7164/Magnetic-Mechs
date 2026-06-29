using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRebindingUI : MonoBehaviour {

    [SerializeField] private GameObject rebindingOverlay;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI moveDownButtonText;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private TextMeshProUGUI jumpButtonText;
    [SerializeField] private TextMeshProUGUI fireButtonText;
    [SerializeField] private TextMeshProUGUI launchMagnetButtonText;
    [SerializeField] private TextMeshProUGUI attractButtonText;
    [SerializeField] private TextMeshProUGUI repelButtonText;
    [SerializeField] private TextMeshProUGUI recoverMagnetButtonText;
    [SerializeField] private Toggle holdToAttractToggle;
    private Image currentButtonImage;

    [Header("Components")]
    public ButtonSelectionManager buttonSelectionManager;

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
        ChangeToWhite();
        //rebindingOverlay.SetActive(false);
    }

    private void InputRebinding_OnHoldToAttractChanged(object sender, System.EventArgs e) {
        UpdateHoldToAttract();
    }

    public void RebindMoveDown() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_DOWN);
        buttonSelectionManager.SetCurrentButton(2);
    }

    public void RebindMoveLeft() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_LEFT);
        buttonSelectionManager.SetCurrentButton(0);
    }

    public void RebindMoveRight() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.MOVE_RIGHT);
        buttonSelectionManager.SetCurrentButton(1);
    }

    public void RebindJump() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.JUMP);
        buttonSelectionManager.SetCurrentButton(3);
    }

    public void RebindFire() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.FIRE);
        buttonSelectionManager.SetCurrentButton(5);
    }

    public void RebindLaunchMagnet() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.LAUNCH_MAGNET);
        buttonSelectionManager.SetCurrentButton(4);
    }

    public void RebindAttract() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.ATTRACT);
        buttonSelectionManager.SetCurrentButton(7);
    }

    public void RebindRepel() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.REPEL);
        buttonSelectionManager.SetCurrentButton(6);
    }

    public void RebindRecoverMagnet() {
        InputRebinding.Instance.RebindBinding(InputRebinding.Binding.RECOVER_MAGNET);
        buttonSelectionManager.SetCurrentButton(8);
        Debug.Log("test");
    }

    public void ResetBindings() => InputRebinding.Instance.ResetAllBindings();

    public void ToggleHoldToAttract() {
        InputRebinding.Instance.SetHoldToAttract(!InputRebinding.Instance.GetHoldToAttract());
    }

    public void UpdateBindingTexts() {
        moveDownButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_DOWN);
        moveLeftButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_LEFT);
        moveRightButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.MOVE_RIGHT);
        jumpButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.JUMP);
        fireButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.FIRE);
        launchMagnetButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.LAUNCH_MAGNET);
        attractButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.ATTRACT);
        repelButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.REPEL);
        recoverMagnetButtonText.text = InputRebinding.Instance.GetBinding(InputRebinding.Binding.RECOVER_MAGNET);
    }

    public void UpdateHoldToAttract() {
        holdToAttractToggle.SetIsOnWithoutNotify(InputRebinding.Instance.GetHoldToAttract());
    }
    public void ChangeToGrey(Image myImage)
    {
        myImage.color = Color.grey;
        currentButtonImage = myImage;
    }
    public void ChangeToWhite()
    {
        if (currentButtonImage == null)
        {
            Debug.Log("No image to change to white");
            return;
        }
        currentButtonImage.color = Color.white;
        currentButtonImage = null;
    }
}
