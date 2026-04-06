using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private float _walkSpeed;

    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        _inputManager.OnMoveInput += Move;
    }

    private void OnDestroy() {
        _inputManager.OnMoveInput -= Move;
    }

    private void Move(Vector2 inputVector) {
        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        _rigidbody.AddForce(moveDir * _walkSpeed * Time.deltaTime);
    }
}
