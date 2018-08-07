using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace ItemChecklist.UIElements
{
	public class UIToggleHoverImageButton : UIImageButton
	{
		//private Texture2D _texture;
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
				// 32x32, overlay is 24x24.
				spriteBatch.Draw(this.overlay, dimensions.Position() + new Vector2(4), Color.White * (base.IsMouseHovering ? this._visibilityActive : this._visibilityInactive));
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

	public class UIImageButton : UIElement
	{
		protected Texture2D _texture;
		private float _visibilityActive = 1f;
		private float _visibilityInactive = 0.4f;

		public UIImageButton(Texture2D texture)
		{
			this._texture = texture;
			this.Width.Set((float)this._texture.Width, 0f);
			this.Height.Set((float)this._texture.Height, 0f);
		}

		public void SetImage(Texture2D texture)
		{
			this._texture = texture;
			this.Width.Set((float)this._texture.Width, 0f);
			this.Height.Set((float)this._texture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			spriteBatch.Draw(this._texture, dimensions.Position(), Color.White * (base.IsMouseHovering ? this._visibilityActive : this._visibilityInactive));
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
		}

		public void SetVisibility(float whenActive, float whenInactive)
		{
			this._visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			this._visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
		}
	}
}