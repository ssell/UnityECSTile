using UnityEngine;

namespace UnityECSTile
{
    public static class CameraUtils
    {
        public static Bounds OrthographicBounds(Camera camera)
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2.0f;

            return new Bounds(camera.transform.position,
                              new Vector3(cameraHeight * aspectRatio, cameraHeight, 0.0f));
        }
    }
}
