using UnityEngine;

public class TutorialTrigger : MonoBehaviour {
    [SerializeField] private TutorialView _tutorialView;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _tutorialView.Show();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            _tutorialView.Hide(); 
        }
    }
}
