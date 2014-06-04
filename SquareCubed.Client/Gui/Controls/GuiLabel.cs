﻿using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using OpenTK;
using SquareCubed.Client.Graphics;

namespace SquareCubed.Client.Gui.Controls
{
	internal class GuiLabel : GuiControl
	{
		private int _fontSize;
		private string _text;
		private Texture2D _texture;

		public GuiLabel(string text, int fontSize = 12)
		{
			_fontSize = fontSize;
			Text = text;
		}

		public string Text
		{
			get { return _text; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				_text = value;
				GenerateTexture();
			}
		}

		public int FontSize
		{
			get { return _fontSize; }
			set
			{
				_fontSize = value;
				GenerateTexture();
			}
		}

		public override Size Size
		{
			get { return _texture.Size; }
			set { throw new ReadOnlyException(); }
		}

		protected override void Dispose(bool managed)
		{
			if (managed)
			{
				_texture.Dispose();
			}

			base.Dispose(managed);
		}

		private void GenerateTexture()
		{
			if (_texture != null) _texture.Dispose();
			_texture = TextHelper.RenderString(_text, _fontSize, EngineColors.Heading);
		}

		public override void Render()
		{
			_texture.Render(
				new Vector2(Position.X, Position.Y + _texture.Height),
				new Vector2(_texture.Width, -_texture.Height));
		}
	}
}