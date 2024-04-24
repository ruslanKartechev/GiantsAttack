using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Text;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using MAXHelper;

[InitializeOnLoad]
public class MPCChecker {
    private static readonly List<string> ObsoleteDirectoriesToDelete = new List<string> {
        "Assets/Amazon",
    };    
    
    private static readonly List<string> ObsoleteFilesToDelete = new List<string> {
        "Assets/MadPixel/MAXHelper/Configs/Amazon_APS.unitypackage",
        "Assets/MadPixel/MAXHelper/Configs/Amazon_APS.unitypackage.meta",
        "Assets/Amazon.meta",
    };

    static MPCChecker() {
        CheckObsoleteFiles();

#if UNITY_ANDROID
        int target = (int)PlayerSettings.Android.targetSdkVersion;
        if (target == 0) {
            int highestInstalledVersion = GetHigestInstalledSDK();
            target = highestInstalledVersion;
        }

        if (target < 33 || PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel24) {
            if (EditorPrefs.HasKey(Key)) {
                string lastMPCVersionChecked = EditorPrefs.GetString(Key);
                string currVersion = MAXHelper.MAXHelperInitWindow.GetVersion();
                if (lastMPCVersionChecked != currVersion) {
                    ShowSwitchTargetWindow(target);
                }
            }
            else {
                ShowSwitchTargetWindow(target);
            }
        }
        SaveKey();
#endif
    }


#if UNITY_ANDROID
    private static string appKey = null;
    private static string Key {
        get {
            if (string.IsNullOrEmpty(appKey)) {
                appKey = GetMd5Hash(Application.dataPath) + "MPCv";
            }

            return appKey;
        }
    }

    private static void ShowSwitchTargetWindow(int target) {
        MPCTargetCheckerWindow.ShowWindow(target, (int)PlayerSettings.Android.targetSdkVersion);

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;
    }


    private static string GetMd5Hash(string input) {
        MD5 md5 = MD5.Create();
        byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++) {
            sb.Append(data[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static void SaveKey() {
        EditorPrefs.SetString(Key, MAXHelper.MAXHelperInitWindow.GetVersion());
    }

    //[MenuItem("Mad Pixel/DeleteKey", priority = 1)]
    public static void DeleteEditorPrefs() {
        EditorPrefs.DeleteKey(Key);
    }

    private static int GetHigestInstalledSDK() {
        string s = Path.Combine(GetHighestInstalledAPI(), "platforms");
        string[] directories = Directory.GetDirectories(s);
        int maxV = 0;
        foreach (string directory in directories) {
            string version = directory.Substring(directory.Length - 2, 2);
            int.TryParse(version, out int v);
            if (v > 0) {
                maxV = Mathf.Max(v, maxV);
            }
        }
        return maxV;
    }

    private static string GetHighestInstalledAPI() {
        return EditorPrefs.GetString("AndroidSdkRoot");
    }
#endif


    private static void CheckObsoleteFiles() {
        bool changesMade = false;
        foreach (var pathToDelete in ObsoleteFilesToDelete) {
            if (CheckExistence(pathToDelete)) {
                FileUtil.DeleteFileOrDirectory(pathToDelete);
                changesMade = true;
            }
        }

        foreach (string directory in ObsoleteDirectoriesToDelete) {
            if (CheckExistence(directory)) {
                FileUtil.DeleteFileOrDirectory(directory);
                changesMade = true;
            }
        }

        MAXHelperDefineSymbols.DefineSymbols(false);

        if (changesMade) {
            AssetDatabase.Refresh();
            Debug.LogWarning("ATTENTION: Amazon removed from this project");
        }
    }


    private static bool CheckExistence(string location) {
        return File.Exists(location) ||
               Directory.Exists(location) ||
               (location.EndsWith("/*") && Directory.Exists(Path.GetDirectoryName(location)));
    }

}
