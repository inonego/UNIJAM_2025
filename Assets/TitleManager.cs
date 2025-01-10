using UnityCommunity.UnitySingleton;

using UnityEngine;

using UnityEngine.SceneManagement;

public class TitleManager : MonoSingleton<TitleManager>
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    } 
}
