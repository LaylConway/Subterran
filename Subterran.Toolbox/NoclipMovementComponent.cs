﻿using System;
using OpenTK;
using OpenTK.Input;
using Subterran.Input;

namespace Subterran.Toolbox
{
	public class NoclipMovementComponent : EntityComponent, IUpdatable
	{
		private readonly Window _window;
		private Vector2 _previousPosition;

		public NoclipMovementComponent(Window window, InputManager input)
		{
			// Default values, once C# 6.0 rolls around we can do this inline
			Speed = 5.0f;
			FastSpeed = 10.0f;

			_window = window;
			input.ShowCursor = false;
		}

		public float Speed { get; set; }
		public float FastSpeed { get; set; }

		public void Update(TimeSpan elapsed)
		{
			UpdateRotation();

			var rotationMatrix =
				Matrix4.CreateRotationX(Entity.Rotation.X)*
				Matrix4.CreateRotationY(Entity.Rotation.Y);

			var backwards = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
			var right = Vector3.Transform(Vector3.UnitX, rotationMatrix);

			// TODO: Move this into the Subterran.OpenTK library
			var keyboard = Keyboard.GetState();
			var targetDirection = new Vector3();

			if (keyboard.IsKeyDown(Key.S))
				targetDirection += backwards;
			if (keyboard.IsKeyDown(Key.W))
				targetDirection -= backwards;
			if (keyboard.IsKeyDown(Key.D))
				targetDirection += right;
			if (keyboard.IsKeyDown(Key.A))
				targetDirection -= right;

			targetDirection.NormalizeFast();
			Entity.Position += elapsed.PerSecond(
				targetDirection*(
					keyboard.IsKeyDown(Key.ShiftLeft)
						? FastSpeed
						: Speed));
		}

		private void UpdateRotation()
		{
			// Get how much the mouse has changed
			var state = Mouse.GetState();
			var deltaPosition = new Vector2(
				state.X - _previousPosition.X,
				state.Y - _previousPosition.Y);

			// Reset the mouse to the middle of the screen
			Mouse.SetPosition(
				_window.Bounds.Left + (_window.Bounds.Width/2),
				_window.Bounds.Top + (_window.Bounds.Height/2));

			// Store the position of the mouse currently so we can get the delta again next update
			state = Mouse.GetState();
			_previousPosition = new Vector2(state.X, state.Y);

			// Update the rotation of the entity based on the difference in mouse position
			var rotation = Entity.Rotation.Xy + (-deltaPosition.Yx*0.0015f);
			Entity.Rotation = new Vector3(
				MathHelper.Clamp(rotation.X, -StMath.Tau*0.25f, StMath.Tau*0.25f),
				rotation.Y, 0);
		}
	}
}