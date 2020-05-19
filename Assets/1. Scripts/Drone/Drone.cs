using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
#pragma warning disable CS0649
    // Start is called before the first frame update
    Vector3 targetPos;

    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float orbitHeight = 3f;    
    [SerializeField] private LayerMask obstacleMask;

    private Rigidbody rb;
    private Vector3 force;
    private SphereCollider col;    
    private float droneSize;
    private Path path;
    private float SEGMENT_LENGTH = 1f;    
    private Transform planetParent;
    public StateMachine<Drone> stateMachine;

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        Init(transform.parent);
        stateMachine = new StateMachine<Drone>(new DroneStateMove(), this);
    }

    public void Init(Transform planet)
    {
        planetParent = planet;
        targetPos = transform.position;
        path = new Path();
        droneSize = col.radius;        
    }

    void Update()
    {
        UpdateTargetPos();
        Move();
    }

    private void FixedUpdate()
    {
        MoveFixed();
    }

    void UpdateTargetPos()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit,
                100f,
                LayerMask.GetMask("Planet")))
            {
                targetPos = hit.point;
                UpdatePath();
            }
        }
    }

    private void MoveFixed()
    {
        Vector3 pos = rb.position;
        Vector3 dir = path.GetDirection(pos, col);
        Vector3 droneVector = pos - planetParent.position;

        force = dir.normalized * speed;
        
        // Hover
        force += droneVector.normalized * (orbitHeight - droneVector.magnitude);
        rb.AddForce(force);
    }

    void Move()
    {
        Vector3 dir = path.GetDirection(transform.position, col);
        if (dir == Vector3.zero) return;

        Vector3 pos = transform.position;
        Vector3 up = pos.normalized;
        Vector3 nextStep = pos + dir.normalized * speed * Time.deltaTime;
        nextStep = planetParent.position + (nextStep.normalized * orbitHeight);

        // Rotate
        Quaternion lookRot = Quaternion.LookRotation(nextStep - pos, up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotateSpeed);
    }

    void UpdatePath()
    {
        Vector3 currentPoint = transform.position;
        List<Vector3> newPath = new List<Vector3>();
        bool hitObstacleLastPoint = false;
        Vector3 lastDir = Vector3.zero;
        int i = 0;
        while (i < 99)
        {
            if (i > 0) currentPoint = newPath[i - 1];

            Vector3 targetDir = (targetPos - currentPoint).normalized;

            Vector3 nextPoint = (planetParent.position + 
                (currentPoint + (targetDir * SEGMENT_LENGTH)).normalized) * orbitHeight;

            Vector3 dir = nextPoint - currentPoint;

            if (hitObstacleLastPoint)
            {
                // alter dir
                dir = (dir.normalized + lastDir.normalized) * 0.5f;
            }

            // Raycast forward to see if we hit an obstacle
            if (Physics.SphereCast(currentPoint, droneSize,
                dir.normalized, out RaycastHit hit, SEGMENT_LENGTH * 1.1f, obstacleMask))
            {
                Debug.Log("Hit obstacle +" + hit.collider.name);
                // Determine quickest way around the obstacle                
                nextPoint = GetClearOfObstacle(dir.normalized, currentPoint);
                hitObstacleLastPoint = true;
            }
            else
                hitObstacleLastPoint = false;

            newPath.Add(nextPoint);

            if (dir.magnitude <= 0.01f)
            {
                Debug.LogFormat("Path found in {0} steps", i);
                break;
            }
            i++;
            lastDir = dir;
        }
        path.SetWaypoints(newPath);

    }
    private Vector3 GetClearOfObstacle(Vector3 dir, Vector3 currentPoint)
    {
        // Raycast forward to see if we hit an obstacle
        int leftTurns = 0, rightTurns = 0;
        float radians = Mathf.Deg2Rad;
        Vector3 forward, right;
        Vector3 rightPoint = Vector3.zero, leftPoint = Vector3.zero;

        forward = dir.normalized;
        right = Vector3.Cross(currentPoint.normalized, forward).normalized;

        Debug.DrawRay(currentPoint, right, Color.red, 4f);
        Debug.DrawRay(currentPoint, forward, Color.blue, 4f);
        Debug.DrawRay(currentPoint, currentPoint.normalized, Color.green, 4f);

        // try for right
        for (int i = 0; i < 179; i++)
        {
            rightTurns = i;
            Vector3 r = Vector3.RotateTowards(currentPoint + forward,
                currentPoint + right - forward, radians * (i + 1), SEGMENT_LENGTH);
            r = (currentPoint + ((r - currentPoint).normalized * SEGMENT_LENGTH)).normalized * orbitHeight;

            Ray ray = new Ray(currentPoint, r - currentPoint);

            Debug.DrawRay(ray.origin, ray.direction * SEGMENT_LENGTH * 1.1f, Color.grey, 4f);
            // Test turning
            if (Physics.SphereCast(ray, droneSize, SEGMENT_LENGTH * 1.1f, obstacleMask))
            {
                //Debug.Log("Hit obstacle, trying again...");
            }
            else
            {
                // We're free!
                rightPoint = r;
                Debug.LogFormat("Found path going right, {0} turns", i);
                break;
            }
        }

        // try for left
        for (int i = 0; i < 179; i++)
        {
            leftTurns = i;
            Vector3 l = Vector3.RotateTowards(currentPoint + forward,
                currentPoint - right - forward, radians * (i + 1), SEGMENT_LENGTH);
            l = (currentPoint + ((l - currentPoint).normalized * SEGMENT_LENGTH)).normalized * orbitHeight;

            Ray ray = new Ray(currentPoint, l - currentPoint);
            Debug.DrawRay(ray.origin, ray.direction * SEGMENT_LENGTH * 1.1f, Color.grey, 4f);
            // Test turning
            if (Physics.SphereCast(ray, droneSize, out RaycastHit hit, SEGMENT_LENGTH * 1.1f, LayerMask.GetMask("Obstacle")))
            {
                //Debug.Log("Hit obstacle, trying again...");
            }
            else
            {
                // We're free!
                leftPoint = l;
                Debug.LogFormat("Found path going left, {0} turns", i);
                break;
            }
        }


        if (rightTurns < leftTurns)
        {
            Debug.Log("Winning turn was right - turns: " + rightTurns);
            return rightPoint;
        }

        if (leftTurns < rightTurns)
        {
            Debug.Log("Winning turn was left - turns: " + leftTurns);
            return leftPoint;
        }

        if (leftTurns == rightTurns && leftTurns != 90)
            return rightPoint;

        Debug.LogError("Unable to find way out! Drone is stuck!");
        return Vector3.zero;

    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(planetParent.position, 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(targetPos, 0.25f);

        if (path.waypoints == null) return;
        for (int i = 0; i < path.waypoints.Count; i++)
        {
            Gizmos.color = i == path.nextWaypoint ? Color.green : Color.cyan;
            Gizmos.DrawSphere(path.waypoints[i], 0.15f);
            if (i != 0)
                Gizmos.DrawLine(path.waypoints[i - 1], path.waypoints[i]);
        }


    }

    [System.Serializable]
    public struct Path
    {
        public List<Vector3> waypoints;
        public int nextWaypoint;
        private Vector3 CurrentWaypoint { get { return waypoints[nextWaypoint]; } }

        public void SetWaypoints(List<Vector3> newPath)
        {
            waypoints = newPath;
            nextWaypoint = 0;
        }

        public Vector3 GetDirection(Vector3 position, SphereCollider col)
        {
            if (waypoints == null) return Vector3.zero;
            
            // Get direction from position
            Vector3 dir = CurrentWaypoint - position;            

            bool isAtDest = col.bounds.Contains(CurrentWaypoint);
            if (isAtDest && nextWaypoint != waypoints.Count - 1)
            {
                nextWaypoint++;
            }

            // If distance is greater threshold, go to next id
            if (isAtDest) return Vector3.zero;

            // otherwise return direction
            return dir;
        }
    }
}
