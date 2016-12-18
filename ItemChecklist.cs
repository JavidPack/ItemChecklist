using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.DataStructures;
using ItemChecklist.UI;
using Microsoft.Xna.Framework;

namespace ItemChecklist
{
	public class ItemChecklist : Mod
	{
		static internal ItemChecklist instance;
		internal static ModHotKey ToggleChecklistHotKey;
		internal static UserInterface ItemChecklistInterface;
		internal ItemChecklistUI ItemChecklistUI;

		public ItemChecklist()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
			};
		}

		public override void Load()
		{
			instance = this;
			ToggleChecklistHotKey = RegisterHotKey("Toggle Item Checklist", "I");
			//if (!Main.dedServ)
			//{
			//	ItemChecklistUI = new ItemChecklistUI();
			//	ItemChecklistUI.Activate();
			//	ItemChecklistInterface = new UserInterface();
			//	ItemChecklistInterface.SetState(ItemChecklistUI);
			//}
		}

		public override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				ItemChecklistUI = new ItemChecklistUI();
				ItemChecklistUI.Activate();
				ItemChecklistInterface = new UserInterface();
				ItemChecklistInterface.SetState(ItemChecklistUI);
			}
		}

		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new MethodSequenceListItem(
					"ItemChecklist: Item Checklist",
					delegate
					{
						if (ItemChecklistUI.visible)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight)
							{
								ItemChecklistInterface.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}

							ItemChecklistInterface.Update(Main._drawInterfaceGameTime);
							ItemChecklistUI.Draw(Main.spriteBatch);

							if (ItemChecklistUI.hoverText != "")
							{
								float x = Main.fontMouseText.MeasureString(ItemChecklistUI.hoverText).X;
								Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f, 16f);
								if (vector.Y > (float)(Main.screenHeight - 30))
								{
									vector.Y = (float)(Main.screenHeight - 30);
								}
								if (vector.X > (float)(Main.screenWidth - x - 30))
								{
									vector.X = (float)(Main.screenWidth - x - 30);
								}
								Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, ItemChecklistUI.hoverText, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
							}

						}
						return true;
					},
					null)
				);
			}
		}
	}
}

