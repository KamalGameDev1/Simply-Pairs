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
        public float mismatchDelay = 0.5f; // set to 0 for instant flip back

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

        #region CardHandlerSpace
        public void HandleCardFlipped(CardScript card)
        {
            Debug.Log("card:" + card.name);

            if (card == null) return;
            if (card.IsMatched) return;
            if (flipQueue.Contains(card)) return;

            flipQueue.Enqueue(card);

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

                if (_allCards.TrueForAll(c => c.IsMatched))
                {
                    Debug.Log("All matched — You Win!");
                }
            }
            else
            {
                Debug.Log($" Mismatched: {card1.id} vs {card2.id}");

                // optional short pause so player sees both cards
                if (mismatchDelay > 0f)
                    yield return new WaitForSeconds(mismatchDelay);

                // flip both back simultaneously
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
