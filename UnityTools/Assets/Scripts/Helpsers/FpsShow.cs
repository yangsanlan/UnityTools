using UnityEngine;
using System.Collections;

public class FpsShow : MonoBehaviour
{
    public static FpsShow Instance;
    private float LastTime = 0;
    private int frame;
    [HideInInspector]
    public float FpsFrame;
    public bool ShowFps;
    public Rect ShowPosition = new Rect(50, 50, 100, 100);

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        LastTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime - LastTime >= 1f)
        {
            FpsFrame = frame / (currentTime - LastTime);
            frame = 0;
            LastTime = Time.realtimeSinceStartup;
        }
    }

    void OnGUI()
    {
        if (ShowFps)
        {
            GUI.skin.label.normal.textColor = Color.black;
            GUI.skin.label.fontSize = 30;
            GUI.Label(ShowPosition, FpsFrame.ToString("F2"));

            //GUI.Label(new Rect(ShowPosition.x, ShowPosition.y + 100f, ShowPosition.width, ShowPosition.height), (SystemInfo.graphicsMemorySize).ToString("F2"));
        }
    }
}
