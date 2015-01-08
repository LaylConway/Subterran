﻿using System.Drawing;

namespace Subterran.Rendering.Components
{
	public sealed class CameraComponent : EntityComponent
	{
		public CameraComponent()
		{
			// Default values
			VerticalFoV = 0.2f*StMath.Tau;
			ZNear = 0.1f;
			ZFar = 500f;
			Color = Color.CornflowerBlue;
		}

		public Point Position { get; set; }
		public Size Size { get; set; }

		public float VerticalFoV { get; set; }

		public float ZNear { get; set; }
		public float ZFar { get; set; }
		public Color Color { get; set; }
	}
}