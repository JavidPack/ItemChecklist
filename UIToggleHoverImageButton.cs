using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;
using ItemChecklist.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Terraria.GameContent.UI.Elements
{
	public class UIToggleHoverImageButton : UIImageButton
	{
		private Texture2D _texture;
		private Texture2D overlay;
		private float _visibilityActive = 1f;
		private float _visibilityInactive = 0.4f;
		bool enabled;
		internal string hoverText;

		public UIToggleHoverImageButton(Texture2D texture, Texture2D overlay, string hoverText, bool enabled = false) : base(texture)
		{
			this._texture = texture;
			this.overlay = overlay;
			this.Width.Set((float)this._texture.Width, 0f);
			this.Height.Set((float)this._texture.Height, 0f);
			this.hoverText = hoverText;
			this.enabled = enabled;
		}

		public void SetEnabled(bool enabled)
		{
			this.enabled = enabled;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			spriteBatch.Draw(this._texture, dimensions.Position(), Color.White * (base.IsMouseHovering ? this._visibilityActive : this._visibilityInactive));
			if (!enabled)
			{
				spriteBatch.Draw(this.overlay, dimensions.Position(), Color.White * (base.IsMouseHovering ? this._visibilityActive : this._visibilityInactive));
			}
			if (IsMouseHovering)
			{
				ItemChecklistUI.hoverText = hoverText;
			}
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
		}
	}
}