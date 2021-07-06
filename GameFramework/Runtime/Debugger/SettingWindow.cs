using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Settings")]
    public class SettingWindow : IDebuggerWindow
    {
        private Vector2 _scrollPos = Vector2.zero;
        private DebuggerManager _debuggerManager;
        public void OnInit(params object[] args)
        {
            //_debuggerManager = GameFrameworkMode.GetModule<DebuggerManager>();
        }

        public void OnClose()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnDraw()
        {
            if (_debuggerManager == null)
            {
                _debuggerManager = GameFrameworkMode.GetModule<DebuggerManager>();
            }
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Position:", GUILayout.Width(60f));
                GUILayout.Label("Drag window caption to move position.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                float width = _debuggerManager.FullRect.width;
                GUILayout.Label("Width:", GUILayout.Width(60f));
                if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                {
                    width--;
                }
                width = GUILayout.HorizontalSlider(width, 100f, Screen.width - 20f);
                if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                {
                    width++;
                }
                width = Mathf.Clamp(width, 100f, Screen.width - 20f);
                if (width != _debuggerManager.FullRect.width)
                {
                    _debuggerManager.FullRect = new Rect(_debuggerManager.FullRect.x, _debuggerManager.FullRect.y, width, _debuggerManager.FullRect.height);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                float height = _debuggerManager.FullRect.height;
                GUILayout.Label("Height:", GUILayout.Width(60f));
                if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                {
                    height--;
                }
                height = GUILayout.HorizontalSlider(height, 100f, Screen.height - 20f);
                if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                {
                    height++;
                }
                height = Mathf.Clamp(height, 100f, Screen.height - 20f);
                if (height != _debuggerManager.FullRect.height)
                {
                    _debuggerManager.FullRect = new Rect(_debuggerManager.FullRect.x, _debuggerManager.FullRect.y, _debuggerManager.FullRect.width, height);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                float scale = _debuggerManager.WindowScale;
                GUILayout.Label("Scale:", GUILayout.Width(60f));
                if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                {
                    scale -= 0.01f;
                }
                scale = GUILayout.HorizontalSlider(scale, 0.5f, 4f);
                if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                {
                    scale += 0.01f;
                }
                scale = Mathf.Clamp(scale, 0.5f, 4f);
                if (scale != _debuggerManager.WindowScale)
                {
                    _debuggerManager.WindowScale = scale;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("0.5x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 0.5f;
                }
                if (GUILayout.Button("1.0x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 1f;
                }
                if (GUILayout.Button("1.5x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 1.5f;
                }
                if (GUILayout.Button("2.0x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 2f;
                }
                if (GUILayout.Button("2.5x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 2.5f;
                }
                if (GUILayout.Button("3.0x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 3f;
                }
                if (GUILayout.Button("3.5x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 3.5f;
                }
                if (GUILayout.Button("4.0x", GUILayout.Height(60f)))
                {
                    _debuggerManager.WindowScale = 4f;
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset Layout", GUILayout.Height(30f)))
            {
                _debuggerManager.ResetLayout();
            }

            GUILayout.EndScrollView();
        }

    }

}