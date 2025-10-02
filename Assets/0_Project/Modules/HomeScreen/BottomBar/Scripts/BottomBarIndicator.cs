using DG.Tweening;
using UnityEngine;

namespace TD.HomeScreen.BottomBar
{
    /// <summary>
    /// A UI indicator that follows a target RectTransform along the X axis.
    /// It can smoothly follow the target using either SmoothDamp or Spring physics. 
    /// </summary>
    /// <remarks>
    /// Does not rely on duration-based tweening for following.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class BottomBarIndicator : MonoBehaviour
    {
        public enum FollowMode { SmoothDamp, Spring }
        public enum TimeMode { Scaled, Unscaled }

        #region Serialized Fields

        [Header("Behavior")]
        [Tooltip("Determines how the indicator follows its target.")]
        [SerializeField] private FollowMode followMode = FollowMode.SmoothDamp;
        [Tooltip("Determines whether time is scaled or unscaled for motion and animations.")]
        [SerializeField] private TimeMode timeMode = TimeMode.Unscaled;

        [Header("Follow (SmoothDamp)")]
        [Tooltip("Approximate time to reach the target. Lower values = faster.")]
        [SerializeField] private float smoothDampTime = 0.16f;
        [Tooltip("Maximum speed for SmoothDamp. Set to 0 for no limit.")]
        [SerializeField] private float smoothDampSpeed = 2500f;

        [Header("Follow (Spring)")]
        [Tooltip("Natural frequency of the spring. Higher values = stiffer spring = faster response.")]
        [SerializeField] private float springFrequency = 12f; // Hz

        [Header("Follow Tuning")]
        [Tooltip("If the target's velocity exceeds this value, apply chase responsiveness.")]
        [SerializeField] private float chaseBoostThreshold = 150f;
        [Tooltip("Multiplier to responsiveness when the target is moving fast.")]
        [SerializeField] private float chaseResponsiveness = 1.25f; 
        [Tooltip("Whether to anticipate the target's motion when following.")]
        [SerializeField] private bool anticipateMotion = true;
        [Tooltip("Maximum velocity of the indicator.")]
        [SerializeField] private float maxVelocity = 10000f;

        [Header("Fade (In/Out)")]
        [Tooltip("Duration of the show/hide animation.")]
        [SerializeField] private float fadeDuration = 0.125f;
        [Tooltip("Easing function for the show/hide animation.")]
        [SerializeField] private Ease fadeEase = Ease.InOutSine;
        [Tooltip("Horizontal scale applied to the hidden size.")]
        [SerializeField, Range(0.0f, 1.0f)] private float hiddenScaleX = 0.5f;
        [Tooltip("Final height when hidden.")]
        [SerializeField] private float hiddenHeight = 0f;

        #endregion

        #region Private Fields

        // Runtime
        private CanvasGroup _group;
        private RectTransform _transform;
        private RectTransform _target;

        // Cached initial size
        private Vector2 _initialSizeDelta;
        private Vector2 _hiddenSizeDelta;

        // Motion state
        private float _velocity;
        private float _previousTargetX; 
        private float _targetVelocity;
        private bool _hadPreviousTargetX;

        // Animation state
        private enum State { Hidden, Showing, Visible, Hiding }
        private State _state = State.Hidden;
        private Sequence _inOutTween;
        private int _tweenVersion; // Invalidates old tween callbacks

        #endregion
        
        #region Public 

        /// <summary>
        /// Set the target RectTransform for the indicator to follow.
        /// </summary>
        /// <remarks>
        /// If the indicator is hidden, it will show itself.
        /// If the target is null, the indicator will hide itself.
        /// </remarks>
        public void SetTarget(RectTransform target)
        {
            if (target == null) return;
            _target = target;

            // Cancel any animations
            _inOutTween?.Kill();
            _inOutTween = null;

            // Reset velocity tracking
            _velocity = 0f; 
            _targetVelocity = 0f; 
            _hadPreviousTargetX = false;

            if (_state is State.Hidden or State.Hiding)
            {
                // Start showing
                PlayShow(_tweenVersion++);
            }
        }

        /// <summary>
        /// Clear the current target and hide the indicator.
        /// </summary>
        /// <remarks>
        /// If already hidden or hiding, does nothing.
        /// </remarks>
        public void ClearTarget()
        {
            if (_state is State.Hidden or State.Hiding)
                return; // already hidden or hiding

            _target = null;
            _velocity = 0f; 
            _targetVelocity = 0f; 
            _hadPreviousTargetX = false;
            
            PlayHide(_tweenVersion++);
        }

        /// <summary>
        /// Immediately hide the indicator and cancel any animations.
        /// </summary>
        /// <remarks>
        /// The indicator will be set to inactive.
        /// </remarks>
        public void HideImmediate()
        {
            _inOutTween?.Kill();
            _inOutTween = null;
            _group.alpha = 0f;
            _transform.sizeDelta = _hiddenSizeDelta;
            gameObject.SetActive(false);
            _state = State.Hidden;
            _target = null;
        }

        #endregion
        
        #region Unity Events

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _group = GetComponent<CanvasGroup>();

            // Cache the startup dimensions exactly once
            _initialSizeDelta = _transform.sizeDelta; // “whatever size it was on start”
            _hiddenSizeDelta = new Vector2(_initialSizeDelta.x * hiddenScaleX, hiddenHeight);

            // Start hidden and collapsed (size, not scale)
            _state = State.Hidden;
            _group.alpha = 0f;
            _transform.sizeDelta = _hiddenSizeDelta;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _inOutTween?.Kill();
            _inOutTween = null;
        }

        /// <summary>
        /// Use LateUpdate to ensure that any target movement in Update is accounted for.
        /// This also helps ensure that if the target is moving due to layout rebuilds, we
        /// get the final position after all layout calculations are done.
        /// </summary>
        private void LateUpdate()
        {
            if (_target == null) 
                return;

            float deltaTime = (timeMode == TimeMode.Unscaled) ? Time.unscaledDeltaTime : Time.deltaTime;
            if (deltaTime <= 0f) 
                return;

            float targetX = GetTargetAnchoredX(_target);
            _targetVelocity = _hadPreviousTargetX ? (targetX - _previousTargetX) / Mathf.Max(0.0001f, deltaTime) : 0f;
            _previousTargetX = targetX;
            _hadPreviousTargetX = true;

            float chaseMultiplier = Mathf.Abs(_targetVelocity) > chaseBoostThreshold ? chaseResponsiveness : 1f;
            Vector2 position = _transform.anchoredPosition;

            switch (followMode)
            {
                case FollowMode.SmoothDamp:
                {
                    float smooth = smoothDampTime / chaseMultiplier;
                    position.x = Mathf.SmoothDamp(position.x, targetX, ref _velocity, smooth, smoothDampSpeed, deltaTime);
                    if (anticipateMotion) 
                        _velocity = Mathf.Lerp(_velocity, _targetVelocity, deltaTime);
                    break;
                }
                case FollowMode.Spring:
                {
                    float w = Mathf.Max(0.01f, springFrequency) * 2f * Mathf.PI * chaseMultiplier;
                    float px = position.x;
                    float pv = _velocity;
                    float tx = targetX;
                    float tv = anticipateMotion ? _targetVelocity : 0f;

                    pv += (w * w * (tx - px) + 2f * w * (tv - pv)) * deltaTime;
                    px += pv * deltaTime;

                    _velocity = pv;
                    position.x = px;
                    break;
                }
            }

            if (maxVelocity > 0f)
                _velocity = Mathf.Clamp(_velocity, -maxVelocity, maxVelocity);

            _transform.anchoredPosition = position;
        }

        #endregion

        #region Tweens

        /// <summary>
        /// Play the show animation.
        /// Uses tweenVersion to ignore stale callbacks if a new tween has started since.
        /// </summary>
        private void PlayShow(int tweenVersion)
        {
            _state = State.Showing;
            MoveToTargetImmediate();
            gameObject.SetActive(true);
            
            _inOutTween?.Kill(); 
            _inOutTween = DOTween.Sequence().SetUpdate(timeMode == TimeMode.Unscaled).SetEase(fadeEase);
            _inOutTween.Join(_transform.DOSizeDelta(_initialSizeDelta, fadeDuration));
            _inOutTween.Join(_group.DOFade(1f, fadeDuration));
            _inOutTween.OnComplete(() =>
            {
                if (tweenVersion != _tweenVersion) return; // stale, ignore
                _state = State.Visible;
            });

            _inOutTween.Play();
        }

        /// <summary>
        /// Play the hide animation.
        /// Uses tweenVersion to ignore stale callbacks if a new tween has started since.
        /// </summary>
        private void PlayHide(int tweenVersion)
        {
            _state = State.Hiding;
            MoveToTargetImmediate();
            
            _inOutTween?.Kill();
            _inOutTween = DOTween.Sequence().SetUpdate(timeMode == TimeMode.Unscaled).SetEase(fadeEase);
            _inOutTween.Join(_transform.DOSizeDelta(_hiddenSizeDelta, fadeDuration));
            _inOutTween.Join(_group.DOFade(0f, fadeDuration));
            _inOutTween.OnComplete(() =>
            {
                if (tweenVersion != _tweenVersion) return; // stale, ignore
                _state = State.Hidden;
                gameObject.SetActive(false);
            });

            _inOutTween.Play();
        }

        #endregion

        #region Utilities
        
        private void MoveToTargetImmediate()
        {
            if (_target == null) return;
            Vector2 position = _transform.anchoredPosition;
            position.x = GetTargetAnchoredX(_target);
            _transform.anchoredPosition = position;
            _velocity = 0f;
            _targetVelocity = 0f;
            _hadPreviousTargetX = false;
        }

        /// <summary>
        /// Get the anchored X position of the target relative to this indicator's parent.
        /// If target is null, returns the current anchored X position of the indicator.
        /// </summary>
        private float GetTargetAnchoredX(RectTransform target)
        {
            if (target == null) return _transform.anchoredPosition.x;
            RectTransform parent = (RectTransform)_transform.parent;
            Vector3 worldCenter = target.TransformPoint(target.rect.center);
            Vector3 localInParent = parent.InverseTransformPoint(worldCenter);
            return localInParent.x;
        }

        #endregion
    }
}