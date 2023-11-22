using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerScript Script.
/// </summary>
public class PlayerScript : Script
{
    public CharacterController PlayerController;
    public EmptyActor CameraTarget;
    public Camera Cam;

    // Mouse Variables
    public bool UseMouse = true;
    public float MouseSensitivity = 0.1f;

    public float FOV = 90;

    // Look Variables
    private float _pitch;
    private float _yaw;
    private float _horizontal;
    private float _vertical;

    //public void ComingSoon()
    //{
    //    UseMouse = !UseMouse;
    //    UI.IsActive = !UI.IsActive;
    //    Screen.CursorVisible = !Screen.CursorVisible;
    //    Screen.CursorLock = Screen.CursorLock == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    //    CameraTarget.LookAt(new Vector3(0, 500, 1490));
    //    _yaw = CameraTarget.EulerAngles.Y;
    //    _pitch = CameraTarget.EulerAngles.X;
    //}

    //public override void OnStart()
    //{
    //    UI.IsActive = false;
    //}

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        if (UseMouse)
        {
            // Cursor
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            // Mouse
            Vector2 mouseDelta = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouseDelta *= MouseSensitivity;
            _pitch = Mathf.Clamp(_pitch + mouseDelta.Y, -88, 88);
            _yaw += mouseDelta.X;
        }

        if (Input.GetKeyUp(KeyboardKeys.Tab))
        {
            UseMouse = !UseMouse;
            //UI.IsActive = !UI.IsActive;
            Screen.CursorVisible = !Screen.CursorVisible;
            Screen.CursorLock = Screen.CursorLock == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public override void OnFixedUpdate()
    {
        if (!Screen.CursorVisible && Input.GetKeyUp(KeyboardKeys.E) && Physics.RayCast(CameraTarget.Position, CameraTarget.Direction, out RayCastHit hit, layerMask: 1, maxDistance: 200) && hit.Collider.HasTag("Console"))
        {
            Debug.Log(hit.Collider.Parent);
            PlayerController.Position = hit.Collider.Parent.Position - new Vector3(0, -150, 75);
            Vector3 point = new(hit.Collider.Parent.Position.X, 166.754f, hit.Collider.Parent.Position.Z - 10.6066f);
            //DebugDraw.DrawSphere(new BoundingSphere(point, 10), Color.Blue, 10);
            CameraTarget.LookAt(point);

            CameraTarget.EulerAngles *= Vector3.Right;
            UseMouse = !UseMouse;
            //UI.IsActive = !UI.IsActive;
            Screen.CursorVisible = !Screen.CursorVisible;
            Screen.CursorLock = Screen.CursorLock == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (Screen.CursorVisible)
            return;

        if (Input.GetAction("Zoom"))
        {
            Cam.FieldOfView = FOV/2;
        }
        else
        {
            Cam.FieldOfView = FOV;
        }

        CameraTarget.LocalOrientation = Quaternion.Lerp(CameraTarget.LocalOrientation, Quaternion.Euler(_pitch, _yaw, 0), 1f);

        Transform look_dir = CameraTarget.Transform;
        look_dir.Orientation = Quaternion.Euler(CameraTarget.LocalEulerAngles * new Float3(0, 1, 0));

        var inputH = Input.GetAxis("Horizontal") + _horizontal;
        var inputV = Input.GetAxis("Vertical") + _vertical;
        _horizontal = 0;
        _vertical = 0;

        var velocity = new Vector3(inputH, 0.0f, inputV);
        velocity.Normalize();
        velocity = look_dir.TransformDirection(velocity);

        // Fix direction
        if (velocity.Length < 0.05f)
            velocity = Vector3.Zero;

        // Apply gravity
        velocity.Y += -Mathf.Abs(Physics.Gravity.Y * 2.5f) * Time.DeltaTime;

        PlayerController.Move(velocity * 500 * Time.DeltaTime);
        //_velocity = velocity;
    }
}
