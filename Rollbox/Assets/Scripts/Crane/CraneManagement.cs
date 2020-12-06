using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneManagement : MonoBehaviour
{
    public static CraneManagement main;

    private void Awake()
    {
        main = this;
    }


    [Tooltip("If disabled, uses mouse/touchscreen sliding controls to move crane.")]
    public bool usesKeyboardControl;

    Vector3 inputPosition;
    [Space]
    [Header("Claw Management")]

    
    public float distanceToChainEnd = .6f; //Distance to the hinge joint.
    Rigidbody2D clawRB;
    [Space]

    [Tooltip("The world space x positions (left/right) that the crane position can be clamped to")]
    public Vector2 craneXBounds;
    [Tooltip("The speed at which the rope snaps to mouseX. 1 = Perfect Snapping, 0 = No Snapping")]
    [Range(0,1)]
    public float craneLerpSpeed;
    [Tooltip("The maximum speed that the crane arm can move in one frame.")]
    public float craneMaxChange;
    [Space]
    public Transform leftClawTransform;
    public Transform rightClawTransform;
    public float maxOpenAmount;
    float currentOpenAmount;


    [Space]
    public Rigidbody2D ropeAnchor;

    float targetCraneX;
    [Space]
    [Header("Crate Management")]
    public Transform crateHolder;
    List<CrateInfo> allDroppedCrates;

    public GameObject crateObj;
    public float crateYOffset;
    [Space]
    public float respawnCrateTime;
    float currentSpawnTime;
    [Space]
    public bool holdingCrate;
    public Transform heldCrate;


    //Input
    bool holdingDownInput;
    bool brokeACrate;
    bool shouldOpenClaws;

    private void Start()
    {
        clawRB = GetComponent<Rigidbody2D>();
        allDroppedCrates = new List<CrateInfo>();
    }

    private void Update()
    {
        holdingDownInput = Input.GetMouseButton(0) || Input.touchCount > 0;

        bool brokeACrate = false;
        TryCrateBreaks(out brokeACrate);
        if (Input.GetMouseButtonDown(0) && !brokeACrate)
        {
            shouldOpenClaws = true;
            DropCrate();
        }

        if (Input.GetMouseButtonUp(0))
        {
            shouldOpenClaws = false;
        }

        UpdateClawPosition();
        UpdateCrateSpawning();
    }

    private void FixedUpdate()
    {
        UpdateRopePosition();
    }

    //Attaches the crane claw to the rope (Called from RopeCreator.cs)
    public void AttachToRopeEnd(Rigidbody2D endRB)
    {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;

        joint.connectedBody = endRB;

        joint.anchor = Vector2.zero;
        joint.useLimits = true;
        JointAngleLimits2D angleLimits = joint.limits;
        angleLimits.min = -20;
        angleLimits.max = 20;
        joint.limits = angleLimits;
        joint.connectedAnchor = new Vector2(0f, -distanceToChainEnd);
    }


    public void TrySpawnCrate()
    {
        if (holdingCrate || holdingDownInput)
        {
            return;
        }

        holdingCrate = true;

        GameObject crate = Instantiate(crateObj, transform);
        heldCrate = crate.transform;

        FixedJoint2D fixedJoint = crate.AddComponent<FixedJoint2D>();
        fixedJoint.autoConfigureConnectedAnchor = false;

        fixedJoint.anchor = Vector2.zero;
        fixedJoint.connectedAnchor = new Vector2(0, -1.15f);

        fixedJoint.connectedBody = clawRB;
    }

    public void DropCrate()
    {
        if (holdingCrate)
        {
            CurrentLevelManager.main.AddCrate();

            holdingCrate = false;
            Destroy(heldCrate.GetComponent<FixedJoint2D>());
            heldCrate.parent = crateHolder;
            allDroppedCrates.Add(heldCrate.GetComponent<CrateInfo>());
            currentSpawnTime = respawnCrateTime;
        }
    }


    public void UpdateRopePosition()
    {
        inputPosition = inputPosition.GetInputPosition();

        targetCraneX = Mathf.Clamp(inputPosition.x, craneXBounds.x, craneXBounds.y);
        float currentCraneX = ropeAnchor.position.x;
        float changeX = Mathf.Lerp(currentCraneX, targetCraneX, craneLerpSpeed) - currentCraneX;
        changeX = Mathf.Clamp(changeX, -craneMaxChange, craneMaxChange);

        Vector3 rbPosition = ropeAnchor.position;
        float newCraneX = currentCraneX + changeX;

        rbPosition.x = newCraneX;
        ropeAnchor.MovePosition(rbPosition);
    }

    void UpdateClawPosition()
    {
        if (holdingDownInput && shouldOpenClaws)
        {
            currentOpenAmount = Mathf.Lerp(currentOpenAmount, maxOpenAmount, .1f);
        }
        else
        {
            currentOpenAmount = Mathf.Lerp(currentOpenAmount, 0, .05f);
        }

        leftClawTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -currentOpenAmount));
        rightClawTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, currentOpenAmount));
    }

    void UpdateCrateSpawning()
    {
        if (currentSpawnTime > 0)
        {
            currentSpawnTime -= Time.deltaTime;
        }
        else
        {
            TrySpawnCrate();
        }
    }

    //If clicked in a position to break a crate, then don't drop current crate
    void TryCrateBreaks(out bool brokeACrate)
    {
        bool pressed = false;
        Vector3 touchPos = Vector3.zero;
        ExtensionMethods.DetectTouches(out pressed, out touchPos);

        bool brokeCrate = false;

        if (pressed && allDroppedCrates.Count > 0)
        {
            for (int i = 0; i < allDroppedCrates.Count; i++)
            {
                if (ExtensionMethods.TouchedHitbox(allDroppedCrates[i].touchHitbox, touchPos))
                {
                    brokeCrate = true;
                    Destroy(allDroppedCrates[i].gameObject);
                    allDroppedCrates.Remove(allDroppedCrates[i]);
                    break;

                }
            }
        }

        brokeACrate = brokeCrate;
    }

    public void DeleteCrate(CrateInfo crate)
    {
        allDroppedCrates.Remove(crate);
        Destroy(crate.gameObject);
    }

    public void ClearAllCrates()
    {
        foreach (Transform child in crateHolder)
        {
            allDroppedCrates = new List<CrateInfo>();
            Destroy(child.gameObject);
        }
    }
}
