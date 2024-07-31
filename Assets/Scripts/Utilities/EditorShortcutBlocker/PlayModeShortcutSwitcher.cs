#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GUIGUI17F
{
    [InitializeOnLoad]
    public static class PlayModeShortcutSwitcher
    {
        private static bool _activated;

        static PlayModeShortcutSwitcher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += OnQuitting;
        }

        public static void BlockShortcut()
        {
            if (_activated)
            {
                return;
            }
            if (ShortcutManager.instance.GetAvailableProfileIds().Contains("Play"))
            {
                ShortcutManager.instance.activeProfileId = "Play";
            }
            else
            {
                ShortcutManager.instance.CreateProfile("Play");
                ShortcutManager.instance.activeProfileId = "Play";
                foreach (string shortcutId in ShortcutManager.instance.GetAvailableShortcutIds())
                {
                    if (shortcutId != "Main Menu/Edit/Play")
                    {
                        ShortcutManager.instance.RebindShortcut(shortcutId, ShortcutBinding.empty);
                    }
                }
            }
            _activated = true;
        }

        public static void RecoverShortcut()
        {
            if (!_activated)
            {
                return;
            }
            ShortcutManager.instance.activeProfileId = "Default";
            _activated = false;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    BlockShortcut();
                    new GameObject("BlockEditorShortcut", typeof(EditorShortcutBlocker));
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    ShortcutManager.instance.activeProfileId = "Default";
                    break;
            }
        }

        private static void OnQuitting()
        {
            RecoverShortcut();
        }
    }
}

#endif