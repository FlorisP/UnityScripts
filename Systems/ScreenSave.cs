#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using System;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class ScreenSave : MonoBehaviour
{
    public string rootFolderPath = @"C:\Screenshots";
    public bool autoCapture = false;
    public float autoCaptureInterval = 5f;
    public Key captureKey = Key.F12;

    [ReadOnly] public string currentSessionFolderPath;
    [ReadOnly] public float nextAutoCaptureTime;
    [ReadOnly] public int screenshotIndex;
    [ReadOnly] public bool captureInProgress;

    public void Update()
    {    
        if (Keyboard.current != null)
        {
            if(Keyboard.current[captureKey].wasPressedThisFrame)
                StartCapture();
        }

        if (autoCapture && Time.unscaledTime >= nextAutoCaptureTime)
        {
            StartCapture();
            nextAutoCaptureTime = Time.unscaledTime + autoCaptureInterval;
        }
    }

    public void StartCapture()
    {
        if (captureInProgress)
        {
            return;
        }

        StartCoroutine(CaptureRawAsync());
    }

    [ContextMenu("Create New Session Folder")]
    public void CreateNewSessionFolder()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        currentSessionFolderPath = Path.Combine(rootFolderPath, timestamp);

        Directory.CreateDirectory(currentSessionFolderPath);

        screenshotIndex = 0;
        nextAutoCaptureTime = Time.unscaledTime + autoCaptureInterval;
    }

    public IEnumerator CaptureRawAsync()
    {
        if (string.IsNullOrWhiteSpace(currentSessionFolderPath))
        {
            CreateNewSessionFolder();
        }

        captureInProgress = true;

        yield return new WaitForEndOfFrame();

        screenshotIndex++;

        string baseName =
            screenshotIndex.ToString("D4") +
            "_" +
            DateTime.Now.ToString("HH-mm-ss-fff");

        string fullPath = Path.Combine(currentSessionFolderPath, baseName + ".rawshot");

        int width = Screen.width;
        int height = Screen.height;

        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);

        bool requestDone = false;
        bool requestHasError = false;
        NativeArray<byte> rawDataCopy = default;

        AsyncGPUReadback.Request(
            renderTexture,
            0,
            TextureFormat.RGBA32,
            delegate (AsyncGPUReadbackRequest request)
            {
                if (request.hasError)
                {
                    requestHasError = true;
                    requestDone = true;
                    return;
                }

                NativeArray<byte> rawData = request.GetData<byte>();
                rawDataCopy = new NativeArray<byte>(rawData.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                rawDataCopy.CopyFrom(rawData);

                requestDone = true;
            });

        while (!requestDone)
        {
            yield return null;
        }

        RenderTexture.ReleaseTemporary(renderTexture);

        if (!requestHasError && rawDataCopy.IsCreated)
        {
            byte[] rawBytes = rawDataCopy.ToArray();
            rawDataCopy.Dispose();

            Task.Run(() => WriteRawShot(fullPath, width, height, rawBytes));
        }

        captureInProgress = false;
    }

    public static void WriteRawShot(string path, int width, int height, byte[] rawBytes)
    {
        using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.None);
        using BinaryWriter writer = new(stream);

        writer.Write(width);
        writer.Write(height);
        writer.Write(rawBytes.Length);
        writer.Write(rawBytes);
    }

    [Button]
    public void ConvertAllRawShots()
    {
        if (!Directory.Exists(rootFolderPath))
        {
            Debug.Log("Root folder does not exist: " + rootFolderPath);
            return;
        }

        string[] rawShotPaths = Directory.GetFiles(rootFolderPath, "*.rawshot", SearchOption.AllDirectories);
        int convertedCount = 0;
        int skippedCount = 0;

        for (int i = 0; i < rawShotPaths.Length; i++)
        {
            string rawShotPath = rawShotPaths[i];
            string pngPath = Path.ChangeExtension(rawShotPath, ".png");

            if (File.Exists(pngPath))
            {
                skippedCount++;
                continue;
            }

            ConvertToPng(rawShotPath, pngPath);
            convertedCount++;

        }

        Debug.Log("RawShot conversion done. Converted: " + convertedCount + ", Skipped: " + skippedCount);
    }

    public static void ConvertToPng(string rawShotPath, string pngPath)
    {
        using FileStream stream = new(rawShotPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BinaryReader reader = new(stream);

        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int dataLength = reader.ReadInt32();
        byte[] rawBytes = reader.ReadBytes(dataLength);

        byte[] flippedBytes = FlipVertically(rawBytes, width, height);

        byte[] pngBytes = ImageConversion.EncodeArrayToPNG(
            flippedBytes,
            GraphicsFormat.R8G8B8A8_UNorm,
            (uint)width,
            (uint)height);

        File.WriteAllBytes(pngPath, pngBytes);
    }

    public static byte[] FlipVertically(byte[] source, int width, int height)
    {
        int bytesPerPixel = 4;
        int rowSize = width * bytesPerPixel;
        byte[] flipped = new byte[source.Length];

        for (int y = 0; y < height; y++)
        {
            int srcRowStart = y * rowSize;
            int dstRowStart = (height - 1 - y) * rowSize;

            Buffer.BlockCopy(source, srcRowStart, flipped, dstRowStart, rowSize);
        }

        return flipped;
    }

    public void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    public void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            ConvertAllRawShots();
        }
    }
}

#endif
