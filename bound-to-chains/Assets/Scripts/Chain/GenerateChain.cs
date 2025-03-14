using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GenerateChain : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private float chainGravity;
    [SerializeField] private float maxChainDistance;
    [SerializeField] private float chainSegmentSize;
    [SerializeField] private int maxChainSegments;
    [SerializeField] private int minChainSegments;
    private int chainSegmentAmount;

    private LineRenderer lineRenderer;
    private List<ChainSegment> chainSegments = new List<ChainSegment>();

    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 gravity;
    private float distance; 


    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        gravity = new Vector2( 0f, -chainGravity);

        Debug.Log(gravity); 

        if (!lineRenderer)
        {
            Debug.Log("No Line renderer found!");
            return;
        }
    }


    private void FixedUpdate()
    {
        if(!lineRenderer)
            return;

        SetMaxDistance(); 
        CreateChain();
        Simulate();
        UpdateLineRenderer();
    }

    private void SetMaxDistance()
    {
        startPoint = this.transform.position;
        endPoint = player.gameObject.transform.position; 

        distance = Vector2.Distance(startPoint, endPoint);

        if(distance > maxChainDistance)
        {
            Vector2 direction = (startPoint - endPoint).normalized;
            Vector2 force = direction * 100f; // Adjust this value to control pull strength

            // Apply force to player's Rigidbody2D
            player.GetComponent<Rigidbody2D>().AddForce(force);
        }
    }

    private void CreateChain()
    {
        Vector2 direction = endPoint - startPoint;

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
    private void Simulate()
    {
        for (int i = 1; i < chainSegmentAmount; i++)
        {
            ChainSegment firstSegment = chainSegments[i];
            Vector2 velocity = firstSegment.newPosition - firstSegment.oldPosition;
            firstSegment.oldPosition = firstSegment.newPosition;
            firstSegment.newPosition += velocity;
            firstSegment.newPosition += gravity * Time.fixedDeltaTime;
            chainSegments[i] = firstSegment;
        }

        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

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