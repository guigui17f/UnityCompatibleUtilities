using UnityEngine;

namespace GUIGUI17F
{
    /// <summary>
    /// adjust a sprite to full screen
    /// </summary>
    public class BackgroundAdjuster : MonoBehaviour
    {
        public Camera AnchorCamera;
        public SpriteRenderer TargetRenderer;

        private int _currentWidth;
        private int _currentHeight;

        private void Start()
        {
            if (null == AnchorCamera)
            {
                AnchorCamera = Camera.main;
            }
            if (null == TargetRenderer)
            {
                TargetRenderer = GetComponent<SpriteRenderer>();
            }
            AdjustBackground();
        }

        private void AdjustBackground()
        {
            _currentWidth = Screen.width;
            _currentHeight = Screen.height;
            Transform cameraTransform = AnchorCamera.transform;
            Transform rendererTransform = TargetRenderer.transform;
            Vector3 originSize = TargetRenderer.bounds.size;
            float originRatio = originSize.x / originSize.y;
            Vector3 originScale = rendererTransform.localScale;

            float cameraDistance = Vector3.Dot(rendererTransform.position - cameraTransform.position, cameraTransform.forward);
            Vector3 newSize = AnchorCamera.ScreenToWorldPoint(new Vector3(_currentWidth, _currentHeight, cameraDistance)) * 2;
            float newRatio = newSize.x / newSize.y;
            if (originRatio > newRatio)
            {
                float scale = newSize.y / originSize.y;
                rendererTransform.localScale = new Vector3(originScale.x * scale, originScale.y * scale, originScale.z);
            }
            else
            {
                float scale = newSize.x / originSize.x;
                rendererTransform.localScale = new Vector3(originScale.x * scale, originScale.y * scale, originScale.z);
            }
        }

        private void Update()
        {
            if (Screen.width != _currentWidth || Screen.height != _currentHeight)
            {
                AdjustBackground();
            }
        }
    }
}
