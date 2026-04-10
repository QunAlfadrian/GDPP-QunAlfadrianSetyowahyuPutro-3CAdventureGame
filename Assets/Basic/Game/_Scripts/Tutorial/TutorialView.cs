using DG.Tweening;
using UnityEngine;

public class TutorialView : MonoBehaviour {
    [SerializeField] private bool _showOnStart;
    [SerializeField] private float _tweenDuration = 0.25f;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private float _showDestination;
    private float _hideDestination;
    private Tween _tween;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        _canvas = GetComponentInParent<Canvas>();
        CalculateMoveDestination();

        if (_showOnStart) {
            Show();
        }
    }

    private void CalculateMoveDestination() {
        float moveAmount = _rectTransform.rect.width / _canvas.scaleFactor;
        _showDestination = _rectTransform.localPosition.x + moveAmount * -1;
        _hideDestination = _rectTransform.localPosition.x + moveAmount;
    }

    public void Show() {
        if (_tween != null && _tween.IsActive()) {
            _tween.Kill();
        }

        _tween = _rectTransform.DOLocalMoveX(_showDestination, _tweenDuration);
    }

    public void Hide() {
        if (_tween != null && _tween.IsActive()) {
            _tween.Kill();
        }

        _rectTransform.DOLocalMoveX(_hideDestination, _tweenDuration);
    }
}
