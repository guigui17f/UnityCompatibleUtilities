using UnityEngine;

namespace GUIGUI17F
{
    public class CameraMatrixHelper : MonoBehaviour
    {
        public float RotateAngle = 0;
        public float Scale = 1;
        public Vector2 Shearing = Vector2.zero;
        public Vector2 Translation = Vector2.zero;
        public Camera TargetCamera;

        private Matrix4x4 _matrixRotate = Matrix4x4.identity;
        private Matrix4x4 _matrixScale = Matrix4x4.identity;
        private Matrix4x4 _matrixXShearing = Matrix4x4.identity;
        private Matrix4x4 _matrixYShearing = Matrix4x4.identity;
        private Matrix4x4 _matrixTranslation = Matrix4x4.identity;
        private Matrix4x4 _originMatrixProjection;

        private void Start()
        {
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main;
            }
            _originMatrixProjection = TargetCamera.projectionMatrix;
        }

        private void LateUpdate()
        {
            _matrixScale[0, 0] = Scale;
            _matrixScale[1, 1] = Scale;

            _matrixRotate[0, 0] = Mathf.Cos(RotateAngle * Mathf.Deg2Rad);
            _matrixRotate[0, 1] = -Mathf.Sin(RotateAngle * Mathf.Deg2Rad);
            _matrixRotate[1, 0] = Mathf.Sin(RotateAngle * Mathf.Deg2Rad);
            _matrixRotate[1, 1] = Mathf.Cos(RotateAngle * Mathf.Deg2Rad);

            _matrixXShearing[0, 1] = Shearing.x;
            _matrixYShearing[1, 0] = Shearing.y;

            _matrixTranslation[0, 2] = Translation.x;
            _matrixTranslation[1, 2] = Translation.y;

            TargetCamera.projectionMatrix = _originMatrixProjection * _matrixScale * _matrixRotate * _matrixXShearing * _matrixYShearing * _matrixTranslation;
        }
    }
}