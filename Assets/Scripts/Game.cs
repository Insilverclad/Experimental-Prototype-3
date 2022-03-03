using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    [HideInInspector] public Camera cam;
    [HideInInspector] public AudioSource mainAudioSource;
    [HideInInspector] public AudioSource secondAudioSource;

    public State state;

    public GameObject[] mainMenuItems;
    public GameObject[,] levelMenuItems;
    
    public GameObject[] levelItemColumns;
    public GameObject levelMenuQuit;

    public GameObject[] colorItems;
    public GameObject colorKeys;

    public SpriteRenderer mainScreen;
    public SpriteRenderer rightScreen;
    public SpriteRenderer leftScreen;

    public GameObject menuHighlight;
    public GameObject colorHighlight;
    public GameObject speedHighlight;

    public SpriteRenderer guideLine;
    public SpriteRenderer guideBox;

    public TMP_Text rightScreenTitle;
    public TMP_Text rightScreenValue;
    public TMP_Text speedTitle;
    public TMP_Text speedValue;
    public TMP_Text speedKey;

    public Sprites textures;
    public Sounds sounds;

    public Level[] levels;
    public int levelIndex;

    public int lastScore;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        textures = new Sprites
        {
            star16 = Resources.Load<Sprite>("Sprites/star_16"),
            star32 = Resources.Load<Sprite>("Sprites/star_32"),
            heart16 = Resources.Load<Sprite>("Sprites/heart_16"),
            heart32 = Resources.Load<Sprite>("Sprites/heart_32"),
            yinyang16 = Resources.Load<Sprite>("Sprites/yin_yang_16"),
            yinyang32 = Resources.Load<Sprite>("Sprites/yin_yang_32"),
            cat32 = Resources.Load<Sprite>("Sprites/cat_32"),
            dog32 = Resources.Load<Sprite>("Sprites/dog_32"),
            diamond32 = Resources.Load<Sprite>("Sprites/diamond_32"),
            bear32 = Resources.Load<Sprite>("Sprites/bear_32"),
            frog32 = Resources.Load<Sprite>("Sprites/frog_32"),
            pot32 = Resources.Load<Sprite>("Sprites/pot_32"),
            coffee32 = Resources.Load<Sprite>("Sprites/coffee_32"),
            triforce32 = Resources.Load<Sprite>("Sprites/triforce_32"),
            monalisa32 = Resources.Load<Sprite>("Sprites/monalisa_32"),
            balloon32 = Resources.Load<Sprite>("Sprites/balloon_32"),
            drop32 = Resources.Load<Sprite>("Sprites/drop_32"),
            icecream32 = Resources.Load<Sprite>("Sprites/icecream_32"),

            square = Resources.Load<Sprite>("Sprites/square")
        };

        sounds = new Sounds
        {
            menuNavigate = Resources.Load<AudioClip>("Sounds/menu_navigate"),
            menuSelect = Resources.Load<AudioClip>("Sounds/menu_select"),
            gameMusic = Resources.Load<AudioClip>("Sounds/game_music"),
            imageLoading = Resources.Load<AudioClip>("Sounds/image_loading"),
            levelComplete = Resources.Load<AudioClip>("Sounds/level_complete"),
            levelFailed = Resources.Load<AudioClip>("Sounds/level_failed"),
            colorPress = Resources.Load<AudioClip>("Sounds/color_press")
        };

        levels = new Level[16];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new Level();

        levels[0].sprite = textures.heart16;
        levels[1].sprite = textures.star16;
        levels[2].sprite = textures.yinyang16;
        levels[3].sprite = textures.heart32;
        levels[4].sprite = textures.star32;
        levels[5].sprite = textures.yinyang32;
        levels[6].sprite = textures.balloon32;
        levels[7].sprite = textures.drop32;
        levels[8].sprite = textures.bear32;
        levels[9].sprite = textures.coffee32;
        levels[10].sprite = textures.cat32;
        levels[11].sprite = textures.dog32;
        levels[12].sprite = textures.triforce32;
        levels[13].sprite = textures.pot32;
        levels[14].sprite = textures.frog32;
        levels[15].sprite = textures.monalisa32;

        levelIndex = 0;
        lastScore = 0;

        LoadLevelSelectItems();

        ObjectsActive(mainMenuItems, false);
        ObjectsActive(levelMenuItems, false);
        ObjectsActive(colorItems, false);
        colorKeys.SetActive(false);

        menuHighlight.SetActive(false);
        colorHighlight.SetActive(false);
        speedHighlight.SetActive(false);

        rightScreen.enabled = false;
        leftScreen.enabled = false;

        guideLine.enabled = false;
        guideBox.enabled = false;

        rightScreenTitle.alpha = 0f;
        rightScreenValue.alpha = 0f;
        speedTitle.alpha = 0f;
        speedValue.alpha = 0f;
        speedKey.alpha = 0f;

        cam = Camera.main;
        mainAudioSource = gameObject.AddComponent<AudioSource>();
        secondAudioSource = gameObject.AddComponent<AudioSource>();
        mainAudioSource.playOnAwake = false;
        secondAudioSource.playOnAwake = false;

        SwitchToState(new StartState(this));
    }

    void Update()
    {
        state.UpdateState();
    }

    public void ObjectsActive(GameObject[] array, bool state)
    {
        foreach (GameObject gameObject in array)
            gameObject.SetActive(state);
    }

    public void ObjectsActive(GameObject[,] array, bool state)
    {
        foreach (GameObject gameObject in array)
            gameObject.SetActive(state);
    }

    public void SwitchToState(State newState)
    {
        if (state != null)
        {
            state.OnExit();
            state = null;
        }

        state = newState;
        state.OnStart();
    }

    private void LoadLevelSelectItems()
    {
        levelMenuItems = new GameObject[2, 9];
        Vector2Int index = new Vector2Int(0, 0);
        int levelIndex = 0;

        foreach (GameObject column in levelItemColumns)
        {
            foreach (Transform child in column.transform)
            {
                child.gameObject.GetComponent<TMP_Text>().text = "Level " + (levelIndex + 1).ToString();
                child.GetComponentInChildren<Image>().sprite = levels[levelIndex].sprite;
                levelMenuItems[index.x, index.y] = child.gameObject;

                index.y += 1;
                levelIndex += 1;
            }
            levelMenuItems[index.x, index.y] = levelMenuQuit;

            index.x += 1;
            index.y = 0;
        }
    }
}
