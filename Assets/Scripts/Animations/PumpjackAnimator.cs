using UnityEngine;

public class PumpjackAnimator : MonoBehaviour
{
    [SerializeField]
    bool isEnabled;

    public void SetEnabled(bool value) => isEnabled = value;

    [SerializeField]
    Transform pump;

    [Header("Pump Range")]
    [SerializeField]
    private float bottom = 0f;

    [SerializeField]
    private float top = 1f;

    [Header("Timing")]
    [SerializeField]
    private float riseDuration = 1.5f; // slow up

    [SerializeField]
    private float fallDuration = 0.3f; // fast down

    [Header("Curves (0-1 in, 0-1 out)")]
    [SerializeField]
    private AnimationCurve riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float timer;
    private bool rising = true;
    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = pump.transform.localPosition;
    }

    void Update()
    {
        if (isEnabled)
        {
            timer += Time.deltaTime;

            float duration = rising ? riseDuration : fallDuration;
            float t = Mathf.Clamp01(timer / duration);
            float curveT = rising ? riseCurve.Evaluate(t) : fallCurve.Evaluate(t);

            float a = rising ? Mathf.Lerp(bottom, top, curveT) : Mathf.Lerp(top, bottom, curveT);

            pump.transform.localPosition = new Vector3(startLocalPos.x, startLocalPos.y, a);

            if (timer >= duration)
            {
                timer = 0f;
                rising = !rising;
            }
        }
    }
}
