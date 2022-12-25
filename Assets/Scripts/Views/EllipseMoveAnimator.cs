using UnityEngine;

namespace GUIGUI17F
{
    public class EllipseMoveAnimator : MonoBehaviour
    {
        private const float ClampUnit = Mathf.PI * 2;

        public Transform MoveTarget;

        [Tooltip("MoveCenter need under the same parent as MoveTarget")]
        public Transform MoveCenter;

        public float HorizontalExtend;
        public float VerticalExtend;
        public float MoveSpeed;

        private bool _enable;
        private Vector3 _centerPosition;
        private Vector3 _targetPosition;
        private float _radianSpeed;
        private float _radianAngle;

        public void ToggleEnable(bool enable)
        {
            if (enable)
            {
                _centerPosition = MoveCenter.localPosition;
                _targetPosition = MoveTarget.localPosition;
                _radianSpeed = MoveSpeed * Mathf.Deg2Rad;
                _radianAngle = Vector3.SignedAngle(Vector3.right, _targetPosition - _centerPosition, Vector3.forward) * Mathf.Deg2Rad;
            }
            _enable = enable;
        }

        private void Update()
        {
            if (_enable)
            {
                _radianAngle += _radianSpeed;
                _radianAngle %= ClampUnit;
                _targetPosition.x = _centerPosition.x + Mathf.Cos(_radianAngle) * HorizontalExtend;
                _targetPosition.y = _centerPosition.y + Mathf.Sin(_radianAngle) * VerticalExtend;
                MoveTarget.localPosition = _targetPosition;
            }
        }
    }
}