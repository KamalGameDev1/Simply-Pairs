using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimplyPairs
{
    public class CardScript : MonoBehaviour, IPointerClickHandler
    {
        [Header("Card Data")]
        public int id;
        public Image backSprite;   // back side of card
        public Image iconSprite;   // front (face) side of card

        [Header("Flip Settings")]
        public float flipDuration = 0.25f;

        [Header("Boolean")]
        public bool IsFlipped;
        public bool IsMatched;

        
        public event Action<CardScript> OnCardFlipped; // notify GameManager

        void Awake()
        {
            ResetCard();
        }

        public void ResetCard()
        {
            IsFlipped = false;
            IsMatched = false;
            backSprite.gameObject.SetActive(true);
            iconSprite.gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        }

        // Required interface
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsMatched || IsFlipped) return;

            StartCoroutine(FlipToFace());
            OnCardFlipped?.Invoke(this); // notify manager
        }

        private IEnumerator FlipToFace()
        {
            IsFlipped = true;

            float half = flipDuration / 2f;

            // scale down
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1f, 0f, t / half);
                transform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }

            // set images
            backSprite.gameObject.SetActive(false);
            iconSprite.gameObject.SetActive(true);

            // scale up
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(0f, 1f, t / half);
                transform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }
        }

        public IEnumerator FlipBack()
        {
            float half = flipDuration / 2f;

            // scale down
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1f, 0f, t / half);
                transform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }

            // set back image
            backSprite.gameObject.SetActive(true);
            iconSprite.gameObject.SetActive(false);

            // scale up
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(0f, 1f, t / half);
                transform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }

            IsFlipped = false;
        }

        public void SetMatched()
        {
            IsMatched = true;
        }
    }
}
