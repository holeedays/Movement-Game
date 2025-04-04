using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Enterhouse : MonoBehaviour
{

    //public GameObject doortext;
    public int sceneBuildIndex;
    public int sceneReset;
    // Start is called before the first frame update
    void Start()
    {
   
        //doortext.SetActive(false);
    }

    // Update is called once per frame

    //public GameObject target;
    //public GameObject player;
    public bool enterTrigger = false;

    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
                SceneManager.LoadScene(sceneBuildIndex);
          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //doortext.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneReset);
        }



    }
}
