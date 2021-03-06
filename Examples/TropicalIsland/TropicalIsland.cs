﻿using OpenTK;
using Subterran;
using Subterran.Rendering;
using Subterran.Toolbox;
using Subterran.WorldState;

namespace TropicalIsland
{
	internal static class TropicalIsland
	{
		public static BasicSubterranGame Create()
		{
			var game = new BasicSubterranGame {Window = {Title = "Tropical Island"}};

			game.World = new Entity
			{
				Children =
				{
					CreateCameraEntity(game.Window)
				}
			};

			return game;
		}

		private static Entity CreateCameraEntity(StandardWindowService window)
		{
			return new Entity
			{
				Transform =
				{
					Position = new Vector3(25, 20, 50),
					Rotation = new Vector3(-0.05f * StMath.Tau, 0, 0)
				},
				Components =
				{
					new CameraComponent(),
					//new NoclipMovementComponent(window)
				}
			};
		}
	}
}