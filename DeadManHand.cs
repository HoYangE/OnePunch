using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static PlayerStatesData;

public class DeadManHand : IEnvironment
{
    Vignette vignette;

    bool isDone;
    bool canTrigger;
    int curCount;

    private void Awake()
    {
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

    public override void Stay(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            // 플레이어가 망자의 손 뒤로 보이도록 레이어 설정해주세요
            if (canTrigger && collision.name == "Player")
            {
                if (actionState != ActionState.Dash && buffManager.DarkDebuffCount < 3)
                {
                    Camera.main.GetComponent<Volume>().profile.TryGet<Vignette>(out vignette);
                    StartCoroutine(DarkDebuff());
                }
            }
        }
        if (collision.gameObject.tag == "Enemy")
        {
            // 적이 망자의 손 위로 보이도록 레이어 설정해주세요
        }
    }

    public override void Exit(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            // 레이어를 원상복구시켜주세요 
        }
        if (collision.gameObject.tag == "Enemy")
        {
            // 레이어를 원상복구시켜주세요 
        }
    }

    IEnumerator CenterToPlayer()
    {
        while (!isDone)
        {
            
            yield return null;
        }
    }

    IEnumerator FadeOut(float time)
    {
        float intensity = vignette.intensity.value;

        while (intensity > 0f)
        {
            vignette.intensity.value = intensity;
            intensity -= Time.deltaTime / time;
            if (intensity <= 0f) vignette.intensity.value = 0;
            yield return null;
        }
    }

    IEnumerator DarkDebuff()
    {
        StartCoroutine(Timer(10));
        if (buffManager.DarkDebuffCount > 0 && buffManager.DarkTimerCoroutine != null && buffManager.DeadManHandObject!=null)
            buffManager.DeadManHandObject.GetComponent<DeadManHand>().StopCoroutine(buffManager.DarkTimerCoroutine);
        if (buffManager.DarkDebuffCount > 0 && buffManager.DarkColorCoroutine != null && buffManager.DeadManHandObject != null)
            buffManager.DeadManHandObject.GetComponent<DeadManHand>().StopCoroutine(buffManager.DarkColorCoroutine);
        isDone = false;
        canTrigger = false;
        buffManager.DarkDebuffCount++;
        curCount = buffManager.DarkDebuffCount;
        buffManager.DeadManHandObject = gameObject;
        GameObject playerObject = GameObject.Find("Player");
        PlayerEffectController effectController = playerObject.GetComponent<PlayerEffectController>();
        effectController.DarkDebuff = true;
        switch (buffManager.DarkDebuffCount)
        {
            case 1:
                vignette.intensity.value = 0.55f;
                break;
            case 2:
                vignette.intensity.value = 0.6f;
                buffManager.DarkColorCoroutine = StartCoroutine(Color());
                break;
            case 3:
                vignette.intensity.value = 0.63f;
                buffManager.DarkColorCoroutine = StartCoroutine(Color());
                break;
        }
        buffManager.DarkTimerCoroutine = StartCoroutine(UITimer(playerObject));
        yield return new WaitForSeconds(15 - 1);
        if (canTrigger && curCount == buffManager.DarkDebuffCount)
        {
            StartCoroutine(FadeOut(1));
            yield return new WaitForSeconds(1);
            isDone = true;
            if (isDone)
            {
                buffManager.DarkDebuffCount = 0;
                effectController.DarkDebuff = false;
                if(buffManager.DarkTimerCoroutine != null)
                    StopCoroutine(buffManager.DarkTimerCoroutine);
                if (buffManager.DarkColorCoroutine != null)
                    StopCoroutine(buffManager.DarkColorCoroutine);
                buffManager.DarkTimerCoroutine = null;
                buffManager.DarkColorCoroutine = null;
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
        GameObject Dark = buffManager.DarkDebuff;
        float r = 1;

        while (Dark.GetComponent<Image>().color.r > 0f)
        {
            Dark.GetComponent<Image>().color = new Color(r, 0,0);
            r -= Time.deltaTime / 1;
            yield return null;
        }
        Dark.GetComponent<Image>().color = new Color(1, 1, 1);
    }
    IEnumerator UITimer(GameObject playerObject)
    {
        float time = 15;

        while (true)
        {
            time -= Time.deltaTime;
            buffManager.DarkTime = (int)time;
            Vector2 pos = Camera.main.WorldToViewportPoint(playerObject.transform.position);
            vignette.center.value = pos;
            yield return null;
        }
    }
}