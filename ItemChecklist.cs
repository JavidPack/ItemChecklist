using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ItemChecklist
{
	// TODO: is ItemChecklistPlayer.foundItems a waste of memory? investigate and trim it down if needed.
	// TODO: World Checklist? MP shared checklist?
	// Has this item ever been seen on this world? - easy. Maintain separate bool array, on change, notify server, relay to clients. 
	// send bool array as byte array?
	// WHY? I want to know everything we can craft yet
	public class ItemChecklist : Mod
	{
		static internal ItemChecklist instance;
		internal static ModHotKey ToggleChecklistHotKey;
		internal static UserInterface ItemChecklistInterface;
		internal ItemChecklistUI ItemChecklistUI;
		internal event Action<int> OnNewItem;

		public ItemChecklist()
		{
		}

		public override void Load()
		{
			// Latest uses ItemID.Sets.IsAMaterial, added 0.10.1.5
			if (ModLoader.version < new Version(0, 10, 1, 5))
			{
				throw new Exception("\nThis mod uses functionality only present in the latest tModLoader. Please update tModLoader to use this mod\n\n");
			}

			instance = this;
			ToggleChecklistHotKey = RegisterHotKey("Toggle Item Checklist", "I");
			MagicStorageIntegration.Load();
			MagicStorageExtraIntegration.Load();

			if (!Main.dedServ)
			{
				UIElements.UICheckbox.checkboxTexture = GetTexture("UIElements/checkBox");
				UIElements.UICheckbox.checkmarkTexture = GetTexture("UIElements/checkMark");
				UIElements.UIHorizontalGrid.moreLeftTexture = GetTexture("UIElements/MoreLeft");
				UIElements.UIHorizontalGrid.moreRightTexture = GetTexture("UIElements/MoreRight");
			}
		}

		public override void Unload()
		{
			ItemChecklistUI.vanillaIDsInSortOrder = null;
			instance = null;
			ToggleChecklistHotKey = null;
			ItemChecklistInterface = null;
			MagicStorageIntegration.Unload();
			MagicStorageExtraIntegration.Unload();

			UIElements.UICheckbox.checkboxTexture = null;
			UIElements.UICheckbox.checkmarkTexture = null;
			UIElements.UIHorizontalGrid.moreLeftTexture = null;
			UIElements.UIHorizontalGrid.moreRightTexture = null;
		}

		public override void AddRecipes()
		{
			if (!Main.dedServ)
			{
				ItemChecklistUI = new ItemChecklistUI();
				//ItemChecklistUI.Activate();
				ItemChecklistInterface = new UserInterface();
				ItemChecklistInterface.SetState(ItemChecklistUI);
			}
		}

		// As of 0.2.1: All this
		// RequestFoundItems must be done in game since foundItem is a reference to an array that is initialized in LoadPlayer.
		public override object Call(params object[] args)
		{
			try
			{
				string message = args[0] as string;
				if (message == "RequestFoundItems")
				{
					if (Main.gameMenu)
					{
						return "NotInGame";
					}
					return Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>().foundItem;
				}
				else if (message == "RegisterForNewItem")
				{
					Action<int> callback = args[1] as Action<int>;
					OnNewItem += callback;
					return "RegisterSuccess";
				}
				else
				{
					Logger.Error("ItemChecklist Call Error: Unknown Message: " + message);
				}
			}
			catch (Exception e)
			{
				Logger.Error("ItemChecklist Call Error: " + e.StackTrace + e.Message);
			}
			return "Failure";
		}

		internal void NewItem(int type)
		{
			OnNewItem?.Invoke(type);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			ItemChecklistInterface?.Update(gameTime); 
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"ItemChecklist: Item Checklist",
					delegate
					{
						if (ItemChecklistUI.Visible)
						{
							ItemChecklistInterface?.Draw(Main.spriteBatch, new GameTime());

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
					InterfaceScaleType.UI)
				);
			}
		}
	}
}

