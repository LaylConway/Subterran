﻿using System;

namespace Subterran
{
	public abstract class EntityComponent
	{
		private bool _initialized;

		protected Entity Entity { get; private set; }

		internal void UpdateEntityBinding(Entity entity)
		{
			if (Entity != null)
			{
				throw new InvalidOperationException("Can not assign the same component to two different entities.");
			}

			Entity = entity;
		}

		public void CheckInitialize()
		{
			if (!_initialized)
			{
				_initialized = true;
				Initialize();
			}
		}

		protected virtual void Initialize()
		{
		}
	}
}