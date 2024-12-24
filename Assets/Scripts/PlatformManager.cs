using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Text

public class PlatformManager : MonoBehaviour
{
    public Material solidMaterial;      // Solid material
    public Material notSolidMaterial;   // Not Solid material

    public GameObject redPlatform;      // Reference to the unchanging red platform
    private int respawnCount = 0;       // Tracks respawns
    private int respawnThreshold = 5;   // Number of respawns to trigger reset

    public Text countdownText;          // Reference to the Text UI element

    public List<GameObject> allPlatforms = new List<GameObject>();



    void Start()
    {
        // Get the current scene's name
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Check if the reset flag exists for this scene
        if (PlayerPrefs.GetInt("LevelReset_" + currentScene, 0) == 0)
        {
            // Reset respawn count and save the reset flag
            respawnCount = 0;
            PlayerPrefs.SetInt("RespawnCount", respawnCount);
            PlayerPrefs.SetInt("LevelReset_" + currentScene, 1); // Mark the reset as done
            PlayerPrefs.Save();

            //Debug.Log("Respawn Count Reset for Scene: " + currentScene);
        }
        else
        {
            // Load the respawn count for this scene
            respawnCount = PlayerPrefs.GetInt("RespawnCount", 0);
            //Debug.Log("Respawn Count Loaded: " + respawnCount);
        }

        InitializePlatforms();
        LoadPlatformState();

        // Randomize platforms only if respawnCount is 0
        if (respawnCount == 0)
        {
            RandomizePlatforms();
            SavePlatformState();
        }

        // Initial countdown text update
        UpdateCountdownText(respawnThreshold - respawnCount);
    }


    private int rowCount = 9;
    private int columnCount = 4;

    void InitializePlatforms()
    {
        allPlatforms.Clear();
        rowCount = transform.childCount; // Count the number of rows
        columnCount = transform.GetChild(0).childCount; // Count columns in the first row (assumes consistent column count)

        int rowIndex = 0;
        foreach (Transform row in transform)
        {
            int colIndex = 0;
            foreach (Transform platform in row)
            {
                allPlatforms.Add(platform.gameObject);
                SetNotSolid(platform.gameObject); // Default to not solid
                //Debug.Log($"Added Platform: Row {rowIndex}, Column {colIndex}");
                colIndex++;
            }
            rowIndex++;
        }

        // Always set the red platform as solid
        if (redPlatform != null)
        {
            SetSolid(redPlatform);
            //Debug.Log("Red Platform Set to Solid");
        }
    }


    public void HandleRespawn()
    {
        respawnCount++;
        int countdown = respawnThreshold - respawnCount;
        //Debug.Log("Respawn Count: " + respawnCount);

        PlayerPrefs.SetInt("RespawnCount", respawnCount);
        PlayerPrefs.Save();

        // Update the countdown text dynamically
        if (countdown > 0)
        {
            UpdateCountdownText(countdown);
        }

        // Check if the respawn threshold has been reached
        if (respawnCount >= respawnThreshold)
        {
            Debug.Log("Threshold reached! Randomizing platforms...");
            RandomizePlatforms();

            // Subtract the threshold instead of resetting to 0
            respawnCount -= respawnThreshold;

            // Save the updated respawn count
            PlayerPrefs.SetInt("RespawnCount", respawnCount);
            PlayerPrefs.Save();
            UpdateCountdownText(respawnThreshold); // Show reset message
        }
    }

    void UpdateCountdownText(int remaining)
    {
        if (countdownText != null)
        {
            if (remaining > 0)
                countdownText.text = $"Navigate your way towards the exit, but beware—some platforms are solid, while others will let you fall. Plan your steps wisely! The platforms will rearrange after {remaining} attempts.";
            else
                countdownText.text = "Platforms reset, you have 5 more chances";
        }
    }

    void RandomizePlatforms()
    {
        Debug.Log("Randomizing Platforms...");

        foreach (GameObject platform in allPlatforms)
        {
            if (platform != redPlatform) // Skip red platform
                SetNotSolid(platform);
        }

        int currentRow = 0;
        int currentColumn = FindRedPlatformColumn();

        SetSolid(redPlatform); // Ensure the red platform is solid
        //Debug.Log($"Starting Randomization at Red Platform: Row {currentRow}, Column {currentColumn}");

        while (currentRow < rowCount) // Use dynamic row count
        {
            List<Vector2Int> candidates = GetAdjacentPlatforms(currentRow, currentColumn);

            candidates.RemoveAll(candidate =>
                allPlatforms[candidate.x * columnCount + candidate.y].GetComponent<BoxCollider>().enabled);

            //Debug.Log($"Current Position: Row {currentRow}, Column {currentColumn}. Valid Adjacent Candidates: {candidates.Count}");

            if (candidates.Count > 0)
            {
                Vector2Int nextPlatform = candidates[Random.Range(0, candidates.Count)];
                //Debug.Log($"Chosen Platform: Row {nextPlatform.x}, Column {nextPlatform.y}");

                GameObject platform = allPlatforms[nextPlatform.x * columnCount + nextPlatform.y];
                SetSolid(platform);

                currentRow = nextPlatform.x;
                currentColumn = nextPlatform.y;
            }
            else
            {
                //Debug.LogWarning("No valid adjacent platforms found! Stopping randomization.");
                break;
            }
        }

        //Debug.Log("Platform path randomized with strict adjacency.");
    }

    void SavePlatformState()
    {
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            bool isSolid = allPlatforms[i].GetComponent<BoxCollider>().enabled;
            PlayerPrefs.SetInt("Platform_" + i, isSolid ? 1 : 0);
        }
        PlayerPrefs.Save();
        //Debug.Log("Platform State Saved!");
    }

    void LoadPlatformState()
    {
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            if (allPlatforms[i] == redPlatform) continue; // Skip red platform

            bool isSolid = PlayerPrefs.GetInt("Platform_" + i, 0) == 1;

            if (isSolid)
                SetSolid(allPlatforms[i]);
            else
                SetNotSolid(allPlatforms[i]);
        }

        // Ensure red platform is always solid
        if (redPlatform != null)
        {
            SetSolid(redPlatform);
        }

        //Debug.Log("Platform State Loaded!");
    }

    List<Vector2Int> GetAdjacentPlatforms(int row, int column)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        if (row + 1 < rowCount) candidates.Add(new Vector2Int(row + 1, column)); // Forward
        if (row + 1 < rowCount && column - 1 >= 0) candidates.Add(new Vector2Int(row + 1, column - 1)); // Forward-left
        if (row + 1 < rowCount && column + 1 < columnCount) candidates.Add(new Vector2Int(row + 1, column + 1)); // Forward-right

        candidates.Sort((a, b) =>
        {
            if (a.y == column) return -1;
            if (b.y == column) return 1;
            return 0;
        });

        candidates.RemoveAll(candidate =>
            allPlatforms[candidate.x * columnCount + candidate.y].GetComponent<BoxCollider>().enabled);

        return candidates;
    }

    int FindRedPlatformColumn()
    {
        // Locate the red platform column in the first row
        for (int i = 0; i < 4; i++)
        {
            if (allPlatforms[i] == redPlatform) return i;
        }
        return 0; // Default fallback
    }

    void SetSolid(GameObject platform)
    {
        platform.GetComponent<Renderer>().material = solidMaterial;
        platform.GetComponent<BoxCollider>().enabled = true; // Enable collision
    }

    void SetNotSolid(GameObject platform)
    {
        platform.GetComponent<Renderer>().material = notSolidMaterial;
        platform.GetComponent<BoxCollider>().enabled = false; // Disable collision
    }

    public void ResetRespawnCount()
    {
        respawnCount = 0;
        PlayerPrefs.SetInt("RespawnCount", respawnCount);
        PlayerPrefs.Save();
        //Debug.Log("Respawn count reset to 0.");
    }
}