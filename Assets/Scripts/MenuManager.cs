using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public MenuSettings menuSettingsData;
    public int sceneToStart = 1;                                        //Index number in build settings of scene to load if changeScenes is true
    public bool changeMusicOnStart;										//Choose whether to continue playing menu music or start a new music clip
    private CanvasGroup fadeOutImageCanvasGroup;                         //Canvas group used to fade alpha of image which fades in before changing scenes
    private Image fadeImage;                                             //Reference to image used to fade out before changing scenes

    [HideInInspector] public bool inMainMenu = true;                    //If true, pause button disabled in main menu (Cancel in input manager, default escape key)
    [HideInInspector] public AnimationClip fadeAlphaAnimationClip;      //Animation clip fading out UI elements alpha
    
    private float fastFadeIn = .01f;                                    //Very short fade time (10 milliseconds) to start playing music immediately without a click/glitch
    private CanvasGroup menuCanvasGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(transform.gameObject);

        //Get a reference to the CanvasGroup attached to the main menu so that we can fade it's alpha
        menuCanvasGroup = GetComponent<CanvasGroup>();
        fadeImage = GameObject.FindWithTag("Fade").GetComponent<Image>();
        fadeOutImageCanvasGroup = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
        fadeImage.color = menuSettingsData.sceneChangeFadeColor;
        changeMusicOnStart = false;
    }

    public void StartPressed()
    {
        if (fadeImage == null)
        {
            fadeImage = GameObject.FindWithTag("Fade").GetComponent<Image>();
            fadeOutImageCanvasGroup = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
        }
        //If changeScenes is true, start fading and change scenes halfway through animation when screen is blocked by FadeImage
        if (menuSettingsData.nextSceneIndex != 0)
        {
            //Use invoke to delay calling of LoadDelayed by half the length of fadeColorAnimationClip
            Invoke("LoadDelayed", menuSettingsData.menuFadeTime);

            StartCoroutine(FadeCanvasGroupAlpha(0f, 1f, fadeOutImageCanvasGroup));
        }

    }

    public void LoadDelayed()
    {
        //Pause button now works if escape is pressed since we are no longer in Main menu.
        inMainMenu = false;

        //Load the selected scene, by scene index number in build settings
        SceneManager.LoadScene(sceneToStart);
    }

    public IEnumerator FadeCanvasGroupAlpha(float startAlpha, float endAlpha, CanvasGroup canvasGroupToFadeAlpha)
    {

        float elapsedTime = 0f;
        float totalDuration = menuSettingsData.menuFadeTime;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / totalDuration);
            canvasGroupToFadeAlpha.alpha = currentAlpha;
            yield return null;
        }
    }
}