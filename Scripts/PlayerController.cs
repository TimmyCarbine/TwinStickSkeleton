using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    // === MOVEMENT ===
    [Export] public float MoveSpeed = 600f;                 // Base movement speed
    [Export] public float Acceleration = 2f;                // How quickly we reach target velocity (move speed)
    [Export] public float Friction = 0.001f;                // How quickly we decelerate

    // === MOVEMENT SPRING ===
    [Export] public float SpringRange = 800f;               // Distance (px) at which the spring will fully engage
    [Export] public float SpringStrength = 0.25f;           // How much pulling force is applied to the player at full distance

    // === DASH ===
    [Export] public float DashSpeed = 1200f;                // Impulse speed during dash
    [Export] public float DashDuration = 0.15f;             // How long the dash lasts in seconds
    [Export] public float DashCooldown = 0.6f;              // Time before dash can be used again
    [Export] public float DashSteer = 0.08f;                // How much steering assist is applied to the player while dashing
    private bool _isDashing = false;
    private float _dashTimeLeft = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector2 _dashDirection;

    // === INTERNAL STATE ===
    private Vector2 _inputDir;
    private Vector2 _velocity;

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // --- Handle Dash Timers ---
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= dt; // Subtract time past (delta) from cooldown
        if (_isDashing)
        {
            _dashTimeLeft -= dt; // Substract time past (delta) from dash time
            if (_dashTimeLeft <= 0f) _isDashing = false;
        }

        // --- Movement Input ---
        // Aim basis (mouse-relative)
        Vector2 toMouse = GetGlobalMousePosition() - GlobalPosition;
        Vector2 fwd = toMouse.LengthSquared() > 0.0001f ? toMouse.Normalized() : Vector2.Up;
        Vector2 right = fwd.Rotated(Mathf.Pi * 0.5f);

        // Read inputs as signed scalars
        float upDown = Input.GetActionStrength("move_up") - Input.GetActionStrength("move_down");
        float leftRight = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        // Desired movement in the mouse-relative frame
        Vector2 desiredDir = fwd * upDown + right * leftRight;
        if (desiredDir.LengthSquared() > 1e-6f) desiredDir = desiredDir.Normalized();

        float spring = 0f;
        if (_inputDir != Vector2.Zero)
        {
            float dist = (GetGlobalMousePosition() - GlobalPosition).Length();
            spring = Mathf.Clamp(dist / SpringRange, 0f, 1f) * SpringStrength;
        }

        if (!_isDashing)
        {
            Vector2 targetVelocity = (desiredDir + fwd * spring) * MoveSpeed;

            // Accelerate towards target velocity
            _velocity = _velocity.Lerp(targetVelocity, 1 - Mathf.Exp(-Acceleration * dt));

            // Apply friction when no input
            if (desiredDir == Vector2.Zero)
                _velocity = _velocity.Lerp(Vector2.Zero, 1 - Mathf.Exp(-Friction * dt));

            // Check for dash input
            if (Input.IsActionJustPressed("dash") && _dashCooldownTimer <= 0)
                StartDash(desiredDir, fwd);
        }
        else
        {
            _dashDirection = _dashDirection.Lerp(desiredDir == Vector2.Zero ? _dashDirection : desiredDir, DashSteer);
        }
        
        // --- Apply Movement ---
            Velocity = _isDashing ? _dashDirection * DashSpeed : _velocity;
        MoveAndSlide();

        // --- Aim Towards Mouse ---
        LookAt(GetGlobalMousePosition());
    }

    private void StartDash(Vector2 desiredDir, Vector2 fwd)
    {
        _isDashing = true;
        _dashTimeLeft = DashDuration;
        _dashCooldownTimer = DashCooldown;

        // Prefer current move direction; fallback to fwd (toward mouse)
        _dashDirection = desiredDir != Vector2.Zero ? desiredDir : fwd;

        // Optional: Spawn dash trail or particles later
    }
}
