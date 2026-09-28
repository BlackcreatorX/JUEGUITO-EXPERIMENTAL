using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class AudioInstructions
{
    public string bgm;
    public string bgmAction;
    public float fadeDuration;
    public string sfx;
}

[System.Serializable]
public class DialogueNode
{
    public Dictionary<string, string> speaker;
    public Dictionary<string, string> text; 
    public AudioInstructions audio;
    public string nextId; 
}

public class DialogueDisplayData
{
    public string speakerName;
    public string dialogueText;
    public AudioInstructions audioData;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuración")]
    public string currentLanguage = "ES";

    private Dictionary<string, DialogueNode> currentChapterDialogue;

    private string currentNodeId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentLanguage = PlayerPrefs.GetString("Language", "ES");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadChapter(TextAsset jsonFile)
    {
        if (jsonFile != null)
        {
            currentChapterDialogue = JsonConvert.DeserializeObject<Dictionary<string, DialogueNode>>(jsonFile.text);
            Debug.Log("Capítulo cargado exitosamente.");
        }
        else
        {
            Debug.LogError("El JSON proporcionado es nulo.");
        }
    }

    public DialogueDisplayData StartDialogue(string startId)
    {
        currentNodeId = startId;
        return GetCurrentNodeData();
    }

    public DialogueDisplayData GetNextDialogue()
    {
        if (currentChapterDialogue.TryGetValue(currentNodeId, out DialogueNode currentNode))
        {
            if (currentNode.nextId == "END" || string.IsNullOrEmpty(currentNode.nextId))
            {
                currentNodeId = "END";
                return null;
            }
            currentNodeId = currentNode.nextId;
            return GetCurrentNodeData();
        }

        return null;
    }

    public DialogueDisplayData GetCurrentNodeData()
    {
        if (currentChapterDialogue.TryGetValue(currentNodeId, out DialogueNode node))
        {
            DialogueDisplayData displayData = new DialogueDisplayData();

            if (node.text.TryGetValue(currentLanguage, out string textData))
            {
                displayData.dialogueText = textData;
            }
            else
            {
                displayData.dialogueText = node.text.ContainsKey("EN") ? node.text["EN"] : "[Texto faltante]";
            }

            if (node.speaker != null)
            {
                if (node.speaker.TryGetValue(currentLanguage, out string speakerData))
                {
                    displayData.speakerName = speakerData;
                }
                else
                {
                    displayData.speakerName = node.speaker.ContainsKey("EN") ? node.speaker["EN"] : "???";
                }
            }
            else
            {
                displayData.speakerName = "";
            }

            displayData.audioData = node.audio;

            return displayData;
        }

        Debug.LogError($"[DialogueManager] No se pudo encontrar el nodo con ID: {currentNodeId}");
        return null;
    }
}