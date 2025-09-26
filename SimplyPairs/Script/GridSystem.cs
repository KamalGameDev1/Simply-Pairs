using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimplyPairs
{
    public class GridSystem : MonoBehaviour
    {
        [Header("Custom Grid Settings")]
        [SerializeField] InputField columnsInput;
        [SerializeField] InputField rowsInput;
        [SerializeField] Text errorText;
        [SerializeField] GameObject customInputPanel;
        [SerializeField] GameObject levelsPanel;
        [SerializeField] GameObject gamePlayPanel;
        [SerializeField] Vector2 spacing = new Vector2(5, 5);

        [Header("References")]
        [SerializeField] RectTransform container;
        [SerializeField] GameObject cardPrefab;

        [Header("Levels")]
        [SerializeField] Button easyButton;
        [SerializeField] Button difficultButton;
        [SerializeField] Button hardButton;
        [SerializeField] Button customButton;

        [Header("Animation")]
        [SerializeField] float animDuration = 0.15f;

        private int rows, columns;

        private void Start()
        {
            if (easyButton != null) easyButton.onClick.AddListener(EasyLevel);
            if (difficultButton != null) difficultButton.onClick.AddListener(DifficultLevel);
            if (hardButton != null) hardButton.onClick.AddListener(HardLevel);
            if (customButton != null) customButton.onClick.AddListener(CustomLevel);

            var (savedRows, savedCols) = SaveLoadManager.instance.LoadLastLevel();
            if (savedRows > 0 && savedCols > 0)
            {
                rows = savedRows;
                columns = savedCols;
                GenerateGrid();
            }
            else
            {
                levelsPanel.SetActive(true);
            }
        }

        void EasyLevel() { rows = 2; columns = 6; GenerateGrid(); }
        void DifficultLevel() { rows = 5; columns = 6; GenerateGrid(); }
        void HardLevel() { rows = 5; columns = 10; GenerateGrid(); }

        void CustomLevel()
        {
            if (int.TryParse(rowsInput.text, out int row) &&
                int.TryParse(columnsInput.text, out int column))
            {
                rows = row;
                columns = column;
                GenerateGrid();
            }
            else
            {
                errorText.text = "Enter valid numbers for rows and columns.";
            }
        }

        public void Restart()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.ClearAllCardListner();
                GameManager.instance.ResetGame();
            }

            GenerateGrid();
        }

        void GenerateGrid()
        {
            levelsPanel.SetActive(false);
            gamePlayPanel.SetActive(true);

            if (GameManager.instance != null)
            {
                GameManager.instance.ClearAllCardListner();
                GameManager.instance.ResetGame();
            }

            ScoreManager.instance?.ResetScore();
            SaveLoadManager.instance?.SaveLastLevel(rows, columns);

            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            int totalChildren = rows * columns;
            if (totalChildren > 64 || totalChildren % 2 != 0)
            {
                errorText.text = "Max grid size = 8x8 (64), must be even.";
                return;
            }

            List<Sprite> availableSprites = new List<Sprite>(GameManager.instance._allCardsSprite);

            List<Sprite> changeSprites = new List<Sprite>();
            for (int i = 0; i < totalChildren / 2; i++)
            {
                int random = Random.Range(0, availableSprites.Count);
                Sprite chosen = availableSprites[random];
                changeSprites.Add(chosen);
                changeSprites.Add(chosen);
                availableSprites.RemoveAt(random);
            }

            // shuffle
            for (int i = 0; i < changeSprites.Count; i++)
            {
                Sprite temp = changeSprites[i];
                int randomIndex = Random.Range(0, changeSprites.Count);
                changeSprites[i] = changeSprites[randomIndex];
                changeSprites[randomIndex] = temp;
            }

            float totalWidth = container.rect.width;
            float totalHeight = container.rect.height;

            float cellWidth = (totalWidth - (columns - 1) * spacing.x) / columns;
            float cellHeight = (totalHeight - (rows - 1) * spacing.y) / rows;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            Vector2 squareCellSize = new Vector2(cellSize, cellSize);

            float gridWidth = columns * squareCellSize.x + (columns - 1) * spacing.x;
            float gridHeight = rows * squareCellSize.y + (rows - 1) * spacing.y;
            Vector2 gridOrigin = new Vector2(-gridWidth / 2f, gridHeight / 2f);

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    GameObject cell = Instantiate(cardPrefab, container, false);
                    RectTransform rect = cell.GetComponent<RectTransform>();

                    CardScript card = cell.GetComponent<CardScript>();
                    card.ResetCard(); // ensure clean state

                    card.id = changeSprites[index].name;
                    card.iconSprite.sprite = changeSprites[index];
                    card.name = changeSprites[index].name;

                    GameManager.instance._allCards.Add(card);
                    card.OnCardFlipped += GameManager.instance.HandleCardFlipped;
                    index++;

                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

                    Vector2 targetPos = new Vector2(
                        gridOrigin.x + col * (squareCellSize.x + spacing.x) + squareCellSize.x / 2f,
                        gridOrigin.y - row * (squareCellSize.y + spacing.y) - squareCellSize.y / 2f
                    );

                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = squareCellSize;
                    cell.name = $"Card_{row}_{col}";

                    StartCoroutine(AnimateToPosition(rect, targetPos, animDuration, (row * columns + col) * 0.02f));
                }
            }
        }

        IEnumerator AnimateToPosition(RectTransform rect, Vector2 targetPos, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector2 startPos = rect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            rect.anchoredPosition = targetPos;
        }
    }
}
