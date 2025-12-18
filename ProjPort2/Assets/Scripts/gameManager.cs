using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuQuestListFull;
    [SerializeField] GameObject menuQuestTracker;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text gameGoalNeededText;
    [SerializeField] TMP_Text questNameText;
    [SerializeField] TMP_Text questObjectiveText;
    [SerializeField] TMP_Text currentAmmoCountText;
    [SerializeField] TMP_Text maxAmmoCountText;
    [SerializeField] TMP_Text totalMagCountText;

    [Header("===Difficulty===")]
    [SerializeField] GameObject hunter;
    [Range(0, 3)][SerializeField] int hunterCount;
    [Range(0, 300)][SerializeField] int targetGold;
    
    [SerializeField] TMP_Text currentQuest;
    
    public GameObject player;
    public playerController playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public Transform currQuestLoc;
    public GameObject hunterSpawner;
    public int hunterAmountCurr;

    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;
    int currentAmmoCount;
    int maxAmmoCount;
    int totalMagCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        hunterSpawner = GameObject.FindWithTag("Hunter Spawner");
    }

    private void Start()
    {
        checkHunters();
        gameGoalNeededText.text = targetGold.ToString("F0");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount >= targetGold)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void updateCurrentQuest(string questName)
    {
        currentQuest.text = questName;
    }

    public void updateAmmoCount(int currentAmmo, int maxAmmo)
    {
        currentAmmoCount = currentAmmo;
        currentAmmoCountText.text = currentAmmoCount.ToString("F0");

        maxAmmoCount = maxAmmo;
        maxAmmoCountText.text = maxAmmoCount.ToString("F0");
    }

    public void updateMagCount(int currentMags)
    {
        totalMagCount = currentMags;
        totalMagCountText.text = totalMagCount.ToString("F0");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void questListFull()
    {
        statePause();
        menuActive = menuQuestListFull;
        menuActive.SetActive(true);
    }

    public void questTracker()
    {
        menuActive = menuQuestTracker;
        menuActive.SetActive(true);

        questNameText.text = playerScript.questName;
        questObjectiveText.text = playerScript.questObjective;
    }

    public void checkHunters()
    {
        if (hunterAmountCurr < hunterCount)
        {
            hunterSpawner.GetComponent<spawner>().questCall(hunter, hunterCount - hunterAmountCurr);
        }
    }
}
