using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ID;

namespace ItemChecklist.UI
{
	class ItemChecklistUI : UIState
	{
		public UIHoverImageButton toggleButton;
		public UIToggleHoverImageButton muteButton;
		public UIPanel checklistPanel;
		public UIGrid checklistList;

		float spacing = 8f;
		public static bool visible = false;
		public static bool showCompleted = true;
		public static bool announce = true;
		public static string hoverText = "";

		ItemSlot[] itemSlots;

		public override void OnInitialize()
		{
			// Is initialize called? (Yes it is called on reload) I want to reset nicely with new character or new loaded mods: visible = false;

			announce = true;

			checklistPanel = new UIPanel();
			checklistPanel.SetPadding(10);
			checklistPanel.Left.Pixels = 0;
			checklistPanel.HAlign = 1f;
			checklistPanel.Top.Set(50f, 0f);
			checklistPanel.Width.Set(250f, 0f);
			checklistPanel.Height.Set(-100, 1f);
			checklistPanel.BackgroundColor = new Color(73, 94, 171);

			toggleButton = new UIHoverImageButton(Main.itemTexture[ItemID.Book], "Toggle Found");
			toggleButton.OnClick += ToggleButtonClicked;
			checklistPanel.Append(toggleButton);

			muteButton = new UIToggleHoverImageButton(Main.itemTexture[ItemID.Megaphone], ItemChecklist.instance.GetTexture("closeButton"), "Toggle Messages", announce);
			muteButton.OnClick += ToggleMuteButtonClicked;
			muteButton.Left.Pixels = spacing * 2 + 28;
			muteButton.Top.Pixels = 4;
			checklistPanel.Append(muteButton);

			checklistList = new UIGrid(5);
			checklistList.Top.Pixels = 32f + spacing;
			checklistList.Width.Set(-25f, 1f);
			checklistList.Height.Set(-32f, 1f);
			checklistList.ListPadding = 12f;
			checklistPanel.Append(checklistList);

			FixedUIScrollbar checklistListScrollbar = new FixedUIScrollbar();
			checklistListScrollbar.SetView(100f, 1000f);
			checklistListScrollbar.Top.Pixels = 32f + spacing;
			checklistListScrollbar.Height.Set(-32f - spacing, 1f);
			checklistListScrollbar.HAlign = 1f;
			checklistPanel.Append(checklistListScrollbar);
			checklistList.SetScrollbar(checklistListScrollbar);

			// Checklistlist populated when the panel is shown: UpdateCheckboxes()

			Append(checklistPanel);

			// load time impact, do this on first show?
			itemSlots = new ItemSlot[Main.itemName.Length];
			for (int i = 0; i < Main.itemName.Length; i++)
			{
				itemSlots[i] = new ItemSlot(i);
			}
			updateneeded = true;
		}

		private void ToggleButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(10, -1, -1, 1);
			showCompleted = !showCompleted;
			UpdateNeeded();
		}

		private void ToggleMuteButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(10, -1, -1, 1);
			announce = !announce;
			muteButton.SetEnabled(announce);
		}

		private bool updateneeded;
		internal void UpdateNeeded()
		{
			updateneeded = true;
		}

		// todo, items on load.
		internal void UpdateCheckboxes()
		{
			if (!updateneeded) { return; }
			updateneeded = false;
			checklistList.Clear();

			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>(ItemChecklist.instance);

			UIElement element = new UIElement();
			for (int i = 0; i < itemChecklistPlayer.findableItems.Length; i++)
			{
				if (itemChecklistPlayer.findableItems[i])
				{
					if (showCompleted || !itemChecklistPlayer.foundItem[i])
					{

						ItemSlot box = itemSlots[i];

						checklistList._items.Add(box);
						checklistList._innerList.Append(box);
					}
				}
			}
			checklistList.UpdateOrder();
			checklistList._innerList.Recalculate();
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
}
