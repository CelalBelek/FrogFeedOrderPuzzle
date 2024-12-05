using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Module module;

    private float speed = 10f;
    private GrapeType type;
    private Vector3 direction;
    private bool isReturning = false;

    public void Initialize(GrapeType type, Vector3 initialDirection, Module module)
    {
        this.type = type;
        direction = -initialDirection;
        this.module = module;
    }

    void Update()
    {
        if (isReturning)
        {
            transform.position -= direction * speed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ClickableObject clickableObject = other.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            isReturning = true;
            return;
        }

        GrapeObject otherObject = other.GetComponent<GrapeObject>();
        if (otherObject != null)
        {
            if (otherObject.type == type)
            {
                otherObject.SetTrigger();

                if (otherObject.CompareTag("End"))
                {
                    isReturning = true;
                    module.StartMoving();
                    Destroy(gameObject);
                }
            }
            else
            {
                isReturning = true;
            }
        }

        DirectionChanger directionChanger = other.GetComponent<DirectionChanger>();
        if (directionChanger != null)
        {
            direction = directionChanger.GetNewDirection();
        }
    }
}
