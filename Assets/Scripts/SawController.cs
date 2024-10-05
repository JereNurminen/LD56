using UnityEngine;

enum MovingTowards
{
    Start,
    End
}

public class SawController : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float speed = 1f;

    private MovingTowards movingTowards = MovingTowards.End;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPosition, 2f);
        Gizmos.DrawWireSphere(endPosition, 2f);
        Gizmos.DrawLine(startPosition, endPosition);
    }

    void Start()
    {
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        switch (movingTowards)
        {
            case MovingTowards.Start:
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    startPosition,
                    speed * Time.deltaTime
                );
                if ((Vector2)transform.position == startPosition)
                {
                    movingTowards = MovingTowards.End;
                }
                break;
            case MovingTowards.End:
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    endPosition,
                    speed * Time.deltaTime
                );
                if ((Vector2)transform.position == endPosition)
                {
                    movingTowards = MovingTowards.Start;
                }
                break;
        }
    }
}
