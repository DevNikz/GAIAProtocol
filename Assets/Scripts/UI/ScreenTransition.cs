using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim;

    void Start()
    {
        transitionAnim.Play("Trans_LevelStart");
    }
}
