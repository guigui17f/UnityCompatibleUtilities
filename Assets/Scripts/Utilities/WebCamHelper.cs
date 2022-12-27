using System;
using System.Collections;
using UnityEngine;

namespace GUIGUI17F
{
    /// <summary>
    /// This script should be attached to a Quad to render the WebCam texture.
    /// And please make sure the scales of the parent gameObjects are all keep to 1.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class WebCamHelper : MonoBehaviour
    {
        /// <summary>
        /// The way we change the quad localScale based on the aspect ratio of the WebCam texture
        /// </summary>
        public enum QuadScaleMode
        {
            /// <summary>
            /// extend the quad to match the aspect ratio of the WebCam texture
            /// </summary>
            ScaleToFill = 0,

            /// <summary>
            /// shrink the quad to match the aspect ratio of the WebCam texture
            /// </summary>
            ScaleToFit = 1,

            /// <summary>
            /// keep current scale (based on the renderArea) and ignore the aspect ratio
            /// </summary>
            KeepOrigin = 2
        }

        private const string LogTag = "WebCamHelper";

        public int WebCamRequestWidth = 2048;
        public int WebCamRequestHeight = 1536;
        public bool UseFrontCamera;
        public QuadScaleMode ScaleMode;

        private Renderer _renderer;
        private WebCamTexture _webcamTexture;
        private int _shaderColorId;
        private string _cameraName;
        private bool _initialized;
        private Camera _baseCamera;
        private bool _cameraTextureReceived;
        private bool _isPlaying;
        private int _lastRotation;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _shaderColorId = Shader.PropertyToID("_Color");
        }

        private void Start()
        {
            if (!_cameraTextureReceived)
            {
                _renderer.material.SetColor(_shaderColorId, Color.black);
            }
        }

        /// <summary>
        /// request device camera permission before call this method
        /// </summary>
        /// <param name="renderArea">the WebCam texture display area, in screen coordinate</param>
        /// <param name="baseCamera">base unity camera used for translate screen positions to world positions</param>
        /// <returns>start device camera success or failed</returns>
        public bool StartCamera(Rect renderArea, Camera baseCamera)
        {
            _baseCamera = baseCamera;
            if (_renderer.material.mainTexture != _webcamTexture)
            {
                _initialized = false;
            }
            if (_initialized)
            {
                _webcamTexture.Play();
                _renderer.enabled = true;
                _isPlaying = true;
                return true;
            }

            GetCameraName();
            if (string.IsNullOrEmpty(_cameraName))
            {
                Debug.unityLogger.LogError(LogTag, "open camera failed, no supported device found");
                return false;
            }

            _cameraTextureReceived = false;
            _lastRotation = 0;
            UpdateQuadTransform(renderArea);
            if (_webcamTexture == null)
            {
                _webcamTexture = new WebCamTexture(WebCamRequestWidth, WebCamRequestHeight, 30);
            }
            _renderer.material.mainTexture = _webcamTexture;
            _webcamTexture.Play();
            _isPlaying = true;
            _renderer.enabled = true;
            _initialized = true;
            return true;
        }

        public void PauseCamera()
        {
            if (_initialized)
            {
                _webcamTexture.Pause();
                _isPlaying = false;
            }
        }

        public void StopCamera()
        {
            if (_initialized)
            {
                _webcamTexture.Stop();
                _isPlaying = false;
            }
        }

        public void ToggleVisual(bool enable)
        {
            _renderer.material.SetColor(_shaderColorId, enable ? Color.white : Color.black);
        }

        public void CaptureScreen(Rect captureRect, Action<Texture2D> callback)
        {
            if (_initialized)
            {
                StartCoroutine(CaptureScreenCoroutine(captureRect, callback));
            }
            else
            {
                callback?.Invoke(null);
            }
        }
        
        private void Update()
        {
            if (_initialized && _webcamTexture.didUpdateThisFrame)
            {
                if (!_cameraTextureReceived)
                {
                    switch (ScaleMode)
                    {
                        case QuadScaleMode.ScaleToFill:
                            ScaleToFill(_webcamTexture.width, _webcamTexture.height, _webcamTexture.videoVerticallyMirrored);
                            break;
                        case QuadScaleMode.ScaleToFit:
                            ScaleToFit(_webcamTexture.width,_webcamTexture.height, _webcamTexture.videoVerticallyMirrored);
                            break;
                    }
                    _renderer.material.SetColor(_shaderColorId, Color.white);
                    _cameraTextureReceived = true;
                }
                if (_webcamTexture.videoRotationAngle != _lastRotation)
                {
                    _lastRotation = _webcamTexture.videoRotationAngle;
                    transform.eulerAngles = new Vector3(0, 0, _lastRotation);
                }
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && _initialized && _isPlaying)
            {
                _webcamTexture.Play();
            }
        }

        private void GetCameraName()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            _cameraName = null;
            float highestResolution = 0;
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].isFrontFacing.Equals(UseFrontCamera))
                {
                    if (devices[i].availableResolutions != null)
                    {
                        for (int j = 0; j < devices[i].availableResolutions.Length; j++)
                        {
                            float resolution = devices[i].availableResolutions[j].width * devices[i].availableResolutions[j].height;
                            if (resolution > highestResolution)
                            {
                                highestResolution = resolution;
                                _cameraName = devices[i].name;
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(_cameraName))
                    {
                        _cameraName = devices[i].name;
                    }
                }
            }
        }

        private void UpdateQuadTransform(Rect renderArea)
        {
            float depth = transform.position.z - _baseCamera.transform.position.z;
            Vector3 worldLeftBottom = _baseCamera.ScreenToWorldPoint(new Vector3(renderArea.xMin, renderArea.yMin, depth));
            Vector3 worldRightTop = _baseCamera.ScreenToWorldPoint(new Vector3(renderArea.xMax, renderArea.yMax, depth));
            transform.position = (worldLeftBottom + worldRightTop) * 0.5f;
            transform.localScale = new Vector3(worldRightTop.x - worldLeftBottom.x, worldRightTop.y - worldLeftBottom.y, 1f);
            transform.rotation = Quaternion.identity;
        }

        private void ScaleToFill(float width, float height, bool verticallyMirrored = false)
        {
            float scaleX = transform.localScale.x;
            float scaleY = transform.localScale.y;
            if (width / height > scaleX / scaleY)
            {
                scaleX = width * scaleY / height;
            }
            else
            {
                scaleY = height * scaleX / width;
            }
            transform.localScale = new Vector3(scaleX, verticallyMirrored ? -scaleY : scaleY, 1f);
        }

        private void ScaleToFit(float width, float height, bool verticallyMirrored = false)
        {
            float scaleX = transform.localScale.x;
            float scaleY = transform.localScale.y;
            if (width / height > scaleX / scaleY)
            {
                scaleY = height * scaleX / width;
            }
            else
            {
                scaleX = width * scaleY / height;
            }
            transform.localScale = new Vector3(scaleX, verticallyMirrored ? -scaleY : scaleY, 1f);
        }

        private IEnumerator CaptureScreenCoroutine(Rect captureRect, Action<Texture2D> callback)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            renderTexture.antiAliasing = 8;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.format = RenderTextureFormat.ARGB32;
            renderTexture.autoGenerateMips = false;
            renderTexture.useMipMap = false;

            _baseCamera.targetTexture = renderTexture;
            _baseCamera.Render();
            RenderTexture.active = renderTexture;
            Texture2D screenShot = new Texture2D((int)captureRect.width, (int)captureRect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(captureRect, 0, 0);
            screenShot.Apply(false);

            _baseCamera.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            callback?.Invoke(screenShot);
        }

        private void OnDestroy()
        {
            if (_webcamTexture != null)
            {
                _webcamTexture.Stop();
                Destroy(_webcamTexture);
            }
        }
    }
}