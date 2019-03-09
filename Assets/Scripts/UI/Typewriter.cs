﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class Typewriter : MonoBehaviour
{
    private Text mLabel;
    private bool mIsAnimating;
    private string mTargetText;

    public bool showingText
    {
        get { return gameObject.activeSelf && transform.localScale.x > 0.01f; }
    }

    public bool isAnimating
    {
        get { return mIsAnimating; }
    }

    private void Awake()
    {
        mLabel = GetComponentInChildren<Text>();
    }

    public void HideText()
    {
        StopAllCoroutines();

        transform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }

    public void ShowText(string text, float speed)
    {
        gameObject.SetActive(true);

        StopAllCoroutines();

        StartCoroutine(ShowTextCoroutine(text, speed));
    }

    public void ForceFinish()
    {
        StopAllCoroutines();
        mLabel.text = mTargetText;
        mIsAnimating = false;
    }

    public IEnumerator ShowTextCoroutine(string text, float speed)
    {
        mIsAnimating = true;
        mTargetText = text;

        bool wasActive = gameObject.activeSelf && gameObject.transform.localScale.x > 0.1f;

        gameObject.SetActive(true);
        
        StringBuilder stringBuilder = new StringBuilder();
        mLabel.text = "";

        text = text.Replace("\\n", "\n");

        // Scale the window in
        if (!wasActive)
        {
            gameObject.transform.localScale = Vector3.zero;
            float scale = 0f;
            while (scale < 1f)
            {
                scale += Time.deltaTime * 5f;
                scale = Mathf.Min(scale, 1f);
                gameObject.transform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i <= text.Length; ++i)
        {
            stringBuilder.Length = 0;

            if (i > 0)
            {
                string visible = text.Substring(0, i);
                stringBuilder.Append(visible);

                if (i < text.Length - 1)
                {
                    string invisible = text.Substring(i);
                    stringBuilder.Append("<color=#00000000>");
                    stringBuilder.Append(invisible);
                    stringBuilder.Append("</color>");
                }
            }

            mLabel.text = stringBuilder.ToString();

            yield return new WaitForSeconds(0.03f);
        }

        mLabel.text = text;
        mIsAnimating = false;
        yield break;
    }
}