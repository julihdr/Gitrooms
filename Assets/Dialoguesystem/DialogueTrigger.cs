using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private float triggerCooldown = 15f; // Cooldown time before trigger can be activated again

    private bool canTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canTrigger)
        {
            other.gameObject.GetComponent<DialogueManager>().DialogueStart(dialogueStrings, NPCTransform, StartCooldown);
            canTrigger = false;
        }
    }

    private void StartCooldown()
    {
        StartCoroutine(ResetTriggerCooldown());
    }

    private IEnumerator ResetTriggerCooldown()
    {
        yield return new WaitForSeconds(triggerCooldown);
        canTrigger = true; // Reset canTrigger after cooldown
    }
}


[System.Serializable]

public class DialogueString
{
    public string text;
    public bool isEnd;

    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Trigger Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;
}