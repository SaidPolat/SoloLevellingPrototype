using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HorizontalMoveScript : MonoBehaviour
{
    private bool drag;

    //private InputPhase phase;

    private Vector2 firstPos;

    private float clampedVal;

    private float lastClampedVal = 0f;

    public Animator animator;

    [SerializeField] private int speed = 30;

    //[SerializeField] private int obstacleHitSpeed = 5;

    [SerializeField]
    private float maxClamp = 5;

    [SerializeField]
    private float minClamp = -5;

    [SerializeField]
    private float senstivity = 1;

    private bool obstacleHit = false;

    Tweener tween;



    // Start is called before the first frame update
    void Start()
    {
        lastClampedVal = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Move();
            //phase = InputPhase.BEGAN;
            firstPos = Input.mousePosition;
            drag = true;
            
            //CreateEventArg(phase);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //phase = InputPhase.ENDED;
            lastClampedVal = clampedVal;
            drag = false;
            //CreateEventArg(phase, clampedVal);
        }
        if (drag)
        {
            Drag2();

        }

    }

    void Move()
    {
        if (tween != null) return;
        
        tween = transform.DOMoveZ(transform.position.z + speed, 3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (!obstacleHit)
            {
                tween = null;
                Move();
            }
                
        });

        animator.SetTrigger("startRun");

        obstacleHit = false;
        
        
    }

    public void ObstacleHitMove()
    {
        obstacleHit = true;

        tween.Kill();

        transform.DOLocalJump(transform.localPosition + Vector3.back * 3,
                2, 1, .5f).OnComplete(() =>
                {
                    tween = null;
                    Move();
                });      
    }

    public void Drag2()
    {
        //phase = InputPhase.MOVED;
        Vector2 draggingPos = Input.mousePosition;
        float magnitudeX = draggingPos.x - firstPos.x;


        if (Mathf.Abs(magnitudeX) <= 0.00001f)
        {
            firstPos = draggingPos;
            return;
        }

        bool left = magnitudeX > 0 ? false : true;
        float magnitude = (magnitudeX / Screen.width) * 1000f * Time.deltaTime * senstivity;
       
        transform.position += new Vector3(magnitude, 0, 0);

        transform.position = GetClampedValue(transform.position, minClamp, maxClamp);

        firstPos = draggingPos;
    }



    Vector3 GetClampedValue(Vector3 pos, float min1, float max1)
    {
        if (pos.x <= min1)
            pos.x = min1;
        else if (pos.x >= max1)
            pos.x = max1;

        return pos;
    }

    public void LevelEnding()
    {
        tween.Kill();
    }
    
}
