using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Action OnChangePerspective;

    [SerializeField] private InputManager _inputManager;
    [SerializeField] public CameraState CameraState;
    [SerializeField] private CinemachineCamera _fpsCamera;
    [SerializeField] private CinemachineCamera _tpsCamera;

    private void OnEnable() {
        _inputManager.OnChangePOVInput += SwitchCamera;
    }

    private void OnDisable() {
        _inputManager.OnChangePOVInput -= SwitchCamera;
    }

    private void SwitchCamera() {
        if (CameraState == CameraState.ThirdPerson) {
            CameraState = CameraState.FirstPerson;
            _tpsCamera.gameObject.SetActive(false);
            _fpsCamera.gameObject.SetActive(true);
        }
        else {
            CameraState = CameraState.ThirdPerson;
            _fpsCamera.gameObject.SetActive(false); 
            _tpsCamera.gameObject.SetActive(true);
        }

        OnChangePerspective?.Invoke();
    }

    public void SetFPSClampedCamera(bool isClamped, Vector3 playerRotation) {
        CinemachinePanTilt panTilt = _fpsCamera.GetComponent<CinemachinePanTilt>();

        if (isClamped) {
            panTilt.PanAxis.Wrap = false;
            panTilt.PanAxis.Range = new Vector2(playerRotation.y - 45, playerRotation.y + 45);
        } else {
            panTilt.PanAxis.Wrap = true;
            panTilt.PanAxis.Range = new Vector2(-180, 180);
        }
    }

    public void SetTPSFieldOfView(float fieldOfView) {
        _tpsCamera.Lens.FieldOfView = fieldOfView;
    }
}
