using UnityEngine;

public class MovingBoard : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1.0f;

    private float startTime;
    private float journeyLength;

    private void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
    }

    private void Update()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, fractionOfJourney);

        if (fractionOfJourney >= 1.0f)
        {
            Transform temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;

            startTime = Time.time;
        }
    }
}

