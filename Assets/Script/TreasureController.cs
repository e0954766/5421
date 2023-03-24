using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    public bool canOpen;
    public bool isOpened;
    private Animator anim;
    private Collider2D col;
    public string FD;
    public GameObject treasure;
    public bool validLHS = true;
    public GameObject boxUI;

    // Start is called before the first frame update
    void Start()
    {
        //print(treasure.GetComponent<Transform>().position);
        anim = GetComponent<Animator>();
        isOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            if (validLHS && canOpen && !isOpened)
            {
                anim.SetTrigger("opening");
            }else if(validLHS==false && canOpen){
                boxUI.SetActive(true);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            canOpen = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            canOpen = false;
            boxUI.SetActive(false);
        }
    }
}
