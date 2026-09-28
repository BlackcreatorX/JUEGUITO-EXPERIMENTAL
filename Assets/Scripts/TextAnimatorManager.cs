using System.Collections;
using UnityEngine;
using TMPro;

public class TextAnimatorManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo en segundos entre cada letra.")]
    public float typingSpeed = 0.03f;

    public bool isTyping { get; private set; }

    private Coroutine typingCoroutine;
    private TextMeshProUGUI textComponent;

    public void AnimateText(TextMeshProUGUI tmpText, string fullText)
    {
        textComponent = tmpText;

        textComponent.text = fullText;

        textComponent.maxVisibleCharacters = 0;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        isTyping = true;

        textComponent.ForceMeshUpdate();
        int totalCharacters = textComponent.textInfo.characterCount;

        while (textComponent.maxVisibleCharacters < totalCharacters)
        {
            textComponent.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void SkipAnimation()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
        isTyping = false;
    }
}