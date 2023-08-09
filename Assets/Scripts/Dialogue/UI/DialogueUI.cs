using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dailogueText;
    public Image faceRight, faceLeft;
    public TextMeshProUGUI nameRight, nameLeft;
    public GameObject continueBox;
    private void Awake()
    {
        continueBox.SetActive(false);
    }
}
