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
        public string id;
        public Image backSprite;   
        public Image iconSprite;   

        [Header("Flip Data")]
        public float flipDuration = 0.25f;
        public float flipBackDuration = 0.15f; 

        [Header("BoolState")]
        public bool IsFlipped;
        public bool IsMatched;

        private bool isAnimating;
        public event Action<CardScript> OnCardFlipped;

        void Awake()
        {
            ResetCard();
        }

        public void ResetCard()
        {
            IsFlipped = false;
            IsMatched = false;
            isAnimating = false;
            backSprite.gameObject.SetActive(true);
            iconSprite.gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsMatched || IsFlipped || isAnimating) return;

            StartCoroutine(FlipToFace());
        }

        private IEnumerator FlipToFace()
        {
            isAnimating = true;
            IsFlipped = true;

            float half = flipDuration / 2f;

            // scale down
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float f = Mathf.SmoothStep(1f, 0f, t / half);
                transform.localScale = new Vector3(f, 1f, 1f);
                yield return null;
            }

            backSprite.gameObject.SetActive(false);
            iconSprite.gameObject.SetActive(true);

            // scale up
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float f = Mathf.SmoothStep(0f, 1f, t / half);
                transform.localScale = new Vector3(f, 1f, 1f);
                yield return null;
            }

            transform.localScale = Vector3.one;
            isAnimating = false;

            //notify AFTER flip is complete
            OnCardFlipped?.Invoke(this);
            SoundManager.instance.PlayFlip();
        }

        public IEnumerator FlipBack()
        {
            if (isAnimating) yield break; 
            isAnimating = true;

            float half = flipBackDuration / 2f;

            // scale down
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float f = Mathf.SmoothStep(1f, 0f, t / half);
                transform.localScale = new Vector3(f, 1f, 1f);
                yield return null;
            }

            backSprite.gameObject.SetActive(true);
            iconSprite.gameObject.SetActive(false);

            // scale up
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float f = Mathf.SmoothStep(0f, 1f, t / half);
                transform.localScale = new Vector3(f, 1f, 1f);
                yield return null;
            }

            IsFlipped = false;
            transform.localScale = Vector3.one;
            isAnimating = false;
            SoundManager.instance.PlayFlip();

        }

        public void SetMatched()
        {
            IsMatched = true;
            StartCoroutine(MatchAnimation());
        }

        private IEnumerator MatchAnimation()
        {
            Vector3 original = transform.localScale;
            Vector3 enlarged = original * 1.15f;

            float dur = 0.12f;
            float t = 0f;

            while (t < dur)
            {
                transform.localScale = Vector3.Lerp(original, enlarged, t / dur);
                t += Time.deltaTime;
                yield return null;
            }

            t = 0f;
            while (t < dur)
            {
                transform.localScale = Vector3.Lerp(enlarged, original, t / dur);
                t += Time.deltaTime;
                yield return null;
            }

            transform.localScale = original;
            yield return new WaitForEndOfFrame();
            transform.gameObject.SetActive(false);
            
        }
    }
}
