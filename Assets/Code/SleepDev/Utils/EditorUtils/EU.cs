using System;
using UnityEngine;

namespace SleepDev.Utils.EditorUtils
{
    /// <summary>
    /// Useful editor methods for building editor classes
    /// </summary>
    public class EU
    {
        public const float button_small_width = 30;
        public const float button_mid_width = 60;
        public const float button_big_width = 90;
        public const float button_large_width = 120;

        public const float button_small_height = 30;
        public const float button_mid_height = 36;
        public const float button_big_height = 50;

        public const int font_size_small = 10;
        public const int font_size_normal = 12;
        public const int font_size_big = 15;
        public const int font_size_large = 18;
        public const int font_size_huge = 25;
        
        public const int space_small = 10;
        public const int space_mid = 20;
        public const int space_big = 30;
        public const int space_large = 40;

        public const int square_btn_size_small = 30;
        public const int square_btn_size_mid = 40;
        public const int square_btn_size_big = 50;
        public const int square_btn_size_large = 60;

        
        #region Colors

        private const float Alpha = 0.85f;
        public static Color White = new(255, 255, 255, Alpha);      
        public static Color Black = new(0, 0, 0, Alpha);      
        /// Red
        public static Color Red = new(255, 0, 0, Alpha);      
        /// Red
        public static Color LightCoral = new(240, 128, 128, Alpha);        
        /// Red
        public static Color Crimson = new (220, 20, 60, Alpha);        
        /// Red
        public static Color FireBrick = new (178, 34, 34, Alpha);        
        /// Pink
        public static Color Pink = new (255, 192, 203, Alpha);        
        /// Pink
        public static Color HotPink = new (255, 105, 180, Alpha);
        /// Pink
        public static Color DeepPink = new (255, 20, 147, Alpha);
        
        /// Yellow
        public static Color DarkOrange = new (255, 140, 0, Alpha);
        /// Yellow
        public static Color Orange = new (255, 165, 0, Alpha);
        /// Yellow
        public static Color Gold = new (255, 215, 0, Alpha);
        /// Yellow
        public static Color LightYellow = new (255, 255, 224, Alpha);
        /// Yellow
        public static Color Moccasin = new (255, 228, 181, Alpha);
        /// Yellow
        public static Color PeachPuff = new (255, 218, 185, Alpha);
        
        /// Purple
        public static Color Lavender = new (230, 230, 250, Alpha);
        /// Purple
        public static Color Plum = new (221, 160, 221, Alpha);
        /// Purple
        public static Color Violent = new (238, 130, 238, Alpha);
        /// Purple
        public static Color Fuchsia = new (255, 0, 255, Alpha);
        /// Purple
        public static Color MediumOrchid = new (186, 85, 211, Alpha);
        /// Purple
        public static Color MediumPurple = new (147, 112, 219, Alpha);
        /// Purple
        public static Color RebeccaPurple = new (102, 51, 153, Alpha);
        /// Purple
        public static Color Purple = new (128, 0, 128, Alpha);
        /// Purple
        public static Color MediumSlateBlue = new (123, 104, 238, Alpha);
        
        /// Green
        public static Color GreenYellow = new (173, 255, 47, Alpha);
        /// Green
        public static Color Chartreuse = new (127, 255, 0, Alpha);
        /// Green
        public static Color Lime = new (0, 255, 0, Alpha);
        /// Green
        public static Color SpringGreen = new (0, 255, 127, Alpha);
        /// Green
        public static Color ForestGreen = new (34, 139, 34, Alpha);
        /// Green
        public static Color DarkGreen = new (0, 100, 0, Alpha);
        /// Green
        public static Color MediumAquamarine = new (102, 205, 170, Alpha);
        /// Green
        public static Color LightSeaGreen = new (32, 178, 170, Alpha);
        /// Green
        public static Color DarkCyan = new (0, 139, 139, Alpha);
        
        /// Blue
        public static Color Aqua = new (0, 255, 255, Alpha);
        /// Blue
        public static Color PaleTurquoise = new (175, 238, 238, Alpha);
        /// Blue
        public static Color Turquoise = new (64, 224, 208, Alpha);
        /// Blue
        public static Color SteelBlue = new (70, 130, 180, Alpha);
        /// Blue
        public static Color DeepSkyBlue = new (0, 191, 255, Alpha);
        /// Blue
        public static Color RoyalBlue = new (65, 105, 225, Alpha);
        /// Blue
        public static Color MediumBlue = new (0, 0, 205, Alpha);
        /// Blue
        public static Color Navy = new (0, 0, 128, Alpha);
        #endregion

        
        #region Public Buttons
        /// <summary>
        /// Small Square button. Use this with no text or one character
        /// </summary>
        public static bool ButtonSmall(string label, Color color)
        {
            return ButtonStyleNormal(label, button_small_width, button_small_height, color);
        }
        
        
        /// <summary>
        /// Normal sized button, suitable for 1-word text
        /// </summary>
        public static bool ButtonMidSize(string label, Color color)
        {
            return ButtonStyleNormal(label, button_mid_width, button_mid_height, color);
        }
        
        public static bool ButtonMidSize2(string label, Color color)
        {
            return ButtonStyleNormal(label, button_mid_width * 1.25f, button_mid_height, color);
        }
        
        /// <summary>
        /// Big Width, mid width
        /// </summary>
        public static bool ButtonBig(string label, Color color)
        {
            return ButtonStyleNormal(label, button_big_width, button_mid_height, color);
        }
        
        /// <summary>
        /// Large Width, mid width
        /// </summary>
        public static bool ButtonLarge(string label, Color color)
        {
            return ButtonStyleNormal(label, button_large_width, button_mid_height, color);
        }

        /// <summary>
        ///  Big Width, Big Height
        /// </summary>
        public static bool ButtonBigWide(string label, Color color)
        {
            return ButtonStyleNormal(label, button_big_width, button_big_height, color);
        }
        
        /// <summary>
        ///  Large Width, Big Height
        /// </summary>
        public static bool ButtonLargeWide(string label, Color color)
        {
            return ButtonStyleNormal(label, button_large_width, button_big_height, color);
        }
        
        public static bool ButtonStyleNormal(string label, float width, float height, Color color)
        {
            var style = GetButtonStyle(width, height);
            style.fontStyle = FontStyle.Bold;
            var prevColor = GUI.color;
            GUI.color = color;
            SetButtonTextColor(style,color);
            var clicked = GUILayout.Button(label, style);
            GUI.color = prevColor;
            return clicked;
        }
        #endregion


        #region Public Labels
        public static void Label(string text, char align = 'c', bool bold = true)
        {
            var style = GetLabelStyle(align, bold);
            GUILayout.Label(text, style);
        }

        public static void Label(string text, int fontSize = font_size_normal, char align = 'c', bool bold = true)
        {
            var style = GetLabelStyle(align, bold, fontSize);
            GUILayout.Label(text, style);
        }
        
        public static void Label(string text, Color color, char align = 'c', bool bold = true)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            Label(text, align, bold);
            GUI.color = prevColor;
        }
        
        public static void Label(string text, Color color, int fontSize = font_size_normal, char align = 'c', bool bold = true)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            Label(text, fontSize, align, bold);
            GUI.color = prevColor;
        }
        #endregion


        #region Spaces
        public static void Space(int size = space_small)
        {
            GUILayout.Space(size);
        }

        #endregion


        #region Button With Label

        public static bool ButtonWithLabelSmall(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white);
        }
        
        public static bool ButtonWithLabelMidSize(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_mid);
        }

        public static bool ButtonWithLabelBig(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_big);
        }
        
        public static bool ButtonWithLabelLarge(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_large);
        }
        
        public static bool ButtonWithLabel(string buttonText, string labelText, 
                            Color buttonColor, Color labelColor,
                            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            GUI.color = buttonColor;
            SetButtonTextColor(buttonStyle, buttonColor);
            var pressed = GUILayout.Button(buttonText, buttonStyle);
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);
            GUILayout.EndHorizontal();
            GUI.color = prevColor;
            return pressed;
        }

        public static bool LabelWithButton(string buttonText, string labelText, 
            Color buttonColor, Color labelColor,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('r', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            GUI.color = buttonColor;
            GUILayout.Label(labelText, labelStyle);
            SetButtonTextColor(buttonStyle, buttonColor);
            var pressed = GUILayout.Button(buttonText, buttonStyle);
            GUI.color = labelColor;
            GUILayout.EndHorizontal();
            GUI.color = prevColor;
            return pressed;
        }
        
        public static void TwoButtonAndLabel(string buttonText1, string buttonText2, string labelText, 
            Color buttonColor1, Color buttonColor2, Color labelColor,
            Action onClick1, Action onClick2,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);

            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }
        
        public static void LabelAndTwoButton(string buttonText1, string buttonText2, string labelText, 
            Color buttonColor1, Color buttonColor2, Color labelColor,
            Action onClick1, Action onClick2,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('r', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }
        
        public static void ThreeButtonAndLabel(string buttonText1, string buttonText2,  string buttonText3, string labelText, 
            Color buttonColor1, Color buttonColor2, Color buttonColor3, Color labelColor,
            Action onClick1, Action onClick2, Action onClick3,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();
            GUI.color = buttonColor3;
            SetButtonTextColor(buttonStyle, buttonColor3);
            if(GUILayout.Button(buttonText3, buttonStyle))
                onClick3?.Invoke();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }

        private static void SetButtonTextColor(GUIStyle style, Color buttonColor)
        {
            var cl = (buttonColor.r + buttonColor.g + buttonColor.b) * buttonColor.a;
            if (cl > 50)
                style.normal.textColor = Black;
            else
                style.normal.textColor = Color.white;
        }
        #endregion
        
        
        #region Style Getters
        public static GUIStyle GetButtonStyle(float width, float height, int fontSize = font_size_normal)
        {
            var style = GetButtonStyle();
            style.fixedWidth = width;
            style.fixedHeight = height;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = fontSize;
            return style;
        }
        
        public static GUIStyle GetButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            return style;
        }
        
        public static GUIStyle GetLabelStyle(char align, bool bold, int fontSize = font_size_normal)
        {
            var style = GetLabelStyle();
            switch (align)
            {
                case 'l':
                    style.alignment = TextAnchor.MiddleLeft;
                    break;
                case 'c':
                    style.alignment = TextAnchor.MiddleCenter;
                    break;
                case 'r':
                    style.alignment = TextAnchor.MiddleRight;
                    break;
            }
            if (bold)
                style.fontStyle = FontStyle.Bold;
            style.fontSize = fontSize;
            return style;    
        }
        
        public static GUIStyle GetLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            return style;
        }    
        #endregion
    }
}