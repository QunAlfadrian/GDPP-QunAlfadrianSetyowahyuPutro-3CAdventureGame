using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCancelClimbInput;
    public Action OnCrouchInput;
    public Action OnGlideInput;
    public Action OnCancelGlide;
    public Action OnChangePOVInput;
    public Action OnPunchInput;
    public Action OnMainMenuInput;

    [SerializeField] private int _targetFrameRate = 30;
    [SerializeField] private bool _hideCursor = true;

    #region Unity Runtime
    private void Awake() {
        HideAndLockCursor();
        SetTargetFrameRate();
    }

    private void Update() {
        CheckMovementInput();
        CheckSprintInput();
        CheckJumpInput();
        CheckCrouchInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckGlideInput();
        CheckCancelInput();
        CheckPunchInput();
        CheckMainMenuInput();
    }
    #endregion

    #region Movement Check
    private void CheckMovementInput() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);

        OnMoveInput?.Invoke(inputVector);
    }

    private void CheckSprintInput() {
        bool isHoldSprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        OnSprintInput?.Invoke(isHoldSprintInput);
    }

    private void CheckJumpInput() {
        bool isPressJumpInput = Input.GetKeyDown(KeyCode.Space);

        if (isPressJumpInput) {
            OnJumpInput?.Invoke();
        }
    }

    private void CheckCrouchInput() {
        bool isHoldCrouchInput = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);

        if (isHoldCrouchInput) {
            OnCrouchInput?.Invoke();
        }
    }

    private void CheckClimbInput() {
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);

        if (isPressClimbInput) {
            OnClimbInput?.Invoke();
        }
    }

    private void CheckCancelInput() {
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);

        if (isPressCancelInput) {
            OnCancelClimbInput?.Invoke();
            OnCancelGlide?.Invoke();
        }
    }

    private void CheckGlideInput() {
        bool isPressGlideInput = Input.GetKeyDown(KeyCode.G);

        if (isPressGlideInput) {
            OnGlideInput?.Invoke();
        }
    }

    private void CheckChangePOVInput() {
        bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);

        if (isPressChangePOVInput) {
            OnChangePOVInput?.Invoke();
        }
    }

    private void CheckPunchInput() {
        bool isPressPunchInput = Input.GetKeyDown(KeyCode.Mouse0);

        if (isPressPunchInput) {
            OnPunchInput?.Invoke();
        }
    }

    private void CheckMainMenuInput() {
        bool isPressMainMenuInput = Input.GetKeyDown(KeyCode.Escape);

        if (isPressMainMenuInput) {
            OnMainMenuInput?.Invoke();
        }
    }
    #endregion

    private void HideAndLockCursor() {
        if (_hideCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SetTargetFrameRate() {
        if (_targetFrameRate > 0) {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}
