using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeTime = 1.0f;
    [SerializeField] private AnimationCurve _fadeCurve;

    public void StartFadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    public void StartFadeOut()
    {
        StartCoroutine(Fade(0, 1));
    }

    IEnumerator Fade(float start, float end)
    {
        if (_image == null)
        {
            yield break;
        }

        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / _fadeTime;

            Color color = _image.color;
            color.a = Mathf.Lerp(start, end, _fadeCurve.Evaluate(percent));
            _image.color = color;

            yield return null;
        }

        Color finalColor = _image.color;
        finalColor.a = end;
        _image.color = finalColor;
    }
}