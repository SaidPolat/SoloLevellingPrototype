using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform player;

    private Vector3 textFirstPos;

    private int playerLevel = 1;

    private int diamondCount = 0;

    public TextMeshProUGUI diamondCountText;

    private bool isGrounded = true;

    public Animator animatorForPlayer;

    PlayerEffectsController effectsController;

    LevelManagerSaid levelManager;

    HorizontalMoveScript horizontalMoveScript;

    public UIScript uiScript;

    EnemyScript enemyScript;

    [Header("LevelTexts")]
    public Transform levelTexts;
    public TextMeshPro levelNumberText;
    public TextMeshPro levelUpNumberText;
    public TextMeshPro levelUpText;
    public GameObject levelUpNumberTextObj;

    [Header("Jump Properities")]
    public float jumpDistance = 10;
    public float jumpPower = 6f;
    public float jumpDuration = 1.5f; 
    
    void Start()
    {
        diamondCountText.text = "" + diamondCount;

        effectsController = GetComponentInChildren<PlayerEffectsController>();

        horizontalMoveScript = GetComponent<HorizontalMoveScript>();

        levelManager = FindObjectOfType<LevelManagerSaid>();

        textFirstPos = levelUpNumberTextObj.transform.localPosition;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyScript = other.GetComponent<EnemyScript>();

            int enemyLevel = enemyScript.enemyLevel;

            if(playerLevel < enemyLevel && enemyLevel > 10)
            {
                LevelDownFunction(other);
                return;
            }

            playerLevel += enemyLevel;

            Said.EventManager.Instance.CheckPlayerLevel(playerLevel);

            //CheckPlayerLevel(enemyScript);

            if (enemyLevel != 1)
            {

                effectsController.LevelUpEffects();

                AnimateLevelUpText(enemyLevel, true);

                if (enemyLevel > 10)                
                    EnemyRegularFunction(other, true);
                else
                    EnemyRegularFunction(other, false);

            }
            else
                EnemyRegularFunction(other, false);


        }

        if (other.CompareTag("Ramp"))
        {
            //Time.timeScale = 0.25f;

            isGrounded = false;

            player.DOLocalJump(player.localPosition + Vector3.forward * jumpDistance,
                jumpPower, 1, jumpDuration).OnComplete(() =>
                {
                    animatorForPlayer.SetTrigger("jumpEnd");
                    effectsController.StartSpinEffect(1);
                    isGrounded = true;
                });

            animatorForPlayer.SetTrigger("jumpStart");

            effectsController.StartSpinEffect(0);

            
        }

        if (other.CompareTag("Diamond"))
        {
            effectsController.StartDiamondPickEffect();

            other.gameObject.SetActive(false);

            diamondCount++;

            uiScript.StartPunchEffect();

            diamondCountText.text = "" + diamondCount;
        }

        if (other.CompareTag("Obstacle"))
        {
            LevelDownFunction(other);
        }

        if (other.CompareTag("LevelEnding"))
        {
            horizontalMoveScript.LevelEnding();

            levelManager.GoToNextLevel();
        }

    }

    public void EnemyRegularFunction(Collider other, bool isBoss)
    {
        if (isGrounded)
        {
            if (!isBoss)
                animatorForPlayer.SetTrigger("attack");
            else
                animatorForPlayer.SetTrigger("bossAttack");
        }

        Animator enemyAnim = other.GetComponent<Animator>();

        effectsController.StartHitEffect();

        SkinnedMeshRenderer enemyMaterial = other.GetComponentInChildren<SkinnedMeshRenderer>();

        enemyMaterial.material.SetColor("_Color", Color.gray);

        enemyAnim.SetTrigger("die");

        other.transform.DOMove(new Vector3(transform.position.x + Random.Range(-60, 60),
                transform.position.y + 100, transform.position.z + 70), 1.5f).OnComplete(() =>
                {
                    other.gameObject.SetActive(false);
                });

        levelNumberText.text = "" + playerLevel;

        ShakeTexts();
    }

    public void ShakeTexts()
    {
        levelTexts.DORewind();
        levelTexts.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), .15f);

    }

    public void AnimateLevelUpText(int enemyLevel, bool lvlUp)
    {
        levelUpNumberText.color = Color.white;
        levelUpText.color = Color.white;

        if (lvlUp)
        {
            levelUpNumberText.text = "+" + enemyLevel;
        }
        else
        {
            levelUpNumberText.text = "-" + enemyLevel;
            levelUpNumberText.color = Color.red;
            levelUpText.color = Color.red;
        } 

        levelUpNumberTextObj.SetActive(true);

        levelUpNumberText.DOFade(0f, 0.8f).SetEase(Ease.Linear);
        levelUpText.DOFade(0f, 0.8f).SetEase(Ease.Linear);

        levelUpNumberTextObj.transform.DOLocalMoveY(transform.localPosition.y + 50f, 0.8f).OnComplete(() =>
        {
            levelUpNumberTextObj.SetActive(false);

            levelUpNumberTextObj.transform.localPosition = textFirstPos;
            levelUpNumberText.DOFade(1f, 0f);
            levelUpText.DOFade(1f, 0f);
            
        });

    }

    public void LevelDownFunction(Collider other)
    {
        horizontalMoveScript.ObstacleHitMove();

        other.gameObject.tag = "Untagged";

        playerLevel -= 10;

        if (playerLevel <= 0)
            playerLevel = 1;

        Said.EventManager.Instance.CheckPlayerLevel(playerLevel);

        levelNumberText.text = "" + playerLevel;

        AnimateLevelUpText(10, false);
    }

    

}
