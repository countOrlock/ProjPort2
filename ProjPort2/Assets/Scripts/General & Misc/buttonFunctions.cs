using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = gameManager.instance.timeScaleOrig;
    }

    public void StartGameBuildVariable()
    {
#if UNITY_WEBGL
        SceneManager.LoadScene("ForestLevel");
        Time.timeScale = 1;
#else
        SceneManager.LoadScene("Loading Screen");
        Time.timeScale = gameManager.instance.timeScaleOrig;
#endif
    }

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void Options()
    {
        gameManager.instance.OptionMenu();
    }

    public void quit()
    {
#if !UNITY_WEBGL
    #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
#endif
    }

    public void SelectQuest1()
    {
        if (questManager.instance.availableQuests.Count > 0)
        {
            questManager.instance.GiveNewQuest(questManager.instance.availableQuests[0]);
        }
    }

    public void SelectQuest2()
    {
        if (questManager.instance.availableQuests.Count > 1)
        {
            questManager.instance.GiveNewQuest(questManager.instance.availableQuests[1]);
        }
    }

    public void SelectQuest3()
    {
        if (questManager.instance.availableQuests.Count > 2)
        {
            questManager.instance.GiveNewQuest(questManager.instance.availableQuests[2]);
        }
    }

    public void SelectQuest4()
    {
        if (questManager.instance.availableQuests.Count > 3)
        {
            questManager.instance.GiveNewQuest(questManager.instance.availableQuests[3]);
        }
    }
}
