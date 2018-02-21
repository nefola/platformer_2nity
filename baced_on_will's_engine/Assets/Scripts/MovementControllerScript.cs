using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementControllerScript : MonoBehaviour {

    public LayerMask collisionLayer;
    List<GameObject> gameObjectsToIgnoreCollisonsWith;
    new BoxCollider2D collider;
    public CollisionState collisionState;
    public int horizontalRayCount;
    public int verticalRayCount;
    public bool ascendingSlope = false;
    public bool decendingSlope = false;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    public float skinWidth;
    public float maxAscentAngle = 80;
    public List<GameObject> attachedObjects;
    public List<GameObject> passengers;

    public class CollisionState
    {
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
        public Vector2 topLeft;
        public Vector2 topRight;

        public List<RaycastHit2D> leftHits;
        public List<RaycastHit2D> topHits;
        public List<RaycastHit2D> bottomHits;
        public List<RaycastHit2D> rightHits;

       public void resetCollisions()
        {
            leftHits = new List<RaycastHit2D>();
            topHits = new List<RaycastHit2D>();
            bottomHits = new List<RaycastHit2D>();
            rightHits = new List<RaycastHit2D>();
        }
        public Vector2 getCorner(int x, int y)
        {
            if (x == -1 && y == -1)
            {
                return bottomLeft;
            }
            else if (x == -1 && y == 1)
            {
                return topLeft;
            }
            else if (x == 1 && y == -1)
            {
                return bottomRight;
            }
            else if (x == 1 && y == 1)
            {
                return topRight;
            }else
            {
                Debug.Log("INVALID CORNER");
                return new Vector2(Mathf.Infinity, Mathf.Infinity);                
            }
        }
    }

    private void Update()
    {
        collisionState.resetCollisions();
    }

    private void LateUpdate()
    {
    //    attachedObjects = new List<GameObject>();
    }

    // Use this for initialization
    void Start () {
        gameObjectsToIgnoreCollisonsWith = new List<GameObject>();
        gameObjectsToIgnoreCollisonsWith.Add(gameObject);
        attachedObjects = new List<GameObject>();
        this.collisionState = new CollisionState();
        collider = GetComponent<BoxCollider2D>();
        calculateRaySpacing();
        UpdateCollisionState();
	}

    void calculateRaySpacing()
    {
        horizontalRaySpacing = ((collider.bounds.extents.y) * 2) / (horizontalRayCount-1);
        verticalRaySpacing = ((collider.bounds.extents.x) * 2) / (verticalRayCount-1);
    }

    void UpdateCollisionState()
    {
        collisionState.bottomLeft = collider.bounds.min;
        collisionState.topRight = collider.bounds.max;
        collisionState.bottomRight = new Vector2(collisionState.topRight.x, collisionState.bottomLeft.y);
        collisionState.topLeft = new Vector2(collisionState.bottomLeft.x, collisionState.topRight.y);
    }

    public List<GameObject> FindPassengers()
    {
        List<RaycastHit2D> hits = VerticalRaycastHits(.001f);
        passengers = new List<GameObject>();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform != null)
            {
                if (!passengers.Contains(hit.transform.gameObject))
                {
                    passengers.Add(hit.transform.gameObject);
                }
            }
        }
        return passengers;
    }

    List<RaycastHit2D> FindRaycastHits(Vector2 direction, float distance)
    {
      //UpdateCollisionState();
        if (direction.y == 0)
        {

            return HorizontalRaycastHits(Mathf.Sign(direction.x)*distance);
        }
        else if (direction.x == 0)
        {
            return VerticalRaycastHits(Mathf.Sign(direction.y) * distance);
        }
        else
        {
            //can only do 1 dimension at a time
            Debug.Log("Invalid direction");
            return null;
        }
    }

    public List<RaycastHit2D> HorizontalRaycastHits(float distance)
    {
        if (distance > 0)
        {
            collisionState.bottomRight = new Vector2(collider.bounds.max.x, collider.bounds.min.y);
        }else if (distance < 0)
        {
            collisionState.bottomLeft = collider.bounds.min;
        }
        else { 
            return null;
        }
            int direction = (int)Mathf.Sign(distance);
            List<RaycastHit2D> horizontalHits = new List<RaycastHit2D>(horizontalRayCount);
            Vector2 corner = collisionState.getCorner(direction, -1);
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(corner.x+skinWidth*direction*-1, horizontalRaySpacing * i + corner.y);
                if (i == 0)
                {
                    rayOrigin.y += skinWidth;
                }
                if (i == horizontalRayCount - 1)
                {
                    rayOrigin.y -= skinWidth;
                 }
            horizontalHits.Add(ObstacleRaycast(rayOrigin, new Vector2(direction, 0), Mathf.Abs(distance)+skinWidth));

        }
        return horizontalHits;
    }


    public List<RaycastHit2D> VerticalRaycastHits(float distance)
    {
        if (distance < 0)
        {
            collisionState.bottomLeft = collider.bounds.min;
        }else if (distance > 0)
        {
            collisionState.topLeft = new Vector2(collider.bounds.min.x, collider.bounds.max .y);
        }
        else
        {
            return null;
        }
        
            int direction = (int) Mathf.Sign(distance);
            List<RaycastHit2D> verticalHits = new List<RaycastHit2D>(verticalRayCount);
            Vector2 corner = collisionState.getCorner(-1, direction);

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(corner.x + verticalRaySpacing * i, corner.y);
                if (i == 0)
                {
                    rayOrigin.x += skinWidth;
                }
                if (i == verticalRayCount - 1)
                {
                    rayOrigin.x -= skinWidth;
                }
           
                verticalHits.Add(ObstacleRaycast(rayOrigin, new Vector2(0,distance), Mathf.Abs(distance)));
            }
            return verticalHits;
        
    }

    RaycastHit2D ObstacleRaycast(Vector2 origin, Vector2 direction, float distance) {
        RaycastHit2D goodHit = new RaycastHit2D();
        RaycastHit2D[] allHits = Physics2D.RaycastAll(origin, direction * distance, collisionLayer);
        Debug.DrawRay(origin, direction * distance, Color.red);
        float checkDistance = distance;
        foreach (RaycastHit2D hit in allHits)
        {
            if (hit.transform != null)
            {
                if (hit.distance <= checkDistance && !gameObjectsToIgnoreCollisonsWith.Contains(hit.transform.gameObject))
                {
                    if (hit.transform != transform)
                    {
                        goodHit = hit;
                        checkDistance = hit.distance;
                    }
                }
            }
        }
        return goodHit;
    }
    float getSlopeUpAngle(float facing)
    {
       // List<RaycastHit2D> bottomHits = VerticalRaycastHits(10);
        if (facing == -1)
        {
            List<RaycastHit2D> leftHits = HorizontalRaycastHits(-.01f);
            RaycastHit2D bottomLeftHit = leftHits[0];
            if (bottomLeftHit.transform != null)
            {
               
                float slopeAngle = Vector2.Angle(bottomLeftHit.normal, Vector2.up);
                return slopeAngle;
                
            }
            else
            {

            }
        }
        else if (facing == 1)
        {
            List<RaycastHit2D> rightHits = HorizontalRaycastHits(.01f);
            RaycastHit2D bottomRightHit = rightHits[0];
            if (bottomRightHit.transform != null)
            {
                
           
                float slopeAngle = Vector2.Angle(bottomRightHit.normal, Vector2.up);
                return slopeAngle;
                
            }
            else
            {

            }
        }

            return 0;
    }
    float getSlopeDownAngle(float facing)
    {
        if(facing == -1)
        {
            List<RaycastHit2D> bottomHits = VerticalRaycastHits(-0.01f);
            RaycastHit2D bottomrightHit = bottomHits[0];
            if (bottomrightHit.transform != null)
            {
                float slopeAngle = Vector2.Angle(Vector2.left, bottomrightHit.normal);
                return slopeAngle;
            }
        } else if(facing == 1)
        {
            List<RaycastHit2D> bottomHits = VerticalRaycastHits(-0.01f);
            RaycastHit2D bottomLeftHit = bottomHits[0];
            if (bottomLeftHit.transform != null)
            {
                float slopeAngle = Vector2.Angle(Vector2.right, bottomLeftHit.normal);
                return slopeAngle;
            }
        }
        return 0;
    }

    public Vector2 Move(Vector2 delta)
    {
        Vector2 amountMoved;
        if (attachedObjects.Count > 0)
        {

            amountMoved = MoveWithGameObjects(delta, attachedObjects);
        }
        else
        {

            //Move attempts to move in the x dimension and then the y
            //it only moves us as far as we can move, and returns however far that was

            Vector2 newDelta = new Vector2(delta.x, delta.y);
            if (delta.x != 0)
            {
                float slopeAngle = getSlopeUpAngle(Mathf.Sign(delta.x));
                if (slopeAngle < maxAscentAngle)
                {
                    float slopeY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(delta.x);
                    Move(new Vector2(0, slopeY));
                    Debug.Log("up is" + slopeAngle);
                    ascendingSlope = true;
                }

                else
                {
                    ascendingSlope = false;
                }
                
               
                UpdateCollisionState();
                newDelta.x = CalculateMoveX(delta);
            }
            transform.Translate(new Vector2(newDelta.x,0));
            if (delta.y != 0)
            {
                float slopeAngle = getSlopeDownAngle(Mathf.Tan(delta.y));
                if (delta.y != 0)
                {
                    float slopeX = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (delta.y);
                    Move(new Vector2(-slopeX, 0));
                    Debug.Log("down is" + slopeAngle);
                    decendingSlope = true;
                }
                else
                { 
                    decendingSlope = false;
                }
                UpdateCollisionState();
                newDelta.y = CalculateMoveY(delta);
            }
            amountMoved = newDelta;
            transform.Translate(new Vector2(0,newDelta.y));
        }

        foreach (GameObject passenger in passengers)
        {
            MovementControllerScript mcs = passenger.GetComponent<MovementControllerScript>();
            if (mcs != null)
            {
                mcs.Move(amountMoved);
            }
        }
        return amountMoved;
    }

    public float CalculateMoveX(Vector2 delta)
    {
        float direction = Mathf.Sign(delta.x);
        List<RaycastHit2D> hits = HorizontalRaycastHits(delta.x);
        if (direction < 0)
        {
            collisionState.leftHits = hits;
        }else
        {
            collisionState.rightHits = hits;
        }

        float distance = Mathf.Abs(delta.x)+skinWidth;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform != null)
            {
                
                if (hit.distance < distance)
                {
                    distance = hit.distance;
                }
               
            }
        }

        return (distance-skinWidth)*direction;
    }

    public float CalculateMoveY(Vector2 delta)
    {
        float direction = Mathf.Sign(delta.y);
        List<RaycastHit2D> hits = VerticalRaycastHits(delta.y);
        if (direction < 0)
        {
            collisionState.bottomHits = hits;
        }
        else
        {
            collisionState.topHits = hits;
        }
        float distance = (Mathf.Abs(delta.y));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform != null)
            {
                if (hit.distance < distance)
                {
                    distance = hit.distance;
                }
            }
        }
        return distance * direction;
    }

    public Vector2 MoveWithGameObjects(Vector2 delta, List<GameObject> attachedObjects)
    {
        gameObjectsToIgnoreCollisonsWith.AddRange(attachedObjects);
        Vector2 newDelta = new Vector2(delta.x, delta.y);
        if (delta.x != 0)
        {
            newDelta.x = CalculateMoveX(delta);
        }
        if (delta.y != 0)
        {
            newDelta.y = CalculateMoveY(delta);
        }

        foreach (GameObject go in attachedObjects)
        {
            MovementControllerScript mcs = go.GetComponent<MovementControllerScript>();
            mcs.gameObjectsToIgnoreCollisonsWith.Add(gameObject);
            Vector2 objectDelta = new Vector2(0, 0);
            if (delta.x != 0)
            {
                objectDelta.x = mcs.CalculateMoveX(delta);
            }
            if (delta.y != 0)
            {
                objectDelta.y = mcs.CalculateMoveY(delta);
            }

            if (Mathf.Abs(objectDelta.x) < Mathf.Abs(newDelta.x))
            {
                newDelta.x = objectDelta.x;
            }
            if (Mathf.Abs(objectDelta.y) < Mathf.Abs(newDelta.y))
            {
                newDelta.y = objectDelta.y;
            }
        }
        
        foreach(GameObject go in attachedObjects)
        {
            MovementControllerScript mcs = go.GetComponent<MovementControllerScript>();
            mcs.gameObjectsToIgnoreCollisonsWith.Remove(gameObject);
            go.transform.Translate(newDelta);
        }

        gameObjectsToIgnoreCollisonsWith = gameObjectsToIgnoreCollisonsWith.Except<GameObject>(attachedObjects).ToList<GameObject>();
        transform.Translate(newDelta);
        return newDelta;
    }
    public void HandleSlope()
    {
     
    }
}
