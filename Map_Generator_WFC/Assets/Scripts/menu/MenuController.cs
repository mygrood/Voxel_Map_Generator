using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]Slider sliderSizeX;
    [SerializeField]Slider sliderSizeY;

    [SerializeField]Slider sliderG;
    [SerializeField]Slider sliderGT;
    [SerializeField] Slider sliderW;
    [SerializeField]Slider sliderGC;
    [SerializeField]Slider sliderGIC;
    [SerializeField]Slider sliderGS;

    [SerializeField] GameObject loadingPanel;
 
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        
        StateSettingsController.stMapSize = new Vector2Int((int)sliderSizeX.value,(int) sliderSizeY.value);
        
        StateSettingsController.stWeightGrass = (int)sliderG.value;
        StateSettingsController.stWeightGrassTree = (int)sliderGT.value;
        StateSettingsController.stWeightWater = (int)sliderW.value;
        StateSettingsController.stWeightGrassCorner = (int)sliderGC.value;
        StateSettingsController.stWeightGrassInnerCorner = (int)sliderGIC.value;
        StateSettingsController.stWeightGrassSide = (int)(sliderGS.value);

        loadingPanel.SetActive(true);

        SceneManager.LoadScene("Main");
        //SceneTransition.SwithToScene(1);
    }
}
