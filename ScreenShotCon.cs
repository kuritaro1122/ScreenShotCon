using UnityEngine;
using System.Collections;
using HyperNova;
//using UnityEngine.InputSystem;
using System.IO;
using HyperNova.SaveAndLoad;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// スクリーンショットをキャプチャするサンプル
/// </summary>
public class ScreenShotCon : MonoBehaviour {
    [SerializeField] Text text_captured;
    private Color textDefaultColor;
    private float elapsedTime = 99f;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start() {
        //Debug.Log(this.gameObject.name, this);
        this.textDefaultColor = this.text_captured.color;
    }

    private void Update() {
        UpdateTextUI();
        // スペースキーが押されたら
#if UNITY_EDITOR
        if (ScreenShotInput()) {
            if (!Directory.Exists(GetFilePath())) Directory.CreateDirectory(GetFilePath());

            // スクリーンショットを保存
            string filePath = System.IO.Path.Combine(GetFilePath(), GetFileName()); //GetFilePath() + GetFileName();
            int index = 1;
            while (true) {
                //string _path = System.IO.Path.Combine(filePath, $"_{index}.png");
                string _path = filePath + $"_{index}.png";
                if (!File.Exists(_path)) {
                    filePath = _path;
                    break;
                }
            }
            _cancellationTokenSource = new CancellationTokenSource();
            CaptureScreenShotAsync(filePath, _cancellationTokenSource.Token).Forget();

            //Task.Run(() => CaptureScreenShot(filePath));
        }
#endif
    }

    private string GetFilePath() {
        return SaveDataIO._ScreenShotFolderPath;
    }
    private string GetFileName() {
        string fileName = "hy_screenShot_";
        fileName += System.DateTime.Now.Year.ToString() + "-";
        fileName += System.DateTime.Now.Month.ToString() + "-";
        fileName += System.DateTime.Now.Day.ToString() + "-";
        fileName += System.DateTime.Now.Hour.ToString() + "-";
        fileName += System.DateTime.Now.Minute.ToString() + "-";
        fileName += System.DateTime.Now.Second.ToString() + "-";
        fileName += System.DateTime.Now.Millisecond.ToString() + "-";
        return fileName;
    }

    // 画面全体のスクリーンショットを保存する
    private void CaptureScreenShot(string filePath) {
        //yield return null;
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"ScreenShotCon/CaptureScreenShot,path:{filePath}");
        this.elapsedTime = 0f;

    }

    private async UniTaskVoid CaptureScreenShotAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken)) {
        if (cancellationToken.IsCancellationRequested) {
            var ex = new OperationCanceledException(cancellationToken);
            Debug.LogError(ex);
            throw ex;
        }

        CaptureScreenShot(filePath);
        Debug.Log("CaptureScreenShotAsync Done!");
    }

    private bool ScreenShotInput() {
        return HyperNova.InputControl.ScreenShotButton;
    }

    private void UpdateTextUI() {
        float t0 = 0.4f;
        int n = 3;
        float t1 = 2f;
        this.elapsedTime += (1f / t0) * Time.unscaledDeltaTime;
        Color color = this.textDefaultColor;
        if (this.elapsedTime < t0 * n) {
            color.a = Mathf.Lerp(0, 1, (this.elapsedTime % t0) / t0);
            color.a = Mathf.Round(color.a);
        } else if (this.elapsedTime < t0 * n * t1) color.a = 1f;
        else color.a = 0f;
        this.text_captured.color = color;
    }
}