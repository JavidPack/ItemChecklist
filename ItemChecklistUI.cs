using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ID;
using System;
using System.Reflection;
using System.Linq;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ItemChecklist.UI
{
	class ItemChecklistUI : UIState
	{
		public UIHoverImageButton foundFilterButton;
		public UIToggleHoverImageButton muteButton;
		public UIHoverImageButton sortButton;
		public UIHoverImageButton modFilterButton;
		public UIPanel checklistPanel;
		public UIGrid checklistGrid;
		public static SortModes sortMode = SortModes.TerrariaSort;

		float spacing = 8f;
		public static bool visible = false;
		public static int showCompleted = 0; // 0: both, 1: unfound, 2: found
		public static bool announce = true;
		public static string hoverText = "";

		ItemSlot[] itemSlots;
		internal static int[] vanillaIDsInSortOrder;

		internal List<string> modnames;
		internal int currentMod = 0;

		public string[] foundFilterStrings = { "All", "Unfound", "Found" };

		public override void OnInitialize()
		{
			// Is initialize called? (Yes it is called on reload) I want to reset nicely with new character or new loaded mods: visible = false;

			announce = true; // overwritten by modplayer

			checklistPanel = new UIPanel();
			checklistPanel.SetPadding(10);
			checklistPanel.Left.Pixels = 0;
			checklistPanel.HAlign = 1f;
			checklistPanel.Top.Set(50f, 0f);
			checklistPanel.Width.Set(250f, 0f);
			checklistPanel.Height.Set(-100, 1f);
			checklistPanel.BackgroundColor = new Color(73, 94, 171);

			foundFilterButton = new UIHoverImageButton(Main.itemTexture[ItemID.Book], "Cycle Found Filter: ??");
			foundFilterButton.OnClick += ToggleFoundFilterButtonClicked;
			checklistPanel.Append(foundFilterButton);

			muteButton = new UIToggleHoverImageButton(Main.itemTexture[ItemID.Megaphone], ItemChecklist.instance.GetTexture("closeButton"), "Toggle Messages", announce);
			muteButton.OnClick += ToggleMuteButtonClicked;
			muteButton.Left.Pixels = spacing * 2 + 28;
			muteButton.Top.Pixels = 4;
			checklistPanel.Append(muteButton);

			sortButton = new UIHoverImageButton(Main.itemTexture[ItemID.ToxicFlask], "Cycle Sort Method: ??");
			sortButton.OnClick += ToggleSortButtonClicked;
			sortButton.Left.Pixels = spacing * 4 + 28 * 2;
			sortButton.Top.Pixels = 4;
			checklistPanel.Append(sortButton);

			modFilterButton = new UIHoverImageButton(ItemChecklist.instance.GetTexture("filterMod"), "Cycle Mod Filter: ??");
			modFilterButton.OnClick += ToggleModFilterButtonClicked;
			modFilterButton.Left.Pixels = spacing * 6 + 28 * 3;
			modFilterButton.Top.Pixels = 4;
			checklistPanel.Append(modFilterButton);

			checklistGrid = new UIGrid(5);
			checklistGrid.Top.Pixels = 32f + spacing;
			checklistGrid.Width.Set(-25f, 1f);
			checklistGrid.Height.Set(-32f, 1f);
			checklistGrid.ListPadding = 12f;
			checklistPanel.Append(checklistGrid);

			FixedUIScrollbar checklistListScrollbar = new FixedUIScrollbar();
			checklistListScrollbar.SetView(100f, 1000f);
			checklistListScrollbar.Top.Pixels = 32f + spacing;
			checklistListScrollbar.Height.Set(-32f - spacing, 1f);
			checklistListScrollbar.HAlign = 1f;
			checklistPanel.Append(checklistListScrollbar);
			checklistGrid.SetScrollbar(checklistListScrollbar);

			// Checklistlist populated when the panel is shown: UpdateCheckboxes()

			Append(checklistPanel);

			// load time impact, do this on first show?
			itemSlots = new ItemSlot[Main.itemName.Length];
			Item[] itemSlotItems = new Item[Main.itemName.Length];
			for (int i = 0; i < Main.itemName.Length; i++)
			{
				itemSlots[i] = new ItemSlot(i);
				itemSlotItems[i] = itemSlots[i].item;
			}

			FieldInfo inventoryGlowHue = typeof(Terraria.UI.ItemSlot).GetField("inventoryGlowHue", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo inventoryGlowTime = typeof(Terraria.UI.ItemSlot).GetField("inventoryGlowTime", BindingFlags.Static | BindingFlags.NonPublic);

			MethodInfo SortMethod = typeof(ItemSorting).GetMethod("Sort", BindingFlags.Static | BindingFlags.NonPublic);
			object[] parametersArray = new object[] { itemSlotItems, new int[0] };

			inventoryGlowHue.SetValue(null, new float[Main.itemName.Length]);
			inventoryGlowTime.SetValue(null, new int[Main.itemName.Length]);
			SortMethod.Invoke(null, parametersArray);
			inventoryGlowHue.SetValue(null, new float[58]);
			inventoryGlowTime.SetValue(null, new int[58]);

			int[] vanillaIDsInSortOrderTemp = itemSlotItems.Select((x) => x.type).ToArray();
			vanillaIDsInSortOrder = new int[Main.itemName.Length];
			for (int i = 0; i < Main.itemName.Length; i++)
			{
				vanillaIDsInSortOrder[i] = Array.FindIndex(vanillaIDsInSortOrderTemp, x => x == i);
			}

			modnames = new List<string>() { "All", "Vanilla" };
			modnames.AddRange(ModLoader.GetLoadedMods()/*.Where(x => x != "ModLoader")*/);

			updateneeded = true;
		}

		private void ToggleFoundFilterButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(SoundID.MenuTick);
			showCompleted = ++showCompleted % 3;
			foundFilterButton.hoverText = "Cycle Found Filter: " + foundFilterStrings[showCompleted];
			UpdateNeeded();
		}

		private void ToggleMuteButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			announce = !announce;
			Main.PlaySound(announce ? SoundID.MenuOpen : SoundID.MenuClose);
			muteButton.SetEnabled(announce);
		}

		private void ToggleSortButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(SoundID.MenuTick);
			sortMode = sortMode.Next();
			sortButton.hoverText = "Cycle Sort Method: " + sortMode.ToFriendlyString();
			UpdateNeeded();
		}

		private void ToggleModFilterButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(SoundID.MenuTick);
			currentMod = (currentMod + 1) % modnames.Count;
			modFilterButton.hoverText = "Cycle Mod Filter: " + modnames[currentMod];
			UpdateNeeded();
		}

		internal void RefreshPreferences()
		{
			foundFilterButton.hoverText = "Cycle Found Filter: " + foundFilterStrings[showCompleted];
			sortButton.hoverText = "Cycle Sort Method: " + sortMode.ToFriendlyString();
			modFilterButton.hoverText = "Cycle Mod Filter: " + modnames[currentMod];
			muteButton.SetEnabled(announce);
			UpdateNeeded();
		}

		private bool updateneeded;
		private int lastfoundID = -1;
		internal void UpdateNeeded(int lastfoundID = -1)
		{
			updateneeded = true;
			if (lastfoundID > 0)
			{
				this.lastfoundID = lastfoundID;
			}
		}

		// todo, items on load.
		internal void UpdateCheckboxes()
		{
			if (!updateneeded) { return; }
			updateneeded = false;
			checklistGrid.Clear();

			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>(ItemChecklist.instance);

			for (int i = 0; i < itemChecklistPlayer.findableItems.Length; i++)
			{
				if (itemChecklistPlayer.findableItems[i])
				{
					// filters here
					if ((showCompleted != 1 && itemChecklistPlayer.foundItem[i]) || (showCompleted != 2 && !itemChecklistPlayer.foundItem[i]))
					{
						if (PassModFilter(itemSlots[i]))
						{
							ItemSlot box = itemSlots[i];

							checklistGrid._items.Add(box);
							checklistGrid._innerList.Append(box);
						}
					}
				}
			}
			checklistGrid.UpdateOrder();
			checklistGrid._innerList.Recalculate();

			if (lastfoundID > 0)
			{
				checklistGrid.Recalculate();
				checklistGrid.Goto(delegate (UIElement element)
				{
					ItemSlot itemSlot = element as ItemSlot;
					return itemSlot != null && itemSlot.id == lastfoundID;
				}, true);
				lastfoundID = -1;
			}
		}

		public override void RecalculateChildren()
		{
			if (checklistGrid != null)
			{
				checklistPanel.Width.Pixels = checklistGrid.cols * 43f + 40f;
			}
			base.RecalculateChildren();
		}

		private bool PassModFilter(ItemSlot itemSlot)
		{
			if (currentMod == 0)
			{
				return true;
			}
			else if (currentMod == 1 && itemSlot.item.modItem == null)
			{
				return true;
			}
			else if (itemSlot.item.modItem != null && itemSlot.item.modItem.mod.Name == modnames[currentMod])
			{
				return true;
			}
			return false;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			UpdateCheckboxes();
			ItemChecklistUI.hoverText = "";
			Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (checklistPanel.ContainsPoint(MousePosition))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
		}
	}

	public enum SortModes
	{
		ID,
		Value,
		AZ,
		Rare,
		TerrariaSort,
	}

	public class FixedUIScrollbar : UIScrollbar
	{
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			UserInterface temp = UserInterface.ActiveInstance;
			UserInterface.ActiveInstance = ItemChecklist.ItemChecklistInterface;
			base.DrawSelf(spriteBatch);
			UserInterface.ActiveInstance = temp;
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			UserInterface temp = UserInterface.ActiveInstance;
			UserInterface.ActiveInstance = ItemChecklist.ItemChecklistInterface;
			base.MouseDown(evt);
			UserInterface.ActiveInstance = temp;
		}
	}

	public static class Extensions
	{
		public static T Next<T>(this T src) where T : struct
		{
			if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf<T>(Arr, src) + 1;
			return (Arr.Length == j) ? Arr[0] : Arr[j];
		}

		public static string ToFriendlyString(this SortModes sortmode)
		{
			switch (sortmode)
			{
				case SortModes.AZ:
					return "Alphabetically";
				case SortModes.ID:
					return "ID";
				case SortModes.Value:
					return "Value";
				case SortModes.Rare:
					return "Rarity";
				case SortModes.TerrariaSort:
					return "Terraria Sort";
			}
			return "Unknown Sort";
		}
	}
}
