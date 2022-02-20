using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : State
{
    private Vector2 frameSize;
    
    private Cell[,] referenceGrid;
    private Cell[,] canvasGrid;
    private GameObject gridLineObject;

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

    public GameState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        frameSize = new Vector2(16.0f, 16.0f);

        Texture2D texture = game.levels[game.levelIndex].texture;

        referenceGrid = LoadImageGrid(texture);
        PaintGrid(referenceGrid, texture.GetPixels());

        canvasGrid = LoadImageGrid(texture);
        PaintGrid(canvasGrid, new Color(1.0f, 1.0f, 1.0f, 0.35f));

        CreateGridLines(canvasGrid);
        InitializeGuides(canvasGrid);

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
        boostSpeed = 20.0f * (texture.width / 16.0f) * 0.75f;

        coloredCellCount = CountColoredCells(referenceGrid);
        incorrectCells = 0;
        accuracy = 100;

        gameOverTimer = 0f;
        gameOverDuration = 3.0f;

        game.accuracyTitle.alpha = 1.0f;
        game.accuracyText.alpha = 1.0f;
        game.accuracyText.text = "100%";

        running = true;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            game.SwitchToState(new LevelSelectState(game));
            return;
        }

        if (running)
        {
            speed = Input.GetKey(KeyCode.LeftShift) == false ? baseSpeed : boostSpeed;

            colorIndex = 0;

            if (Input.GetKey(KeyCode.Q))
                colorIndex = 1;
            else if (Input.GetKey(KeyCode.W))
                colorIndex = 2;
            else if (Input.GetKey(KeyCode.E))
                colorIndex = 3;
            else if (Input.GetKey(KeyCode.R))
                colorIndex = 4;

            if (colorIndex != 0)
            {
                canvasGrid[cellIndex.x, cellIndex.y].rend.color = paintColors[colorIndex];
                hasColored = true;
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

                accuracy = ((coloredCellCount - incorrectCells) * 100) / coloredCellCount;
                game.accuracyText.text = accuracy.ToString() + "%";

                if (cellIndex.x < canvasGrid.GetLength(0) - 1)
                {
                    cellIndex.x += 1;
                    game.guideBox.transform.position = new Vector3(
                        canvasGrid[cellIndex.x, cellIndex.y].transform.position.x,
                        canvasGrid[cellIndex.x, cellIndex.y].transform.position.y,
                        -1);
                }
                else
                {
                    if (cellIndex.y > 0)
                    {
                        cellIndex.x = 0;
                        cellIndex.y -= 1;
                        game.guideLine.transform.position = new Vector3(
                            0,
                            canvasGrid[cellIndex.x, cellIndex.y].transform.position.y,
                            -1);

                        game.guideBox.transform.position = new Vector3(
                            canvasGrid[cellIndex.x, cellIndex.y].transform.position.x,
                            canvasGrid[cellIndex.x, cellIndex.y].transform.position.y,
                            -1);
                    }
                    else
                    {
                        running = false;
                        game.guideLine.enabled = false;
                        game.guideBox.enabled = false;
                    }
                }
                timer = 0f;
            }
        }
        else
        {
            gameOverTimer += Time.deltaTime;
            if (gameOverTimer >= gameOverDuration)
            {
                game.lastScore = accuracy;
                game.SwitchToState(new MainMenuState(game));
                return;
            }
            game.accuracyText.alpha = (Mathf.Sin(Time.realtimeSinceStartup * 7.5f) + 1.0f) / 2.0f;
        }
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

        for (int x = 0; x < referenceGrid.GetLength(0) + 1; x++)
        {
            GameObject newLine = new GameObject("Line X" + x.ToString());
            newLine.transform.position = new Vector3(xStart + x * cellSize, yStart, 0);
            newLine.transform.parent = gridLineObject.transform;

            LineRenderer lineRend = newLine.AddComponent<LineRenderer>();
            lineRend.material = lineMat;
            lineRend.startWidth = 0.03f;
            lineRend.endWidth = 0.03f;

            lineRend.positionCount = 2;
            lineRend.SetPosition(0, newLine.transform.position);
            lineRend.SetPosition(1, new Vector3(newLine.transform.position.x, newLine.transform.position.y + grid.GetLength(1) * cellSize, grid[0, 0].transform.position.z - 1));
        }

        for (int y = 0; y < referenceGrid.GetLength(1) + 1; y++)
        {
            GameObject newLine = new GameObject("Line Y" + y.ToString());
            newLine.transform.position = new Vector3(xStart, yStart + y * cellSize, 0);
            newLine.transform.parent = gridLineObject.transform;

            LineRenderer lineRend = newLine.AddComponent<LineRenderer>();
            lineRend.material = lineMat;
            lineRend.startWidth = 0.03f;
            lineRend.endWidth = 0.03f;

            lineRend.positionCount = 2;
            lineRend.SetPosition(0, newLine.transform.position);
            lineRend.SetPosition(1, new Vector3(newLine.transform.position.x + grid.GetLength(0) * cellSize, newLine.transform.position.y, grid[0, 0].transform.position.z - 1));
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

        game.accuracyTitle.alpha = 0f;
        game.accuracyText.alpha = 0f;
    }
}
