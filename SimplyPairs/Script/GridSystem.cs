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
        [SerializeField] float animDuration = 0.5f;


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

        void EasyLevel()
        {
            GenerateGrid(2, 6);
        }
        void DifficultLevel()
        {
            GenerateGrid(5, 6);
        }
        void HardLevel()
        {
            GenerateGrid(5, 10);
        }
        void CustomLevel()
        {
            if (rowsinput.text != "" && columnsInput.text!="")
            {
                int row = int.Parse(rowsinput.text);
                int column = int.Parse(rowsinput.text);
                GenerateGrid(row, column);
            }
            else
            {
                errorText.text = "Enter valid Input";
            }
            
        }
        void GenerateGrid(int rows,int columns)
        {
            // Clear previous children
            foreach (Transform child in container)
                Destroy(child.gameObject);

            int totalChildren = rows * columns;
            Debug.Log($"Generating Grid: {columns}x{rows} = {totalChildren} cells");

            if (totalChildren <= 64)
            {
                float totalWidth = container.rect.width;
                float totalHeight = container.rect.height;

                //First calculate cell width and height separately
                float cellWidth = (totalWidth - (columns - 1) * spacing.x) / columns;
                float cellHeight = (totalHeight - (rows - 1) * spacing.y) / rows;

                //Pick the smaller one to keep cells square
                float cellSize = Mathf.Min(cellWidth, cellHeight);
                Vector2 squareCellSize = new Vector2(cellSize, cellSize);

                //Now recalc actual grid size
                float gridWidth = columns * squareCellSize.x + (columns - 1) * spacing.x;
                float gridHeight = rows * squareCellSize.y + (rows - 1) * spacing.y;

                //Middle Center alignment (origin is top-left of the centered grid)
                Vector2 gridOrigin = new Vector2(
                    -gridWidth / 2f,
                    gridHeight / 2f
                );

                Debug.Log("rows small:" + rows);

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        GameObject cell = Instantiate(cardPrefab, container, false);
                        RectTransform rect = cell.GetComponent<RectTransform>();

                        rect.anchorMin = new Vector2(0.5f, 0.5f);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.pivot = new Vector2(0.5f, 0.5f);

                        // Target pos (centered grid)
                        Vector2 targetPos = new Vector2(
                            gridOrigin.x + col * (squareCellSize.x + spacing.x) + squareCellSize.x / 2f,
                            gridOrigin.y - row * (squareCellSize.y + spacing.y) - squareCellSize.y / 2f
                        );

                        // Start at center then animate out
                        rect.anchoredPosition = Vector2.zero;


                        rect.sizeDelta = squareCellSize;
                        cell.name = $"Cell_{row}_{col}";
                    }
                }
            }
            else
            {
                Debug.Log("Please be authantic");
            }
        }

       
    }
}

