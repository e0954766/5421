using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public GameObject treasureWindow; // 弹窗对象
    public GameObject ruleWindow; // 弹窗对象
    public TextMeshProUGUI atts;
    public TextMeshProUGUI cks;
    public TextMeshProUGUI rs;
    public TextMeshProUGUI finishSelect;

    public ArrayList CK = new ArrayList();
    public ArrayList ATT = new ArrayList();
    public ArrayList newCK = new ArrayList();
    public ArrayList R = new ArrayList();

    public GameObject CKChoice_1;
    public GameObject CKChoice_2;
    public GameObject CKChoice_3;
    public GameObject CKChoice_4;
    public GameObject CKChoice_5;
    public GameObject CKChoice_6;


    public bool flag = true;

    //public Button button;

    void Update()
    {
        print(R.Count);
    }
    public void Finished()
    {
        // 更新CK
        changeCK(newCK);
        updateATT(newCK);
        // 关闭弹窗
        GameObject[] CKChoices = GameObject.FindGameObjectsWithTag("CKChoice");
        for(int i = 0; i < CKChoices.Length; i++)
        {
            TextMeshProUGUI text = CKChoices[i].GetComponentsInChildren<TextMeshProUGUI>()[0];
            text.text = " ";
            CKChoices[i].SetActive(false);
        }
        newCK = new ArrayList();
        finishSelect.text = "Don't add attributes";
        treasureWindow.SetActive(false);
    }
    // add attributes list [] into CK
    bool changeCK(ArrayList new_atts)
    {
        CK = new ArrayList(cks.text.Split(", "));

        foreach (string att in new_atts)
        {
            if (!CK.Contains(att))
            {
                CK.Add(att);
            }
        }
        CK.Sort();
        CK.Remove(" ");
        CK.Remove(""); 
        cks.text = string.Join(", ", (string[])CK.ToArray(typeof(string)));
        //cks.text = string.Join(", ", (string[])CK.ToArray(typeof(string)));
        return true;
    }
    bool updateATT(ArrayList at)
    {
        foreach (string att in at)
        {
            if (!ATT.Contains(att))
            {
                ATT.Add(att);
            }
        }
        ATT.Sort();
        ATT.Remove(" ");
        ATT.Remove(""); 
        atts.text = string.Join(", ", (string[])ATT.ToArray(typeof(string)));
        //atts.text = string.Join(", ", (string[])ATT.ToArray(typeof(string)));
        return true;
    }
    public void showUi(GameObject chooseUI){
        if(flag){
            R = new ArrayList(rs.text.Split(", "));
            flag = false;
        }
        
        chooseUI.SetActive(true);
        ATT = new ArrayList(atts.text.Split(", "));
        //print(ATT.Count);
        ArrayList sh = new ArrayList();
        foreach(string at in R){
            print(at);
            if(!ATT.Contains(at)){
                sh.Add(at);
            }
        }
        //print(sh.Count);
        if(sh.Count == 0){
            return;
        }
        if(sh.Count >= 1){
            CKChoice_1.SetActive(true);
        }
        if (sh.Count >= 2)
        {
            CKChoice_2.SetActive(true);
        }
        if (sh.Count >= 3)
        {
            CKChoice_3.SetActive(true);
        }
        if (sh.Count >= 4)
        {
            CKChoice_4.SetActive(true);
        }

        if (sh.Count >= 5)
        {
            CKChoice_5.SetActive(true);
        }
        if (sh.Count >= 6)
        {
            CKChoice_6.SetActive(true);
        }
        GameObject[] CKChoices = GameObject.FindGameObjectsWithTag("CKChoice");
        for (int i = 0; i < CKChoices.Length; i++) {
            TextMeshProUGUI text = CKChoices[i].GetComponentsInChildren<TextMeshProUGUI>()[0];
            text.text = sh[i].ToString();
        }
    }
    public void Select(GameObject CKChoice)
    {
        //ArrayList newCK = new ArrayList();
        TextMeshProUGUI selection = CKChoice.GetComponentsInChildren<TextMeshProUGUI>()[0];
        //print(selection.text);
        if(newCK.Contains(selection.text)){
            newCK.Remove(selection.text);
        }else{
            newCK.Add(selection.text);
        }
        //string conts = "Select Attribute: ";
        if(newCK.Count==0){
            finishSelect.text = "Don't add attributes";
        }else{
            finishSelect.text = "Select Attribute: " + string.Join(", ", (string[])newCK.ToArray(typeof(string)));
        }
        //button.interactable = false;

    }
    public void openRule()
    {
        ruleWindow.SetActive(true);
    }
    public void closeRule()
    {
        ruleWindow.SetActive(false);
    }

    public void MainToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void MenuToEasy()
    {
        SceneManager.LoadScene("level1");
    }
    public void MenuToMid()
    {
        SceneManager.LoadScene("level2");
    }
    public void MenuToHard()
    {
        SceneManager.LoadScene("level3");
    }
    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
