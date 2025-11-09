using System;
using System.Collections;
using UnityEngine;

namespace MainMenuLogic
{
    public class CreditsScrollBehavior : MonoBehaviour
    {
        public float scrollingSpeed = 5f;
        public float scrollingEnd = Single.PositiveInfinity;
        public GameObject backButton;

        private bool scroll = false;
        
        private void Start()
        {
            StartCoroutine(WaitBeforeStartingScroll());
        }

        private void Update()
        {
            if (scrollingEnd < transform.localPosition.y)
            {
                scroll = false;
                StartCoroutine(WaitBeforeShowingBackButton());
            }
            if (scroll)
            {
                transform.localPosition += new Vector3(0, scrollingSpeed * Time.deltaTime, 0);
            }
        }

        private IEnumerator WaitBeforeStartingScroll()
        {
            yield return new WaitForSeconds(2);
            scroll = true;
        }

        private IEnumerator WaitBeforeShowingBackButton()
        {
            yield return new WaitForSeconds(1);
            if(backButton) backButton.SetActive(true);
        }
    }
}