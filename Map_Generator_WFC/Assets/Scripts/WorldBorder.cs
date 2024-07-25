using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    //������� �����
    private float mapWidth;
    private float mapLenght;
    private float mapHeight;
    private float mapHeightMin;
    private float mapHeightMax;

    //���������� ������ �����
    public float mapLeft;
    public float mapRight;
    public float mapTop;
    public float mapBottom;

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            // ��������� ���� ���������� � �������� �������� � ����������� �� ������
            Renderer[] rr = transform.GetComponentsInChildren<Renderer>();
            Bounds b = rr[0].bounds;
            foreach (Renderer r in rr) { b.Encapsulate(r.bounds); }


            //������� ����� �� ������ ������
            mapHeight = b.extents.y;
            mapWidth = b.extents.x;
            mapLenght = b.extents.z;
            //Debug.Log($"Map size: width(x) {mapWidth}, lenght(z) {mapLenght},  height(y) {mapHeight}");

            //����������� ������ �����
            mapLeft = b.min.x;
            mapRight = b.max.x;
            mapTop = b.max.z;
            mapBottom = b.min.z;            
            mapHeightMin = b.min.y;
            mapHeightMax = b.max.y;

            Debug.Log($"Map border points: leftBottom ({mapLeft} {mapBottom}),rightBottom({mapRight} {mapBottom}), leftTop({mapLeft} {mapTop}),rightTop({mapRight} {mapTop})");

        }
    }
    private void OnDrawGizmos() //��������� ������ ����� � ���������
    {
        Gizmos.color = Color.magenta;

        //�� ������ ������� ���� �� ������� ������� ����
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapBottom), new Vector3(mapRight, mapHeightMax, mapBottom));
        //�� ������ �������� ���� �� ������� �������� ����
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapTop), new Vector3(mapRight, mapHeightMax, mapTop));
        //�� ������ ������� ���� �� ������ �������� ����
        Gizmos.DrawLine(new Vector3(mapLeft, mapHeightMax, mapBottom), new Vector3(mapLeft, mapHeightMax, mapTop));
        //�� ������� ������� ���� �� ������� �������� ����
        Gizmos.DrawLine(new Vector3(mapRight, mapHeightMax, mapBottom), new Vector3(mapRight, mapHeightMax, mapTop));

    }
}
