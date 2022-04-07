using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMgr;

public class Stone : IEnvironment
{
    private MainUIMgr ui;
    public bool isUsed;
    public GameObject fx;

    bool canShow;
    bool canClose;

    private void Awake()
    {
        isUsed = false;
        canShow = true;
        canClose = false;
    }

    void Start()
    {
        ui = GameObject.Find("UI").GetComponent<MainUIMgr>();
    }

    public override void Stay(Collider2D collision)
    {

    }

    public override void Exit(Collider2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && ui.useLShift == false && canShow == true)
        {
            ui.ShowLShiftText();
            canShow = false;
            canClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && ui.useLShift == false && canClose == true)
        {
            StartCoroutine(Delay());
            canShow = true;
            canClose = false;
        }
    }

    IEnumerator Delay() 
    {
        yield return new WaitForSeconds(1);
        ui.CloseLShiftText();
        canShow = true;
        canClose = false;
    }
}
