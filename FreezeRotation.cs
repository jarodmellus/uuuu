namespace VXRLabs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FreezeRotation : MonoBehaviour
    {
        [SerializeField] bool freezeX, freezeY, freezeZ;
        bool hasFrozeX, hasFrozeY, hasFrozeZ;
        float xVal, yVal, zVal;


        private void FixedUpdate() => Freeze();
        private void Update() => Freeze();
        private void LateUpdate() => Freeze();

        void Freeze()
        {
            if (freezeX != hasFrozeX)
            {
                if (freezeX)
                    xVal = transform.localEulerAngles.x;
                hasFrozeX = freezeX;
            }
            if (freezeY != hasFrozeY)
            {
                if (freezeY)
                    yVal = transform.localEulerAngles.y;
                hasFrozeY = freezeY;
            }
            if (freezeZ != hasFrozeZ)
            {
                if (freezeZ)
                    zVal = transform.localEulerAngles.z;
                hasFrozeZ = freezeZ;
            }

            if (hasFrozeX || hasFrozeY || hasFrozeZ)
            {
                var x = hasFrozeX ? xVal : transform.localEulerAngles.x;
                var y = hasFrozeY ? yVal : transform.localEulerAngles.y;
                var z = hasFrozeZ ? zVal : transform.localEulerAngles.z;
                transform.localEulerAngles = new Vector3(x, y, z);
            }
        }
    }
}