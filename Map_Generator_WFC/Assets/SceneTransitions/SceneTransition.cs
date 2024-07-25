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
        yield return new WaitForSeconds(0.6f); // Время проигрывания анимации фейда
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

        // Чтобы сцена не начала переключаться пока играет анимация closing:
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

            // Чтобы если следующий переход будет обычным SceneManager.LoadScene, не проигрывать анимацию opening:
            shouldPlayOpeningAnimation = false;
        }
    }

    private void Update()
    {
        if (loadingSceneOperation != null)
        {
            LoadingPercentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";

            // Просто присвоить прогресс
            //LoadingProgressBar.fillAmount = loadingSceneOperation.progress; 

            // Присвоить прогресс с быстрой анимацией, чтобы ощущалось плавнее:
            LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress,
                Time.deltaTime * 4);
            Debug.Log(loadingSceneOperation.progress);
        }
       // Debug.Log(instance.loadingSceneOperation.allowSceneActivation);
    }

    public void OnAnimationOver()
    {
        // Чтобы при открытии сцены, куда мы переключаемся, проигралась анимация opening:
        shouldPlayOpeningAnimation = true;

        loadingSceneOperation.allowSceneActivation = true;
    }
}