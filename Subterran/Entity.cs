﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using OpenTK;

namespace Subterran
{
	public sealed class Entity
	{
		private Vector3 _scale;

		public Entity()
		{
			// Scale by default needs to be 1 because 0 will give invisible entities
			Scale = new Vector3(1, 1, 1);

			Children = new ObservableCollection<Entity>();
			Children.CollectionChanged += Children_CollectionChanged;

			Components = new ObservableCollection<EntityComponent>();
			Components.CollectionChanged += Components_CollectionChanged;
		}

		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }

		public Vector3 Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				InverseScale = new Vector3(1/value.X, 1/value.Y, 1/value.Z);
			}
		}

		public Vector3 InverseScale { get; private set; }
		public Entity Parent { get; private set; }
		public ObservableCollection<Entity> Children { get; private set; }
		public ObservableCollection<EntityComponent> Components { get; private set; }

		public Vector3 WorldPosition
		{
			get
			{
				return Parent != null
					? Vector3.Transform(Position, Parent.WorldMatrix)
					: Position;
			}
		}

		public Matrix4 Matrix
		{
			get
			{
				// Create a multiply matrix representing this entity
				return
					Matrix4.CreateScale(Scale)*
					Matrix4.CreateRotationX(Rotation.X)*
					Matrix4.CreateRotationY(Rotation.Y)*
					Matrix4.CreateRotationZ(Rotation.Z)*
					Matrix4.CreateTranslation(Position);
			}
		}

		public Matrix4 WorldMatrix
		{
			get
			{
				// Multiply the entity matrix with the parent's world matrix
				return Parent != null
					? Parent.WorldMatrix*Matrix
					: Matrix;
			}
		}

		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			StCollection.ExecuteForAdded<Entity>(args, i => i.Parent = this);
			StCollection.ExecuteForRemoved<Entity>(args, i => i.Parent = null);
		}

		private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			StCollection.ExecuteForAdded<EntityComponent>(args, i => i.UpdateEntityBinding(this));
			StCollection.ExecuteForRemoved<EntityComponent>(args, i => i.UpdateEntityBinding(this));
		}

		public T GetComponent<T>()
			where T : class
		{
			return GetComponents<T>().FirstOrDefault();
		}

		public IEnumerable<T> GetComponents<T>()
			where T : class
		{
			for (var i = 0; i < Components.Count; i++)
			{
				var tComponent = Components[i] as T;
				if (tComponent != null)
					yield return tComponent;
			}

			//return Components.OfType<T>();
		}

		public T RequireComponent<T>()
			where T : class
		{
			var value = GetComponent<T>();

			if (value == null)
				throw new InvalidOperationException("This component requires a " + typeof (T).Name);

			return value;
		}

		public T RequireComponent<T>(Func<T, bool> predicate)
			where T : class
		{
			var value = GetComponents<T>().FirstOrDefault(predicate);

			if (value == null)
				throw new InvalidOperationException(
					"This component requires a " + typeof (T).Name +
					" matching given requirements.");

			return value;
		}

		/// <summary>
		///     Calls the function func for each component matching T.
		///     Propagates method call to children.
		/// </summary>
		/// <typeparam name="T">The type of the components to call on.</typeparam>
		/// <param name="func">The function to call.</param>
		public void ForEach<T>(Action<T> func)
			where T : class
		{
			foreach (var component in GetComponents<T>())
			{
				func(component);
			}

			foreach (var child in Children)
			{
				child.ForEach(func);
			}
		}
	}
}