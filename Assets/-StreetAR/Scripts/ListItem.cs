using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

	public Text NameText;
	public Text CheckinsText;

	// Use this for initialization
	void Start () {
		
	}

	public void SetLocationData(string name, int checkins)
	{
		NameText.text = name;
		CheckinsText.text = checkins.ToString ();
		gameObject.SetActive (true);
	}

    public void SetLocationData(string name, string strVal)
    {
        NameText.text = name;
        CheckinsText.text = strVal;
        gameObject.SetActive(true);
    }

    //public void SetLocationData2(string name, string test)
    //{
    //	NameText.text = name;
    //	CheckinsText.text = test.Split (',') [0].Trim (); //test;
    //	gameObject.SetActive (true);
    //}

    public void ClearData()
	{
		NameText.text = "";
		CheckinsText.text = "";
		gameObject.SetActive (false);
	}
}
