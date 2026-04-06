using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public Action<Vector2> OnMoveInput;

    #region Unity Runtime
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

        if (isHoldSprintInput) {
            Debug.Log("Sprinting");
        }
    }

    private void CheckJumpInput() {
        bool isPressJumpInput = Input.GetKeyDown(KeyCode.Space);

        if (isPressJumpInput) {
            Debug.Log("Jump");
        }
    }

    private void CheckCrouchInput() {
        bool isHoldCrouchInput = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);

        if (isHoldCrouchInput) {
            Debug.Log("Crouch");
        }
    }

    private void CheckClimbInput() {
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);

        if (isPressClimbInput) {
            Debug.Log("Climb");
        }
    }

    private void CheckGlideInput() {
        bool isPressGlideInput = Input.GetKeyDown(KeyCode.G);

        if (isPressGlideInput) {
            Debug.Log("Glide");
        }
    }

    private void CheckCancelInput() {
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);

        if (isPressCancelInput) {
            Debug.Log("Cancel Climb or Glide");
        }
    }

    private void CheckChangePOVInput() {
        bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);

        if (isPressChangePOVInput) {
            Debug.Log("Change POV");
        }
    }

    private void CheckPunchInput() {
        bool isPressPunchInput = Input.GetKeyDown(KeyCode.Mouse0);

        if (isPressPunchInput) {
            Debug.Log("Punch");
        }
    }

    private void CheckMainMenuInput() {
        bool isPressMainMenuInput = Input.GetKeyDown(KeyCode.Escape);

        if (isPressMainMenuInput) {
            Debug.Log("Back To Main Menu");
        }
    }
    #endregion
}
