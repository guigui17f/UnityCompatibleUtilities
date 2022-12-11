using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUIGUI17F
{
    public enum AppScene
    {
    }

    public enum SceneState
    {
        Normal = 0,
        FadeOut = 1,
        Transition = 2,
        FadeIn = 2,
    }

    public class AppSceneManager : IDisposable
    {
        private const string TransitionSceneName = "Transition";

        private static readonly Dictionary<AppScene, string> SceneNameDictionary = new Dictionary<AppScene, string>
        {
        };

        public static AppSceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSceneManager();
                }
                return _instance;
            }
        }

        private static AppSceneManager _instance;

        public event Action<AppScene, AppScene> OnTransitionStart;
        public event Action<AppScene, AppScene> OnSceneChanged;
        public event Action<AppScene, AppScene> OnTransitionEnd;
        public AppScene CurrentScene { get; private set; }
        public AppScene PreviousScene { get; private set; }
        public SceneState CurrentState { get; private set; }

        private bool _assetsLoaded;
        private AsyncOperation _loadSceneOperation;

        private AppSceneManager()
        {
        }

        public void LoadScene(AppScene targetScene)
        {
            if (CurrentState != SceneState.Normal)
            {
                return;
            }
            CurrentState = SceneState.FadeOut;
            if (ShouldAsyncLoad(CurrentScene, targetScene))
            {
                _assetsLoaded = false;
                PlayFadeOutAnimation(() =>
                {
                    OnTransitionStart?.Invoke(CurrentScene, targetScene);
                    SceneManager.LoadScene(TransitionSceneName);
                    CurrentState = SceneState.Transition;
                    ReleaseAssets();
                    if (_assetsLoaded)
                    {
                        GoToTargetScene(targetScene);
                    }
                });
                LoadSceneAsync(targetScene);
            }
            else
            {
                OnTransitionStart?.Invoke(CurrentScene, targetScene);
                CurrentState = SceneState.Transition;
                SceneManager.LoadScene(SceneNameDictionary[targetScene]);
                ReleaseAssets();
                PreviousScene = CurrentScene;
                CurrentScene = targetScene;
                OnSceneChanged?.Invoke(PreviousScene, CurrentScene);
                CurrentState = SceneState.Normal;
                OnTransitionEnd?.Invoke(PreviousScene, CurrentScene);
            }
        }

        private bool ShouldAsyncLoad(AppScene currentScene, AppScene targetScene)
        {
            return true;
        }
        
        private async void LoadSceneAsync(AppScene targetScene)
        {
            _loadSceneOperation = SceneManager.LoadSceneAsync(SceneNameDictionary[targetScene]);
            _loadSceneOperation.allowSceneActivation = false;
            while (_loadSceneOperation.progress < 0.9f)
            {
                await Task.Delay(20);
            }
            _assetsLoaded = true;
            if (CurrentState == SceneState.Transition)
            {
                GoToTargetScene(targetScene);
            }
        }

        private void GoToTargetScene(AppScene targetScene)
        {
            if (_loadSceneOperation != null)
            {
                _loadSceneOperation.allowSceneActivation = true;
            }
            PreviousScene = CurrentScene;
            CurrentScene = targetScene;
            OnSceneChanged?.Invoke(PreviousScene, CurrentScene);
            WaitForSceneReady(() =>
            {
                CurrentState = SceneState.FadeIn;
                PlayFadeInAnimation(() =>
                {
                    CurrentState = SceneState.Normal;
                    OnTransitionEnd?.Invoke(PreviousScene, CurrentScene);
                });
            });
        }

        private void PlayFadeOutAnimation(Action callback)
        {
            callback();
        }

        private void PlayFadeInAnimation(Action callback)
        {
            callback();
        }

        private void WaitForSceneReady(Action callback)
        {
            callback();
        }

        private void ReleaseAssets()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        
        public void Dispose()
        {
            _instance = null;
        }
    }
}