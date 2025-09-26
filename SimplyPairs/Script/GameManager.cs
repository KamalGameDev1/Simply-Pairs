using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplyPairs
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Cards Sprites")]
        public List<Sprite> _allCardsSprite = new List<Sprite>();

        [Header("Cards in Scene")]
        public List<CardScript> _allCards = new List<CardScript>();

        private Queue<CardScript> flipQueue = new Queue<CardScript>();
        private Coroutine processCoroutine;

        [Header("Mismatch Settings")]
        public float mismatchDelay = 0.5f; // delay before flip back

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            foreach (var card in _allCards)
            {
                card.OnCardFlipped += HandleCardFlipped;
            }
        }

        #region Card Handler
        public void HandleCardFlipped(CardScript card)
        {
            Debug.Log("card:" + card.name);

            if (card == null) return;
            if (card.IsMatched) return;
            if (flipQueue.Contains(card)) return;

            flipQueue.Enqueue(card);

            // Add turn every time player flips 2 cards
            if (flipQueue.Count % 2 == 0)
                ScoreManager.instance?.AddTurn();

            if (processCoroutine == null)
                processCoroutine = StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            while (flipQueue.Count >= 2)
            {
                var c1 = flipQueue.Dequeue();
                var c2 = flipQueue.Dequeue();

                if (c1 == null || c2 == null) continue;
                if (c1.IsMatched || c2.IsMatched) continue;

                yield return StartCoroutine(CheckPair(c1, c2));
            }

            processCoroutine = null;
        }

        private IEnumerator CheckPair(CardScript card1, CardScript card2)
        {
            if (card1.id == card2.id)
            {
                Debug.Log($"Matched Pair: {card1.id}");
                card1.SetMatched();
                card2.SetMatched();

                //Scoring for match
                ScoreManager.instance.OnMatch();
               

                // Check win condition
                if (_allCards.TrueForAll(c => c.IsMatched))
                {
                    Debug.Log("All matched ~ You Win!");
                    ScoreManager.instance.winText.text = "All matched ~ You Win";
                    SoundManager.instance.PlayGameOver();
                }
            }
            else
            {
                Debug.Log($"Mismatched: {card1.id} vs {card2.id}");

                // Penalty for mismatch
                ScoreManager.instance.OnMismatch();
                SoundManager.instance.PlayMismatch();

                if (mismatchDelay > 0f)
                    yield return new WaitForSeconds(mismatchDelay);

                Coroutine flip1 = StartCoroutine(card1.FlipBack());
                Coroutine flip2 = StartCoroutine(card2.FlipBack());

                yield return flip1;
                yield return flip2;
            }
        }
        #endregion

        public void ExitApplication()
        {
            Application.Quit();
        }
    }
}
