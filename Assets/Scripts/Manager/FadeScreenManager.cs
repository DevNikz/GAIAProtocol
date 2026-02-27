using PrimeTween;
using UnityEditor;
using UnityEngine;

public class FadeScreenManager : MonoBehaviour
{
    public static FadeScreenManager Instance { get; private set; }
    [Header("Reference")]
    [SerializeField] GameObject canvas;
    [SerializeField] RectTransform image;
    
    [Header("Settings")]
    [SerializeField] TweenSettings<float> fadePosXToCenter;
    [SerializeField] TweenSettings<float> fadePosXToEnd;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Animate()
    {
        Sequence.SequenceCycleMode test = Sequence.SequenceCycleMode.Restart;
        Sequence.Create(cycles: 1, test)
            .Chain(Tween.PositionX(image, fadePosXToCenter))
            .Chain(Tween.PositionX(image, fadePosXToEnd))
            .ChainCallback(() => Debug.Log("Animation Completed."));
    }

    public void FadeIn()
    {
        Tween.PositionX(image, fadePosXToCenter);
    }

    public void FadeOut()
    {
        Tween.PositionX(image, fadePosXToEnd);
    }

}