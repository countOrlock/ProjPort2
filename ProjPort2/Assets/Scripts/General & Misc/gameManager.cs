using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Audio;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("===Menus===")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuQuests;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuOptions;

    [Header("=====Popups=====")]
    [SerializeField] GameObject interactPopup;

    [Header("===Displayed Text===")]
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text gameGoalNeededText;
    [SerializeField] TMP_Text questNameText;
    [SerializeField] TMP_Text questObjectiveText;
    [SerializeField] TMP_Text currentAmmoCountText;
    [SerializeField] TMP_Text maxAmmoCountText;
    [SerializeField] TMP_Text totalMagCountText;
    [SerializeField] TMP_Text throwableItemText;
    [SerializeField] TMP_Text itemCountText;
    [SerializeField] TMP_Text dayTimeMinutes;
    [SerializeField] TMP_Text dayTimeSeconds;

    [Header("===Displayed Active Quest Text===")]
    [SerializeField] TMP_Text activeQuest1Title;
    [SerializeField] TMP_Text activeQuest2Title;
    [SerializeField] TMP_Text activeQuest1Current;
    [SerializeField] TMP_Text activeQuest2Current;
    [SerializeField] TMP_Text activeQuest1Target;
    [SerializeField] TMP_Text activeQuest2Target;

    [Header("===Displayed Quest Menu Text===")]
    [SerializeField] TMP_Text availableQuest1Title;
    [SerializeField] TMP_Text availableQuest1Description;

    [SerializeField] TMP_Text availableQuest2Title;
    [SerializeField] TMP_Text availableQuest2Description;

    [SerializeField] TMP_Text availableQuest3Title;
    [SerializeField] TMP_Text availableQuest3Description;

    [SerializeField] TMP_Text availableQuest4Title;
    [SerializeField] TMP_Text availableQuest4Description;

    [Header("===Difficulty===")]
    [SerializeField] GameObject hunter;
    [Range(0, 3)][SerializeField] int hunterCount;
    [Range(0, 300)][SerializeField] int targetGold;

    [Header("===Misc Variables===")]
    [SerializeField] AudioClip DefaultInGameMusic;
    //[SerializeField] AudioMixer mixer;
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;
    public Interactor playerInteract;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject hunterSpawner;
    public int hunterAmountCurr;

    public bool isPaused;

    public float timeScaleOrig;
    int gameGoalCount;
    int currentAmmoCount;
    int maxAmmoCount;
    int totalMagCount;
    int itemCount;

    //public const string MASTER_KEY = "masterVolume";
    //public const string MUSIC_KEY  = "musicVolume";
    //public const string SFX_KEY    = "sfxVolume";

    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerInteract = player.GetComponent<Interactor>();

        hunterSpawner = GameObject.FindWithTag("Hunter Spawner");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

    }

    private void Start()
    {
        checkHunters();
        gameGoalCount = 0;
        MusicManager.instance.defaultTrack = DefaultInGameMusic;
        MusicManager.instance.ReturnToDefaultTrack();

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
            else if (menuActive == menuPause || menuActive == menuShop)
            {
                stateUnpause();
            }
            else if (menuActive == menuOptions)
            {
                menuActive.SetActive(false);
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
        }

        // For Quest Menu (REMOVED)
        //if (Input.GetButtonDown("Quests"))
        //{
        //    if (menuActive == null)
        //    {
        //        statePause();
        //        menuActive = menuQuests;
        //        menuActive.SetActive(true);
        //    }
        //    else if (menuActive == menuQuests)
        //    {
        //        stateUnpause();
        //    }
        //}
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

    public void ShopMenu()
    {
        statePause();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void OptionMenu()
    {
        statePause();
        menuActive = menuOptions;
        menuActive.SetActive(true);
    }

    public void InteractOn()
    {
        interactPopup.SetActive(true);
    }

    public void InteractOff()
    {
        interactPopup.SetActive(false);
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount >= questManager.instance.rentAmountDue)
        {
            youWin();
        }
    }

    public void updateGameGoalNeeded(int amount)
    {
        gameGoalNeededText.text = amount.ToString();
    }

    public void updateAvailableQuests()
    {
        questInfo defaultQuest = ScriptableObject.Instantiate(questManager.instance.defaultQuest);

        if (questManager.instance.availableQuests.Count > 0)
        {
            availableQuest1Title.text       = questManager.instance.availableQuests[0].questName;
            availableQuest1Description.text = questManager.instance.availableQuests[0].questObjective;
        }
        else
        {
            availableQuest1Title.text       = defaultQuest.questName;
            availableQuest1Description.text = defaultQuest.questObjective;
        }

        if (questManager.instance.availableQuests.Count > 1)
        {
            availableQuest2Title.text       = questManager.instance.availableQuests[1].questName;
            availableQuest2Description.text = questManager.instance.availableQuests[1].questObjective;
        }
        else
        {
            availableQuest2Title.text       = defaultQuest.questName;
            availableQuest2Description.text = defaultQuest.questObjective;
        }

        if (questManager.instance.availableQuests.Count > 2)
        {
            availableQuest3Title.text       = questManager.instance.availableQuests[2].questName;
            availableQuest3Description.text = questManager.instance.availableQuests[2].questObjective;
        }
        else
        {
            availableQuest3Title.text       = defaultQuest.questName;
            availableQuest3Description.text = defaultQuest.questObjective;
        }

        if (questManager.instance.availableQuests.Count > 3)
        {
            availableQuest4Title.text       = questManager.instance.availableQuests[3].questName;
            availableQuest4Description.text = questManager.instance.availableQuests[3].questObjective;
        }
        else
        {
            availableQuest4Title.text       = defaultQuest.questName;
            availableQuest4Description.text = defaultQuest.questObjective;
        }
    }

    public void updateActiveQuest1(string questName, int current, int target)
    {
        activeQuest1Title.text   = questName;
        activeQuest1Current.text = current.ToString();
        activeQuest1Target.text  = target.ToString();
    }

    public void updateActiveQuest2(string questName, int current, int target)
    {
        activeQuest2Title.text   = questName;
        activeQuest2Current.text = current.ToString();
        activeQuest2Target.text  = target.ToString();
    }

    public void updateDayTime(float timeInSeconds)
    {
        int minutesLeft = (int)(timeInSeconds / 60);
        int secondsLeft = (int)(timeInSeconds % 60);

        dayTimeMinutes.text = minutesLeft.ToString();

        if (secondsLeft < 10)
        {
            dayTimeSeconds.text = (padSingleDigitString(secondsLeft.ToString(), 1));
        }
        else
        {
            dayTimeSeconds.text = secondsLeft.ToString();
        }
    }

    public string padSingleDigitString(string singleDigit, int padding)
    {
        string newDigit = "";
        for (int i = 0; i < padding; i++)
        {
            newDigit += "0";
        }
        newDigit += singleDigit;
        return newDigit;

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

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void checkHunters()
    {
        if (hunterAmountCurr < hunterCount)
        {
            int amountToSpawn = hunterCount - hunterAmountCurr;
            hunterSpawner.GetComponent<spawner>().spawnAssign(hunter, amountToSpawn);
            hunterAmountCurr += amountToSpawn;
        }
    }

    public void updateItem(string name)
    {
        throwableItemText.text = name;
    }

    public void updateItemCount(int ammo)
    {
        itemCount = ammo;
        itemCountText.text = itemCount.ToString("F0");
    }
}
