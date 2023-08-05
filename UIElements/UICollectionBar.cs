using System.Collections.Generic;
using System.Linq;
using Terraria.UI;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;


namespace ItemChecklist.UIElements
{
	// Adapted from Terraria.ModLoader.UI.UIBestiaryBar
	class UICollectionBar : UIElement
	{
		private class UICollectionBarItem
		{
			internal readonly string Tooltop;
			internal readonly int EntryCount;
			internal readonly int CompletedCount;
			internal readonly Color DrawColor;

			public UICollectionBarItem(string tooltop, int entryCount, int completedCount, Color drawColor) {
				Tooltop = tooltop;
				EntryCount = entryCount;
				CompletedCount = completedCount;
				DrawColor = drawColor;
			}
		}

		private List<UICollectionBarItem> collectionBarItems;

		private const string LocalizationKey = "Mods.ItemChecklist.UICollectionBar.";
		private LocalizedText collectionTotalText;
		private LocalizedText collectionTerrariaText;
		private LocalizedText collectionModText;

		public UICollectionBar() {
			collectionBarItems = new List<UICollectionBarItem>();

			collectionTotalText = Language.GetOrRegister(LocalizationKey + nameof(collectionTotalText));
			collectionTerrariaText = Language.GetOrRegister(LocalizationKey + nameof(collectionTerrariaText));
			collectionModText = Language.GetOrRegister(LocalizationKey + nameof(collectionModText));

			RecalculateBars();
		}

		private readonly Color[] _colors = {
			new Color(232, 76, 61),//red
			new Color(155, 88, 181),//purple
			new Color(27, 188, 155),//aqua
			new Color(243, 156, 17),//orange
			new Color(45, 204, 112),//green
			new Color(241, 196, 15),//yellow
		};

		public void RecalculateBars() {
			collectionBarItems.Clear();
			if (Main.gameMenu)
				return;

			var ItemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>();

			//Add the bestiary total to the bar
			int total = ItemChecklistPlayer.totalItemsToFind;
			int totalCollected = ItemChecklistPlayer.totalItemsFound;
			collectionBarItems.Add(new UICollectionBarItem(collectionTotalText.Format(totalCollected,total,$"{(float)totalCollected / total * 100f:N2}"), total, totalCollected, Main.OurFavoriteColor));

			var data = ItemChecklistPlayer.foundItem.Zip(ContentSamples.ItemsByType.Values);
			//.GroupBy(x=>x.Second.ModItem?.Mod.Name ?? "Terraria").ToDictionary(y=>y., x=>x);
			var items = data.Where(x => x.Second.ModItem == null).ToList();

			//Add Terraria's item entries
			int collected = items.Count(x => x.First);
			collectionBarItems.Add(new UICollectionBarItem(collectionTerrariaText.Format( collected,items.Count,$"{(float)collected / items.Count * 100f:N2})"), items.Count, collected, _colors[0]));

			//Add the mod's item entries
			for (int i = 0; i < ModLoader.Mods.Length; i++) {
				items = data.Where(x => x.Second.ModItem?.Mod == ModLoader.Mods[i]).ToList();
				if (!items.Any()) // No Items added by mod.
					continue;
				collected = items.Count(x => x.First);
				collectionBarItems.Add(new UICollectionBarItem(collectionModText.Format( ModLoader.Mods[i].DisplayName,collected,items.Count,$"{(float)collected / items.Count * 100f:N2}"), items.Count, collected, _colors[i % _colors.Length]));
			}
		}

		protected override void DrawSelf(SpriteBatch sb) {
			int xOffset = 0;
			var rectangle = GetDimensions().ToRectangle();
			const int bottomHeight = 3; //The height of the total completion bar
			rectangle.Height -= bottomHeight;

			bool drawHover = false;
			UICollectionBarItem hoverData = null;
			var ItemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>();

			//Draw the mod progress bars
			for (int i = 1; i < collectionBarItems.Count; i++) {
				var barData = collectionBarItems[i];

				int offset = (int)(rectangle.Width * (barData.EntryCount / (float)ItemChecklistPlayer.totalItemsToFind));
				if (i == collectionBarItems.Count - 1) {
					offset = rectangle.Width - xOffset;
				}
				int width = (int)(offset * (barData.CompletedCount / (float)barData.EntryCount));

				var drawArea = new Rectangle(rectangle.X + xOffset, rectangle.Y, width, rectangle.Height);
				var outlineArea = new Rectangle(rectangle.X + xOffset, rectangle.Y, offset, rectangle.Height);
				xOffset += offset;

				sb.Draw(TextureAssets.MagicPixel.Value, outlineArea, barData.DrawColor * 0.3f);
				sb.Draw(TextureAssets.MagicPixel.Value, drawArea, barData.DrawColor);

				if (!drawHover && outlineArea.Contains(new Point(Main.mouseX, Main.mouseY))) {
					drawHover = true;
					hoverData = barData;
				}
			}
			//Draw the bottom progress bar
			var bottomData = collectionBarItems[0];
			int bottomWidth = (int)(rectangle.Width * (bottomData.CompletedCount / (float)bottomData.EntryCount));
			var bottomDrawArea = new Rectangle(rectangle.X, rectangle.Bottom, bottomWidth, bottomHeight);
			var bottomOutlineArea = new Rectangle(rectangle.X, rectangle.Bottom, rectangle.Width, bottomHeight);

			sb.Draw(TextureAssets.MagicPixel.Value, bottomOutlineArea, bottomData.DrawColor * 0.3f);
			sb.Draw(TextureAssets.MagicPixel.Value, bottomDrawArea, bottomData.DrawColor);

			if (!drawHover && bottomOutlineArea.Contains(new Point(Main.mouseX, Main.mouseY))) {
				drawHover = true;
				hoverData = bottomData;
			}

			if (drawHover && hoverData != null) {
				Main.instance.MouseText(hoverData.Tooltop, 0, 0);
			}
		}
	}
}
