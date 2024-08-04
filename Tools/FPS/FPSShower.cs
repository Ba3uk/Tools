using UnityEngine;

public class FPSShower : MonoBehaviour
{
    public float fpsUpdateInterval = 1.0f;
    public bool printAvgFps = true;
    public Vector2 pos;

    private int fps = 60;
    private int fpsAvg = 60;
    private float fpsLastUpdate;
    private float fpsLastFrame;

    GUIStyle style = new();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    void Start()
    {
        InvokeRepeating(nameof(UpdateFps), fpsUpdateInterval, fpsUpdateInterval);

        style.normal.textColor = Color.white;
        style.fontSize = 32;
        style.fontStyle = FontStyle.Bold;
    }

    void UpdateFps()
    {
        fps = Mathf.RoundToInt((Time.frameCount - fpsLastFrame) / (Time.time - fpsLastUpdate));
        fpsAvg = Mathf.RoundToInt(Time.frameCount / Time.time);
        fpsLastUpdate = Time.time;
        fpsLastFrame = Time.frameCount;
    }


    void OnGUI()
    {
        GUI.Label(new Rect(pos.x, pos.y, 100, 20), "FPS:" + fps, style);

        if (printAvgFps)
            GUI.Label(new Rect(pos.x, pos.y + 20, 100, 20), "FPS avg:" + fpsAvg, style);
    }
#endif
}