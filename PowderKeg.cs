using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMgr;

public class PowderKeg : IEnvironment
{
    public GameObject explosion;
    public GameObject explosionFX;
    float Hp;
    bool use = false;

    private void Start()
    {
        Hp = EnvironmentData.EnvironmentTable["PowderKeg"].durability;
    }

    public override void Stay(Collider2D collision)
    {

    }

    public override void Exit(Collider2D collision)
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag =="Untagged" && collision.gameObject.layer == 6 && GetComponent<BoxCollider2D>().enabled == false)
        {
            GM.ChangeHP(-50);
            GM.ChangeBackHP(-50);
        }
        if (collision.gameObject.tag == "EnemyHitColl" && GetComponent<BoxCollider2D>().enabled == false)
        {
            if(collision.transform.parent.GetComponent<EnemyController>().attTypeValue == 2 || collision.transform.parent.GetComponent<EnemyController>().attTypeValue == 4)
                collision.transform.parent.GetComponent<EnemyController>().Hit(10000);
            else
                collision.transform.parent.GetComponent<EnemyController>().Hit(10000);
        }
    }

    public void Hit(float damage)
    {
        Hp -= damage;
        if (Hp <= 0 && !use)
        {
            use = true;
            StartCoroutine(StartExplosion());
        }
    }

    IEnumerator StartExplosion()
    {
        GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "animation";
        yield return new WaitForSeconds(1.75f);
        GetComponent<AudioSource>().Play();
        StartEffect();
        StartCoroutine(Explosion());
    }

    void StartEffect()
    {
        explosionFX.SetActive(true);
        explosionFX.GetComponent<ParticleSystem>().Play();
        //불꽃 파티클 재생
        Destroy(explosionFX, 2f);
    }
    IEnumerator Explosion()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        explosion.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        explosion.GetComponent<BoxCollider2D>().enabled = false;
    }
}
