using TMPro;
using UnityEngine;

public class FpsDebug : MonoBehaviour
{
    public static FpsDebug Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI fpsText;

    [SerializeField]
    private float updateInterval = 0.5f; // Refresh text twice a second

    private float accumTime = 0f;
    private int numFrames = 0;
    private float timeLeft;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (fpsText == null)
        {
            fpsText = GetComponent<TextMeshProUGUI>();
        }
        timeLeft = updateInterval;
    }

    void Update()
    {
        // Use unscaledDeltaTime so the counter works even if Time.timeScale is 0 (paused)
        timeLeft -= Time.unscaledDeltaTime;
        accumTime += Time.unscaledDeltaTime;
        numFrames++;

        // Interval ended - update text display
        if (timeLeft <= 0.0f)
        {
            float fps = numFrames / accumTime;
            fpsText.text = $"DEBUG VERSION | FPS: {(int)fps}";

            // Reset measurements
            timeLeft = updateInterval;
            accumTime = 0.0f;
            numFrames = 0;
        }
    }
}
