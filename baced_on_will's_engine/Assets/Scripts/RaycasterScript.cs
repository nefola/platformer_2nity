using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:  Descending Slopes: 4:26

[RequireComponent(typeof(BoxCollider2D))]
public class RaycasterScript : MonoBehaviour
{
    public LayerMask collisionLayer;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public float horizontalRaySpacing;
    public float verticalRaySpacing;
    public CollisionState collisionState;
    public CollisionState prevCollisionState;
    new BoxCollider2D collider;
    public float skinWidth = .015f;
    public RaycastOrigins raycastOrigins;
    public float maxClimbableSlope;
    public float maxDescendableSlope;
    Vector2 originalDelta;
    public List<GameObject> gameObjectsIshouldIgnore;
    public List<GameObject> newCollisionsThisFrame;
    MovementControllerScript movementControllerScript;


    public bool caresAboutSlopes;


    public struct CollisionState
    {
        public GameObject[] topHits;
        public GameObject[] bottomHits;
        public GameObject[] leftHits;
        public GameObject[] rightHits;
        int horizontalRayCount, verticalRayCount;
        public float slopeAngleRight;
        public float slopeAngleLeft;
        public bool descendingSlope;
        public float descendingSlopeAngle;

        public bool climbingSlope()

        {
            if (descendingSlope)
            {
                return false;
            }

            return (slopeAngleLeft > 0 || slopeAngleRight > 0);
        }

        public List<GameObject> getAllCollisions()
        {
            List<GameObject> allCollisions = new List<GameObject>();
            for (int i = 0; i < horizontalRayCount; i++)
            {
                if (leftHits[i] != null && allCollisions.Contains(leftHits[i]) == false)
                {
                    allCollisions.Add(leftHits[i]);
                }
                if (rightHits[i] != null && allCollisions.Contains(rightHits[i]) == false)
                {
                    allCollisions.Add(rightHits[i]);
                }
            }
            for (int i = 0; i < verticalRayCount; i++)
            {
                if (topHits[i] != null && allCollisions.Contains(topHits[i]) == false)
                {
                    allCollisions.Add(topHits[i]);
                }
                if (bottomHits[i] != null && allCollisions.Contains(bottomHits[i]) == false)
                {
                    allCollisions.Add(bottomHits[i]);
                }
            }

            return allCollisions;
        }


        public CollisionState(int horizontalRayCount, int verticalRayCount)
        {
            this.horizontalRayCount = horizontalRayCount;
            this.verticalRayCount = verticalRayCount;
            this.topHits = new GameObject[verticalRayCount];
            this.bottomHits = new GameObject[verticalRayCount];
            this.leftHits = new GameObject[horizontalRayCount];
            this.rightHits = new GameObject[horizontalRayCount];
            this.slopeAngleRight = 0;
            this.slopeAngleLeft = 0;
            this.descendingSlope = false;
            this.descendingSlopeAngle = 0;
        }

        public void reset()
        {
            this.topHits = new GameObject[verticalRayCount];
            this.bottomHits = new GameObject[verticalRayCount];
            this.leftHits = new GameObject[horizontalRayCount];
            this.rightHits = new GameObject[horizontalRayCount];
            this.slopeAngleRight = 0;
            this.slopeAngleLeft = 0;
            this.descendingSlope = false;
            this.descendingSlopeAngle = 0;
        }

        public bool anyRightHits()
        {
            for (int i = 0; i < rightHits.Length; i++)
            {
                if (rightHits[i] != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool anyLeftHits()
        {
            for (int i = 0; i < rightHits.Length; i++)
            {
                if (leftHits[i] != null)
                {
                    return true;
                }
            }
            return false;
        }


        public bool anyTopHits()
        {
            for (int i = 0; i < topHits.Length; i++)
            {
                if (topHits[i] != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool anyBottomHits()
        {
            for (int i = 0; i < bottomHits.Length; i++)
            {
                if (bottomHits[i] != null)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

        public Vector2 getCorner(int x, int y)
        {

            if (x == -1 && y == -1)
            {
                return bottomLeft;
            }
            else if (x == 1 && y == -1)
            {
                return bottomRight;
            }
            else if (x == -1 && y == 1)
            {
                return topLeft;
            }
            else if (x == 1 && y == 1)
            {
                return topRight;
            }
            else
            {
                Debug.Log("INVALID CORNER!!!  " + x + ", " + y);
                return new Vector2(0, 0);
            };
        }
    }

    Bounds getBounds()
    {
        Bounds b = collider.bounds;
        b.Expand(skinWidth * -2);
        return b;
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = getBounds();
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = getBounds();
        raycastOrigins.bottomLeft = bounds.min;
        raycastOrigins.topRight = bounds.max;
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y); ;
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y); ;
    }

    // Use this for initialization
    void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
        newCollisionsThisFrame = new List<GameObject>();
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        collisionState = new CollisionState(horizontalRayCount, verticalRayCount);
        gameObjectsIshouldIgnore = new List<GameObject>();
        gameObjectsIshouldIgnore.Add(gameObject);
    }

    public void HorizontalCollisions(ref Vector2 delta, bool ignoreSlopes)
    {
     
            HorizontalCollisions(ref delta);
        
    }

    public void HorizontalCollisions(ref Vector2 delta)
    {
        originalDelta = delta;
        UpdateRaycastOrigins();
        int directionX = (int)Mathf.Sign(delta.x);
        float rayLength = Mathf.Abs(delta.x) + skinWidth;
        findRaycastHits(new Vector2(directionX, 0), ref delta);
    }

    private void LateUpdate()
    {

    }

    public void ResetRaycasterScript()
    {
        prevCollisionState = collisionState;
        collisionState.reset();
        gameObjectsIshouldIgnore = new List<GameObject>();
        gameObjectsIshouldIgnore.Add(gameObject);
    }
    void findRaycastHits(Vector2 direction, ref Vector2 delta) { 
        findRaycastHits(direction, ref delta, false);
    }

    void findRaycastHits(Vector2 direction, ref Vector2 delta, bool shouldHitMultipleDistances)
    {
        int cornerX = (int)direction.x;
        int cornerY = (int)direction.y;
        int rayCount;
        float distance = skinWidth;
        Vector2 perpendicularDirection;
        float raySpacing;
        float extraDelta = 0;

        if (direction.x == 0) // meaning direction is vertical
        {
            cornerX = -1;
            rayCount = verticalRayCount;
            raySpacing = verticalRaySpacing;
            distance += Mathf.Abs(delta.y);
            perpendicularDirection = Vector2.right;
            extraDelta = delta.x;
        }
        else //direction is horizontal
        {
            cornerY = -1;
            rayCount = horizontalRayCount;
            raySpacing = horizontalRaySpacing;
            distance += Mathf.Abs(delta.x);
            perpendicularDirection = Vector2.up;
            extraDelta = 0;
        }

        for (int i = rayCount - 1; i >= 0; i--)
        {
            Vector2 rayOrigin = raycastOrigins.getCorner(cornerX, cornerY);
            rayOrigin += perpendicularDirection * (raySpacing * i + extraDelta);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, distance, collisionLayer);
            RaycastHit2D hit = new RaycastHit2D();
            hit.distance = Mathf.Infinity;

            bool goodHit = false;

            foreach (RaycastHit2D checkHit in hits)
            {
                if (!gameObjectsIshouldIgnore.Contains(checkHit.transform.gameObject))
                {
                    if (checkHit.distance < hit.distance)
                    {
                        hit = checkHit;
                        goodHit = true;
                    }
                }
            }

            if (goodHit)
            {
                if (!shouldHitMultipleDistances)
                {
                    distance = hit.distance;
                }
                if (direction.x == 0) // IS VERTICAL HIT
                {
                    delta.y = (hit.distance - skinWidth) * direction.y;
                    if (direction.y > 0)
                    {
                        collisionState.topHits[i] = hit.transform.gameObject;
                    }
                    else
                    {
                        collisionState.bottomHits[i] = hit.transform.gameObject;
                    }
                }//END VERTICAL HIT
                else  //START HORIZONTAL HIT
                {
                    if (direction.x > 0)
                    {
                        collisionState.rightHits[i] = hit.transform.gameObject;
                    }
                    else
                    {
                        collisionState.leftHits[i] = hit.transform.gameObject;
                    }



                    delta.x = (hit.distance - skinWidth) * direction.x;

                    if (i == 0)
                    {
                        Debug.DrawRay(rayOrigin, direction * distance, Color.blue);
                    }
                }
            }
        }
    }

    public void VerticalCollisions(ref Vector2 delta)
    {
        VerticalCollisions(ref delta, false);

    }

    public void VerticalCollisions(ref Vector2 delta, bool multipleDistances)
    {
        UpdateRaycastOrigins();
        int directionY = (int)Mathf.Sign(delta.y);
        float rayLength = Mathf.Abs(delta.y) + skinWidth;
        findRaycastHits(new Vector2(0, directionY), ref delta, multipleDistances);
    }

    void HandleSlopeClimb(ref Vector2 delta, float slopeAngle)
    {
        Vector2 newDelta = new Vector2(delta.x * Mathf.Cos(slopeAngle * Mathf.Deg2Rad), delta.x * Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(delta.x));
        delta.x = newDelta.x;

        if (originalDelta.y <= newDelta.y)
        {
            delta.y = newDelta.y;
        }
        Vector2 verticalCheck = new Vector2(0, skinWidth * -2f);
        VerticalCollisions(ref verticalCheck);
    }
}