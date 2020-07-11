using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData data;
    private bool selected;

    private void Awake()
    {
        selected = false;
    }

    private void OnMouseDown()
    {
        selected = true;
        SelectedAnimation();
    }

    private void SelectedAnimation()
    {
        if(data.scale.Length>0)
            StartCoroutine(Scale(0));
        if (data.rotation.Length > 0)
            StartCoroutine(Rotate(0));
    }

    IEnumerator Scale(int index)
    {
        float duration = data.scale[index].duration;
        Vector3 target = data.scale[index].target;
        AnimationCurve curve = data.scale[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localScale;

        while (selected && currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(start, target * curve.Evaluate(currentTime), currentTime / duration);
            yield return null;
        }

        if (selected && index < data.scale.Length - 1)
            StartCoroutine(Scale(++index));
        else if (index == data.scale.Length - 1)
            StartCoroutine(Scale(0));
        else
            this.transform.localScale = Vector3.one;
    }

    IEnumerator Rotate(int index)
    {
        float duration = data.rotation[index].duration;
        Vector3 target = data.rotation[index].target;
        AnimationCurve curve = data.rotation[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localEulerAngles;

        while (selected && currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localEulerAngles = Vector3.Lerp(start, target * curve.Evaluate(currentTime), currentTime / duration);
            yield return null;
        }

        if (selected && index < data.scale.Length - 1)
            StartCoroutine(Rotate(++index));
        else if (index == data.rotation.Length - 1)
            StartCoroutine(Rotate(0));
        else
            this.transform.localScale = Vector3.zero;
    }



}
