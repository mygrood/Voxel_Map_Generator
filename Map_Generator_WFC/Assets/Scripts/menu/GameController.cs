using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField] VoxelTilePlacerWFC generator;
    [SerializeField] VoxelTile tileGrass;
    [SerializeField] VoxelTile tileGrassTree;
    [SerializeField] VoxelTile tileWater;
    [SerializeField] VoxelTile tileGrassCorner;
    [SerializeField] VoxelTile tileGrassInnerCorner;
    [SerializeField] VoxelTile tileGrassSide;


    private void Awake()
    {
        generator.MapSize = StateSettingsController.stMapSize;
        tileGrass.Weight = StateSettingsController.stWeightGrass;
        tileGrassTree.Weight = StateSettingsController.stWeightGrassTree;
        tileWater.Weight = StateSettingsController.stWeightWater;
        tileGrassCorner.Weight = StateSettingsController.stWeightGrassCorner;
        tileGrassInnerCorner.Weight = StateSettingsController.stWeightGrassInnerCorner;
        tileGrassSide.Weight = StateSettingsController.stWeightGrassSide;               
    }
    public void GoBack()
    {
        SceneManager.LoadScene("Menu");
        //SceneTransition.SwithToScene(0);
    }
}
