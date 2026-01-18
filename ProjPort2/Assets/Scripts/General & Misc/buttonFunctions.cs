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

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.respawnPlayer();
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
