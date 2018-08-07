using Microsoft.Xna.Framework.Graphics;

namespace ItemChecklist.UIElements
{
	internal class UIHoverImageButton : UIImageButton
	{
		internal string hoverText;

		public UIHoverImageButton(Texture2D texture, string hoverText) : base(texture)
		{
			this.hoverText = hoverText;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (IsMouseHovering)
			{
				ItemChecklistUI.hoverText = hoverText;
			}
		}
	}
}
