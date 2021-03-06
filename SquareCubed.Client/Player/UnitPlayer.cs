﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using SquareCubed.Common.Data;

namespace SquareCubed.Client.Player
{
	public class UnitPlayer : IPlayer
	{
		private const float Speed = 2;
		private readonly Client _client;
		private readonly PlayerNetwork _network;
		private PlayerUnit _playerUnit;

		public UnitPlayer(Client client)
		{
			Debug.Assert(client != null);

			_client = client;

			_client.Window.MouseUp += OnMouseUp;
			_client.UpdateTick += OnUpdate;

			_network = new PlayerNetwork(_client.Network, this);
		}

		public bool LockInput { get; set; }

		public Vector2 Position
		{
			get { return _playerUnit.Position; }
			set
			{
				_playerUnit.Position = value;
				_client.Graphics.Camera.Position = _playerUnit.Position;
			}
		}

		public IPositionable Parent
		{
			get { return _playerUnit.Structure; }
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			// Make sure the player is correctly set up, if not just ignore this click
			if (_playerUnit == null) return;
			if (_playerUnit.Structure == null) return;

			foreach (
				var obj in from obj in _playerUnit.Structure.Objects
					let boundingBox = new AaBb
					{
						Position = new Vector2(obj.Position.X - 0.4f, obj.Position.Y - 0.4f),
						Size = new Vector2(0.8f, 0.8f)
					}
					where boundingBox.Contains(_client.Input.MouseState.RelativePosition)
					select obj)
			{
				obj.OnUse();
				return;
			}
		}

		public void OnPlayerData(int id)
		{
			var unit = _client.Units.GetAndRemove(id);
			_playerUnit = new PlayerUnit(unit);
			_client.Units.Add(_playerUnit);

			// Update the old unit's structure entry so it isn't kept alive by the structure
			// TODO: Improve this to be done better somehow
			unit.Structure = null;
		}

		private void OnUpdate(object s, TickEventArgs args)
		{
			// Make sure the player is correctly set up, if not just ignore this tick
			if (_playerUnit == null) return;
			if (_playerUnit.Structure == null) return;

			// If the input is locked, don't move
			if (!LockInput)
			{
				// Calculate total movement data requested
				var velocity = _client.Input.Axes*args.ElapsedTime*Speed;

				// Check if there is movement data
				var xHasVel = (Math.Abs(velocity.X) > 0.001);
				var yHasVel = (Math.Abs(velocity.Y) > 0.001);

				// If there's velocity, we need to do stuff
				if (xHasVel || yHasVel)
				{
					// Get the collider data we'll need to detect collisions
					var statics = _playerUnit.Structure
						.GetChunksWithin(_playerUnit.Position.GetChunkPosition(), 1)
						.SelectMany(c => c.GetColliders()).ToArray();

					// If there's vertical movement
					if (xHasVel)
					{
						if (velocity.X > 0) // If the player is going right
						{
							velocity.X = ResolveAxisDirection(
								_playerUnit.AaBb,
								velocity.X,
								player => player.Right,
								statics,
								wall => wall.Left);
						}
						else // If the player is going left
						{
							velocity.X = ResolveAxisDirection(
								_playerUnit.AaBb,
								velocity.X,
								player => player.Left,
								statics,
								wall => wall.Right);
						}
					}

					// If there's horizontal movement
					if (yHasVel)
					{
						if (velocity.Y > 0) // If the player is going up
						{
							velocity.Y = ResolveAxisDirection(
								_playerUnit.AaBb,
								velocity.Y,
								player => player.Up,
								statics,
								wall => wall.Down);
						}
						else // If the player is going down
						{
							velocity.Y = ResolveAxisDirection(
								_playerUnit.AaBb,
								velocity.Y,
								player => player.Down,
								statics,
								wall => wall.Up);
						}
					}

					// TODO: Add collision resolving in the case of exact corner collision
					// This happens because axises are being checked in isolation from eachother.
					// Thus if the player is at an exact corner going in the direction of the corner,
					// individually it won't hit. However in the next frame the player is in the wall
					// and thus the player snaps back. Insert pun about "edge cases" here.

					// Update the player's position
					_playerUnit.Position += velocity;
					_network.SendPlayerPhysics(_playerUnit);
				}
			}

			// Make sure the camera is parented correctly
			_client.Graphics.Camera.Parent = _playerUnit.Structure;
			_client.Graphics.Camera.Position = _playerUnit.Position;
		}

		private float ResolveAxisDirection(AaBb dynamic, float velocity, Func<AaBb, AaSide> dynamicSideFunc,
			IEnumerable<AaBb> statics, Func<AaBb, AaSide> staticSideFunc)
		{
			// Sometimes with aligned walls they might not entirely align, causing something to stick out a bit out of the wall.
			// This is really small and practically invisible. To fix it we just remove a tiny invisible bit from the length of the lines.
			const float floatCompensation = 0.000001f;

			// Create some data to work with
			var directionMultiplier = velocity < 0 ? -1.0f : 1.0f;
			var dynamicSide = dynamicSideFunc(dynamic);

			// Apply velocity in the correct direction
			var dynamicTangent = dynamicSide.Tangent;
			dynamicSide.Tangent += velocity;
			dynamicSide.Tangent *= directionMultiplier;
			var dynamicCenterTangent = dynamicSide.CenterTangent;
			dynamicSide.CenterTangent += velocity;
			dynamicSide.CenterTangent *= directionMultiplier;

			// Get all the sides that overlap on the Y axis (which won't change during resolving)
			var sides = statics.Select(staticSideFunc).Where(s =>
				(dynamicSide.End > s.Start + floatCompensation) &&
				(dynamicSide.Start < s.End - floatCompensation)).ToArray();

			while (true)
			{
				// Find sides that collided (Give every side a line on their tangential axis from the side to the center of the AaBb then see if there's overlap)
				var side = sides.FirstOrDefault(s =>
					// Only include where the player's side is more than the static side
					(dynamicSide.Tangent > s.Tangent*directionMultiplier) &&
					// Only include where the player's center is less than the static side's center
					(dynamicSide.CenterTangent < s.CenterTangent*directionMultiplier));

				if (side != null)
				{
					// Calculate difference in target position needed to resolve collision
					var diffVel = dynamicSide.Tangent - (side.Tangent*directionMultiplier);

					// Remove difference from velocity
					velocity -= diffVel*directionMultiplier;

					// Update the data in the dynamic side
					dynamicSide.Tangent = dynamicTangent;
					dynamicSide.Tangent += velocity;
					dynamicSide.Tangent *= directionMultiplier;
					dynamicSide.CenterTangent = dynamicCenterTangent;
					dynamicSide.CenterTangent += velocity;
					dynamicSide.CenterTangent *= directionMultiplier;
				}
				else break;
			}

			return velocity;
		}

		public void Render()
		{
			// TODO: Move to structure and add debugging flag on the structures

			// Make sure the player is correctly set up, if not just ignore this tick
			/*if (_playerUnit == null) return;
			if (_playerUnit.Structure == null) return;

			GL.PushMatrix();
			GL.Translate(_playerUnit.Structure.Position.X, _playerUnit.Structure.Position.Y, 0);
			GL.Rotate(-_playerUnit.Structure.Rotation, 0, 0, 1);
			GL.Translate(-_playerUnit.Structure.Center.X, -_playerUnit.Structure.Center.Y, 0);

			// Render all AaBbs
			foreach (var collider in _playerUnit.Structure.GetChunksWithin(_playerUnit.Position.GetChunkPosition(), 1).SelectMany(c => c.GetColliders()))
			{
				GL.PushMatrix();
				GL.Translate(collider.Position.X, collider.Position.Y, 0);

				GL.Begin(PrimitiveType.LineLoop);

				GL.Color3(Color.Blue);

				GL.Vertex2(0.0f, 0.0f);
				GL.Vertex2(collider.Size.X, 0.0f);
				GL.Vertex2(collider.Size.X, collider.Size.Y);
				GL.Vertex2(0.0f, collider.Size.Y);

				GL.End();

				GL.PopMatrix();
			}

			GL.PopMatrix();*/
		}
	}
}