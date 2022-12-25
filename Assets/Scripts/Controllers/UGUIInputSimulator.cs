using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GUIGUI17F
{
    public class UGUIInputSimulator : MonoBehaviour
    {
        public static UGUIInputSimulator Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("UGUIInputSimulator");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<UGUIInputSimulator>();
                }
                return _instance;
            }
        }

        private static UGUIInputSimulator _instance;

        private EventSystem _eventSystem;
        private List<PointerEventData> _usableDataList;
        private int _dataIndex;

        private void Awake()
        {
            _eventSystem = EventSystem.current;
            _usableDataList = new List<PointerEventData>();
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            StopAllCoroutines();
            _eventSystem = EventSystem.current;
            _usableDataList.Clear();
        }

        public void TriggerClick(GameObject target, float tapDuration = 0f)
        {
            StartCoroutine(TriggerClickCoroutine(target, tapDuration));
        }

        private IEnumerator TriggerClickCoroutine(GameObject target, float tapDuration)
        {
            PointerEventData pointer = GetUsableEventData();
            pointer.eligibleForClick = true;
            pointer.pointerPress = target;

            ExecuteEvents.ExecuteHierarchy(target, pointer, ExecuteEvents.pointerDownHandler);
            do
            {
                yield return null;
                tapDuration -= Time.unscaledDeltaTime;
            } while (tapDuration > 0);
            ExecuteEvents.ExecuteHierarchy(target, pointer, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.ExecuteHierarchy(target, pointer, ExecuteEvents.pointerUpHandler);

            RecycleEventData(pointer);
        }

        private PointerEventData GetUsableEventData()
        {
            PointerEventData pointer;
            if (_usableDataList.Count > 0)
            {
                pointer = _usableDataList[0];
                _usableDataList.RemoveAt(0);
            }
            else
            {
                pointer = new PointerEventData(_eventSystem);
            }
            pointer.pointerId = _dataIndex++;
            return pointer;
        }

        private void RecycleEventData(PointerEventData pointer)
        {
            if (pointer.currentInputModule == _eventSystem.currentInputModule)
            {
                pointer.Reset();
                _usableDataList.Add(pointer);
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            _instance = null;
        }
    }
}