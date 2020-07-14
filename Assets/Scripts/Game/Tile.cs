using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public TileData data;
    public static Level level;
    public TileIndex index;
    public TileIndex oldIndex;
    private static bool swipe;
    public static Tile lastSelected;


    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            if(value!=selected)
            {
                if (value)
                {
                    oldIndex = new TileIndex(index.x, index.y);
                    level.AddTileSelected(oldIndex);
                    SelectedAnimation();
                }
                else
                {
                    level.RemoveTileSelected(index);
                    ResetAnimation();
                }

            }

            selected = value;


        }
    }
    private void Awake()
    {
        selected = false;
        swipe = false;

    }
   
    private void OnMouseDown()
    {
        swipe = true;
        Selected = true;

    }
    private void OnMouseEnter()
    {
        if (level.IsNeighbour(index) && swipe)
        {
            Selected = level.CheckLinking(index);
            lastSelected = this;
        }

    }

    private void OnMouseExit()
    {
        if (Selected)
        {
            lastSelected = this;
        }

    }

    private void OnMouseUp()
    {
        swipe = false;
        level.EndLinking();

    }

    private void SelectedAnimation()
    {
        if(data.scale.Length>0)
            StartCoroutine(Scale(0,data.scale));
        if (data.rotation.Length > 0)
            StartCoroutine(Rotate(0,data.rotation));
    }
    public void DestroyTileAnimation()
    {
        UiGameController.UpdateTargets(data);
        ResetAnimation();
        StartCoroutine(DestroyTileDelayed());
    }


    #region animation

    IEnumerator Scale(int index , AnimationData[] scale)
    {
        float duration = scale[index].duration;
        Vector3 target = scale[index].target;
        AnimationCurve curve = scale[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localScale;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(start, target*curve.Evaluate(currentTime),  currentTime / duration);
            yield return null;
        }

        if (index < data.scale.Length - 1)
            StartCoroutine(Scale(++index,scale));
        else if (index == data.scale.Length - 1)
            StartCoroutine(Scale(0,scale));

    }
    IEnumerator Rotate(int index, AnimationData[] rotation)
    {
        float duration = rotation[index].duration;
        Vector3 target = rotation[index].target;
        AnimationCurve curve = rotation[index].curve;

        float currentTime = 0;
        Vector3 start = this.transform.localEulerAngles;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            this.transform.localEulerAngles = Vector3.Lerp(start, target*curve.Evaluate(currentTime), currentTime / duration);
            yield return null;
        }

        if (index < data.scale.Length - 1)
            StartCoroutine(Rotate(++index,rotation));
        else if (index == data.rotation.Length - 1)
            StartCoroutine(Rotate(0,rotation));

    }
    IEnumerator DestroyTileDelayed()
    {
        if (data.destroyScale.Length > 0)
        {
            StartCoroutine(Scale(0, data.destroyScale));
        }

        yield return new WaitForSeconds(data.destroyDelay);
        level.DropTiles(oldIndex);
        ResetAnimation();
        TilePooler.Instance.ReturnTileToPool(this.gameObject);

    }
    private void ResetAnimation()
    {
        StopAllCoroutines();
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.one;
    }

    #endregion
}
