using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMgr;

public class Jangseung : IEnvironment
{
    public Sprite Origin;
    public AudioClip[] clips;
    //public Sprite BrokenBody;
    //public GameObject BrokenHead;
    float Hp;
    //float RespownTime;
    bool isAlive;

    private void Start()
    {
        Hp = EnvironmentData.EnvironmentTable["Jangseung"].durability;
        //RespownTime = EnvironmentData.EnvironmentTable["Jangseung"].respawn;
        isAlive = true;
    }

    public override void Stay(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {

        }
        if (collision.gameObject.tag == "Enemy")
        {

        }
    }

    public override void Exit(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {

        }
        if (collision.gameObject.tag == "Enemy")
        {

        }
    }

    public void Hit(float damage)
    {
        Hp -= damage;
        GetComponent<AudioSource>().clip = clips[0];
        GetComponent<AudioSource>().Play();
        if (Hp <= 0)
        {
            GetComponent<AudioSource>().clip = clips[1];
            GetComponent<AudioSource>().Play();
            if (GameObject.Find("Player").GetComponent<PlayerController>().LeftOrRight() == true)
                GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "Left";
            else
                GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "Right";
            //GetComponent<SpriteRenderer>().sprite = BrokenBody;
            //BrokenHead.SetActive(true);
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(FadeOut(2));
            //StartCoroutine(Respawn(EnvironmentData.EnvironmentTable["Jangseung"].respawn));
        }
    }

    /*IEnumerator Respawn(float time)
    {
        yield return new WaitForSeconds(time);
        isAlive = true;
        GetComponent<BoxCollider2D>().enabled = true;
        Hp = EnvironmentData.EnvironmentTable["Jangseung"].durability;
        GetComponent<SpriteRenderer>().sprite = Origin;
        BrokenHead.SetActive(false);
    }*/

    IEnumerator FadeOut(float time)
    {
        yield return new WaitForSeconds(0.5f);
        float intensity = 1;

        while (intensity > 0f)
        {
            intensity -= Time.deltaTime / time;
            GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, intensity);
            //BrokenHead.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, intensity);
            if (GetComponent<MeshRenderer>().material.color.a <= 0f) new Color(1, 1, 1, 0);
            yield return null;
        }
    }
}
