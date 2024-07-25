using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoxelTilePlacerWFC : MonoBehaviour
{
    public List<VoxelTile> TilePrefabs; //����� ��� ��������� �����
    public List<VoxelTile> BorderTiles; //����� ��� ��������� ������ �����
    public List<VoxelTile> OutsideTiles; //����� ��� ��������� ������ �����

    public Vector2Int MapSize = new Vector2Int(10, 10); //������ �����

    private VoxelTile[,] spawnedTiles; 
    public Transform map;

    private Queue<Vector2Int> recalcPossibleTilesQueue = new Queue<Vector2Int>(); // ������� ��� ���������� ���������� ��������� ������
    private List<VoxelTile>[,] possibleTiles; //��������� ����� ��� ������ ������� �� �����



    private void Start()
    {
        map.gameObject.SetActive(false);

        spawnedTiles = new VoxelTile[MapSize.x, MapSize.y];

        // ���������� ������ ������ ��� ���� �������� ������
        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            tilePrefab.CalculateSidesColors();
        }

        // ���������� ��������� ������ � ������ ��������
        int countBeforeAdding = TilePrefabs.Count;
        for (int i = 0; i < countBeforeAdding; i++)
        {
            VoxelTile clone;
            switch (TilePrefabs[i].Rotation)
            {
                case VoxelTile.RotationType.OnlyRotation:
                    break;

                case VoxelTile.RotationType.TwoRotations:
                    TilePrefabs[i].Weight /= 2;
                    if (TilePrefabs[i].Weight <= 0) TilePrefabs[i].Weight = 1;

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right,
                        Quaternion.identity);
                    clone.Rotate90();
                    TilePrefabs.Add(clone);
                    break;

                case VoxelTile.RotationType.FourRotations:
                    TilePrefabs[i].Weight /= 4;
                    if (TilePrefabs[i].Weight <= 0) TilePrefabs[i].Weight = 1;

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right,
                        Quaternion.identity);
                    clone.Rotate90();
                    TilePrefabs.Add(clone);

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right * 2,
                        Quaternion.identity);
                    clone.Rotate90();
                    clone.Rotate90();
                    TilePrefabs.Add(clone);

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right * 3,
                        Quaternion.identity);
                    clone.Rotate90();
                    clone.Rotate90();
                    clone.Rotate90();
                    TilePrefabs.Add(clone);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Generate();
        map.gameObject.SetActive(true);        
    }

    private void Update()
    {

        // ������������� ����� ��� ������� ������� Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (VoxelTile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);

            }

            Generate();
        }
    }

    private void Generate()
    {
        possibleTiles = new List<VoxelTile>[MapSize.x, MapSize.y];

        int maxAttempts = 10;
        int attempts = 0;
        while (attempts++ < maxAttempts)
        {
            // ���������� ��������� ������ ��� ������ �������
            for (int x = 0; x < MapSize.x; x++)
                for (int y = 0; y < MapSize.y; y++)
                {
                    possibleTiles[x, y] = new List<VoxelTile>(TilePrefabs);
                }
            // ��������� ��������� ����� � ����� �����
            VoxelTile tileInCenter = GetRandomTile(TilePrefabs);
            possibleTiles[MapSize.x / 2, MapSize.y / 2] = new List<VoxelTile> { tileInCenter };

            recalcPossibleTilesQueue.Clear();
            EnqueueNeighboursToRecalc(new Vector2Int(MapSize.x / 2, MapSize.y / 2));

            // ��������� ���� ��������� ������
            bool success = GenerateAllPossibleTiles();

            if (success) break;
        }
        // ���������� ��������� ������,�������� ����� � ���������� ������� �� ��������� ����� �����
        PlaceBorderTiles();
        PlaceAllTiles();
        FillOutsideMapWithWater(70);


    }

    private bool GenerateAllPossibleTiles()
    {
        int maxIterations = MapSize.x * MapSize.y;
        int iterations = 0;
        int backtracks = 0;

        while (iterations++ < maxIterations)
        {
            int maxInnerIterations = 500;
            int innerIterations = 0;

            while (recalcPossibleTilesQueue.Count > 0 && innerIterations++ < maxInnerIterations)
            {
                Vector2Int position = recalcPossibleTilesQueue.Dequeue();
                if (position.x == 0 || position.y == 0 ||
                    position.x == MapSize.x - 1 || position.y == MapSize.y - 1)
                {
                    continue;
                }

                List<VoxelTile> possibleTilesHere = possibleTiles[position.x, position.y];

                // �������� ����������� ������ ��� ������ �������
                int countRemoved = possibleTilesHere.RemoveAll(t => !IsTilePossible(t, position));
                if (countRemoved > 0) EnqueueNeighboursToRecalc(position);

                if (possibleTilesHere.Count == 0)
                {
                    // ���� ��� ������ ������� �� �������� ��������� ������, ��������� �����                   
                    possibleTilesHere.AddRange(TilePrefabs);
                    possibleTiles[position.x + 1, position.y] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x - 1, position.y] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x, position.y + 1] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x, position.y - 1] = new List<VoxelTile>(TilePrefabs);

                    EnqueueNeighboursToRecalc(position);

                    backtracks++;
                }
            }

            if (innerIterations == maxInnerIterations) break;

            // ����� ������� � ���������� ������ ��������� ������
            List<VoxelTile> maxCountTile = possibleTiles[1, 1];
            Vector2Int maxCountTilePosition = new Vector2Int(1, 1);

            for (int x = 1; x < MapSize.x - 1; x++)
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (possibleTiles[x, y].Count > maxCountTile.Count)
                    {
                        maxCountTile = possibleTiles[x, y];
                        maxCountTilePosition = new Vector2Int(x, y);
                    }
                }

            if (maxCountTile.Count == 1)
            {
                Debug.Log($"Generated for {iterations} iterations, with {backtracks} backtracks");
                return true;
            }

            // ����� ���������� ����� ��� ������ �������
            VoxelTile tileToCollapse = GetRandomTile(maxCountTile);
            possibleTiles[maxCountTilePosition.x, maxCountTilePosition.y] = new List<VoxelTile> { tileToCollapse };
            EnqueueNeighboursToRecalc(maxCountTilePosition);
        }

        Debug.Log($"Failed, run out of iterations with {backtracks} backtracks");
        return false;
    }

    private bool IsTilePossible(VoxelTile tile, Vector2Int position)
    {
        // ��������, �������� �� ���������� ���� �� ������ �������, �������� �������� �����
        bool isAllRightImpossible = possibleTiles[position.x - 1, position.y]
            .All(rightTile => !CanAppendTile(tile, rightTile, Direction.Right));
        if (isAllRightImpossible) return false;

        bool isAllLeftImpossible = possibleTiles[position.x + 1, position.y]
            .All(leftTile => !CanAppendTile(tile, leftTile, Direction.Left));
        if (isAllLeftImpossible) return false;

        bool isAllForwardImpossible = possibleTiles[position.x, position.y - 1]
            .All(fwdTile => !CanAppendTile(tile, fwdTile, Direction.Forward));
        if (isAllForwardImpossible) return false;

        bool isAllBackImpossible = possibleTiles[position.x, position.y + 1]
            .All(backTile => !CanAppendTile(tile, backTile, Direction.Back));
        if (isAllBackImpossible) return false;

        return true;
    }

    private void PlaceAllTiles() // ���������� ���� ������ �� �����
    {
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }
        }
    }


    private void EnqueueNeighboursToRecalc(Vector2Int position)
    {
        // ���������� �������� ������� � ������� ��� ���������� ���������� ��������� ������
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x + 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x - 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y + 1));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y - 1));
    }

    private void PlaceTile(int x, int y) // ���������� ����� �� ���������� �������
    {
        if (possibleTiles[x, y].Count == 0) return;

        VoxelTile selectedTile = GetRandomTile(possibleTiles[x, y]);
        Vector3 position = selectedTile.VoxelSize * selectedTile.TileSideVoxels * new Vector3(x, 0, y);
        spawnedTiles[x, y] = Instantiate(selectedTile, position, selectedTile.transform.rotation, map);


    }

    private VoxelTile GetRandomTile(List<VoxelTile> availableTiles)
    {
        // ����� ���������� ����� �� ������ ��������� � ������ �� �����
        List<float> chances = new List<float>();
        for (int i = 0; i < availableTiles.Count; i++)
        {
            chances.Add(availableTiles[i].Weight);
        }

        float value = Random.Range(0, chances.Sum());
        float sum = 0;

        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return availableTiles[i];
            }
        }

        return availableTiles[availableTiles.Count - 1];
    }

    private bool CanAppendTile(VoxelTile existingTile, VoxelTile tileToAppend, Direction direction)
    {
        // ��������, ����� �� ���������� ���� ����� � ������������, �������� �� �������
        if (existingTile == null) return true;

        if (direction == Direction.Right)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
        }
        else if (direction == Direction.Left)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
        }
        else if (direction == Direction.Forward)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
        }
        else if (direction == Direction.Back)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
                nameof(direction));
        }
    }

    private void PlaceBorderTiles()
    {
        int borderLayerCount = 3; // ���������� ����� ������

        for (int layer = 0; layer < borderLayerCount; layer++)
        {
            int offset = layer; // �������� ��� �������� ����

            // ������� � ������ �������
            for (int x = offset; x < MapSize.x - offset; x++)
            {
                PlaceBorderTile(x, offset, Direction.Forward); // ������ �������
                PlaceBorderTile(x, MapSize.y - 1 - offset, Direction.Back); // ������� �������
            }

            // ����� � ������ �������
            for (int y = offset; y < MapSize.y - offset; y++)
            {
                PlaceBorderTile(offset, y, Direction.Left); // ����� �������
                PlaceBorderTile(MapSize.x - 1 - offset, y, Direction.Right); // ������ �������
            }
        }
    }

    private void PlaceBorderTile(int x, int y, Direction direction)
    {
        VoxelTile borderTile = GetRandomTile(BorderTiles); // �������� ��������� ���� ��� �������
        Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);

        // ��������� ���� ���, ����� �� �������� �� �����������
        Quaternion rotation = Quaternion.identity;
        switch (direction)
        {
            case Direction.Left:
                rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Direction.Right:
                rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Direction.Forward:
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Back:
                rotation = Quaternion.Euler(0, 180, 0);
                break;
        }

        // ���������, ����� �� ��������� ���� �� �������
        if (CanFitBorderTile(x, y, borderTile, direction))
        {
            spawnedTiles[x, y] = Instantiate(borderTile, position, rotation, map);
        }
    }

    private bool CanFitBorderTile(int x, int y, VoxelTile borderTile, Direction direction)
    {
        // ��������� ������������ ����� ��� ������� � ��������� �������
        VoxelTile neighborTile = null;
        switch (direction)
        {
            case Direction.Left:
                if (x > 0) neighborTile = spawnedTiles[x - 1, y];
                return neighborTile == null || CanAppendTile(neighborTile, borderTile, Direction.Right);
            case Direction.Right:
                if (x < MapSize.x - 1) neighborTile = spawnedTiles[x + 1, y];
                return neighborTile == null || CanAppendTile(borderTile, neighborTile, Direction.Left);
            case Direction.Forward:
                if (y > 0) neighborTile = spawnedTiles[x, y - 1];
                return neighborTile == null || CanAppendTile(neighborTile, borderTile, Direction.Back);
            case Direction.Back:
                if (y < MapSize.y - 1) neighborTile = spawnedTiles[x, y + 1];
                return neighborTile == null || CanAppendTile(borderTile, neighborTile, Direction.Forward);
            default:
                return true;
        }
    }

    private void FillRemainingWithWater()
    {      
        VoxelTile waterTile = GetRandomTile(BorderTiles);

        // �������� �� ���� ����� � ������������� ����� ���� � ������ ������
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                if (spawnedTiles[x, y] == null) // ���� ���� ��� �� ����������
                {
                    Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);
                    spawnedTiles[x, y] = Instantiate(waterTile, position, Quaternion.identity, map);
                }
            }
        }
    }


    private void FillOutsideMapWithWater(int borderSize)
    {
       
        VoxelTile waterTile = GetRandomTile(OutsideTiles);
              

        // ��������� ������� � ������ ������� �� ���������
        for (int x = -borderSize; x < MapSize.x + borderSize; x++)
        {
            for (int y = -borderSize; y < 0; y++) // ������� �������
            {
                Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);
                Instantiate(waterTile, position, Quaternion.identity, map);
            }

            for (int y = MapSize.y; y < MapSize.y + borderSize; y++) // ������ �������
            {
                Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);
                Instantiate(waterTile, position, Quaternion.identity, map);
            }
        }

        // ��������� ����� � ������ ������� �� ���������
        for (int y = -borderSize; y < MapSize.y + borderSize; y++)
        {
            for (int x = -borderSize; x < 0; x++) // ����� �������
            {
                Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);
                Instantiate(waterTile, position, Quaternion.identity, map);
            }

            for (int x = MapSize.x; x < MapSize.x + borderSize; x++) // ������ �������
            {
                Vector3 position = new Vector3(x * 0.8f, 0, y * 0.8f);
                Instantiate(waterTile, position, Quaternion.identity, map);
            }
        }
    }

}