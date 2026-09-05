using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ShareManager : MonoBehaviour
{
    [SerializeField] private GameObject[] hiddenWhileCapture;
    [SerializeField] private RectTransform resultCardRoot;
    [SerializeField] private Camera captureCamera;
    [SerializeField] private Vector2Int captureResolution = new Vector2Int(1080, 1920);

    private bool isCapturing;

    public void CaptureAndSaveResult()
    {
        if (isCapturing)
        {
            return;
        }

        StartCoroutine(CaptureRoutine());
    }

    private IEnumerator CaptureRoutine()
    {
        isCapturing = true;
        SetHiddenUi(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenshot = null;
        if (captureCamera != null)
        {
            screenshot = CaptureWithCamera();
        }
        else
        {
            screenshot = CaptureScreen();
        }

        if (screenshot != null)
        {
            byte[] pngData = screenshot.EncodeToPNG();
            string fileName = "firework_result_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string savePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(savePath, pngData);

            Debug.Log("Result card saved: " + savePath);

            Destroy(screenshot);

            // NativeShare 연동 지점:
            // new NativeShare().AddFile(savePath).SetSubject("서울 불꽃축제 결과").Share();
        }

        SetHiddenUi(true);
        isCapturing = false;
    }

    private Texture2D CaptureWithCamera()
    {
        int width = Mathf.Max(1, captureResolution.x);
        int height = Mathf.Max(1, captureResolution.y);

        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previous = captureCamera.targetTexture;
        RenderTexture currentActive = RenderTexture.active;

        captureCamera.targetTexture = rt;
        captureCamera.Render();
        RenderTexture.active = rt;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        captureCamera.targetTexture = previous;
        RenderTexture.active = currentActive;
        rt.Release();
        Destroy(rt);

        return texture;
    }

    private Texture2D CaptureScreen()
    {
        int width = Screen.width;
        int height = Screen.height;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        return texture;
    }

    private void SetHiddenUi(bool visible)
    {
        if (hiddenWhileCapture == null)
        {
            return;
        }

        for (int i = 0; i < hiddenWhileCapture.Length; i++)
        {
            GameObject go = hiddenWhileCapture[i];
            if (go != null)
            {
                go.SetActive(visible);
            }
        }

        if (resultCardRoot != null)
        {
            resultCardRoot.gameObject.SetActive(true);
        }
    }
}
