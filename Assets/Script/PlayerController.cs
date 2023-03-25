using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp1;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D col;

    public float speed;
    public float jumpforce;

    public Transform groundCheck;
    public LayerMask ground;

    public bool isGround;
    public bool isJump;

    bool jumpPressed;
    int jumpCount;

    public GameObject retryHint;
    public GameObject treasureWindow;

    //public GameObject[] CKChoices = new GameObject[] { CKChoice_2, CKChoice_3, CKChoice_4 };

    public TextMeshProUGUI atts;
    public TextMeshProUGUI cks;
    public TextMeshProUGUI fds;

    public GameObject winUI;
    public GameObject loseUI;
    public GameObject hintUI;
    public GameObject collectUI;

    public ArrayList R = new ArrayList();
    public ArrayList CK = new ArrayList();
    public ArrayList FD = new ArrayList();
    public ArrayList ATT = new ArrayList();
    public ArrayList RCK = new ArrayList();


    public GameObject[] boxes;
    public TextMeshProUGUI boxText;

    public TreasureController treasure;
    public bool canOpen;


    //
    // function to update: use openBox to check the fd, if 'none' then stop
    // if rhs, update ATT with rhs
    //ask player to choose from rhs and call changeCK to update CK
    //
    // add new function
    ArrayList splitAttr(string side)
    {
        ArrayList res = new ArrayList(side.Split(','));
        return res;
    }
    // get the fd from boxText, then use openBox function to detect if player can open the box
    // if rhs is ["None"], it means player cannot open the box
    // otherwise, return the value of attributes players can add (rhs-CK)
    ArrayList openBox(string fd)
    {
        ArrayList lr = new ArrayList(fd.Split("->"));
        ArrayList lhs = splitAttr(lr[0].ToString());
        foreach (string att in lhs)
        {
            if (!ATT.Contains(att))
            {
                return new ArrayList() { "None" };
            }
        }
        ArrayList rhs = splitAttr(lr[1].ToString());
        /*
        ArrayList res = new ArrayList();
        foreach (string arr in rhs)
        {
            if (!CK.Contains(arr))
            {
                res.Add(arr);
            }
        }
        if(res.Count==0){
            res.Add("Skip");
        }*/
        return rhs;
    }
    // add attributes list [] into CK
    bool changeCK(ArrayList atts)
    {
        foreach (string att in atts)
        {
            if (!CK.Contains(att))
            {
                CK.Add(att);
            }
        }
        return true;
    }
    // update ATT
    bool updateATT(ArrayList at)
    {
        foreach (string att in at)
        {
            if (!ATT.Contains(att))
            {
                ATT.Add(att);
            }
        }
        atts.text = string.Join(", ", (string[])ATT.ToArray(typeof(string)));
        return true;
    }
    // check if user are able to go for next level ATT == R -> true, otherwise -> false
    bool checkDoor()
    {
        foreach (string att in R)
        {
            if (!ATT.Contains(att))
            {
                return false;
            }
        }
        return true;
    }
    // check if user get the right candidate key, only called at the end of each level
    bool checkRCK()
    {
        CK.Sort();
        string key = string.Join(",", (string[])CK.ToArray(typeof(string)));
        return RCK.Contains(key);
    }

    void Test()
    {
        ArrayList test = new ArrayList();
        
        print("test1");
        test = openBox("A->B");
        print(test[0]);
        print("test2");
        test = openBox("B->C,D");
        print(test[0]);
        print("test3");
        test = openBox("A->A,E");
        print(test[0]);
        //print(test[1]);
        bool flag = checkDoor();
        print(flag);
        flag = checkRCK();
        print(flag);
    }


    // Start is called before the first frame update
    void Start()
    {
        InitPlayer();
        InitAttr();
        InitUI();
        //Test();

    }
    void InitUI()
    {
        //retryHint = GameObject.Find("Retry").GetComponent();
        retryHint.SetActive(false);
        boxes = GameObject.FindGameObjectsWithTag("BoxText");
        int cnt = 0;
        foreach (GameObject box in boxes)
        {
            if (box != null)
            {
                boxText = box.GetComponent<TextMeshProUGUI>();
                if (boxText != null)
                {
                    //Debug.Log(boxText.text);
                    boxText.text = FD[cnt] + "->" + FD[cnt + 1];
                    //openBox(FD[cnt] + "->" + FD[cnt + 1]);
                    cnt = cnt + 2;
                }
                else
                {
                    Debug.Log("what?");
                }
            }
        }
    }
    void InitPlayer(){
        transform.localPosition = new Vector3(-6.2f, -1.4f, 0);
        rb = this.GetComponent<Rigidbody2D>();    
        col= this.GetComponent<BoxCollider2D>();
        anim= this.GetComponent<Animator>(); 
    }
    void InitAttr(string difficulty = "easy")
    {
        /*
        R = new string[]{"A","B","C","D","E"};
        //A->B, B->C, C->D,E
        FD = new string[]{"A","B","B","C","C","D,E"};
        CK = new string[]{"A"};
        ATT = new string[]{"A"};*/
        GameConfig config = new GameConfig(difficulty);
        Generator generator = new Generator(config);
        JObject data = generator.Generate();
        
        var rArray = (JArray)data["R"];
        foreach (var attr in rArray)
        {
            R.Add(attr.ToString());
        }

        var fdArray = (JArray)data["FD"];
        foreach (var fd in fdArray)
        {
            var lhsArray = (JArray)fd[0];
            var lhs = string.Join("", lhsArray.Select(x => x.ToString()));
            var rhsArray = (JArray)fd[1];
            var rhs = string.Join("", rhsArray.Select(x => x.ToString()));
            FD.Add(lhs);
            FD.Add(rhs);
        }
        
        var ckArray = (JArray)data["CK"];
        foreach (var ck in ckArray)
        {
            var keyArray = (JArray)ck;
            var key = string.Join("", keyArray.Select(x => x.ToString()));
            RCK.Add(key);
        }

        atts.text = string.Join(", ", (string[])ATT.ToArray(typeof(string)));
        cks.text = string.Join(", ", (string[])CK.ToArray(typeof(string)));
        string strf = "";
        for (int i = 0; i < FD.Count; i++)
        {
            strf += FD[i] + " -> " + FD[i + 1] + " ; ";
            i++;
        }
        fds.text = strf;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount>0)
        {
            jumpPressed = true;
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            ArrayList al = openBox(treasure.treasure.GetComponentsInChildren<TextMeshProUGUI>()[0].text);
            //print(al[0]);
            if (al[0].ToString() != "None" && canOpen && !treasure.isOpened)
            {
                updateATT(al);
                collectUI.SetActive(true);
                TextMeshProUGUI ctext = collectUI.GetComponentsInChildren<TextMeshProUGUI>()[0];
                ctext.text = "You have collected attributes:" + string.Join(",", (string[])al.ToArray(typeof(string)));
                Invoke("shutC", 2.0f); 
            }
        }
        


    }
    void shutC(){
        collectUI.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        Movement();
        Jump();
        SwitchAnim();
        FallDetection();
        WallDetection();
        updatCKUI();
    }
    void updatCKUI(){
        CK = new ArrayList(cks.text.Split(", "));
        ATT = new ArrayList(atts.text.Split(", "));
    }
    void Movement()
    {
        //Move
        float horizontalmove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalmove * speed, rb.velocity.y);
        //Face direction
        if (horizontalmove  != 0)
        {
            transform.localScale = new Vector3(horizontalmove, 1, 1);
        }
        
    }
    void Jump()
    {
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            jumpCount--;
            jumpPressed = false;
        }else if (jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            jumpCount--;
            jumpPressed = false;
        }
    }
    void SwitchAnim()
    {
        //switch animation: Idle & Run
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
        if (isGround)
        {
            //switch animation: Fall to Idle
            anim.SetBool("falling", false);
        }else if (!isGround && rb.velocity.y>0)
        {
            //switch animation: Idle/Run to Jump
            anim.SetBool("jumping", true);
        }else if (rb.velocity.y < 0)
        {
            //switch animation: Jump to Fall
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }
        if(rb.velocity.y < 0)
        {
            //switch animation: Idle/Run to Fall
            anim.SetBool("falling", true);
        }
    }
    void WallDetection(){
        if(transform.localPosition.x >42){ // player fall below the window, restart the game
            transform.localPosition = new Vector3(42, transform.localPosition.y, 0);
        }
        if(transform.localPosition.y >14){ // player fall below the window, restart the game
            transform.localPosition = new Vector3(transform.localPosition.x, 14, 0);
        }
    }
    void FallDetection(){
        if(transform.localPosition.y <-5){ // player fall below the window, restart the game
            transform.localPosition = new Vector3(transform.localPosition.x, -10f, 0);
            retryHint.SetActive(true); 
            Invoke("ReStartThisScene", 2.0f); 
        }
    }
    void ReStartThisScene()
    {
        retryHint.SetActive(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Box")
        {
            treasure = other.GetComponent<TreasureController>();
            treasure.validLHS = openBox(treasure.treasure.GetComponentsInChildren<TextMeshProUGUI>()[0].text)[0].ToString()!="None";
            canOpen = true;
        }
        if(other.tag == "Finish"){
            if(checkDoor() && checkRCK()){
                winUI.SetActive(true); 
                Invoke("ReStartThisScene", 3.0f); //换成下一关
            } else if(checkDoor()==false){
                //hint
                hintUI.SetActive(true);
            }else if(checkRCK()==false){
                loseUI.SetActive(true); 
                Invoke("ReStartThisScene", 3.0f);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Box")
        {
            canOpen = false;
        }
        if(other.tag == "Finish"){
            winUI.SetActive(false); 
            loseUI.SetActive(false); 
            hintUI.SetActive(false);
        }
    }


    
}
