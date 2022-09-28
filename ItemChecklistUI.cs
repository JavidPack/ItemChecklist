using ItemChecklist.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

using UIItemSlot = ItemChecklist.UIElements.UIItemSlot;
using Steamworks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ItemChecklist
{
	class ItemChecklistUI : UIState
	{
		internal static ItemChecklistUI instance;
		public UIHoverImageButton foundFilterButton;
		public UIToggleHoverImageButton muteButton;
		//public UIHoverImageButton sortButton;
		public UIHoverImageButton modFilterButton;
		public UIToggleHoverImageButton collectChestItemsButton;
		public UIToggleHoverImageButton showBadgeButton;
		private bool buttonsHaveDummyTextures;
		public UIDragableElement mainPanel;
		public UIPanel checklistPanel;
		internal NewUITextBox itemNameFilter;
		internal NewUITextBox itemDescriptionFilter;
		internal SharedUI sharedUI;
		public UIGrid checklistGrid;
		public UICollectionBar collectionBar;
		//public static SortModes sortMode = SortModes.TerrariaSort;

		float spacing = 8f;
		public static bool Visible
		{
			get { return ItemChecklist.ItemChecklistInterface.CurrentState == ItemChecklist.instance.ItemChecklistUI; }
			set { ItemChecklist.ItemChecklistInterface.SetState(value ? ItemChecklist.instance.ItemChecklistUI : null); }
		}
		public static int showCompleted = 0; // 0: both, 1: unfound, 2: found
		public static bool announce = true;
		public static bool collectChestItems = true;
		public static bool showBadge = true;
		public static string hoverText = "";

		UIItemSlot[] itemSlots;
		internal static int[] vanillaIDsInSortOrder;

		internal List<string> modnames;
		internal int currentMod = 0;

		public string[] foundFilterStrings = { "All", "Unfound", "Found" };

		public ItemChecklistUI()
		{
			instance = this;
		}

		public override void OnInitialize()
		{
			// Is initialize called? (Yes it is called on reload) I want to reset nicely with new character or new loaded mods: visible = false;

			announce = false; // overwritten by modplayer
			collectChestItems = false;
			showBadge = false;

			mainPanel = new UIDragableElement();
			//mainPanel.SetPadding(0);
			//mainPanel.PaddingTop = 4;
			mainPanel.Left.Set(400f, 0f);
			mainPanel.Top.Set(400f, 0f);
			mainPanel.Width.Set(475f, 0f); // + 30
			mainPanel.MinWidth.Set(415f, 0f);
			mainPanel.MaxWidth.Set(884f, 0f);
			mainPanel.Height.Set(370, 0f);
			mainPanel.MinHeight.Set(263, 0f);
			mainPanel.MaxHeight.Set(1000, 0f);
			//mainPanel.BackgroundColor = Color.LightBlue;

			var config = ModContent.GetInstance<ItemChecklistClientConfig>();
			mainPanel.Left.Set(config.ItemChecklistPosition.X, 0f);
			mainPanel.Top.Set(config.ItemChecklistPosition.Y, 0f);
			mainPanel.Width.Set(config.ItemChecklistSize.X, 0f);
			mainPanel.Height.Set(config.ItemChecklistSize.Y, 0f);

			int top = 0;
			int left = 0;
			
			checklistPanel = new UIPanel();
			checklistPanel.SetPadding(10);
			checklistPanel.BackgroundColor = new Color(73, 94, 171);;
			checklistPanel.Top.Set(0, 0f);
			checklistPanel.Height.Set(0, 1f);
			checklistPanel.Width.Set(0, 1f);
			mainPanel.Append(checklistPanel);
			mainPanel.AddDragTarget(checklistPanel);

			// Because OnInitialize Happens during load and because we need it to happen for OnEnterWorld, use dummy sprite.
			buttonsHaveDummyTextures = true;

			foundFilterButton = new UIHoverImageButton(TextureAssets.MagicPixel.Value, "Cycle Found Filter: ??");
			foundFilterButton.OnClick += (a, b) => ToggleFoundFilterButtonClicked(a, b, true);
			foundFilterButton.OnRightClick += (a, b) => ToggleFoundFilterButtonClicked(a, b, false);
			foundFilterButton.Top.Pixels = top;
			checklistPanel.Append(foundFilterButton);
			left += (int)spacing * 2 + 28;

			//sortButton = new UIHoverImageButton(sortButtonTexture, "Cycle Sort Method: ??");
			//sortButton.OnClick += (a, b) => ToggleSortButtonClicked(a, b, true);
			//sortButton.OnRightClick += (a, b) => ToggleSortButtonClicked(a, b, false);
			//sortButton.Left.Pixels = spacing * 4 + 28 * 2;
			//sortButton.Top.Pixels = top;
			//checklistPanel.Append(sortButton);

			modFilterButton = new UIHoverImageButton(TextureAssets.MagicPixel.Value, "Cycle Mod Filter: ??");
			modFilterButton.OnClick += (a, b) => ToggleModFilterButtonClicked(a, b, true);
			modFilterButton.OnRightClick += (a, b) => ToggleModFilterButtonClicked(a, b, false);
			modFilterButton.Left.Pixels = left;
			modFilterButton.Top.Pixels = top;
			checklistPanel.Append(modFilterButton);
			left += (int)spacing * 2 + 28;

			muteButton = new UIToggleHoverImageButton(TextureAssets.MagicPixel.Value, ItemChecklist.instance.Assets.Request<Texture2D>("UIElements/closeButton", AssetRequestMode.ImmediateLoad).Value, "Toggle Messages", announce);
			muteButton.OnClick += ToggleMuteButtonClicked;
			muteButton.Left.Pixels = left;
			muteButton.Top.Pixels = top;
			checklistPanel.Append(muteButton);
			left += (int)spacing * 2 + 28;

			collectChestItemsButton = new UIToggleHoverImageButton(TextureAssets.MagicPixel.Value, ItemChecklist.instance.Assets.Request<Texture2D>("UIElements/closeButton", AssetRequestMode.ImmediateLoad).Value, "Toggle Collect Chest Items", collectChestItems);
			collectChestItemsButton.OnClick += ToggleFindChestItemsButtonClicked;
			collectChestItemsButton.Left.Pixels = left;
			collectChestItemsButton.Top.Pixels = top;
			checklistPanel.Append(collectChestItemsButton);
			left += (int)spacing * 2 + 28;

			showBadgeButton = new UIToggleHoverImageButton(TextureAssets.MagicPixel.Value, ItemChecklist.instance.Assets.Request<Texture2D>("UIElements/closeButton", AssetRequestMode.ImmediateLoad).Value, "Show Sort Value Text", showBadge);
			showBadgeButton.OnClick += ToggleShowBadgeButtonClicked;
			showBadgeButton.Left.Pixels = left;
			showBadgeButton.Top.Pixels = top;
			checklistPanel.Append(showBadgeButton);
			left += (int)spacing * 2 + 28;

			top += 34;

			itemNameFilter = new NewUITextBox("Filter by Name");
			itemNameFilter.OnTextChanged += () => { ValidateItemFilter(); updateNeeded = true; };
			itemNameFilter.OnTabPressed += () => { itemDescriptionFilter.Focus(); };
			itemNameFilter.Top.Pixels = top;
			//itemNameFilter.Left.Set(-152, 1f);
			itemNameFilter.Width.Set(0, 1f);
			itemNameFilter.Height.Set(25, 0f);
			checklistPanel.Append(itemNameFilter);

			top += 30;

			itemDescriptionFilter = new NewUITextBox("Filter by tooltip");
			itemDescriptionFilter.OnTextChanged += () => { ValidateItemDescription(); updateNeeded = true; };
			itemDescriptionFilter.OnTabPressed += () => { itemNameFilter.Focus(); };
			itemDescriptionFilter.Top.Pixels = top;
			//itemDescriptionFilter.Left.Set(-152, 1f);
			itemDescriptionFilter.Width.Set(0, 1f);
			itemDescriptionFilter.Height.Set(25, 0f);
			checklistPanel.Append(itemDescriptionFilter);

			top += 30;

			sharedUI = new SharedUI();
			sharedUI.Initialize();

			sharedUI.sortsAndFiltersPanel.Top.Set(top, 0f);
			sharedUI.sortsAndFiltersPanel.Width.Set(0f, 1);
			sharedUI.sortsAndFiltersPanel.Height.Set(60, 0f);
			checklistPanel.Append(sharedUI.sortsAndFiltersPanel);

			top += 68;

			checklistGrid = new UIGrid();
			checklistGrid.alternateSort = ItemGridSort;
			checklistGrid.Top.Pixels = top;
			checklistGrid.Width.Set(-25f, 1f);
			checklistGrid.Height.Set(-top - 24, 1f);
			checklistGrid.ListPadding = 2f;
			checklistPanel.Append(checklistGrid);

			FixedUIScrollbar checklistListScrollbar = new FixedUIScrollbar();
			checklistListScrollbar.SetView(100f, 1000f);
			checklistListScrollbar.Top.Pixels = top;
			checklistListScrollbar.Height.Set(-top - 24, 1f);
			checklistListScrollbar.HAlign = 1f;
			checklistPanel.Append(checklistListScrollbar);
			checklistGrid.SetScrollbar(checklistListScrollbar);

			// Checklistlist populated when the panel is shown: UpdateCheckboxes()
			collectionBar = new UICollectionBar() {
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(15f, 0f),
				VAlign = 1f,
			};

			checklistPanel.Append(collectionBar);

			Append(mainPanel);

			// load time impact, do this on first show?
			itemSlots = new UIItemSlot[ItemLoader.ItemCount];
			Item[] itemSlotItems = new Item[ItemLoader.ItemCount];
			for (int i = 0; i < ItemLoader.ItemCount; i++)
			{
				Item item = ContentSamples.ItemsByType[i];
				if (item.type == ItemID.None)
					continue;
				itemSlots[i] = new UIItemSlot(item, i);
				itemSlotItems[i] = item;
			}

			FieldInfo inventoryGlowHue = typeof(Terraria.UI.ItemSlot).GetField("inventoryGlowHue", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo inventoryGlowTime = typeof(Terraria.UI.ItemSlot).GetField("inventoryGlowTime", BindingFlags.Static | BindingFlags.NonPublic);

			MethodInfo SortMethod = typeof(ItemSorting).GetMethod("Sort", BindingFlags.Static | BindingFlags.NonPublic);
			object[] parametersArray = new object[] { itemSlotItems, new int[0] };

			inventoryGlowHue.SetValue(null, new float[ItemLoader.ItemCount]);
			inventoryGlowTime.SetValue(null, new int[ItemLoader.ItemCount]);
			//SortMethod.Invoke(null, parametersArray);
			inventoryGlowHue.SetValue(null, new float[58]);
			inventoryGlowTime.SetValue(null, new int[58]);

			int[] vanillaIDsInSortOrderTemp = itemSlotItems.Where(x => x != null).Select((x) => x.type).ToArray();
			vanillaIDsInSortOrder = new int[ItemLoader.ItemCount];
			for (int i = 0; i < ItemLoader.ItemCount; i++)
			{
				vanillaIDsInSortOrder[i] = Array.FindIndex(vanillaIDsInSortOrderTemp, x => x == i);
			}

			List<Item> list = ContentSamples.ItemsByType.Values.ToList();
			IOrderedEnumerable<IGrouping<ContentSamples.CreativeHelper.ItemGroup, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup>> orderedEnumerable = from x in list
																								   select new ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup(x) into x
																								   group x by x.Group into @group
																								   orderby (int)@group.Key
																								   select @group;
			int order = 0;
			foreach (IGrouping<ContentSamples.CreativeHelper.ItemGroup, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> item2 in orderedEnumerable) {
				foreach (ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup item3 in item2) {
					//vanillaIDsInSortOrder[order] = item3.ItemType;
					// TODO...rename?
					vanillaIDsInSortOrder[item3.ItemType] = order;
					order++;
				}
			}


			modnames = new List<string>() { "All", "Vanilla" };
			modnames.AddRange(ModLoader.Mods.Where(mod => mod.GetContent<ModItem>().Any()).Select(x => x.Name)/*.Where(x => x != "ModLoader")*/);

			updateNeeded = true;
		}

		private int ItemGridSort(UIElement x, UIElement y)
		{
			UIItemSlot a = x as UIItemSlot;
			UIItemSlot b = y as UIItemSlot;
			if (SharedUI.instance.SelectedSort != null)
				return SharedUI.instance.SelectedSort.sort(a.item, b.item);
			return a.item.type.CompareTo(b.item.type);
		}

		private void ToggleFoundFilterButtonClicked(UIMouseEvent evt, UIElement listeningElement, bool left)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			showCompleted = (3 + showCompleted + (left ? 1 : -1)) % 3;
			foundFilterButton.hoverText = "Cycle Found Filter: " + foundFilterStrings[showCompleted];
			UpdateNeeded();
		}

		private void ToggleMuteButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			announce = !announce;
			SoundEngine.PlaySound(announce ? SoundID.MenuOpen : SoundID.MenuClose);
			muteButton.SetEnabled(announce);
		}

		//private void ToggleSortButtonClicked(UIMouseEvent evt, UIElement listeningElement, bool left)
		//{
		//	Main.PlaySound(SoundID.MenuTick);
		//	sortMode = left ? sortMode.NextEnum() : sortMode.PreviousEnum();
		//	sortButton.hoverText = "Cycle Sort Method: " + sortMode.ToFriendlyString();
		//	UpdateNeeded();
		//}

		private void ToggleModFilterButtonClicked(UIMouseEvent evt, UIElement listeningElement, bool left)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			currentMod = (modnames.Count + currentMod + (left ? 1 : -1)) % modnames.Count;
			modFilterButton.hoverText = "Cycle Mod Filter: " + modnames[currentMod];
			UpdateNeeded();
		}

		private void ToggleShowBadgeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			showBadge = !showBadge;
			SoundEngine.PlaySound(showBadge ? SoundID.MenuOpen : SoundID.MenuClose);
			showBadgeButton.SetEnabled(showBadge);
		}

		private void ToggleFindChestItemsButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			collectChestItems = !collectChestItems;
			SoundEngine.PlaySound(collectChestItems ? SoundID.MenuOpen : SoundID.MenuClose);
			collectChestItemsButton.SetEnabled(collectChestItems);
		}

		internal void RefreshPreferences()
		{
			foundFilterButton.hoverText = "Cycle Found Filter: " + foundFilterStrings[showCompleted];
			//sortButton.hoverText = "Cycle Sort Method: " + sortMode.ToFriendlyString();
			modFilterButton.hoverText = "Cycle Mod Filter: " + modnames[currentMod];
			muteButton.SetEnabled(announce);
			collectChestItemsButton.SetEnabled(collectChestItems);
			UpdateNeeded();
		}

		private bool updateNeeded;
		private int lastfoundID = -1;
		internal void UpdateNeeded(int lastfoundID = -1)
		{
			updateNeeded = true;
			if (lastfoundID > 0)
			{
				this.lastfoundID = lastfoundID;
			}
		}

		// todo, items on load.
		internal void UpdateCheckboxes()
		{
			if (!updateNeeded) { return; }
			updateNeeded = false;
			checklistGrid.Clear();
			collectionBar.RecalculateBars();

			if (buttonsHaveDummyTextures)
			{
				Main.instance.LoadItem(ItemID.Megaphone);
				Main.instance.LoadItem(ItemID.Book);

				Texture2D foundFilterTexture = Utilities.ResizeImage(ItemChecklist.instance.Assets.Request<Texture2D>("Images/filterFound", AssetRequestMode.ImmediateLoad), 32, 32);
				Texture2D muteButtonTexture = Utilities.ResizeImage(TextureAssets.Item[ItemID.Megaphone], 32, 32);
				//Texture2D sortButtonTexture = Utilities.ResizeImage(Main.itemTexture[ItemID.ToxicFlask], 32, 32);
				Texture2D modFilterButtonTexture = Utilities.ResizeImage(ItemChecklist.instance.Assets.Request<Texture2D>("Images/filterMod", AssetRequestMode.ImmediateLoad), 32, 32);
				Texture2D collectChestItemsButtonTexture = Utilities.ResizeImage(TextureAssets.Cursors[8], 32, 32);
				Texture2D showBadgeButtonTexture = Utilities.ResizeImage(TextureAssets.Item[ItemID.Book], 32, 32); // Main.extraTexture[58]

				foundFilterButton.SetImage(foundFilterTexture);
				muteButton.SetImage(muteButtonTexture);
				modFilterButton.SetImage(modFilterButtonTexture);
				collectChestItemsButton.SetImage(collectChestItemsButtonTexture);
				showBadgeButton.SetImage(showBadgeButtonTexture);

				buttonsHaveDummyTextures = false;
			}

			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>();
			var temp = new List<UIItemSlot>();
			for (int i = 0; i < itemChecklistPlayer.findableItems.Length; i++)
			{
				if (itemChecklistPlayer.findableItems[i])
				{
					if (itemSlots[i] == null) continue;

					// filters here
					if ((showCompleted != 1 && itemChecklistPlayer.foundItem[i]) || (showCompleted != 2 && !itemChecklistPlayer.foundItem[i]))
					{
						if (!PassModFilter(itemSlots[i]))
							continue;

						if (SharedUI.instance.SelectedCategory != null)
						{
							if (!SharedUI.instance.SelectedCategory.belongs(itemSlots[i].item) && !SharedUI.instance.SelectedCategory.subCategories.Any(x => x.belongs(itemSlots[i].item)))
								continue;
						}

						bool filtered = false;
						foreach (var filter in SharedUI.instance.availableFilters)
						{
							if (filter.button.selected)
								if (!filter.belongs(itemSlots[i].item))
									filtered = true;
						}
						if (filtered)
							continue;

						if (itemSlots[i].item.Name.IndexOf(itemNameFilter.currentString, StringComparison.OrdinalIgnoreCase) == -1)
							continue;

						if (itemDescriptionFilter.currentString.Length > 0)
						{
							if ((itemSlots[i].item.ToolTip != null && GetTooltipsAsString(itemSlots[i].item.ToolTip).IndexOf(itemDescriptionFilter.currentString, StringComparison.OrdinalIgnoreCase) != -1) /*|| (recipe.createItem.toolTip2 != null && recipe.createItem.toolTip2.ToLower().IndexOf(itemDescriptionFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)*/)
							{
							}
							else
							{
								continue;
							}
						}

						UIItemSlot box = itemSlots[i];
						box.badge = SharedUI.instance.SelectedSort?.badge(box.item) ?? "";
						temp.Add(box);
					}
				}
			}
			checklistGrid.AddRange(temp);

			if (lastfoundID > 0)
			{
				checklistGrid.Recalculate();
				if (showCompleted != 1) // Don't Goto when unfound is the display mode.
				{
					checklistGrid.Goto(delegate (UIElement element)
					{
						UIItemSlot itemSlot = element as UIItemSlot;
						return itemSlot != null && itemSlot.itemType == lastfoundID;
					}, true);
				}
				lastfoundID = -1;
			}
		}

		private bool PassModFilter(UIItemSlot itemSlot)
		{
			if (currentMod == 0)
			{
				return true;
			}
			else if (currentMod == 1 && itemSlot.item.ModItem == null)
			{
				return true;
			}
			else if (itemSlot.item.ModItem != null && itemSlot.item.ModItem.Mod.Name == modnames[currentMod])
			{
				return true;
			}
			return false;
		}

		private void ValidateItemFilter()
		{
			if (itemNameFilter.currentString.Length > 0)
			{
				bool found = false;
				foreach (var itemSlot in itemSlots)
				{
					if (itemSlot == null) continue;

					if (itemSlot.item.Name.IndexOf(itemNameFilter.currentString, StringComparison.OrdinalIgnoreCase) != -1)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					itemNameFilter.SetText(itemNameFilter.currentString.Substring(0, itemNameFilter.currentString.Length - 1));
				}
			}
			updateNeeded = true;
		}

		private void ValidateItemDescription()
		{
			updateNeeded = true;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (buttonsHaveDummyTextures)
				return;
			base.Draw(spriteBatch);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			ItemChecklistUI.hoverText = "";
			Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (mainPanel.ContainsPoint(MousePosition))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			sharedUI.Update();
			UpdateCheckboxes();
		}

		private string GetTooltipsAsString(ItemTooltip toolTip)
		{
			StringBuilder sb = new StringBuilder();
			for (int j = 0; j < toolTip.Lines; j++)
			{
				sb.Append(toolTip.GetLine(j) + "\n");
			}
			return sb.ToString().ToLower();
		}

		internal static void OnScrollWheel_FixHotbarScroll(UIScrollWheelEvent evt, UIElement listeningElement)
		{
			Main.LocalPlayer.ScrollHotbar(Terraria.GameInput.PlayerInput.ScrollWheelDelta / 120);
		}
	}

	//public enum SortModes
	//{
	//	ID,
	//	Value,
	//	AZ,
	//	Rare,
	//	TerrariaSort,
	//}

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

	//public static class Extensions
	//{
	//	public static string ToFriendlyString(this SortModes sortmode)
	//	{
	//		switch (sortmode)
	//		{
	//			case SortModes.AZ:
	//				return "Alphabetically";
	//			case SortModes.ID:
	//				return "ID";
	//			case SortModes.Value:
	//				return "Value";
	//			case SortModes.Rare:
	//				return "Rarity";
	//			case SortModes.TerrariaSort:
	//				return "Terraria Sort";
	//		}
	//		return "Unknown Sort";
	//	}
	//}
}
