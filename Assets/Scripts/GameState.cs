using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : State
{
    private Vector2 frameSize;
    
    private Cell[,] referenceGrid;
    private Cell[,] canvasGrid;
    private GameObject gridLineObject;

    private Color[] imageColors;

    private Color[] paintColors;
    private int colorIndex;

    private Vector2Int cellIndex;

    private bool hasColored;
    private float timer;
    private float speed;
    private float baseSpeed;
    private float boostSpeed;

    private int coloredCellCount;
    private int incorrectCells;
    private int accuracy;

    private float gameOverTimer;
    private float gameOverDuration;

    private bool running;

    private float initializeTimer;
    private float initializeDuration;
    private int pixelsColored;

    private bool initialized;

    public GameState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        frameSize = new Vector2(16.0f, 16.0f);

        Texture2D texture = game.levels[game.levelIndex].sprite.texture;
        imageColors = texture.GetPixels();

        referenceGrid = LoadImageGrid(texture);
        PaintGrid(referenceGrid, new Color(1.0f, 1.0f, 1.0f, 1.0f));

        canvasGrid = LoadImageGrid(texture);
        PaintGrid(canvasGrid, new Color(1.0f, 1.0f, 1.0f, 0.35f));

        CreateGridLines(canvasGrid);

        paintColors = new Color[5];
        paintColors[0] = Color.white;
        paintColors[1] = Color.black;
        paintColors[2] = Color.red;
        paintColors[3] = Color.green;
        paintColors[4] = Color.blue;
        colorIndex = 0;

        cellIndex = new Vector2Int(0, canvasGrid.GetLength(1) - 1);

        hasColored = false;
        timer = 0f;
        speed = 0f;
        baseSpeed = 6.0f;
        boostSpeed = 20.0f * (texture.width / 16.0f) * 0.5f + 10.0f;

        incorrectCells = 0;
        accuracy = 100;

        game.rightScreenTitle.alpha = 1.0f;
        game.rightScreenTitle.text = "Loading...";
        game.rightScreenValue.alpha = 1.0f;
        game.rightScreenValue.text = "";
        game.speedTitle.alpha = 1f;
        game.speedValue.alpha = 1f;
        game.speedKey.alpha = 1f;

        game.rightScreen.enabled = true;
        game.leftScreen.enabled = true;

        game.ObjectsActive(game.colorItems, true);
        game.colorKeys.SetActive(true);

        gameOverTimer = 0f;
        gameOverDuration = 3.0f;

        running = false;

        initializeTimer = 0f;
        initializeDuration = 3.0f;
        pixelsColored = 0;

        initialized = false;
        game.mainAudioSource.clip = game.sounds.imageLoading;
        game.mainAudioSource.loop = true;
        game.mainAudioSource.Play();
        game.secondAudioSource.clip = game.sounds.colorPress;
        game.secondAudioSource.loop = true;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            game.mainAudioSource.Stop();
            game.SwitchToState(new LevelSelectState(game));
            return;
        }

        if (!initialized)
        {
            initializeTimer += Time.deltaTime;

            int countDown = (int)(initializeDuration - initializeTimer) + 1;
            game.rightScreenValue.text = countDown.ToString();

            if (initializeTimer > 0.25f)
            {
                float alpha = 3 * (Mathf.Sin((initializeTimer - 0.25f) * Mathf.PI * 2) + 1) / 2;
                game.rightScreenValue.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            }

            float imageT = Mathf.Clamp(initializeTimer / 2.0f, 0f, 1f);
            int pixelsToColor = (int)(imageT * imageColors.Length);

            if (pixelsColored < pixelsToColor)
            {
                while (pixelsColored < pixelsToColor)
                {
                    referenceGrid[cellIndex.x, cellIndex.y].rend.color = imageColors[cellIndex.x + referenceGrid.GetLength(0) * cellIndex.y];
                    pixelsColored += 1;

                    if (cellIndex.x < referenceGrid.GetLength(0) - 1)
                    {
                        cellIndex.x += 1;
                    }
                    else
                    {
                        if (cellIndex.y > 0)
                        {
                            cellIndex.x = 0;
                            cellIndex.y -= 1;
                        }
                    }
                }
            }
            if (pixelsColored == imageColors.Length && game.mainAudioSource.isPlaying)
                game.mainAudioSource.Stop();

            if (initializeTimer >= initializeDuration)
            {
                cellIndex = new Vector2Int(0, canvasGrid.GetLength(1) - 1);
                coloredCellCount = CountColoredCells(referenceGrid);
                pixelsColored = 0;
                InitializeGuides(canvasGrid);

                game.rightScreenTitle.alpha = 1.0f;
                game.rightScreenTitle.text = "Accuracy:";
                game.rightScreenValue.text = "100%";
                game.rightScreenValue.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                initialized = true;
                running = true;
                game.mainAudioSource.clip = game.sounds.gameMusic;
                game.mainAudioSource.Play();
            }
        }
        else
        {
            if (running)
            {
                bool boosting = Input.GetKey(KeyCode.LeftShift);
                if (boosting)
                {
                    speed = boostSpeed;
                    game.speedValue.text = ">>>";
                    game.speedHighlight.SetActive(true);
                    game.secondAudioSource.pitch = 1.075f;
                }
                else
                {
                    speed = baseSpeed;
                    game.speedValue.text = ">";
                    game.speedHighlight.SetActive(false);
                    game.secondAudioSource.pitch = 1.0f;
                }

                int newColorIndex = 0;

                if (Input.GetKey(KeyCode.Q))
                    newColorIndex = 1;
                else if (Input.GetKey(KeyCode.W))
                    newColorIndex = 2;
                else if (Input.GetKey(KeyCode.E))
                    newColorIndex = 3;
                else if (Input.GetKey(KeyCode.R))
                    newColorIndex = 4;

                if (newColorIndex != colorIndex)
                {
                    if (newColorIndex > 0)
                    {
                        if (!game.secondAudioSource.isPlaying)
                        {
                            game.secondAudioSource.loop = true;
                            game.secondAudioSource.Play();
                        }
                    }
                    else
                        game.secondAudioSource.loop = false;

                    colorIndex = newColorIndex;
                }

                if (colorIndex > 0)
                {
                    canvasGrid[cellIndex.x, cellIndex.y].rend.color = paintColors[colorIndex];
                    hasColored = true;

                    Vector3 newPosition = game.colorHighlight.transform.position;
                    newPosition.x = game.colorItems[colorIndex - 1].transform.position.x;
                    game.colorHighlight.transform.position = newPosition;
                    game.colorHighlight.SetActive(true);
                }
                else
                {
                    game.colorHighlight.SetActive(false);
                }

                timer += speed * Time.deltaTime;
                if (timer >= 1.0f)
                {
                    if (!hasColored)
                        canvasGrid[cellIndex.x, cellIndex.y].rend.color = paintColors[0];
                    else
                        hasColored = false;

                    if (canvasGrid[cellIndex.x, cellIndex.y].rend.color != referenceGrid[cellIndex.x, cellIndex.y].rend.color)
                        incorrectCells += 1;

                    if (referenceGrid[cellIndex.x, cellIndex.y].rend.color != Color.white)
                        pixelsColored += 1;

                    accuracy = ((coloredCellCount - incorrectCells) * 100) / coloredCellCount;
                    accuracy = Mathf.Clamp(accuracy, 0, 100);
                    game.rightScreenValue.text = accuracy.ToString() + "%";

                    if (accuracy == 0 || pixelsColored == coloredCellCount)
                    {
                        GameOver();
                    }

                    if (cellIndex.x < canvasGrid.GetLength(0) - 1)
                    {
                        cellIndex.x += 1;
                        UpdateGuideBox();
                    }
                    else
                    {
                        if (cellIndex.y > 0)
                        {
                            cellIndex.x = 0;
                            cellIndex.y -= 1;
                            UpdateGuideLine();
                            UpdateGuideBox();
                        }
                        else
                        {
                            GameOver();
                        }
                    }
                    timer = 0f;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    game.mainAudioSource.Stop();
                    game.lastScore = accuracy;
                    game.SwitchToState(new LevelSelectState(game));
                    return;
                }
                game.rightScreenValue.alpha = (Mathf.Sin(Time.realtimeSinceStartup * 7.5f) + 1.0f) / 2.0f;
            }
        }
    }

    private void UpdateGuideBox()
    {
        game.guideBox.transform.position = new Vector3(
            canvasGrid[cellIndex.x, cellIndex.y].transform.position.x,
            canvasGrid[cellIndex.x, cellIndex.y].transform.position.y,
            -1);
    }

    private void UpdateGuideLine()
    {
        game.guideLine.transform.position = new Vector3(
            0,
            canvasGrid[cellIndex.x, cellIndex.y].transform.position.y,
            -1);
    }

    private void GameOver()
    {
        game.mainAudioSource.Stop();
        game.mainAudioSource.loop = false;
        game.secondAudioSource.Stop();
        game.secondAudioSource.loop = false;

        if (accuracy == 0)
        {
            game.mainAudioSource.clip = game.sounds.levelFailed;
        }
        else
        {
            game.mainAudioSource.clip = game.sounds.levelComplete;
        }

        game.mainAudioSource.Play();

        running = false;
        game.guideLine.enabled = false;
        game.guideBox.enabled = false;
        
        gridLineObject.SetActive(false);
    }

    private LineRenderer CreateGridLineObject(Material mat)
    {
        GameObject newLine = new GameObject("Line");

        LineRenderer lineRend = newLine.AddComponent<LineRenderer>();
        lineRend.material = mat;
        lineRend.startWidth = 0.03f;
        lineRend.endWidth = 0.03f;

        return lineRend;
    }

    private void CreateGridLines(Cell[,] grid)
    {
        gridLineObject = new GameObject("Grid Lines");
        gridLineObject.transform.parent = game.transform;

        float cellSize = grid[0, 0].transform.localScale.x;

        float xStart = grid[0, 0].transform.position.x - cellSize * 0.5f;
        float yStart = grid[0, 0].transform.position.y - cellSize * 0.5f;

        Material lineMat = new Material(Shader.Find("Particles/Standard Unlit"));
        lineMat.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));

        for (int x = 0; x < grid.GetLength(0) + 1; x++)
        {
            LineRenderer lineRend = CreateGridLineObject(lineMat);
            lineRend.transform.position = new Vector3(xStart + x * cellSize, yStart, 0);
            lineRend.transform.parent = gridLineObject.transform;

            lineRend.positionCount = 2;
            lineRend.SetPosition(0, lineRend.transform.position);
            lineRend.SetPosition(1, new Vector3(lineRend.transform.position.x, lineRend.transform.position.y + grid.GetLength(1) * cellSize, grid[0, 0].transform.position.z - 1));
        }

        for (int y = 0; y < grid.GetLength(1) + 1; y++)
        {
            LineRenderer lineRend = CreateGridLineObject(lineMat);
            lineRend.transform.position = new Vector3(xStart, yStart + y * cellSize, 0);
            lineRend.transform.parent = gridLineObject.transform;

            lineRend.positionCount = 2;
            lineRend.SetPosition(0, lineRend.transform.position);
            lineRend.SetPosition(1, new Vector3(lineRend.transform.position.x + grid.GetLength(0) * cellSize, lineRend.transform.position.y, grid[0, 0].transform.position.z - 1));
        }
    }

    private Cell[,] LoadImageGrid(Texture2D texture)
    {
        Cell[,] newImageGrid = new Cell[texture.width, texture.height];

        float xSize = frameSize.x / texture.width;
        float ySize = frameSize.y / texture.height;

        float cellSize = xSize < ySize ? xSize : ySize;
        float cellExtents = cellSize / 2;

        float xStart = newImageGrid.GetLength(0) * 0.5f * -cellSize;
        float yStart = newImageGrid.GetLength(1) * 0.5f * -cellSize;

        GameObject gridObject = new GameObject("Grid");
        gridObject.transform.parent = game.transform;

        for (int y = 0; y < newImageGrid.GetLength(1); y++)
        {
            for (int x = 0; x < newImageGrid.GetLength(0); x++)
            {
                GameObject newCellObj = new GameObject("Cell " + (x + y * newImageGrid.GetLength(0)).ToString());
                newCellObj.transform.position = new Vector3(xStart + x * cellSize + cellExtents, yStart + y * cellSize + cellExtents, 0);
                newCellObj.transform.localScale = new Vector3(cellSize, cellSize);
                newCellObj.transform.parent = gridObject.transform;

                Cell newCell = newCellObj.AddComponent<Cell>();
                newCell.rend = newCell.gameObject.AddComponent<SpriteRenderer>();
                newCell.rend.sprite = game.textures.square;
                newImageGrid[x, y] = newCell;
            }
        }

        return newImageGrid;
    }

    private void PaintGrid(Cell[,] grid, Color[] colors)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
            for (int x = 0; x < grid.GetLength(0); x++)
                grid[x, y].rend.color = colors[x + y * grid.GetLength(0)];
    }

    private void PaintGrid(Cell[,] grid, Color color)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
            for (int x = 0; x < grid.GetLength(0); x++)
                grid[x, y].rend.color = color;
    }

    private void InitializeGuides(Cell[,] grid)
    {
        Vector2 size = grid[0, 0].transform.localScale;

        game.guideLine.size = new Vector2(20.0f, size.y);
        game.guideBox.size = new Vector2(size.x, size.y);

        game.guideLine.transform.position = new Vector3(
            0,
            referenceGrid[0, grid.GetLength(1) - 1].transform.position.y,
            -1);

        game.guideBox.transform.position = new Vector3(
            grid[0, grid.GetLength(1) - 1].transform.position.x,
            grid[0, grid.GetLength(1) - 1].transform.position.y,
            -1);

        game.guideLine.enabled = true;
        game.guideBox.enabled = true;
    }

    private int CountColoredCells(Cell[,] grid)
    {
        int pixels = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].rend.color != Color.white)
                    pixels += 1;
            }
        }

        return pixels;
    }

    private void DestroyGrid(Cell[,] grid)
    {
        GameObject gridObject = grid[0, 0].transform.parent.gameObject;

        foreach (Transform child in gridObject.transform)
            Object.Destroy(child.gameObject);

        Object.Destroy(gridObject);
    }

    private void DestroyGridLines()
    {
        foreach (Transform child in gridLineObject.transform)
            Object.Destroy(child.gameObject);

        Object.Destroy(gridLineObject);
    }

    public override void OnExit()
    {
        DestroyGrid(referenceGrid);
        DestroyGrid(canvasGrid);
        DestroyGridLines();

        game.guideLine.enabled = false;
        game.guideBox.enabled = false;

        game.rightScreenTitle.alpha = 0f;
        game.rightScreenValue.alpha = 0f;
        game.speedTitle.alpha = 0f;
        game.speedValue.alpha = 0f;
        game.speedKey.alpha = 0f;

        game.rightScreen.enabled = false;
        game.leftScreen.enabled = false;

        game.ObjectsActive(game.colorItems, false);
        game.colorKeys.SetActive(false);
        game.colorHighlight.SetActive(false);
        game.speedHighlight.SetActive(false);
    }
}
