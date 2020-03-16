using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class DeerController : MonoBehaviour
{
    [SerializeField]
    private bool isBaby = false;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float walkRadius;
    [SerializeField]
    private Vector2 waitTimeMinMax;
    [SerializeField]
    private Vector2 breedTimeMinMax;
    [SerializeField]
    private GameObject babyPrefab;

    private Animator animator;

    private Vector2 startPos;
    private Vector2 nextPos;
    private float proximity = 0.05f;
    private float startingWalkRadius;

    private float waitTimer;
    private float growingTimer = 30f;
    private float makingBBTimer;

    private bool isInFences;
    private bool isFed;
    private bool isWatered;

    private TextMeshProUGUI timerText;
    private GameObject waterNeed;
    private GameObject foodNeed;

    private Transform grabPoint;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        timerText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        waterNeed = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        foodNeed = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;

        timerText.SetText("");
        waterNeed.SetActive(false);
        foodNeed.SetActive(false);

        startingWalkRadius = walkRadius;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        nextPos = startPos;
        animator.SetBool("isBaby", isBaby);
        makingBBTimer = Random.Range(breedTimeMinMax.x, breedTimeMinMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Deer: fence {isInFences}, fed: {isFed}, water: {isWatered}, adult: {!isBaby}");
        if(isInFences)
        {
            if (!isFed || !isWatered)
            {
                foodNeed.SetActive(!isFed);
                waterNeed.SetActive(!isWatered);
                timerText.SetText("");
                return;
            }
            else
            {
                foodNeed.SetActive(false);
                waterNeed.SetActive(false);
            }
            if (isBaby)
            {
                if (growingTimer > 0)
                {
                    growingTimer -= Time.deltaTime;
                    timerText.SetText(growingTimer.ToString("F0")); ;
                }
                else
                {
                    isBaby = false;
                    animator.SetBool("isBaby", false);
                    isFed = false;
                    isWatered = false;
                }
                
            }
            else
            {
                if (makingBBTimer > 0)
                {
                    makingBBTimer -= Time.deltaTime;
                    timerText.SetText(makingBBTimer.ToString("F0")); ;
                }
                else
                {
                    makingBBTimer = Random.Range(breedTimeMinMax.x, breedTimeMinMax.y);
                    isFed = false;
                    isWatered = false;
                    MakeBaby();
                }
            }
            if (transform.localScale.x < 0) timerText.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            else timerText.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        if(grabPoint != null)
        {
            transform.position = grabPoint.position;
            return;
        }

        if(waitTimer>0)
        {
            animator.SetFloat("speed", 0f);
            waitTimer -= Time.deltaTime;
            return;
        }

        if (Mathf.Abs((new Vector2(transform.position.x, transform.position.y) - nextPos).magnitude) < proximity)
        {
            waitTimer = Random.Range(waitTimeMinMax.x, waitTimeMinMax.y);
            nextPos = new Vector2(Random.Range(startPos.x - walkRadius, startPos.x + walkRadius), Random.Range(startPos.y - walkRadius, startPos.y + walkRadius));
            if (transform.position.x - nextPos.x < 0) transform.localScale = new Vector3(-1f, 1f, 1f);
            else transform.localScale = Vector3.one;
        }
        animator.SetFloat("speed", speed);
        transform.position = Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    public void Grab(Transform grabPoint)
    {
        animator.SetFloat("speed", 0f);
        timerText.SetText("");
        this.grabPoint = grabPoint;
        isInFences = false;
        waterNeed.SetActive(false);
        foodNeed.SetActive(false);
    }

    public void Release(bool isInFences)
    {
        if (!this.isInFences && isInFences)
            GameManager.instance.IncreaseScore();
        else if (this.isInFences && !isInFences)
            GameManager.instance.DecreaseScore();

        this.isInFences = isInFences;
        if(isInFences)
        {
            nextPos = new Vector2(3.75f, 0f);
            startPos = nextPos;
            walkRadius = startingWalkRadius*1.5f;
        }
        else
        {
            nextPos = grabPoint.position;
            startPos = nextPos;
            walkRadius = startingWalkRadius;
        }

        grabPoint = null;
        transform.position = nextPos;
        if(isInFences)
        {
            waterNeed.SetActive(!isWatered);
            foodNeed.SetActive(!isFed);
        }
    }

    private void MakeBaby()
    {
        GameObject newDeer = Instantiate(babyPrefab);
        DeerController dc = newDeer.GetComponent<DeerController>();
        dc.SetBaby();
        GameManager.instance.IncreaseScore();
    }

    public void Feed()
    {
        if (isInFences) isFed = true;
    }

    public void Water()
    {
        if (isInFences) isWatered = true;
    }

    public void SetBaby()
    {
        isBaby = true;
        animator.SetBool("isBaby", isBaby);
        isInFences = true;
        nextPos = new Vector2(3.75f, 0f);
        startPos = nextPos;
        walkRadius = startingWalkRadius * 1.5f;
    }
}
