using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    //размеры карты
    private float mapWidth;
    private float mapLenght;
    private float mapHeight;
    private float mapHeightMin;
    private float mapHeightMax;

    //координаты границ карты
    public float mapLeft;
    public float mapRight;
    public float mapTop;
    public float mapBottom;

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            // получение всех рендереров в дочерних объектах и объединение их границ
            Renderer[] rr = transform.GetComponentsInChildren<Renderer>();
            Bounds b = rr[0].bounds;
            foreach (Renderer r in rr) { b.Encapsulate(r.bounds); }


            //размеры карты на основе границ
            mapHeight = b.extents.y;
            mapWidth = b.extents.x;
            mapLenght = b.extents.z;
            //Debug.Log($"Map size: width(x) {mapWidth}, lenght(z) {mapLenght},  height(y) {mapHeight}");

            //кооридинаты границ карты
            mapLeft = b.min.x;
            mapRight = b.max.x;
            mapTop = b.max.z;
            mapBottom = b.min.z;            
            mapHeightMin = b.min.y;
            mapHeightMax = b.max.y;

            Debug.Log($"Map border points: leftBottom ({mapLeft} {mapBottom}),rightBottom({mapRight} {mapBottom}), leftTop({mapLeft} {mapTop}),rightTop({mapRight} {mapTop})");

        }
    }
    private void OnDrawGizmos() //отрисовка границ карты в редакторе
    {
        Gizmos.color = Color.magenta;

        //от левого нижнего угла до правого нижнего угла
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapBottom), new Vector3(mapRight, mapHeightMax, mapBottom));
        //от левого верхнего угла до правого верхнего угла
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapTop), new Vector3(mapRight, mapHeightMax, mapTop));
        //от левого нижнего угла до левого верхнего угла
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapBottom), new Vector3(mapLeft, mapHeightMax, mapTop));
        //от правого нижнего угла до правого верхнего угла
        Gizmos.DrawLine(new Vector3(mapRight, mapHeightMax, mapBottom), new Vector3(mapRight, mapHeightMax, mapTop));

    }
}
