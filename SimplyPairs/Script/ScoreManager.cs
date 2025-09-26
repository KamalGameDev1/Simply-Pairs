using UnityEngine;
using UnityEngine.UI;

namespace SimplyPairs
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        [Header("Scoring Settings")]
        public int basePoints = 100;
        public int mismatchPenalty = 20;
        public float comboWindow = 5f;

        [Header("UI")]
        public Text scoreText;
        public Text comboText;
        public Text turnText;
        public Text mismatchText;

        [Header("Win")]
        public GameObject winnerPanel;
        public Text winText;
        public Text highScoreText;
        public Text bestTurnsText;

        // Runtime state
        public int CurrentScore { get; private set; }
        private int comboCount = 0;
        private float lastMatchTime = -100f;
        private int turns = 0;
        private int mismatches = 0;

        public int Turns => turns; // expose for GameManager

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            ResetScore();

            UpdatePersistentUI(); // show saved highscore/bestturns at start

            if (winnerPanel != null) winnerPanel.SetActive(false);
        }

        // Reset for each new game
        public void ResetScore()
        {
            CurrentScore = 0;
            comboCount = 0;
            turns = 0;
            mismatches = 0;
            lastMatchTime = -100f;

            if (winText != null) winText.text = "";
            if (winnerPanel != null) winnerPanel.SetActive(false);

            UpdateUI();
        }

      
        public void AddTurn()
        {
            turns++;
            UpdateUI();
        }

       
        public void OnMatch()
        {
            float now = Time.time;
            
            if (now - lastMatchTime <= comboWindow)
                comboCount++;
            else
                comboCount = 1;

            lastMatchTime = now;
            
            int add = basePoints * comboCount;
            CurrentScore += add;

            Debug.Log($"Match! Combo = {comboCount}, Added {add}, Total = {CurrentScore}");

            UpdateUI();
            FlashText(comboText, Color.green);
        }

       
        public void OnMismatch()
        {
            mismatches++;
            CurrentScore = Mathf.Max(0, CurrentScore - mismatchPenalty);
            comboCount = 0;

            Debug.Log($"Mismatch! -{mismatchPenalty} points, Total = {CurrentScore}");

            UpdateUI();
            FlashText(scoreText, Color.red);
        }

        private void UpdateUI()
        {
            if (scoreText != null) scoreText.text = "Score: " + CurrentScore;
            if (comboText != null) comboText.text = "Combo x" + comboCount;
            if (turnText != null) turnText.text = "Turns: " + turns;
            if (mismatchText != null) mismatchText.text = "Mismatches: " + mismatches;
        }

        // called at start and when saving
        public void UpdatePersistentUI()
        {
            int hs = SaveLoadManager.instance?.LoadHighScore() ?? 0;
            int bt = SaveLoadManager.instance?.LoadBestTurns() ?? int.MaxValue;

            if (highScoreText != null)
                highScoreText.text = "High Score: " + hs;

            if (bestTurnsText != null)
                bestTurnsText.text = bt == int.MaxValue ? "Best Turns: -" : "Best Turns: " + bt;
        }

       
        public void OnWin()
        {
            if (winnerPanel != null) winnerPanel.SetActive(true);
            if (winText != null) winText.text = "🎉 All matched — You Win!";

          
            SaveLoadManager.instance?.SaveHighScore(CurrentScore);
            SaveLoadManager.instance?.SaveBestTurns(Turns);

            UpdatePersistentUI();
        }

        
        private void FlashText(Text text, Color flashColor)
        {
            if (text == null) return;
            text.color = flashColor;

            CancelInvoke(nameof(ResetTextColors));
            Invoke(nameof(ResetTextColors), 0.5f);
        }

        private void ResetTextColors()
        {
            if (scoreText != null) scoreText.color = Color.white;
            if (comboText != null) comboText.color = Color.white;
        }

        
        public void ResetAllData()
        {
            PlayerPrefs.DeleteAll(); 
            ResetScore();
            UpdatePersistentUI();
        }
    }
}
