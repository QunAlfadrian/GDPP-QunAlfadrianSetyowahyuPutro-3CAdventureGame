using UnityEngine;

public class ResetCheckpoint : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerMovement>().ResetPositionToCheckpoint();
        }
    }
}
