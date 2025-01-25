using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{

    #region Singleton

    public static SceneController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
    
    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            SoundManager.instance.CheckSceneSound();
            FadeManager.instance.FadeIn();
        };
    }
    
    public void LoadScene(string sceneName)
    {
        FadeManager.instance.FadeOut(onComplete: () => { SceneManager.LoadScene(sceneName); });
    }
}
