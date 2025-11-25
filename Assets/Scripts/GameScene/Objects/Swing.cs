using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField] float swingAngle = 5f;
    [SerializeField] float swingSpeed = 2f;
    public float angleOffset = 0f;

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }
}  

