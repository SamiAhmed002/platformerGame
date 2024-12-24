using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid3DLayout : MonoBehaviour
{
    public GameObject platformPrefab; // The platform prefab to instantiate
    public int rows = 5; // Number of rows
    public int columns = 8; // Number of columns
    public Vector3 startPosition = Vector3.zero; // Starting position for the grid
    public Vector3 spacing = new Vector3(9f, 0f, 9f); // Spacing between platforms
    public Material solidMaterial; // Solid material
    public Material notSolidMaterial; // Not solid material
    public int respawnThreshold = 5; // Number of deaths before reset

    public int respawnCount = 0; // Independent respawn counter
    public Text countdownText; // Reference to the countdown UI text

    private GameObject redPlatform; // Red platform reference (start point)
    private List<GameObject> allPlatforms = new List<GameObject>();

    private HashSet<GameObject> pathPlatforms = new HashSet<GameObject>();

    void Start()
    {
        // Load respawn count from PlayerPrefs
        respawnCount = PlayerPrefs.GetInt("GridRespawnCount", 0);
        UpdateCountdownText();

        CreateGrid();
        LoadPlatformState(); // Restore saved platform states
    }

    public void HandleRespawn()
    {
        respawnCount++;
        PlayerPrefs.SetInt("GridRespawnCount", respawnCount);
        PlayerPrefs.Save();

        if (respawnCount >= respawnThreshold)
        {
            Debug.Log("Respawn threshold reached. Randomizing grid...");
            RandomizeGrid();
            respawnCount = 0; // Reset the counter
            PlayerPrefs.SetInt("GridRespawnCount", respawnCount);
            PlayerPrefs.Save();
        }

        SavePlatformState(); // Save updated platform states
        UpdateCountdownText();
    }

    void CreateGrid()
    {
        allPlatforms.Clear();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate position
                Vector3 position = startPosition + new Vector3(col * spacing.x, 0, row * spacing.z);

                // Instantiate platform
                GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);

                // Mark the first platform (row 0, col 0) as the red platform
                if (row == 0 && col == 0)
                {
                    redPlatform = platform;
                    SetSolid(platform); // Make the red platform solid
                }
                else
                {
                    // Initially randomize platform state
                    bool isSolid = Random.value > 0.5f;
                    SetPlatformState(platform, isSolid);
                }

                // Parent the platform for hierarchy organization
                platform.transform.parent = transform;

                // Add the platform to the list
                allPlatforms.Add(platform);
            }
        }

        Debug.Log("Grid created.");
    }

    void RandomizeGrid()
    {
        Debug.Log("Randomizing grid...");

        // Generate a path using the refined logic
        GeneratePath();

        // Randomize remaining platforms
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            GameObject platform = allPlatforms[i];

            // Skip the red platform and path platforms
            if (platform == redPlatform || pathPlatforms.Contains(platform))
                continue;

            // Randomize platforms that are not part of the path
            bool isSolid = Random.value > 0.3f; // Make most non-path platforms non-solid
            SetPlatformState(platform, isSolid);
        }

        Debug.Log("Grid randomized.");
    }

    void GeneratePath()
    {
        Debug.Log("Generating guaranteed path...");

        pathPlatforms.Clear();

        int currentRow = 0;
        int currentCol = 0;

        // Add the red platform as the starting point
        pathPlatforms.Add(allPlatforms[currentRow * columns + currentCol]);
        SetSolid(allPlatforms[currentRow * columns + currentCol]);

        while (currentRow < rows - 1)
        {
            List<Vector2Int> options = new List<Vector2Int>();

            // Include gaps in calculations
            Vector3 gapAdjustedSpacing = new Vector3(spacing.x, 0, spacing.z);

            // Check left diagonal
            if (currentRow + 1 < rows && currentCol - 1 >= 0)
                options.Add(new Vector2Int(currentRow + 1, currentCol - 1));

            // Check right diagonal
            if (currentRow + 1 < rows && currentCol + 1 < columns)
                options.Add(new Vector2Int(currentRow + 1, currentCol + 1));

            // Avoid going directly in front after the first move
            if (currentRow != 0 && currentRow + 1 < rows)
                options.Add(new Vector2Int(currentRow + 1, currentCol));

            // Pick a random valid move
            if (options.Count > 0)
            {
                Vector2Int nextMove = options[Random.Range(0, options.Count)];
                currentRow = nextMove.x;
                currentCol = nextMove.y;

                pathPlatforms.Add(allPlatforms[currentRow * columns + currentCol]);
                SetSolid(allPlatforms[currentRow * columns + currentCol]);

                Debug.Log($"Solid platform added at Row: {currentRow}, Column: {currentCol}");
            }
            else
            {
                Debug.LogWarning("No valid moves available!");
                break;
            }
        }

        Debug.Log("Path generation completed.");
    }


    void SavePlatformState()
    {
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            GameObject platform = allPlatforms[i];
            bool isSolid = platform.GetComponent<BoxCollider>().enabled;

            // Save platform state
            PlayerPrefs.SetInt($"GridPlatform_{i}_State", isSolid ? 1 : 0);
        }

        PlayerPrefs.Save();
        Debug.Log("Platform states saved.");
    }

    void LoadPlatformState()
    {
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            GameObject platform = allPlatforms[i];

            // Load platform state
            bool isSolid = PlayerPrefs.GetInt($"GridPlatform_{i}_State", 1) == 1;
            SetPlatformState(platform, isSolid);
        }

        Debug.Log("Platform states loaded.");
    }

    void UpdateCountdownText()
    {
        if (countdownText != null)
        {
            int remainingTries = respawnThreshold - respawnCount;

            if (remainingTries > 0)
            {
                countdownText.text = $"Platforms will rearrange after {remainingTries} attempts.";
            }
            else
            {
                countdownText.text = "Platforms have been rearranged. You have 5 more tries!";
            }
        }
        else
        {
            Debug.LogWarning("Countdown text is not assigned!");
        }
    }

    void SetPlatformState(GameObject platform, bool isSolid)
    {
        platform.GetComponent<Renderer>().material = isSolid ? solidMaterial : notSolidMaterial;
        platform.GetComponent<BoxCollider>().enabled = isSolid;
    }

    void SetSolid(GameObject platform)
    {
        SetPlatformState(platform, true);
    }
}
