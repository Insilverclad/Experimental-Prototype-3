using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    public Camera cam;
    public State state;

    public MenuButtons mainMenu;
    public MenuButtons levelMenu;
    public GameObject selectionBox;

    public SpriteRenderer rightScreen;
    public SpriteRenderer guideLine;
    public SpriteRenderer guideBox;

    public TMP_Text startText;
    public TMP_Text accuracyTitle;
    public TMP_Text accuracyText;

    public Textures textures;

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

            square = Resources.Load<Sprite>("Sprites/square")
        };

        levels = new Level[9];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new Level();

        levels[0].texture = textures.heart16;
        levels[1].texture = textures.star16;
        levels[2].texture = textures.yinyang16;
        levels[3].texture = textures.heart32;
        levels[4].texture = textures.star32;
        levels[5].texture = textures.yinyang32;
        levels[6].texture = textures.diamond32;
        levels[7].texture = textures.cat32;
        levels[8].texture = textures.dog32;

        levelIndex = 0;
        lastScore = 0;

        mainMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
        selectionBox.SetActive(false);

        rightScreen.enabled = true;

        guideLine.enabled = false;
        guideBox.enabled = false;

        startText.alpha = 0f;
        accuracyTitle.alpha = 0f;
        accuracyText.alpha = 0f;

        cam = Camera.main;

        SwitchToState(new StartState(this));
    }

    void Update()
    {
        state.UpdateState();
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
}
