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

    #region Unity Runtime
    private void Awake() {
        //HideAndLockCursor();
        Application.targetFrameRate = 60;
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

        if (isHoldSprintInput) {
            Debug.Log("Sprinting");
        }
    }

    private void CheckJumpInput() {
        bool isPressJumpInput = Input.GetKeyDown(KeyCode.Space);

        if (isPressJumpInput) {
            OnJumpInput?.Invoke();

            Debug.Log("Jump");
        }
    }

    private void CheckCrouchInput() {
        bool isHoldCrouchInput = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);

        if (isHoldCrouchInput) {
            OnCrouchInput?.Invoke();

            Debug.Log("Crouch");
        }
    }

    private void CheckClimbInput() {
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);

        if (isPressClimbInput) {
            OnClimbInput?.Invoke();

            Debug.Log("Climb");
        }
    }

    private void CheckCancelInput() {
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);

        if (isPressCancelInput) {
            OnCancelClimbInput?.Invoke();
            OnCancelGlide?.Invoke();

            Debug.Log("Cancel Climb or Glide");
        }
    }

    private void CheckGlideInput() {
        bool isPressGlideInput = Input.GetKeyDown(KeyCode.G);

        if (isPressGlideInput) {
            OnGlideInput?.Invoke();

            Debug.Log("Glide");
        }
    }

    private void CheckChangePOVInput() {
        bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);

        if (isPressChangePOVInput) {
            OnChangePOVInput?.Invoke();

            Debug.Log("Change POV");
        }
    }

    private void CheckPunchInput() {
        bool isPressPunchInput = Input.GetKeyDown(KeyCode.Mouse0);

        if (isPressPunchInput) {
            OnPunchInput?.Invoke();

            Debug.Log("Punch");
        }
    }

    private void CheckMainMenuInput() {
        bool isPressMainMenuInput = Input.GetKeyDown(KeyCode.Escape);

        if (isPressMainMenuInput) {
            OnMainMenuInput?.Invoke();

            Debug.Log("Back To Main Menu");
        }
    }
    #endregion

    private void HideAndLockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
