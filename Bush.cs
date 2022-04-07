using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using static GameMgr;

public class Bush : IEnvironment
{
    public int BerryBushSize;
    public bool isBerryBush = false;
    public GameObject[] BerryObject;
    private PlayerController player;

    Coroutine StartRecoveryCoroutine = null;
    int berryCount;
    bool isDone;
    bool canTrigger;
    int curCount;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        berryCount = BerryBushSize;
        isDone = false;
        canTrigger = true;
        curCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    override public void Stay(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if(collision.name=="Player")
                collision.transform.Find("Spine Animation").GetComponent<MeshRenderer>().material.color = (new Color(1, 1, 1, 0.5f));

            player.Bush(true);
            if (isBerryBush && StartRecoveryCoroutine == null && berryCount > 0 && GM.playerHP < GM.playerMaxHP)
                StartRecoveryCoroutine = StartCoroutine(Recovery());
            if(canTrigger && buffManager.SlowDebuffCount < 3 && isBerryBush == false)
                StartCoroutine(SlowDebuff());
        }
        if (collision.gameObject.tag == "Enemy")
        {
            collision.transform.GetComponent<EnemyController>().isInBush = true;
        }
    }
    override public void Exit(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (collision.name == "Player")
                collision.transform.Find("Spine Animation").GetComponent<MeshRenderer>().material.color = (new Color(1, 1, 1, 1f));
            player.Bush(false);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            collision.transform.GetComponent<EnemyController>().isInBush = false;
        }
    }
    IEnumerator Recovery()
    {
        berryCount--;
        BerryObject[berryCount].SetActive(false);
        GM.ChangeBackHP(GM.playerMaxHP * 0.05f);
        float targetBackHP = GM.playerBackHP;
        float timer = 2;
        float curTime = 0;
        while (true)
        {
            curTime += Time.deltaTime;
            GM.ChangeHP((GM.playerMaxHP * 0.05f) * (Time.deltaTime / timer));
            if (curTime >= timer) break;
            if (targetBackHP > GM.playerBackHP)
            {
                GM.playerBackHP = GM.playerHP;
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        StartRecoveryCoroutine = null;
    }

    IEnumerator SlowDebuff()
    {
        StartCoroutine(Timer(10));
        if (buffManager.SlowDebuffCount > 0 && buffManager.SlowTimerCoroutine != null && buffManager.BushObject != null)
            buffManager.BushObject.GetComponent<Bush>().StopCoroutine(buffManager.SlowTimerCoroutine);
        if (buffManager.SlowDebuffCount > 0 && buffManager.SlowColorCoroutine != null && buffManager.BushObject != null)
            buffManager.BushObject.GetComponent<Bush>().StopCoroutine(buffManager.SlowColorCoroutine);
        isDone = false;
        canTrigger = false;
        buffManager.SlowDebuffCount++;
        curCount = buffManager.SlowDebuffCount;
        buffManager.BushObject = gameObject;
        GameObject playerObject = GameObject.Find("Player");
        PlayerEffectController effectController = playerObject.GetComponent<PlayerEffectController>();
        effectController.SlowDebuff = true;
        if(buffManager.SlowDebuffCount > 1)
            buffManager.SlowColorCoroutine = StartCoroutine(Color());
        buffManager.SlowTimerCoroutine = StartCoroutine(UITimer());
        yield return new WaitForSeconds(10 * buffManager.SlowDebuffCount - 1);
        if (curCount == buffManager.SlowDebuffCount)
        {
            yield return new WaitForSeconds(1);
            isDone = true;
            if (isDone)
            {
                buffManager.SlowDebuffCount = 0;
                effectController.SlowDebuff = false;
                if(buffManager.SlowTimerCoroutine != null)
                    StopCoroutine(buffManager.SlowTimerCoroutine);
                if (buffManager.SlowColorCoroutine != null)
                    StopCoroutine(buffManager.SlowColorCoroutine);
                buffManager.SlowTimerCoroutine = null;
                buffManager.SlowColorCoroutine = null;
            }
        }
    }
    IEnumerator Timer(int time)
    {
        yield return new WaitForSeconds(time);
        canTrigger = true;
    }

    IEnumerator Color()
    {
        GameObject Bush = buffManager.SlowDebuff;
        float r = 1;

        while (Bush.GetComponent<Image>().color.r > 0f)
        {
            Bush.GetComponent<Image>().color = new Color(r, 0, 0);
            r -= Time.deltaTime / 1;
            yield return null;
        }
        Bush.GetComponent<Image>().color = new Color(1, 1, 1);
    }
    IEnumerator UITimer()
    {
        float time = 10 * buffManager.SlowDebuffCount;

        while (true)
        {
            time -= Time.deltaTime;
            buffManager.SlowTime = (int)time;
            yield return null;
        }
    }
}