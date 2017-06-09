using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycasterScript))]
public class MovementControllerScript : MonoBehaviour {


    public int facing = 1;
    bool movedAlready;
    public bool caresAboutSlopes;
    public Vector2 amountLeftToMove;
    public Vector2 totalMovedThisFrame;
    public Vector2 baseAmountLeftToMove;
    public float maxDescendableSlope;
    public Vector2 prevVelocity;
    public Vector2 baseVelocity;
    public Vector2 targetBaseVelocity;
    public Vector2 baseVelocityDif;
    public Vector2 targetVelocity;
    public Vector2 totalBaseMoveThisFrame;
    public float descendAccelerationConstant = .5f;
    public float maxAscendableSlope;
    int count = 0;
    float smallNum = .0001f;
    public bool partialMove;
    public bool isGrounded;
    public bool isOnSlope;

    MovementManagerScript movementManager;
    public enum ControllerType {
        PLAYER,
        MOVING_PLATFORM,
        BOX
    }

    public ControllerType controllerType;
    public RaycasterScript raycasterScript;
    public Vector2 velocity;
    public PlayerScript playerScript;
    public MovingPlatformScript movingPlatformScript;
    public GameObject thingImStandingOn;
    public bool isJumping;
    public Vector2 prevBaseVelocity;
    public Vector2 velocityDif;
    
	// Use this for initialization
	void Start () {
        movementManager = FindObjectOfType<MovementManagerScript>();
        movementManager.movementControllers.Add(this);

        movedAlready = false;
        raycasterScript = GetComponent<RaycasterScript>();
        velocity = new Vector2(0, 0);
        baseVelocity = new Vector2(0, 0);
        prevBaseVelocity = baseVelocity;

        prevVelocity = velocity;
        if (controllerType == ControllerType.PLAYER)
        {
            playerScript = GetComponent<PlayerScript>();
        } else if (controllerType == ControllerType.MOVING_PLATFORM)
        {
            movingPlatformScript = GetComponent<MovingPlatformScript>();
        }
	}
	
    public void MovementUpdate()
    {

        if (caresAboutSlopes  && !isJumping)
        {
            HandleSlope();
        }
        transform.localScale = new Vector3(facing,1,1);
        totalMovedThisFrame += Move(amountLeftToMove);
        amountLeftToMove = velocity - totalMovedThisFrame;
    }

    public void MovementSetup ()
    {
        isOnSlope = false;
        raycasterScript.ResetRaycasterScript();
        totalMovedThisFrame = new Vector2();
        totalBaseMoveThisFrame = new Vector2();
        amountLeftToMove = velocity;
        baseAmountLeftToMove = baseVelocity;
        if (Mathf.Abs(velocity.x) != 0)
        {
            facing = (int)Mathf.Sign(velocity.x);
        }
    }

    public void HandleSlope()
    {
        //        Vector2 HinitialVelocity = velocity;
        if (Mathf.Abs(velocity.x) > .001){
            HandleSlopeAscent(ref amountLeftToMove);
        }
        if (Mathf.Abs(baseVelocity.x) > .001)
        {
            HandleSlopeAscent(ref baseAmountLeftToMove);
        }
    }

    public void BaseMoveUpdate()
    {

        totalBaseMoveThisFrame += Move(baseAmountLeftToMove);
        baseAmountLeftToMove = baseVelocity - totalBaseMoveThisFrame;
    }


    public void MovementCleanup()
    {
        velocity = new Vector2(totalMovedThisFrame.x, velocity.y);
        totalMovedThisFrame = new Vector2();
        if (isGrounded)
        {
            Vector2 downVec = new Vector2(0, -1);
            Move(downVec);
        }
        movedAlready = false;
        isJumping = false;
        prevVelocity = velocity;
        prevBaseVelocity = baseVelocity;
        baseVelocity = new Vector2();
        raycasterScript.newCollisionsThisFrame = new List<GameObject>();
    }


    public Vector2 Move(Vector2 delta)
    {
            if (delta.x != 0)
            {
                raycasterScript.HorizontalCollisions(ref delta);
            }

            if (delta.y != 0)
            {
                raycasterScript.VerticalCollisions(ref delta);
            }

        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) > .001)
        {
            movementManager.somethingMoved = true;
        }
            transform.Translate(delta);
            return delta;        
    }

    private void Update()
    {
        thingImStandingOn = null;
    }

    void HandleSlopeAscent(ref Vector2 delta)
    {
        float directionX = Mathf.Sign(delta.x);
        Vector2 rayOrigin = (directionX == 1) ? raycasterScript.raycastOrigins.bottomRight : raycasterScript.raycastOrigins.bottomLeft;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, Mathf.Infinity, raycasterScript.collisionLayer);

        RaycastHit2D hit = new RaycastHit2D();
        float checkDistance = Mathf.Infinity;
        bool goodHit = false;

        foreach (RaycastHit2D testHit in hits)
        {
            if (!raycasterScript.gameObjectsIshouldIgnore.Contains(testHit.transform.gameObject)
                && hit.distance < checkDistance)
            {
                hit = testHit;
                checkDistance = hit.distance;
                goodHit = true;
            }
        }

        if (goodHit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            
            if (slopeAngle != 0 && slopeAngle <= maxAscendableSlope)
            {
                if (Mathf.Sign(hit.normal.x) != directionX)
                {
                    if (hit.distance - raycasterScript.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(delta.x))
                    {
                    //    float slopinessFactor = slopeAngle / maxDescendableSlope;
                      //  Debug.Log("slopinessFactor = " + slopinessFactor);
                //        float multiplier = 1 + slopinessFactor * descendAccelerationConstant;
                  //      delta *= multiplier;
                        float moveDistance = Mathf.Abs(delta.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        //            delta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(delta.x);
                        //          delta.y += descendVelocityY;
                        Vector2 newDelta = new Vector2(delta.x * Mathf.Cos(slopeAngle * Mathf.Deg2Rad), delta.x * Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(delta.x));
                            delta.x = newDelta.x;
                        if (delta.y <= newDelta.y)
                        {
                            isGrounded = true;
                            delta.y = newDelta.y;
                            isOnSlope = true;
                            raycasterScript.collisionState.descendingSlopeAngle = slopeAngle;
                        }

                        //                raycasterScript.collisionState.ascendingSlopeAngle = slopeAngle;
                        //              raycasterScript.collisionState.descendingSlope = true;
                    }
                }
            }
        }
    }

    /*
        if (caresAboutSlopes)
        {
            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
        else
        {
            //if we don't care about slopes we can just make the angle infinity then we never look at slope code
            slopeAngle = Mathf.Infinity;
        }*/

        /*
        //Handle Slope Ascending for vertical hit
        if (delta.x != 0 && caresAboutSlopes)
        {
            if (collisionState.climbingSlope())
            {
                float theta;
                if (delta.x < 0)
                {
                    theta = collisionState.slopeAngleLeft;
                }
                else
                {
                    theta = collisionState.slopeAngleRight;
                }
                if (theta != 0)
                {
                    delta.x = delta.y / Mathf.Tan(theta * Mathf.Deg2Rad) * Mathf.Sign(delta.x);
                    //                                movementControllerScript.velocity.x = delta.x;
                }
            }
        }
        ///End slope ascending code
        */
    

    void HandleSlopeDescent(ref Vector2 delta)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycasterScript.raycastOrigins.bottomRight : raycasterScript.raycastOrigins.bottomLeft;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, Mathf.Infinity, raycasterScript.collisionLayer);

        RaycastHit2D hit = new RaycastHit2D();
        float checkDistance = Mathf.Infinity;
        bool goodHit = false;

        foreach (RaycastHit2D testHit in hits)
        {
            if (!raycasterScript.gameObjectsIshouldIgnore.Contains(testHit.transform.gameObject)
                && hit.distance < checkDistance)
            {
                hit = testHit;
                checkDistance = hit.distance;
                goodHit = true;
            }
        }

        if (goodHit)
        {   
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendableSlope)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - raycasterScript.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(delta.x))
                    {
                        float slopinessFactor = slopeAngle/maxDescendableSlope;
                        float multiplier = 1 + slopinessFactor*descendAccelerationConstant;
                        delta *= multiplier;
                        float moveDistance = Mathf.Abs(delta.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        delta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(delta.x);
                        delta.y -= descendVelocityY;
//                        isGrounded = true;

                        raycasterScript.collisionState.descendingSlopeAngle = slopeAngle;
                        raycasterScript.collisionState.descendingSlope = true;
                    }
                }
            }
        }
    }

}
