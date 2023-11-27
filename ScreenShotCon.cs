using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
# endif
using System.IO;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// スクリーンショットをキャプチャする
/// </summary>
public class ScreenShotCon : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;

    private static string FolderPath => System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), Application.productName);

    private static string GetFilePath()
    {
        var folderPath = FolderPath;

        var fileName = "screenShot_";
        fileName += System.DateTime.Now.Year.ToString() + "-";
        fileName += System.DateTime.Now.Month.ToString() + "-";
        fileName += System.DateTime.Now.Day.ToString() + "-";
        fileName += System.DateTime.Now.Hour.ToString() + "-";
        fileName += System.DateTime.Now.Minute.ToString() + "-";
        fileName += System.DateTime.Now.Second.ToString() + "-";
        fileName += System.DateTime.Now.Millisecond.ToString() + "-";

        var tempFilePath = System.IO.Path.Combine(folderPath, fileName + $"_{0}.png");

        var existFileCount = Directory.GetFiles(folderPath).Length;
        for (int i = 1; i < existFileCount; i++)
        {
            tempFilePath = System.IO.Path.Combine(folderPath, fileName + $"_{i}.png");
            if (!File.Exists(System.IO.Path.Combine(folderPath, tempFilePath)))
            {
                break;
            }
        }
        return tempFilePath;
    }

    // 画面全体のスクリーンショットを保存する
    public void StartCaptureScreenShotAsync()
    {
        var folderPath = FolderPath;
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string filePath = GetFilePath();

        _cancellationTokenSource = new CancellationTokenSource();
        CaptureScreenShotAsync(filePath, _cancellationTokenSource.Token).Forget();
    }

    private void CaptureScreenShot(string filePath)
    {
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"ScreenShotCon/CaptureScreenShot,path:{filePath}");
    }

    private async UniTaskVoid CaptureScreenShotAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (cancellationToken.IsCancellationRequested)
        {
            var ex = new OperationCanceledException(cancellationToken);
            Debug.LogError(ex);
            throw ex;
        }
        CaptureScreenShot(filePath);
        Debug.Log("CaptureScreenShotAsync Done!");
    }
}

# if UNITY_EDITOR
[CustomEditor(typeof(ScreenShotCon))]
public class ScreenShotConEditor : Editor
{
    private ScreenShotCon _target = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _target = target as ScreenShotCon;

        if (GUILayout.Button("Capture Screen Shot!"))
        {
            _target.StartCaptureScreenShotAsync();
        }
    }
}
# endif
