using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader.UI;
using System.Reflection;
using Terraria.Graphics;

namespace ItemChecklist.UI
{
	class ItemChecklistUI : UIState
	{
		public UIHoverImageButton toggleButton;
		public UIToggleHoverImageButton muteButton;
		public UIPanel checklistPanel;
		//public UIList checklistList;
		public UIGrid checklistList;
		//	private FieldInfo uilistinnerlist;

		float spacing = 8f;
		public static bool visible = false;
		public static bool showCompleted = true;
		public static bool announce = true;
		public static string hoverText = "";

		ItemSlot[] itemSlots;

		public override void OnInitialize()
		{
			// Is initialize called? (Yes it is called on reload) I want to reset nicely with new character or new loaded mods: visible = false;

			//	uilistinnerlist = typeof(UIList).GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic);
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
			//toggleButton.Left.Pixels = spacing;
			//toggleButton.Top.Pixels = spacing;
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
			// TODO, game window resize issue  --> UserInterface.ActiveInstance.Recalculate(); on resize? new hook?
			// TODO, Scrollbar can't click
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
			//	var uilistinner = (UIElement)uilistinnerlist.GetValue(checklistList);

			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>(ItemChecklist.instance);
			//var itemChecklistPlayer = ItemChecklistPlayer.localInstance;

			UIElement element = new UIElement();
			int count = 0;
			for (int i = 0; i < itemChecklistPlayer.findableItems.Length; i++)
			{
				if (itemChecklistPlayer.findableItems[i])
				{
					if (showCompleted || !itemChecklistPlayer.foundItem[i])
					{

						//ItemSlot box = new ItemSlot(i);
						ItemSlot box = itemSlots[i];
						//keyImage.Left.Set(xOffset, 1f);
						//element.Append(keyImage);
						//count++;
						//if(count == 4)
						//{
						//	count = 0;
						//	element = new UIElement();
						//}
						//UIHoverImageButton box = new UIHoverImageButton(Main.itemTexture[i], Main.itemName[i]);

						//UICheckbox box = new UICheckbox(i, Main.itemName[i], 1f, false);
						//box.Selected = itemChecklistPlayer.foundItem[i];
						//checklistList.Add(box); n squared


						checklistList._items.Add(box);
						checklistList._innerList.Append(box);
						//					uilistinner.Append(box);
					}
				}
			}
			checklistList.UpdateOrder();
			checklistList._innerList.Recalculate();
			//			uilistinner.Recalculate();
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

	//public class FixedUIScrollbar : UIElement
	public class FixedUIScrollbar : UIScrollbar
	{
		//private float _viewPosition;
		//private float _viewSize = 1f;
		//private float _maxViewSize = 20f;
		//private bool _isDragging;
		//private bool _isHoveringOverHandle;
		//private float _dragYOffset;
		//private Texture2D _texture;
		//private Texture2D _innerTexture;

		//public float ViewPosition
		//{
		//	get
		//	{
		//		return this._viewPosition;
		//	}
		//	set
		//	{
		//		this._viewPosition = MathHelper.Clamp(value, 0f, this._maxViewSize - this._viewSize);
		//	}
		//}

		//public FixedUIScrollbar()
		//{
		//	this.Width.Set(20f, 0f);
		//	this.MaxWidth.Set(20f, 0f);
		//	this._texture = TextureManager.Load("Images/UI/Scrollbar");
		//	this._innerTexture = TextureManager.Load("Images/UI/ScrollbarInner");
		//	this.PaddingTop = 5f;
		//	this.PaddingBottom = 5f;
		//}

		//public void SetView(float viewSize, float maxViewSize)
		//{
		//	viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
		//	this._viewPosition = MathHelper.Clamp(this._viewPosition, 0f, maxViewSize - viewSize);
		//	this._viewSize = viewSize;
		//	this._maxViewSize = maxViewSize;
		//}

		//public float GetValue()
		//{
		//	return this._viewPosition;
		//}

		//private Rectangle GetHandleRectangle()
		//{
		//	CalculatedStyle innerDimensions = base.GetInnerDimensions();
		//	if (this._maxViewSize == 0f && this._viewSize == 0f)
		//	{
		//		this._viewSize = 1f;
		//		this._maxViewSize = 1f;
		//	}
		//	return new Rectangle((int)innerDimensions.X, (int)(innerDimensions.Y + innerDimensions.Height * (this._viewPosition / this._maxViewSize)) - 3, 20, (int)(innerDimensions.Height * (this._viewSize / this._maxViewSize)) + 7);
		//}

		//private void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
		//{
		//	spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle?(new Rectangle(0, 0, texture.Width, 6)), color);
		//	spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle?(new Rectangle(0, 9, texture.Width, 2)), color);
		//	spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle?(new Rectangle(0, texture.Height - 6, texture.Width, 6)), color);
		//}

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

		//public override void MouseUp(UIMouseEvent evt)
		//{
		//	base.MouseUp(evt);
		//	this._isDragging = false;
		//}
	}

	//public class BossInfo
	//{
	//	internal Func<bool> available;
	//	internal Func<bool> downed;
	//	internal string name;
	//	internal float progression;

	//	public BossInfo(string name, float progression, Func<bool> available, Func<bool> downed)
	//	{
	//		this.name = name;
	//		this.progression = progression;
	//		this.available = available;
	//		this.downed = downed;
	//	}
	//}
}
