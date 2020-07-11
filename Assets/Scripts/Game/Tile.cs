using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public TileData data;
    public Level level;
    private bool selected;
    private static bool swipe;
    private const int MIN_MATCH = 3;

    public Index2D index;

    private void Awake()
    {
        selected = false;
        swipe = false;
    }

    private void OnMouseDown()
    {
        swipe = true;
        SelectTile();
    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseEnter()
    {
        if (level.IsNeighbour(index) && !selected && swipe)
        {
            SelectTile();
            level.StartLinking(index);
            
        }
      
    }

    private void OnMouseUp()
    {
        swipe = false;

        if (level.selectedTiles.Count >= MIN_MATCH)
        {
            level.DestroyTiles();
        }
        else
        {
            level.DeselectTiles();

        }

    }

  

    public void SelectTile()
    {
        level.selectedTiles.Add(this);
        selected = true;
        SelectedAnimation();
    }
    public void DeselectTile()
    {
        selected = false;
    }

    private void SelectedAnimation()
    {
        if(data.scale.Length>0)
            StartCoroutine(Scale(0,data.scale));
        if (data.rotation.Length > 0)
            StartCoroutine(Rotate(0,data.rotation));
    }
    public void DestroyTile()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyTileDelayed());
    }

    IEnumerator Scale(int index , AnimationData[] scale, bool ignoreSelected=false)
    {
        selected = selected || ignoreSelected;
        float duration = scale[index].duration;
        Vector3 target = scale[index].target;
        AnimationCurve curve = scale[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localScale;

        while (selected && currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(start, target * curve.Evaluate(currentTime), currentTime / duration);
            yield return null;
        }

        if (selected && index < data.scale.Length - 1)
            StartCoroutine(Scale(++index,scale));
        else if (index == data.scale.Length - 1)
            StartCoroutine(Scale(0,scale));
        else
            this.transform.localScale = Vector3.one;
    }

    IEnumerator Rotate(int index, AnimationData[] rotation, bool ignoreSelected=false)
    {
        selected = selected || ignoreSelected;
        float duration = rotation[index].duration;
        Vector3 target = rotation[index].target;
        AnimationCurve curve = rotation[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localEulerAngles;

        while (selected && currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localEulerAngles = Vector3.Lerp(start, target * curve.Evaluate(currentTime), currentTime / duration);
            yield return null;
        }

        if (selected && index < data.scale.Length - 1)
            StartCoroutine(Rotate(++index,rotation));
        else if (index == data.rotation.Length - 1)
            StartCoroutine(Rotate(0,rotation));
        else
         this.transform.localEulerAngles = Vector3.zero;
    }

    IEnumerator DestroyTileDelayed()
    {
        ResetAnimation();

        if (data.destroyScale.Length > 0)
        {
            StartCoroutine(Scale(0, data.destroyScale,true));
        }

        yield return new WaitForSeconds(data.destroyDelay);

        Destroy(this.gameObject);
    }

    private void ResetAnimation()
    {
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.one;

    }

}
