using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UICanvasSwitcher : MonoBehaviour
{
    [SerializeField] AnimationCurve _alphaCurve;
    [SerializeField] bool _startsHidden;
    CanvasGroup _myCanvasGroup;

    void Awake()
    {
        _myCanvasGroup = GetComponent<CanvasGroup>();
        
    }

    void Start()
    {
        if (_startsHidden)
        {
            _myCanvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(Fade(0,1,.5f,false));
    }
    public void Hide()
    {
        StartCoroutine(Fade(1,0,.5f,true));
    }

    IEnumerator Fade(float fromAlpha, float toAlpha, float duration, bool disableAfterFade)
    {
        _myCanvasGroup.alpha = fromAlpha;
        float t0 = Time.time;
        float r = 0;
        do
        {
            r = (Time.time - t0) / duration;
            _myCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, _alphaCurve.Evaluate(r));
            yield return null;
        } while (r < 1f);

        _myCanvasGroup.alpha = toAlpha;
        
        if(disableAfterFade)
            gameObject.SetActive(false);
    }

    
}
