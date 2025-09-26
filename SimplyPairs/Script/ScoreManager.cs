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
        public Text winText; 

        // Runtime state
        public int CurrentScore { get; private set; }
        private int comboCount = 0;
        private float lastMatchTime = -100f;
        private int turns = 0;
        private int mismatches = 0;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            UpdateUI();
           
        }

        // Called when 2 cards are flipped
        public void AddTurn()
        {
            turns++;
            UpdateUI();
        }

        // Called on a successful match
        public void OnMatch()
        {
            float now = Time.time;

            // Combo logic
            if (now - lastMatchTime <= comboWindow)
                comboCount++;
            else
                comboCount = 1;

            lastMatchTime = now;

            // Add score with combo multiplier
            int add = basePoints * comboCount;
            CurrentScore += add;

            Debug.Log($" Match! Combo = {comboCount}, Added {add}, Total = {CurrentScore}");

            UpdateUI();
            FlashText(comboText, Color.green);

            SoundManager.instance.PlayMatch();
        }

        // Called on mismatch
        public void OnMismatch()
        {
            mismatches++;
            CurrentScore = Mathf.Max(0, CurrentScore - mismatchPenalty);
            comboCount = 0; // reset combo

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

        // Simple text flash effect
        private void FlashText(Text text, Color flashColor)
        {
            if (text == null) return;
            text.color = flashColor;
            CancelInvoke(nameof(ResetTextColor));
            Invoke(nameof(ResetTextColor), 0.5f);
        }

        private void ResetTextColor()
        {
            if (scoreText != null) scoreText.color = Color.white;
            if (comboText != null) comboText.color = Color.white;
        }

        
    }
}
