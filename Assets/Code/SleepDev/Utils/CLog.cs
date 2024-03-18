﻿using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public static class CLog
    {
        [HideInCallstack]
        public static void Log(string message)
        {
            Debug.Log(message);
        }
        
        [HideInCallstack]
        public static void Log(string message, string color)
        {
            Debug.Log(message.Color(color));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        public static void LogWHeader(string header, string message, string headerColor, string messageColor)
        {
            Debug.Log(Header(header, headerColor) + message.Color(messageColor));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        public static void LogWHeader(string header, string message, string color)
        {
            Debug.Log(Header(header, color) + message.Color(color));
        }
        
        [HideInCallstack]
        public static void LogBlue(string message)
        {
            Debug.Log(message.Color("b"));
        }

        [HideInCallstack]
        public static void LogGreen(string message)
        {
            Debug.Log(message.Color("g"));
        }

        [HideInCallstack]
        public static void LogRed(string message)
        {
            Debug.Log(message.Color("r"));
        }
        
        [HideInCallstack]
        public static void LogPink(string message)
        {
            Debug.Log(message.Color("p"));
        }

        [HideInCallstack]
        public static void LogYellow(string message)
        {
            Debug.Log(message.Color("y"));
        }
        
        [HideInCallstack]
        public static void LogWhite(string message)
        {
            Debug.Log(message.Color("w"));
        }

        [HideInCallstack]
        public static string Header(string name, string color = "white")
        {
            return $"[{name.Color(color)}] ";
        }


        private static Dictionary<string, string> ColorKeys = new Dictionary<string, string>()
        {
            {"w", "FFFFFF"},
            {"black", "000000"},
            {"b", "blue"},
            {"c", "cyan"},
            {"g", "green"},
            {"y", "yellow"},
            {"r", "red"},
        };

        /// <summary>
        /// "r" - red, "y" - yellow, "b" - blue, "g" - green, "c" = cyan, "p" - pink
        /// "w" - white, "black" - black
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static string Color(this string message, string color)
        {
            #if !UNITY_EDITOR
            return message;
            #endif
            var colorPrefix = "FFFFFF";
            ColorKeys.TryGetValue(color, out colorPrefix);
            return $"<color={colorPrefix}>" + message + "</color>";
        }
    }
}