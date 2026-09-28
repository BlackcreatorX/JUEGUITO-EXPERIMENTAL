using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public TextAsset capitulo1JSON;

    [Header("Referencias UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoNombrePersonaje;
    public TextMeshProUGUI textoLanguage;

    [Header("Managers")]
    public TextAnimatorManager textAnimator;

    void Start()
    {
        
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (textAnimator.isTyping)
            {
                textAnimator.SkipAnimation();
            }
            else
            {
                nextDialogue();
            }
        }
    }

    public void InitDialogue(string startId)
    {
        DialogueDisplayData primeraLinea = DialogueManager.Instance.StartDialogue(startId);
        ShowData(primeraLinea);
    }

    public void nextDialogue()
    {
        DialogueDisplayData siguienteLinea = DialogueManager.Instance.GetNextDialogue();

        if (siguienteLinea == null) 
            EndConversation();
        else
            ShowData(siguienteLinea);
    }

    private void ShowData(DialogueDisplayData data)
    {
        panelDialogo.SetActive(true);
        textoNombrePersonaje.text = data.speakerName;
        textAnimator.AnimateText(textoDialogo, data.dialogueText);
        SoundManager.Instance.ProcessAudioInstructions(data.audioData);
    }

    private void EndConversation()
    {
        panelDialogo.SetActive(false);
    }

    public void ToggleLanguage()
    {
        DialogueManager.Instance.currentLanguage =
            (DialogueManager.Instance.currentLanguage == "ES") ? "EN" : "ES";

        textoLanguage.SetText(DialogueManager.Instance.currentLanguage);
        PlayerPrefs.SetString("Language", DialogueManager.Instance.currentLanguage);
        PlayerPrefs.Save();

        DialogueDisplayData datosActualizados = DialogueManager.Instance.GetCurrentNodeData();

        if (datosActualizados != null)
        {
            textoNombrePersonaje.text = datosActualizados.speakerName;

            textoDialogo.text = datosActualizados.dialogueText;
            textoDialogo.maxVisibleCharacters = datosActualizados.dialogueText.Length;

            if (textAnimator.isTyping)
            {
                textAnimator.SkipAnimation();
            }
        }
    }
}