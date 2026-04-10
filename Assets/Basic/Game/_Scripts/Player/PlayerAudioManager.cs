using UnityEngine;

public class PlayerAudioManager : MonoBehaviour {
    [SerializeField] private AudioSource _footstepSfx;
    [SerializeField] private AudioSource _glideSfx;
    [SerializeField] private AudioSource _landingSfx;
    [SerializeField] private AudioSource _punchSfx;

    private void PlayFootstepSfx(AnimationEvent evt) {
        if (evt.animatorClipInfo.weight > .5f) {
            _footstepSfx.volume = Random.Range(0.7f, 1f);
            _footstepSfx.pitch = Random.Range(0.5f, 2.5f);
            _footstepSfx.Play();
        }
    }

    public void PlayGlideSfx() {
        _glideSfx.Play();
    }

    public void StopGlideSfx() {
        _glideSfx.Stop();
    }

    private void PlayLandingSfx() {
        _landingSfx.Play();
    }

    private void PlayPunchSfx() {
        _footstepSfx.volume = Random.Range(0.7f, 1f);
        _footstepSfx.pitch = Random.Range(0.75f, 1.25f);
        _footstepSfx.Play();
    }
}
