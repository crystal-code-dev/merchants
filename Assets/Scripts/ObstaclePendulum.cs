using UnityEngine;

public class ObstaclePendulum : Obstacle
{
  public float maxAngle = 45.0f, swingSpeed = 1.5f;
  public Transform pendulumObject;
  [System.Flags]
  public enum Axis
  {
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2
  }
  public Axis selectedAxis;
  private float startAngle;

  void Start()
  {
    startAngle = transform.localEulerAngles.z;
  }

  void Update()
  {
    float angle = maxAngle * Mathf.Sin(Time.time * swingSpeed);
    float angleX = 0;
    float angleY = 0;
    float angleZ = 0;

    if ((selectedAxis & Axis.X) == Axis.X) angleX = startAngle + angle;
    if ((selectedAxis & Axis.Y) == Axis.Y) angleY = startAngle + angle;
    if ((selectedAxis & Axis.Z) == Axis.Z) angleZ = startAngle + angle;

    pendulumObject.localRotation = Quaternion.Euler(angleX, angleY, angleZ);

  }
}
