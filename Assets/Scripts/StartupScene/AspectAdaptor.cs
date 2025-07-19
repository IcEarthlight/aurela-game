using UnityEngine;

public class AspectAdaptor : MonoBehaviour
{
    public readonly Vector2 frameSize = new(1920, 1280);
    public float additionalScaling;

    void Update()
    {
        float screenAspectRatio = (float)Screen.width / Screen.height;
        float frameAspectRatio = frameSize.x / frameSize.y;


        if (screenAspectRatio < frameAspectRatio)
        {
            transform.localScale = new Vector3(frameAspectRatio / screenAspectRatio, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, screenAspectRatio / frameAspectRatio, 1f);
        }

        transform.localScale *= additionalScaling;
    }
}
