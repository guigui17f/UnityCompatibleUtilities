#if UNITY_EDITOR

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    public class EditorShortcutBlocker : MonoBehaviour
    {
        private delegate int EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int EnumThreadWindows(uint dwThreadId, EnumThreadWndProc lpfn, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern int GetClassNameA(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int SetMenu(IntPtr hWnd, IntPtr hMenu);

        [DllImport("user32.dll")]
        private static extern int DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLongA(IntPtr hWnd, int nIndex, int dwNewLong);

        private static bool _enableHideMenuBar;

        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_SYSMENU = 0x00080000;

        private IntPtr _hFrameWnd;
        private IntPtr _hFrameMenu;
        private bool _menuBarHided;

        [MenuItem("Tools/Toggle Hide Menu Bar in PlayMode")]
        private static void ToggleHideMenuBar()
        {
            _enableHideMenuBar = !_enableHideMenuBar;
            EditorUtility.DisplayDialog("Warning", _enableHideMenuBar ? "Enabled hide menu bar in PlayMode." : "Disabled hide menu bar in PlayMode.", "OK");
        }

        private void Awake()
        {
            int EnumThreadWndProc(IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder className = new StringBuilder(64);
                GetClassNameA(hWnd, className, 64);
                if (className.ToString() == "UnityContainerWndClass")
                {
                    _hFrameWnd = hWnd;
                    _hFrameMenu = GetMenu(hWnd);
                    return 0;
                }
                return 1;
            }
            EnumThreadWindows(GetCurrentThreadId(), EnumThreadWndProc, IntPtr.Zero);
            DontDestroyOnLoad(gameObject);
        }

        private void ActivateMenuBar()
        {
            if (!_menuBarHided)
            {
                return;
            }
            SetWindowLongA(_hFrameWnd, GWL_STYLE, GetWindowLongA(_hFrameWnd, GWL_STYLE) | (WS_CAPTION | WS_SYSMENU));
            SetMenu(_hFrameWnd, _hFrameMenu);
            DrawMenuBar(_hFrameWnd);
            _menuBarHided = false;
        }

        private void DeactivateMenuBar()
        {
            if (!_enableHideMenuBar || _menuBarHided)
            {
                return;
            }
            SetWindowLongA(_hFrameWnd, GWL_STYLE, GetWindowLongA(_hFrameWnd, GWL_STYLE) & ~(WS_CAPTION | WS_SYSMENU));
            SetMenu(_hFrameWnd, IntPtr.Zero);
            DrawMenuBar(_hFrameWnd);
            _menuBarHided = true;
        }

        private void Update()
        {
            if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Game")
            {
                PlayModeShortcutSwitcher.BlockShortcut();
                DeactivateMenuBar();
            }
            else
            {
                PlayModeShortcutSwitcher.RecoverShortcut();
                ActivateMenuBar();
            }
            if (Time.frameCount == 60)
            {
                DeactivateMenuBar();
            }
        }

        private void OnDestroy()
        {
            ActivateMenuBar();
        }
    }
}

#endif