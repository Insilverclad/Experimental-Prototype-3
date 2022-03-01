using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    [HideInInspector] public Camera cam;
    [HideInInspector] public AudioSource audioSource;

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

    public Textures textures;
    public Sounds sounds;

    public Level[] levels;
    public int levelIndex;

    public int lastScore;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        textures = new Textures
        {
            star16 = Resources.Load<Texture2D>("Sprites/star_16"),
            star32 = Resources.Load<Texture2D>("Sprites/star_32"),
            heart16 = Resources.Load<Texture2D>("Sprites/heart_16"),
            heart32 = Resources.Load<Texture2D>("Sprites/heart_32"),
            yinyang16 = Resources.Load<Texture2D>("Sprites/yin_yang_16"),
            yinyang32 = Resources.Load<Texture2D>("Sprites/yin_yang_32"),
            cat32 = Resources.Load<Texture2D>("Sprites/cat_32"),
            dog32 = Resources.Load<Texture2D>("Sprites/dog_32"),
            diamond32 = Resources.Load<Texture2D>("Sprites/diamond_32"),
            bear32 = Resources.Load<Texture2D>("Sprites/bear_32"),
            frog32 = Resources.Load<Texture2D>("Sprites/frog_32"),
            pot32 = Resources.Load<Texture2D>("Sprites/pot_32"),
            coffee32 = Resources.Load<Texture2D>("Sprites/coffee_32"),
            triforce32 = Resources.Load<Texture2D>("Sprites/triforce_32"),
            monalisa32 = Resources.Load<Texture2D>("Sprites/monalisa_32"),

            square = Resources.Load<Sprite>("Sprites/square")
        };

        sounds = new Sounds
        {
            menuNavigate = Resources.Load<AudioClip>("Sounds/menu_navigate"),
            menuSelect = Resources.Load<AudioClip>("Sounds/menu_select"),
            gameMusic = Resources.Load<AudioClip>("Sounds/game_music")
        };

        levels = new Level[16];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new Level();

        levels[0].texture = textures.heart16;
        levels[1].texture = textures.star16;
        levels[2].texture = textures.yinyang16;
        levels[3].texture = textures.heart32;
        levels[4].texture = textures.star32;
        levels[5].texture = textures.yinyang32;
        levels[6].texture = textures.bear32;
        levels[7].texture = textures.coffee32;
        levels[8].texture = textures.cat32;
        levels[9].texture = textures.dog32;
        levels[10].texture = textures.triforce32;
        levels[11].texture = textures.pot32;
        levels[12].texture = textures.frog32;
        levels[13].texture = textures.diamond32;
        levels[14].texture = textures.heart32;
        levels[15].texture = textures.monalisa32;

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
        audioSource = GetComponent<AudioSource>();

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

        foreach (GameObject column in levelItemColumns)
        {
            foreach (Transform child in column.transform)
            {
                levelMenuItems[index.x, index.y] = child.gameObject;

                index.y += 1;
            }
            levelMenuItems[index.x, index.y] = levelMenuQuit;

            index.x += 1;
            index.y = 0;
        }
    }
}
