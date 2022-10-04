using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyScript : MonoBehaviour
{
    public int enemyLevel;

    Animator anim;
    ParticleSystem bossEffect;
    MeshRenderer mesh;
    TextMeshPro levelNumberText;

    private void Start()
    {
        //mesh = GetComponentInChildren<MeshRenderer>();

        if (enemyLevel > 10)
        {
            anim = GetComponent<Animator>();

            levelNumberText = transform.Find("BossLevelText").GetComponent<TextMeshPro>();

            mesh = transform.Find("LvlBoard").GetComponent<MeshRenderer>();

            bossEffect = GetComponentInChildren<ParticleSystem>();

            anim.SetTrigger("bossIdle");
            bossEffect.Play();

            levelNumberText.text = "Lv" + enemyLevel;
        }
            

    }

    private void OnEnable()
    {
        Said.EventManager.OnCheckPlayerLevel += EventManager_OnCheckPlayerLevel;
    }

    private void OnDisable()
    {
        Said.EventManager.OnCheckPlayerLevel -= EventManager_OnCheckPlayerLevel;
    }

    private void EventManager_OnCheckPlayerLevel(int lvl)
    {
        if (enemyLevel <= 10) return;

        bool killable;

        if (lvl >= enemyLevel)
        {
            killable = true;
        }
        else
            killable = false;

        ChangeBoardColor(killable);
    }

    public void ChangeBoardColor(bool killable)
    {
        if(killable)
        {
            mesh.material.color = Color.green;
            bossEffect.Stop();
        }
        else
        {
            mesh.material.color = Color.red;
            bossEffect.Play();
        }
    }

}
