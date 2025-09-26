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
        public float mismatchDelay = 0.15f;

        [Header("Lock System")]
        public bool IsLocked;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
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
            if (IsLocked) return;
            if (card == null || card.IsMatched) return;
            if (flipQueue.Contains(card)) return;

            flipQueue.Enqueue(card);

            if (flipQueue.Count == 2)
            {
                ScoreManager.instance?.AddTurn();

                if (processCoroutine == null)
                    processCoroutine = StartCoroutine(ProcessQueue());
            }
        }

        private IEnumerator ProcessQueue()
        {
            while (flipQueue.Count >= 2)
            {
                var c1 = flipQueue.Dequeue();
                var c2 = flipQueue.Dequeue();

                if (c1 == null || c2 == null) continue;
                if (c1.IsMatched || c2.IsMatched) continue;

                yield return CheckPair(c1, c2);
            }

            processCoroutine = null;
        }

        private IEnumerator CheckPair(CardScript card1, CardScript card2)
        {
            IsLocked = true;

            if (card1.id == card2.id)
            {
                card1.SetMatched();
                card2.SetMatched();

                ScoreManager.instance?.OnMatch();
                SoundManager.instance?.PlayMatch();

                if (_allCards.Count > 0 && _allCards.TrueForAll(c => c != null && c.IsMatched))
                {
                    ScoreManager.instance?.OnWin();
                    SoundManager.instance?.PlayGameOver();
                }
            }
            else
            {
                ScoreManager.instance?.OnMismatch();
                SoundManager.instance?.PlayMismatch();

                if (mismatchDelay > 0f)
                    yield return new WaitForSeconds(mismatchDelay);

                Coroutine c1 = StartCoroutine(card1.FlipBack());
                Coroutine c2 = StartCoroutine(card2.FlipBack());
                yield return c1;
                yield return c2;
            }

            IsLocked = false;
        }
        #endregion

        public void ClearAllCardListner()
        {
            foreach (var card in _allCards)
            {
                if (card != null)
                    card.OnCardFlipped -= HandleCardFlipped;
            }
            _allCards.Clear();
        }

        public void ResetGame()
        {
            IsLocked = false;
            flipQueue.Clear();

            if (processCoroutine != null)
            {
                StopCoroutine(processCoroutine);
                processCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            ClearAllCardListner();
        }

        public void ExitApplication()
        {
            Application.Quit();
        }
    }
}
