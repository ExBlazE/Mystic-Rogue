using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FadeTutorial : MonoBehaviour
{
    [SerializeField] private float introDuration = 1f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float outroDuration = 1f;

    private int activeFades = 0;

    void Start()
    {
        // Find all Image and TextMeshProUGUI components within this object's children
        Image[] imagesToFade = GetComponentsInChildren<Image>();
        TextMeshProUGUI[] textsToFade = GetComponentsInChildren<TextMeshProUGUI>();

        // Set the number of fades to complete
        activeFades = imagesToFade.Length + textsToFade.Length;

        // Start the coroutines for each component type
        foreach (Image img in imagesToFade)
        {
            StartCoroutine(Fade(img));
        }

        foreach (TextMeshProUGUI txt in textsToFade)
        {
            StartCoroutine(Fade(txt));
        }
    }

    // Coroutine to fade the image passed as parameter in and out
    private IEnumerator Fade(Image image)
    {
        // Read the original full color
        Color originalColor = new Color(image.color.r, image.color.g, image.color.b, 1);
        Color fadedColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // Set the image to start with alpha zero
        image.color = fadedColor;

        // Fade-in logic
        Color startColor = image.color;
        Color endColor = originalColor;
        float elapsedTime = 0;

        while (elapsedTime < introDuration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, elapsedTime / introDuration);
            yield return null;
        }

        image.color = endColor;

        // Wait in full solid color
        yield return new WaitForSeconds(waitTime);

        // Fade-out logic
        startColor = image.color;
        endColor = fadedColor;
        elapsedTime = 0;

        while (elapsedTime < outroDuration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, elapsedTime / outroDuration);
            yield return null;
        }

        image.color = endColor;

        CheckIfDone();
    }

    // Overload for above coroutine to do the same but for text parameter
    private IEnumerator Fade(TextMeshProUGUI text)
    {
        // Read the original full color
        Color originalColor = new Color(text.color.r, text.color.g, text.color.b, 1);
        Color fadedColor = new Color(text.color.r, text.color.g, text.color.b, 0);

        // Set the text to start with alpha zero
        text.color = fadedColor;

        // Fade-in logic
        Color startColor = text.color;
        Color endColor = originalColor;
        float elapsedTime = 0;

        while (elapsedTime < outroDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsedTime / outroDuration);
            yield return null;
        }

        text.color = endColor;

        // Wait in full solid color
        yield return new WaitForSeconds(waitTime);

        // Fade-out logic
        startColor = text.color;
        endColor = fadedColor;
        elapsedTime = 0;

        while (elapsedTime < outroDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsedTime / outroDuration);
            yield return null;
        }

        text.color = endColor;

        CheckIfDone();
    }

    // Runs at the end of each coroutine
    private void CheckIfDone()
    {
        activeFades--;

        // If all components have finished fading, set the tutorial object inactive
        if (activeFades <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}