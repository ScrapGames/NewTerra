using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public static class EditorEPM
{
    public struct Textures
    {
        public static Texture2D Question
        {
            get {
                if (_Question == null)
                    _Question = Resources.Load<Texture2D>("EPM/question");
                return _Question;
            }
        }
        static Texture2D _Question;

        public static Texture2D Logo
        {
            get {
                if (_Logo == null)
                    _Logo = Resources.Load<Texture2D>("EPM/logo");
                return _Logo;
            }
        }
        static Texture2D _Logo;

        public static Texture2D DropBox
        {
            get {
                if (_DropBox == null)
                    _DropBox = Resources.Load<Texture2D>("EPM/dropbox");
                return _DropBox;
            }
        }
        static Texture2D _DropBox;

        public static Texture2D PoolBackground
        {
            get {
                if (_PoolBackground == null)
                {
                    _PoolBackground = GetGradient(200, new Color32(2, 12, 51, 255), new Color32(67, 160, 255, 255));
                }
                return _PoolBackground;
            }
        }
        static Texture2D _PoolBackground;

        public static Texture2D DeleteButton
        {
            get {
                if (_DeleteButton == null)
                {
                    _DeleteButton = GetGradient(50, new Color32(63,0,0, 255), new Color32(201,38,38, 255));
                }
                return _DeleteButton;
            }
        }
        static Texture2D _DeleteButton;

        public static Texture2D StatusBackground
        {
            get {
                if (_StatusBackground == null)
                {
                    _StatusBackground = GetGradient(512, new Color32(21, 48, 10, 255), new Color32(55, 155, 15, 255));
                }
                return _StatusBackground;
            }
        }
        static Texture2D _StatusBackground;
    }

    static Texture2D GetGradient(int size, Color colorA, Color colorB)
    {
        Texture2D tex = new Texture2D(1, size);
        for (int i = 0; i < size; i++)
        {
            tex.SetPixel(0, i, Color.Lerp(colorA, colorB, i / (float)size));
        }
        tex.Apply();
        return tex;
    }

    public struct Styles
    {
        public static GUIStyle Logo
        {
            get {
                if (_Logo == null)
                    _Logo = new GUIStyle() { fixedWidth = 128, fixedHeight = 128 };
                return _Logo;
            }
        }
        static GUIStyle _Logo;

        public static GUIStyle Dropbox
        {
            get {
                if (_Dropbox == null)
                    _Dropbox = new GUIStyle() { fixedWidth = 150, fixedHeight = 150};
                return _Dropbox;
            }
        }
        static GUIStyle _Dropbox;

        public static GUIStyle DropboxText
        {
            get {
                if (_DropboxText == null)
                    _DropboxText = new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 14, alignment = TextAnchor.MiddleCenter };
                return _DropboxText;
            }
        }
        static GUIStyle _DropboxText;

        public static GUIStyle CenterText
        {
            get {
                if (_CenterText == null)
                    _CenterText = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
                return _CenterText;
            }
        }
        static GUIStyle _CenterText;

        public static GUIStyle PoolArea
        {
            get {
                if (_PoolArea == null)
                    _PoolArea = new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        { background = Textures.PoolBackground }
                    };
                return _PoolArea;
            }
        }
        static GUIStyle _PoolArea;

        public static GUIStyle StatusArea
        {
            get {
                if (_StatusArea == null)
                    _StatusArea = new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        { background = Textures.StatusBackground }
                    };
                return _StatusArea;
            }
        }
        static GUIStyle _StatusArea;


        public static GUIStyle DeleteButton
        {
            get {
                if (_DeleteButton == null)
                    _DeleteButton = new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = Color.white, background = Textures.DeleteButton }
                        , fixedWidth = 20, fixedHeight = 20, alignment = TextAnchor.MiddleCenter,
                        fontSize = 14
                    };
                return _DeleteButton;
            }
        }
        static GUIStyle _DeleteButton;
    }

    
}
