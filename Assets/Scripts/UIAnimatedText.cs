using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Refresh a Text with animation
/// </summary>
[RequireComponent(typeof(Text))]
public class UIAnimatedText : MonoBehaviour
{
    Text _myText;
    Transform _myTransform;
    void Awake()
    {
        _myText = GetComponent<Text>();
        _myTransform = transform;
    }

    public void NewTextAnimated(string newText)
    {
        _myText.text = newText;
        _myTransform.localScale = Vector3.one*1.5f;
    }

    void Update()
    {
        _myTransform.localScale = Vector3.Lerp(_myTransform.localScale,Vector3.one,Time.deltaTime*12f);
    }
}
