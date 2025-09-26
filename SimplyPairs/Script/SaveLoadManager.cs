using UnityEngine;

namespace SimplyPairs
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager instance;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        // === SCORE ===
        public void SaveHighScore(int score)
        {
            int best = PlayerPrefs.GetInt("HighScore", 0);
            if (score > best)
            {
                PlayerPrefs.SetInt("HighScore", score);
                PlayerPrefs.Save();
                Debug.Log($"New High Score Saved: {score}");
            }
        }

        public int LoadHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }

        // === TURNS ===
        public void SaveBestTurns(int turns)
        {
            int best = PlayerPrefs.GetInt("BestTurns", int.MaxValue);
            if (turns < best)
            {
                PlayerPrefs.SetInt("BestTurns", turns);
                PlayerPrefs.Save();
                Debug.Log($"New Best Turns Saved: {turns}");
            }
        }

        public int LoadBestTurns()
        {
            return PlayerPrefs.GetInt("BestTurns", int.MaxValue);
        }

        // GRID LEVEL 
        public void SaveLastLevel(int rows, int cols)
        {
            PlayerPrefs.SetInt("LastRows", rows);
            PlayerPrefs.SetInt("LastCols", cols);
            PlayerPrefs.Save();
            Debug.Log($"Last Level Saved: {rows}x{cols}");
        }

        public (int rows, int cols) LoadLastLevel()
        {
            if (PlayerPrefs.HasKey("LastRows") && PlayerPrefs.HasKey("LastCols"))
            {
                int rows = PlayerPrefs.GetInt("LastRows");
                int cols = PlayerPrefs.GetInt("LastCols");
                return (rows, cols);
            }

            return (0, 0); // means no saved grid
        }


        // CLEAR ALL DATA
        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All saved data cleared!");
        }
    }
}
