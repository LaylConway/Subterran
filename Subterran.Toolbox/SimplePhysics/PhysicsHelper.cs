﻿using System;
using System.Collections.Generic;
using Subterran.WorldState;

namespace Subterran.Toolbox.SimplePhysics
{
	public static class PhysicsHelper
	{
		public static bool CheckCollision(BoundingBox rigidBox, BoundingBox fixedBox)
		{
			return rigidBox.Start.Y < fixedBox.End.Y && rigidBox.End.Y > fixedBox.Start.Y &&
			       rigidBox.Start.X < fixedBox.End.X && rigidBox.End.X > fixedBox.Start.X &&
			       rigidBox.Start.Z < fixedBox.End.Z && rigidBox.End.Z > fixedBox.Start.Z;
		}

		public static List<Tuple<Entity, RigidbodyComponent>> FindRigidBodies(Entity worldEntity)
		{
			var rigidBodies = new List<Tuple<Entity, RigidbodyComponent>>();

			foreach (var entity in worldEntity.Children)
			{
				var rigidBody = entity.GetOne<RigidbodyComponent>();
				if (rigidBody != null && rigidBody.Enabled)
				{
					rigidBodies.Add(new Tuple<Entity, RigidbodyComponent>(entity, rigidBody));
				}
			}

			return rigidBodies;
		}

		public static List<BoundingBox> FindFixedBoundingBoxes(Entity worldEntity)
		{
			var boxes = new List<BoundingBox>();

			foreach (var entity in worldEntity.Children)
			{
				var fixedBody = entity.GetOne<FixedbodyComponent>();
				if (fixedBody != null)
				{
					boxes.Add(BoundingBox.FromPositionAndCollider(
						entity.Transform.Position,
						fixedBody.Collider));
				}
			}

			return boxes;
		}

		public static IEnumerable<BoundingBox> FindSmartBoundingBoxes(Entity worldEntity, BoundingBox collisionArea)
		{
			foreach (var entity in worldEntity.Children)
			{
				var component = entity.GetOne<ISmartFixedbodySource>();
				if (component != null)
				{
					foreach (var boundingBox in component.GetBoundingBoxesWithin(collisionArea))
					{
						yield return boundingBox;
					}
				}
			}
		}
	}
}