using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransition : MonoBehaviour
{
    public TMP_Text LoadingPercentage;
    public Image LoadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayOpeningAnimation = false;

    private Animator componentAnimator;
    private AsyncOperation loadingSceneOperation;

    public static void SwithToScene(int SceneNum)
    {
        instance.StartCoroutine(instance.LoadingScene(SceneNum));
    }
    IEnumerator LoadingScene(int SceneNum)
    {
        componentAnimator.SetTrigger("isClosed");
        yield return new WaitForSeconds(0.6f); // ����� ������������ �������� �����
        loadingSceneOperation = SceneManager.LoadSceneAsync(SceneNum);
        loadingSceneOperation.allowSceneActivation = false;
        while (loadingSceneOperation.progress < 0.9f)
        {
            yield return null;
        }
        shouldPlayOpeningAnimation = true;
        loadingSceneOperation.allowSceneActivation = true;
    }

    /*
    public static void SwitchToScene(string sceneName)
    {
        instance.componentAnimator.SetTrigger("scene_closing");

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);

        // ����� ����� �� ������ ������������� ���� ������ �������� closing:
        instance.loadingSceneOperation.allowSceneActivation = false;

        instance.LoadingProgressBar.fillAmount = 0;
    }
    */

    private void Start()
    {
        instance = this;

        componentAnimator = GetComponent<Animator>();

        if (shouldPlayOpeningAnimation)
        {
            componentAnimator.SetTrigger("scene_opening");
            instance.LoadingProgressBar.fillAmount = 1;

            // ����� ���� ��������� ������� ����� ������� SceneManager.LoadScene, �� ����������� �������� opening:
            shouldPlayOpeningAnimation = false;
        }
    }

    private void Update()
    {
        if (loadingSceneOperation != null)
        {
            LoadingPercentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";

            // ������ ��������� ��������
            //LoadingProgressBar.fillAmount = loadingSceneOperation.progress; 

            // ��������� �������� � ������� ���������, ����� ��������� �������:
            LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress,
                Time.deltaTime * 4);
            Debug.Log(loadingSceneOperation.progress);
        }
       // Debug.Log(instance.loadingSceneOperation.allowSceneActivation);
    }

    public void OnAnimationOver()
    {
        // ����� ��� �������� �����, ���� �� �������������, ����������� �������� opening:
        shouldPlayOpeningAnimation = true;

        loadingSceneOperation.allowSceneActivation = true;
    }
}