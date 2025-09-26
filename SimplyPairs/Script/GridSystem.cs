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
        [SerializeField] InputField rowsinput;
        [SerializeField] Text errorText;
        [SerializeField] GameObject customInputPanel;
        [SerializeField] GameObject Levelspanel;
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

        private void Start()
        {
            if (easyButton != null)
                easyButton.onClick.AddListener(EasyLevel);
            if (difficultButton != null)
                difficultButton.onClick.AddListener(DifficultLevel);
            if (hardButton != null)
                hardButton.onClick.AddListener(HardLevel);
            if (customButton != null)
                customButton.onClick.AddListener(CustomLevel);
        }
        int rows, columns;
        void EasyLevel()
        {
            rows = 2;
            columns = 6;
            GenerateGrid();
        }
        void DifficultLevel()
        {
            rows = 5;
            columns = 6;
            GenerateGrid();
        }
        void HardLevel()
        {
            rows = 5;
            columns = 10;
            GenerateGrid();
        }


        void CustomLevel()
        {
            if (rowsinput.text != "" && columnsInput.text != "")
            {
                int row = int.Parse(rowsinput.text);
                int column = int.Parse(columnsInput.text);
                rows = row;
                columns = column;
                GenerateGrid();
            }
            else
            {
                errorText.text = "Enter valid Input";
            }
        }

        public void Restart()
        {
            GenerateGrid();
        }

        void GenerateGrid()
        {
            Levelspanel.SetActive(false);

            // Clear previous children
            foreach (Transform child in container)
                Destroy(child.gameObject);

            int totalChildren = rows * columns;

            if (totalChildren > 64)
            {
                Debug.LogError("Grid must have an even number of cells for proper pairs!");
                errorText.text = "Enter value not greater than 8X8=64 items";
                return;
            }

            List<Sprite> availableSprites = new List<Sprite>(GameManager.instance._allCardsSprite);

            // Create a pool of pairs
            List<Sprite> changesprites = new List<Sprite>();
            for (int i = 0; i < totalChildren / 2; i++)
            {
                // Pick a random sprite
                int random = UnityEngine.Random.Range(0, availableSprites.Count);
                Sprite chosen = availableSprites[random];

                // Add it twice (for the pair)
                changesprites.Add(chosen);
                changesprites.Add(chosen);


                availableSprites.RemoveAt(random); // Remove from available list so no duplicate pairs of the same sprite
            }

            // Shuffle the list so pairs are randomly distributed
            for (int i = 0; i < changesprites.Count; i++)
            {
                Sprite temp = changesprites[i];
                int randomIndex = UnityEngine.Random.Range(0, changesprites.Count);
                changesprites[i] = changesprites[randomIndex];
                changesprites[randomIndex] = temp;
            }

            Debug.Log($"Generating Grid: {columns}x{rows} = {totalChildren} cells");

            if (totalChildren <= 64)
            {
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

                        // Assign random shuffled sprite
                        card.id = changesprites[index].name;
                        card.name = changesprites[index].name;
                        card.iconSprite.sprite = changesprites[index];
                        GameManager.instance._allCards.Add(card);
                        //SoundManager.instance.PlayPlace();
                        // Subscribe to the event here
                        card.OnCardFlipped += GameManager.instance.HandleCardFlipped;
                        index++;

                        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

                        Vector2 targetPos = new Vector2(
                            gridOrigin.x + col * (squareCellSize.x + spacing.x) + squareCellSize.x / 2f,
                            gridOrigin.y - row * (squareCellSize.y + spacing.y) - squareCellSize.y / 2f
                        );

                        rect.anchoredPosition = Vector2.zero;
                        rect.sizeDelta = squareCellSize;
                        cell.name = $"Cell_{row}_{col}";

                        // Animate using coroutine
                        StartCoroutine(AnimateToPosition(rect, targetPos, animDuration, (row * columns + col) * 0.02f));
                    }
                }
            }
            else
            {
                Debug.Log("Too many cells! Limit is 64.");
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
