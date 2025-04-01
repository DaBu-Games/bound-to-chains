using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateChain : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Header("Chain values")]
    [SerializeField] private float chainGravity;
    [SerializeField] private float maxChainLength;
    [SerializeField] private float minChainLength;
    [SerializeField] private float chainSegmentSize;
    [SerializeField] private float elongateSpeed;
    [SerializeField] private int maxChainSegments;
    [SerializeField] private int minChainSegments;
    [SerializeField] private int simulationTickRate;

    [Header("Joint")]
    [SerializeField] private DistanceJoint2D distanceJoined2D;
    private int chainSegmentAmount;

    private LineRenderer lineRenderer;
    private List<ChainSegment> chainSegments = new List<ChainSegment>();

    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 gravity;

    public bool autoLength {  get; private set; }

    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        gravity = new Vector2( 0f, -chainGravity);
        SetLengthDistanceJoined(maxChainLength);
        autoLength = true; 

        if (!lineRenderer)
        {
            Debug.Log("No Line renderer found!");
            return;
        }
    }

    public bool IsChainMaxLength(float margin)
    {
        return GetChainLength() >= (maxChainLength - margin);
    }

    public bool IsChainMinLength(float margin)
    {
        return GetChainLength() <= (minChainLength + margin);
    }

    public void SetAutoLength(bool autoLength)
    {
        this.autoLength = autoLength;
    }

    public float GetChainLength()
    {
        if(autoLength)
            return distanceJoined2D.distance;

        return Vector2.Distance(this.transform.position, playerTransform.position); 
    }

    public void SetCurrentChainLength()
    {
        SetLengthDistanceJoined(GetChainLength());
    }

    public void ShortenDistanceJoint(float shortenSpeed)
    {
        SetLengthDistanceJoined(distanceJoined2D.distance - (shortenSpeed * Time.fixedDeltaTime));
    }

    public void ElongateDistanceJoint(float elongateSpeed)
    {
        SetLengthDistanceJoined(distanceJoined2D.distance + (elongateSpeed * Time.fixedDeltaTime));
    }

    public void SetLengthDistanceJoined(float length)
    {
        distanceJoined2D.distance = length;
    }

    private void FixedUpdate()
    {
        if(!lineRenderer)
            return;

        if (!IsChainMaxLength(0f) && autoLength)
            ElongateDistanceJoint(elongateSpeed); 

        CreateChain(); 
        Simulate();
        UpdateLineRenderer();
    }

    /// <summary>
    /// Calculates how many chainSegments are needed to make a chain between the player and the ball. 
    /// If there is a diffrence between the current and calculated amount add or delte segments. 
    /// </summary>
    private void CreateChain()
    {
        startPoint = this.transform.position;
        endPoint = playerTransform.position;
        Vector2 direction = endPoint - startPoint;
        float distance = GetChainLength();

        chainSegmentAmount = math.clamp(Mathf.FloorToInt(distance / chainSegmentSize), minChainSegments, maxChainSegments);

        int segmentDiffrence = chainSegmentAmount - chainSegments.Count;

        if (segmentDiffrence > 0) 
        {
            for (int i = 0; i < segmentDiffrence; i++)
            {
                float t = i / (chainSegmentAmount - 1);
                Vector2 position = startPoint + direction * t;
                chainSegments.Add(new ChainSegment(position));
            }
        }
        else if(segmentDiffrence < 0)
        {
            for( int i = 0; i < math.abs(segmentDiffrence); i++)
            {
                chainSegments.RemoveAt(chainSegments.Count - 1);
            }
        }
    }

    /// <summary>
    /// Calculate the new position of every chainSegment bye using gravity and velocity 
    /// </summary>
    private void Simulate()
    {
        startPoint = this.transform.position;
        endPoint = playerTransform.position;
        for (int i = 1; i < chainSegmentAmount; i++)
        {
            ChainSegment firstSegment = chainSegments[i];
            Vector2 velocity = firstSegment.newPosition - firstSegment.oldPosition;
            firstSegment.oldPosition = firstSegment.newPosition;
            firstSegment.newPosition += velocity;
            firstSegment.newPosition += gravity * Time.fixedDeltaTime;
            chainSegments[i] = firstSegment;
        }

        for (int i = 0; i < simulationTickRate; i++)
        {
            ApplyConstraint();
        }
    }

    /// <summary>
    /// add constraints to every chainSegment
    /// </summary>
    private void ApplyConstraint()
    {
        chainSegments[0].newPosition = startPoint;
        chainSegments[chainSegments.Count - 1].newPosition = endPoint;

        for (int i = 0; i < chainSegmentAmount - 1; i++)
        {
            ChainSegment firstSeg = chainSegments[i];
            ChainSegment secondSeg = chainSegments[i + 1];

            float dist = (firstSeg.newPosition - secondSeg.newPosition).magnitude;
            float error = Mathf.Abs(dist - chainSegmentSize);
            Vector2 direction = Vector2.zero;

            if (dist > chainSegmentSize)
            {
                direction = (firstSeg.newPosition - secondSeg.newPosition).normalized;
            }
            else if (dist < chainSegmentSize)
            {
                direction = (secondSeg.newPosition - firstSeg.newPosition).normalized;
            }

            Vector2 changeAmount = direction * error;
            if (i != 0)
            {
                firstSeg.newPosition -= changeAmount * 0.5f;
                chainSegments[i] = firstSeg;
                secondSeg.newPosition += changeAmount * 0.5f;
                chainSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.newPosition += changeAmount;
                chainSegments[i + 1] = secondSeg;
            }
        }
    }

    /// <summary>
    /// Add every chainSegment to the linderender to visualise the chain
    /// </summary>
    private void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[chainSegments.Count + 1];
        positions[0] = startPoint;

        for (int i = 0; i < chainSegments.Count; i++)
        {
            positions[i + 1] = chainSegments[i].newPosition;
        }

        lineRenderer.positionCount = chainSegments.Count + 1;
        lineRenderer.SetPositions(positions);
    }

}

public class ChainSegment
{
    public Vector2 oldPosition;
    public Vector2 newPosition;

    public ChainSegment( Vector2 pos )
    {
        this.oldPosition = pos;
        this.newPosition = pos;
    }
}