using Godot;
using System;

public class Player : KinematicBody
{

	private const float GRAVITY = -24.8f;
    private const int MAX_SPEED = 20;
    private const int JUMP_SPEED = 18;
    private const float ACCEL = 4.5f;
    private const int DEACCEL = 16;
    private const int MAX_SLOPE_ANGLE = 40;

    private const float MOUSE_SENSITIVITY = 0.05f;

    private const int MAX_SPRINT_SPEED = 30;
    private const int SPRINT_ACCEL = 18;
    private bool is_sprinting = false;

    private SpotLight flashlight;

    private Vector3 vel = new Vector3();
    private Vector3 dir = new Vector3();

    private Camera camera;
    private Spatial rotation_helper;

    public override void _Ready()
    {
        camera = (Camera)GetNode(new NodePath("Rotation_Helper/Camera"));
        rotation_helper = (Spatial)GetNode(new NodePath("Rotation_Helper"));

        flashlight = (SpotLight)GetNode(new NodePath("Rotation_Helper/Flashlight"));

        Input.SetMouseMode(Input.MouseMode.Captured);

    }

    public override void _PhysicsProcess(float delta)
    {
        process_input(delta);
        process_movement(delta);
    }

    private void process_input(float delta)
    {
        /*###############################################*/
        /*WALKING*/
        dir = new Vector3();
        Transform cam_xform = camera.GetGlobalTransform();

        Vector2 input_movement_vector = new Vector2();

        if (Input.IsActionPressed("movement_forward"))
            input_movement_vector.y += 1;
        if (Input.IsActionPressed("movement_backward"))
            input_movement_vector.y -= 1;
        if (Input.IsActionPressed("movement_left"))
            input_movement_vector.x -= 1;
        if (Input.IsActionPressed("movement_right"))
            input_movement_vector.x += 1;

        input_movement_vector = input_movement_vector.Normalized();

        dir += -cam_xform.basis.z.Normalized() * input_movement_vector.y;
        dir += cam_xform.basis.x.Normalized() * input_movement_vector.x;
        /*###############################################*/

        /*###############################################*/
        /*JUMPING*/
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("movement_jump"))
            {
                vel.y = JUMP_SPEED;
            }
        }
        /*###############################################*/

        /*###############################################*/
        /*SPRINTING*/
        if (Input.IsActionJustPressed("movement_sprint"))
            is_sprinting = true;
        else
            is_sprinting = false;
        /*###############################################*/

        /*###############################################*/
        /*Turning the flashlight on/off*/
        if (Input.IsActionJustPressed("flashlight"))
        {
            if (flashlight.IsVisibleInTree())
                flashlight.Hide();
            else
                flashlight.Show();
        }
        /*###############################################*/

        /*###############################################*/
        /*Capturing/Freeing the cursor*/
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
                Input.SetMouseMode(Input.MouseMode.Captured);
            else
                Input.SetMouseMode(Input.MouseMode.Visible);
        }
        /*###############################################*/

    }

    private void process_movement(float delta)
    {

        dir.y = 0;
        dir = dir.Normalized();

        vel.y += delta * GRAVITY;

        var hvel = vel;
        hvel.y = 0;

        var target = dir;
        if (is_sprinting)
            target *= MAX_SPRINT_SPEED;
        else
            target *= MAX_SPEED;

        float accel;
        if (dir.Dot(hvel) > 0)
        {
            if (is_sprinting)
                accel = SPRINT_ACCEL;
            else
                accel = ACCEL;
        }
        else
        {
            accel = DEACCEL;
        }

        hvel = hvel.LinearInterpolate(target, accel * delta);
        vel.x = hvel.x;
        vel.z = hvel.z;
        vel = MoveAndSlide(vel, new Vector3(0, 1, 0), 0.05f, 4, Mathf.Deg2Rad(MAX_SLOPE_ANGLE));

    }

    public override void _Input(InputEvent @event)
    {
        
        if(@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            //Cast is needed to be able to access correct properties
            InputEventMouseMotion mouse_event = (InputEventMouseMotion)@event;

            rotation_helper.RotateX(Mathf.Deg2Rad(mouse_event.Relative.y * MOUSE_SENSITIVITY));

            this.RotateY(Mathf.Deg2Rad(mouse_event.Relative.x * MOUSE_SENSITIVITY * -1));

            var camera_rot = rotation_helper.RotationDegrees;
            camera_rot.x = Mathf.Clamp(camera_rot.x, -70, 70);
            rotation_helper.RotationDegrees = camera_rot;

        }

    }

}
