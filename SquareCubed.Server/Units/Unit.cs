﻿using OpenTK;
using SquareCubed.Server.Worlds;

namespace SquareCubed.Server.Units
{
	public class Unit
	{
		private World _world;

		public uint Id { get; set; }

		public virtual World World
		{
			get { return _world; }
			set
			{
				// If already this, don't do anything
				if (value == _world) return;

				var oldWorld = value;
				_world = value;

				if(oldWorld != null)
					oldWorld.UpdateUnitEntry(this);
				if(_world != null)
					_world.UpdateUnitEntry(this);
			}
		}

		public Vector2 Position { get; set; }
	}
}