using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public float followCoef;
    public float animCoef;

    void Start()
    {

    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        // Debug.Log("鼠标屏幕位置: " + mousePosition);
        Vector3 targetPosition = Vector3.Lerp(
            new Vector3(Screen.width, Screen.height, 0) / 2,
            new Vector3(Mathf.Clamp(mousePosition.x, 0, Screen.width), Mathf.Clamp(mousePosition.y, 0, Screen.height), 0),
            followCoef
        );
        transform.position = Vector3.Lerp(transform.position, targetPosition, animCoef);
    }

}
