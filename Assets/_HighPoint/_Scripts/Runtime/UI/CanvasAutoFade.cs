using DG.Tweening;
using UnityEngine; 

[RequireComponent(typeof(CanvasGroup))]
public class CanvasAutoFade : MonoBehaviour
{
    enum FadeMode {
        IN,
        OUT
    }

    [SerializeField] float _delay;
    [SerializeField] float _fadeTime = 2f;
    [SerializeField] FadeMode _fadeMode;

    CanvasGroup _grp;

    void Awake()
    {
        _grp = GetComponent<CanvasGroup>();

        switch (_fadeMode)
        {
            case FadeMode.IN:
                _grp.alpha = 0f;
                _grp.blocksRaycasts = false;
                break;

            case FadeMode.OUT:
                _grp.alpha = 1f;
                _grp.blocksRaycasts = true;
                break;
        }
    }

    void Start()
    {
        // Disable in editor
// #if UNITY_EDITOR
//         _delay = 0f;
//         _fadeTime = 0f;
// #endif

        switch (_fadeMode)
        {
            case FadeMode.IN:
                _grp.DOFade(1f, _fadeTime).SetDelay(_delay);
                _grp.blocksRaycasts = true;
                break;

            case FadeMode.OUT:
                _grp.DOFade(0f, _fadeTime).SetDelay(_delay);
                _grp.blocksRaycasts = false;
                break;
        } 
    }
}