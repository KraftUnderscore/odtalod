using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float crouchSlowdown;
    [SerializeField]
    private float sprintIncrease;
    [SerializeField]
    private Transform grabPoint;
    [SerializeField]
    private float grabRadius;
    [SerializeField]
    private float waterRadius;
    [SerializeField]
    private float feedRadius;
    [SerializeField]
    private AudioClip steelDeer;
    [SerializeField]
    private AudioClip water;
    [SerializeField]
    private AudioClip feed;
    [SerializeField]
    private AudioSource walking;
    [SerializeField]
    private AudioSource actions;

    private new Rigidbody2D rigidbody;
    private Animator animator;

    private DeerController grabbedDeer;
    private bool isGrabbing = false;
    private bool grabCheck = true;
    private bool isInFences = false;

    // Start is called before the first frame update
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Feeding();
        Watering();
        Throwing();
        Grabbing();
        GrabChecker();
        Movement();
    }

    private void Feeding()
    {
        if (isGrabbing) return;
        if(Input.GetButtonDown("Feed"))
        {
            animator.SetTrigger("feed");
            actions.clip = feed;
            actions.Play();
            Collider2D[] deers = Physics2D.OverlapCircleAll(grabPoint.position, feedRadius, 1 << LayerMask.NameToLayer("Deers"));
            foreach(Collider2D d in deers)
            {
                DeerController dc = d.GetComponent<DeerController>();
                if (dc != null) dc.Feed();
            }
        }
    }

    private void Watering()
    {
        if (isGrabbing) return;
        if(Input.GetButtonDown("Water"))
        {
            animator.SetTrigger("water");
            actions.clip = water;
            actions.Play();
            Collider2D[] deers = Physics2D.OverlapCircleAll(grabPoint.position, waterRadius, 1 << LayerMask.NameToLayer("Deers"));
            foreach (Collider2D d in deers)
            {
                DeerController dc = d.GetComponent<DeerController>();
                if (dc != null) dc.Water();
            }
        }
    }

    private void Grabbing()
    {
        if (isGrabbing) return;
        if (!grabCheck) return;
        if(Input.GetButtonDown("Grab"))
        {
            Collider2D deer = Physics2D.OverlapCircle(grabPoint.position, grabRadius, 1 << LayerMask.NameToLayer("Deers"));
            if (deer == null) return;
            
            grabbedDeer = deer.GetComponent<DeerController>();
            grabbedDeer.Grab(grabPoint);
            isGrabbing = true;
            animator.SetTrigger("grab");
            grabCheck = false;
            actions.clip = steelDeer;
            actions.Play();
        }
    }

    private void Throwing()
    {
        if (!isGrabbing) return;
        if (!grabCheck) return;
        if (Input.GetButtonDown("Grab"))
        {
            isGrabbing = false;
            animator.SetTrigger("yeet");
            grabbedDeer.Release(isInFences);
            grabbedDeer = null;
            grabCheck = false;
        }
    }

    private void GrabChecker()
    {
        if (Input.GetButtonUp("Grab")) 
            grabCheck = true;
    }

    private void Movement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        float speedModifier = Input.GetButton("Crouch") ? crouchSlowdown : 1f;
        speedModifier = Input.GetButton("Sprint") ? sprintIncrease : speedModifier;
        animator.SetFloat("speedMod", speedModifier);

        rigidbody.velocity = new Vector2(moveX * speed * speedModifier, moveY * speed * speedModifier);
        animator.SetFloat("walkSpeed", Mathf.Abs(rigidbody.velocity.magnitude));
        walking.volume = Mathf.Clamp01(rigidbody.velocity.magnitude * 0.025f);
        if (moveX < 0f) transform.localScale = new Vector3(-1f, 1f, 1f);
        else transform.localScale = Vector3.one;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Fences"))
        {
            isInFences = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Fences"))
        {
            isInFences = false;
        }
    }
}
