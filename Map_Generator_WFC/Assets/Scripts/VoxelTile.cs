using System;
using UnityEngine;

public class VoxelTile : MonoBehaviour
{

    public float VoxelSize = 0.1f; //размер одного вокселя в юнити
    public int TileSideVoxels = 8; //количество вокселей по сторонам 

    [Range(1, 100)]
    public int Weight = 50; //вес тайла при генерации

    public RotationType Rotation; //варианты поворота тайла

    public enum RotationType
    {
        OnlyRotation,
        TwoRotations,
        FourRotations
    }

    //цвета тайлов по сторонам
    [HideInInspector] public int[] ColorsRight; 
    [HideInInspector] public int[] ColorsForward;
    [HideInInspector] public int[] ColorsLeft;
    [HideInInspector] public int[] ColorsBack;

    public void CalculateSidesColors() //получение цветов тайла по сторонам
    {
        ColorsRight = new int[TileSideVoxels * TileSideVoxels];
        ColorsForward = new int[TileSideVoxels * TileSideVoxels];
        ColorsLeft = new int[TileSideVoxels * TileSideVoxels];
        ColorsBack = new int[TileSideVoxels * TileSideVoxels];

        // Заполнение массивов цветов для каждой стороны
        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
                ColorsRight[y * TileSideVoxels + i] = GetVoxelColor(y, i, Direction.Right);
                ColorsForward[y * TileSideVoxels + i] = GetVoxelColor(y, i, Direction.Forward);
                ColorsLeft[y * TileSideVoxels + i] = GetVoxelColor(y, i, Direction.Left);
                ColorsBack[y * TileSideVoxels + i] = GetVoxelColor(y, i, Direction.Back);
            }
        }
    }

    public void Rotate90()
    {
        // Поворот объекта на 90 градусов вокруг оси Y
        transform.Rotate(0, 90, 0);

        int[] colorsRightNew = new int[TileSideVoxels * TileSideVoxels];
        int[] colorsForwardNew = new int[TileSideVoxels * TileSideVoxels];
        int[] colorsLeftNew = new int[TileSideVoxels * TileSideVoxels];
        int[] colorsBackNew = new int[TileSideVoxels * TileSideVoxels];

        // Обновление цветов сторон в соответствии с новым положением после поворота
        for (int layer = 0; layer < TileSideVoxels; layer++)
        {
            for (int offset = 0; offset < TileSideVoxels; offset++)
            {
                colorsRightNew[layer * TileSideVoxels + offset] = ColorsForward[layer * TileSideVoxels + TileSideVoxels - offset - 1];
                colorsForwardNew[layer * TileSideVoxels + offset] = ColorsLeft[layer * TileSideVoxels + offset];
                colorsLeftNew[layer * TileSideVoxels + offset] = ColorsBack[layer * TileSideVoxels + TileSideVoxels - offset - 1];
                colorsBackNew[layer * TileSideVoxels + offset] = ColorsRight[layer * TileSideVoxels + offset];
            }
        }

        ColorsRight = colorsRightNew;
        ColorsForward = colorsForwardNew;
        ColorsLeft = colorsLeftNew;
        ColorsBack = colorsBackNew;
    }

    //получение цвета вокселя: слой и координата по горизонтали
    private int GetVoxelColor(int verticalLayer, int horizontalOffset, Direction direction)
    {
        var meshCollider = GetComponentInChildren<MeshCollider>();

        float vox = VoxelSize;
        float half = VoxelSize / 2;

        // определение начальной точки и направления луча в зависимости от заданного направления
        Vector3 rayStart;
        Vector3 rayDir;
        if (direction == Direction.Right)
        {
            rayStart = meshCollider.bounds.min +
                       new Vector3(-half, 0, half + horizontalOffset * vox);
            rayDir = Vector3.right;
        }
        else if (direction == Direction.Forward)
        {
            rayStart = meshCollider.bounds.min +
                       new Vector3(half + horizontalOffset * vox, 0, -half);
            rayDir = Vector3.forward;
        }
        else if (direction == Direction.Left)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(half, 0, -half - (TileSideVoxels - horizontalOffset - 1) * vox);
            rayDir = Vector3.left;
        }
        else if (direction == Direction.Back)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(-half - (TileSideVoxels - horizontalOffset - 1) * vox, 0, half);
            rayDir = Vector3.back;
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Direction.left/right/back/forward",
                nameof(direction));
        }

        // установка высоты начальной точки луча
        rayStart.y = meshCollider.bounds.min.y + half + verticalLayer * vox;
        //Debug.DrawRay(rayStart, rayDir, Color.red,500) ;


        //луч попал в направлении. длина луча 1 воксель
        if (Physics.Raycast(new Ray(rayStart, rayDir), out RaycastHit hit, vox))
        {
            //получаем цвет в который попал луч
            int colorIndex = (int)(hit.textureCoord.x * 256);

            if (colorIndex == 0) Debug.LogWarning("Found color 0 in mesh palette, this can cause conflicts");

            return colorIndex;
        }

        return -1; //если луч не попал ни в какой объект
    }
}