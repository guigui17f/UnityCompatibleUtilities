using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GUIGUI17F
{
    /// <summary>
    /// a simple utility to play an animation clip without using the animator controller
    /// </summary>
    public class AnimationClipPlayer : MonoBehaviour, IDisposable
    {
        public Animator TargetAnimator;
        public AnimationClip Clip;

        private PlayableGraph _graph;
        private Playable _playable;
        private bool _initialized;

        public void Initialize()
        {
            if (!_initialized)
            {
                _graph = PlayableGraph.Create();
                _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

                _playable = AnimationClipPlayable.Create(_graph, Clip);
                var output = AnimationPlayableOutput.Create(_graph, "Animation", TargetAnimator);
                output.SetSourcePlayable(_playable);

                _initialized = true;
            }
        }

        /// <param name="restart">whether to play the clip again if the animation already started</param>
        public void Play(bool restart = false)
        {
            if (_initialized)
            {
                if (restart)
                {
                    _playable.SetTime(0);
                }
                _graph.Play();
            }
        }

        public void Stop()
        {
            if (_initialized)
            {
                _graph.Stop();
            }
        }

        public void Dispose()
        {
            if (_initialized)
            {
                _graph.Destroy();
                _initialized = false;
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}