﻿using OpenTK.Input;
using SQCore.Client.Gui;
using SquareCubed.Client.Structures;
using SquareCubed.Client.Structures.Objects;

namespace SQCore.Client.Objects
{
	internal class PilotSeatObjectType : IClientObjectType
	{
		private readonly SquareCubed.Client.Client _client;
		private readonly ContextInfoPanel _panel;

		public PilotSeatObjectType(SquareCubed.Client.Client client, ContextInfoPanel panel)
		{
			_client = client;
			_panel = panel;

			_client.Input.TrackKey(Key.X);
		}

		public IClientObject CreateNew(ClientStructure parent)
		{
			return new PilotSeatObject(_client, _panel, parent);
		}
	}
}