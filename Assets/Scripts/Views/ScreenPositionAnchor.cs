using UnityEngine;

namespace GUIGUI17F
{
    public class ScreenPositionAnchor : MonoBehaviour
    {
        public Camera BaseCamera;
        public Transform Target;
        public Vector2 ViewportPosition;
        public bool IgnoreSafeArea;

        private int _screenWidth;
        private int _screenHeight;
        private bool _shouldUpdate;

        private void Start()
        {
            if (BaseCamera == null)
            {
                BaseCamera = Camera.main;
            }
            if (Target == null)
            {
                Target = transform;
            }
            UpdatePosition();
        }

        private void Update()
        {
            if (BaseCamera == null)
            {
                BaseCamera = Camera.main;
                _shouldUpdate = true;
            }
            else if (_screenWidth != Screen.width || _screenHeight != Screen.height)
            {
                _shouldUpdate = true;
            }
            if (_shouldUpdate)
            {
                UpdatePosition();
            }
        }

        public void UpdatePosition()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            float cameraDistance = Vector3.Dot(Target.position - BaseCamera.transform.position, BaseCamera.transform.forward);
            Vector3 minPoint;
            Vector3 maxPoint;
            if (IgnoreSafeArea)
            {
                minPoint = BaseCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance));
                maxPoint = BaseCamera.ScreenToWorldPoint(new Vector3(_screenWidth, _screenHeight, cameraDistance));
            }
            else
            {
                Rect safeArea = Screen.safeArea;
                minPoint = BaseCamera.ScreenToWorldPoint(new Vector3(safeArea.xMin, safeArea.yMin, cameraDistance));
                maxPoint = BaseCamera.ScreenToWorldPoint(new Vector3(safeArea.xMax, safeArea.yMax, cameraDistance));
            }
            Target.position = new Vector3(minPoint.x + (maxPoint.x - minPoint.x) * ViewportPosition.x, minPoint.y + (maxPoint.y - minPoint.y) * ViewportPosition.y, minPoint.z);
            _shouldUpdate = false;
        }

#if UNITY_EDITOR
        [ContextMenu("Update Position")]
        private void ManualUpdate()
        {
            Start();
        }
#endif
    }
}