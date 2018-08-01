namespace TinyBee.Debug
{
    using UnityEngine;

    public class DeviceWindow : TWindow
    {
        public DeviceWindow(int id, string title, Rect rect, bool enabled) : base(id, title, rect, enabled) {}

        public override void Dispose() {}

        protected override void DrawWindow(int id)
        {
            GUILayout.Label("OS: " + SystemInfo.operatingSystem);
            GUILayout.Label("DeviceID: " + SystemInfo.deviceUniqueIdentifier);
            GUILayout.Label("DeviceName: " + SystemInfo.deviceName);
            GUILayout.Label("DeviceModel: " + SystemInfo.deviceModel);
            GUILayout.Label("DeviceType: " + SystemInfo.deviceType.ToString());
            GUILayout.Label("CPU: " + SystemInfo.processorType);
            GUILayout.Label("CPU Count: " + SystemInfo.processorCount.ToString());
            GUILayout.Label("GPUID: " + SystemInfo.graphicsDeviceID.ToString());
            GUILayout.Label("GPU: " + SystemInfo.graphicsDeviceName);
            GUILayout.Label("GPU Ver: " + SystemInfo.graphicsDeviceVersion);
            GUILayout.Label("GPUMemory: " + SystemInfo.graphicsMemorySize.ToString() + " MB");
            GUILayout.Label("Reserved Memory: " + SystemInfo.systemMemorySize.ToString() + " MB");
            //GUILayout.Label("Used Memory: " + (Profiler.GetTotalReservedMemory() / 1024 / 1024).ToString() + " MB");
            GUILayout.Label("Display Count: " + Display.displays.Length.ToString());
            GUILayout.Label("Screen Height: " + Screen.height.ToString());
            GUILayout.Label("Screen Width: " + Screen.width.ToString());
            GUILayout.Label("Screen Dpi: " + Screen.dpi.ToString());
            GUI.DragWindow();
        }
    }
}