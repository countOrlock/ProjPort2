using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void SelectQuest1()
    {
        questManager.instance.GiveNewQuest(questManager.instance.availableQuests[0]);
        gameManager.instance.updateAvailableQuests();
    }

    public void SelectQuest2()
    {
        questManager.instance.GiveNewQuest(questManager.instance.availableQuests[1]);
        gameManager.instance.updateAvailableQuests();
    }

    public void SelectQuest3()
    {
        questManager.instance.GiveNewQuest(questManager.instance.availableQuests[2]);
        gameManager.instance.updateAvailableQuests();
    }

    public void SelectQuest4()
    {
        questManager.instance.GiveNewQuest(questManager.instance.availableQuests[3]);
        gameManager.instance.updateAvailableQuests();
    }
}
