using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float turningSpeed = 1.0f;
    [SerializeField] private AudioSource typingAudioSource;

    private List<DialogueString> dialogueList;

    [Header("Player")]
    [SerializeField] private MonoBehaviour firstPersonController;
    private Transform playerCamera;

    private int CurrentDialogueIndex = 0;

    private UnityAction dialogueFinishCallback; // Callback function to be invoked when dialogue is finished

    private Coroutine skipDialogueCoroutine; // Coroutine reference for skipping dialogue

    private void Start()
    {
        dialogueParent.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    public void DialogueStart(List<DialogueString> textToPrint, Transform NPC, UnityAction finishCallback)
    {
        dialogueParent.SetActive(true);
        firstPersonController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(TurnCameraTowardsNPC(NPC));
        dialogueList = textToPrint;
        CurrentDialogueIndex = 0;

        DisableButtons();

        dialogueFinishCallback = finishCallback; // Assign the callback function
        StartCoroutine(PrintDialogue());

        skipDialogueCoroutine = StartCoroutine(WaitForSkip()); // Start coroutine for skipping dialogue
    }

    private void DisableButtons()
    {
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TMP_Text>().text = "";
        option2Button.GetComponentInChildren<TMP_Text>().text = "";
    }

    private IEnumerator TurnCameraTowardsNPC(Transform NPC)
    {
        Quaternion startRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position);
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            playerCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * turningSpeed;
            yield return null;
        }
        playerCamera.rotation = targetRotation;
    }

    private bool optionSelected = false;

    private IEnumerator PrintDialogue()
    {
        while (CurrentDialogueIndex < dialogueList.Count)
        {
            DialogueString line = dialogueList[CurrentDialogueIndex];
            line.startDialogueEvent?.Invoke();

            DisableButtons(); // Disable buttons before typing starts
            yield return StartCoroutine(TypeText(line.text));

            if (line.isQuestion || line.isEnd) // Check if it's a question or the end
            {

                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;

                // Clear existing listeners
                option1Button.onClick.RemoveAllListeners();
                option2Button.onClick.RemoveAllListeners();

                // Add new listeners
                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSelected);
                optionSelected = false; // Reset the flag after handling the option
               
            }

            line.endDialogueEvent?.Invoke();
            if (line.isEnd)
            {
                // End the dialogue if this is the last dialogue string
                DialogueStop();
                break;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DialogueStop();
                yield break; // Exit the coroutine if spacebar is pressed
            }
        }
    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSelected = true;
        CurrentDialogueIndex = indexJump;// Stop waiting for skip if an option is selected
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        // Play the typing audio clip once at the beginning of typing
        if (typingAudioSource && typingAudioSource.clip)
        {
            typingAudioSource.PlayOneShot(typingAudioSource.clip);
        }

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

            // Check if spacebar is pressed to stop typewriter sound
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (typingAudioSource)
                {
                    typingAudioSource.Stop();
                    DialogueStop();
                    yield break;
                }
            }
        }

        // Stop the typing audio clip when the text is fully typed
        if (typingAudioSource)
        {
            typingAudioSource.Stop();
        }

        // Enable buttons if it's a question or end dialogue
        if (dialogueList[CurrentDialogueIndex].isQuestion || dialogueList[CurrentDialogueIndex].isEnd)
        {
            option1Button.interactable = true;
            option2Button.interactable = true;
        }

        // Wait for user input if it's not a question
        if (!dialogueList[CurrentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        // Check if this is the last dialogue string
        if (dialogueList[CurrentDialogueIndex].isEnd)
        {
            DialogueStop();
        }
    }

    private IEnumerator WaitForSkip()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                DialogueStop();
                yield break; // Exit the coroutine once spacebar is held down
            }
            yield return null;
        }
    }

    private void DialogueStop()
    {
        if (dialogueList[CurrentDialogueIndex].isQuestion && dialogueList[CurrentDialogueIndex].isEnd)
        {
            StartCoroutine(DialogueStopWithMouseClick()); // Start the DialogueStopWithMouseClick coroutine
        }
        else
        {
            StopAllCoroutines();
            dialogueText.text = "";
            dialogueParent.SetActive(false);
            firstPersonController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;

            // Stop the typing audio clip when the dialogue stops
            if (typingAudioSource)
            {
                typingAudioSource.Stop();
            }

            if (dialogueFinishCallback != null)
            {
                dialogueFinishCallback.Invoke(); // Invoke the callback function when dialogue ends
                dialogueFinishCallback = null; // Reset the callback
            }
        }
    }

    private IEnumerator DialogueStopWithMouseClick()
    {
        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked = true;
            }
            yield return null;
        }

        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);
        firstPersonController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        // Stop the typing audio clip when the dialogue stops
        if (typingAudioSource)
        {
            typingAudioSource.Stop();
        }

        if (dialogueFinishCallback != null)
        {
            dialogueFinishCallback.Invoke(); // Invoke the callback function when dialogue ends
            dialogueFinishCallback = null; // Reset the callback
        }
    }
}
