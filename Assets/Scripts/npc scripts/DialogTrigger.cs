using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actors;
    public GameObject talk;
    public GameObject actor;



    private DialogM dM;

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && DialogM.isActive == false)
        {
            talk.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                StartDialogue();
                talk.SetActive(false);
                //actor.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        talk.SetActive(false);
    }
    public void StartDialogue()
    {
        dM.OpenDialogue(messages, actors);
    }
    private void Start()
    {
        talk.SetActive(false);

        dM = FindFirstObjectByType<DialogM>();
    }



}
[System.Serializable]
public class Message{
    public int actorId;
    public string message;
}
[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}