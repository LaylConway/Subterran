﻿using System;
using NSubstitute;
using Xunit;

namespace Subterran.Tests
{
	public class LoopTests
	{
		[Fact]
		public void Constructor_NoExplicitRate_ExecutesTickOnce()
		{
			// Arrange
			var func = Substitute.For<Action>();
			var loop = new Loop(_ => func());

			// Act
			loop.ExecuteTicks(TimeSpan.FromSeconds(1));

			// Assert
			func.Received(1).Invoke();
		}

		[Fact]
		public void Constructor_TwoPerSecond_ExecutesTickTwice()
		{
			// Arrange
			var func = Substitute.For<Action>();

			// Act
			var loop = new Loop(_ => func(), 2);

			// Assert
			loop.ExecuteTicks(TimeSpan.FromSeconds(1));
			func.Received(2).Invoke();
		}
	}
}