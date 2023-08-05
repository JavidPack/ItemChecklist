using ItemChecklist.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;

using static ItemChecklist.Utilities;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

// Copied from my Recipe Browser mod.
namespace ItemChecklist
{
	class SharedUI
	{
		internal static SharedUI instance;
		internal bool updateNeeded;

		internal UIPanel sortsAndFiltersPanel;
		internal UIHorizontalGrid categoriesGrid;
		internal InvisibleFixedUIHorizontalScrollbar categoriesGridScrollbar;
		internal UIHorizontalGrid subCategorySortsFiltersGrid;
		internal InvisibleFixedUIHorizontalScrollbar lootGridScrollbar2;

		private Sort selectedSort;
		internal Sort SelectedSort {
			get { return selectedSort; }
			set {
				if (selectedSort != value) {
					updateNeeded = true;
					ItemChecklistUI.instance.UpdateNeeded();
				}
				selectedSort = value;
			}
		}

		private Category selectedCategory;
		internal Category SelectedCategory {
			get { return selectedCategory; }
			set {
				if (selectedCategory != value) {
					updateNeeded = true;
					ItemChecklistUI.instance.UpdateNeeded();
				}
				selectedCategory = value;
				if (selectedCategory != null && selectedCategory.sorts.Count > 0)
					SelectedSort = selectedCategory.sorts[0];
				else if (selectedCategory != null && selectedCategory.parent != null && selectedCategory.parent.sorts.Count > 0)
					SelectedSort = selectedCategory.parent.sorts[0];
			}
		}

		private const string LocalizationKey = "Mods.ItemChecklist.SharedUI.";
		private static LocalizedText cycleAmmoTypesText;
		private static LocalizedText cycleUsedAmmoTypesText;
		private static LocalizedText mutuallyExclusiveFilterVanityText;
		private static LocalizedText mutuallyExclusiveFilterArmorText;
		private static LocalizedText categoryAllText;
		private static LocalizedText categoryMeleeText;
		private static LocalizedText categoryYoyoText;
		private static LocalizedText categoryMagicText;
		private static LocalizedText categoryRangedText;
		private static LocalizedText sortUseAmmoTypeText;
		private static LocalizedText categoryThrowingText;
		private static LocalizedText categorySummonText;
		private static LocalizedText categorySentryText;
		private static LocalizedText sortDamageText;
		private static LocalizedText categoryToolsText;
		private static LocalizedText categoryPickaxesText;
		private static LocalizedText categoryAxesText;
		private static LocalizedText categoryHammersText;
		private static LocalizedText categoryArmorText;
		private static LocalizedText categoryWeaponsText;
		private static LocalizedText sortItemIdText;
		private static LocalizedText sortValueText;
		private static LocalizedText sortAlphabeticalText;
		private static LocalizedText sortRarityText;
		private static LocalizedText sortTerrariaSortText;
		private static LocalizedText filterMaterialsText;
		private static LocalizedText sortPickPower;
		private static LocalizedText sortAxePower;
		private static LocalizedText sortHammerPower;
		private static LocalizedText categoryHead;
		private static LocalizedText categoryBody;
		private static LocalizedText categoryLegs;
		private static LocalizedText sortDefense;
		private static LocalizedText categoryTiles;
		private static LocalizedText categoryContainersText;
		private static LocalizedText categoryWiringText;
		private static LocalizedText categoryStatuesText;
		private static LocalizedText categoryDoorsText;
		private static LocalizedText categoryChairsText;
		private static LocalizedText categoryTablesText;
		private static LocalizedText categoryLightSourcesText;
		private static LocalizedText categoryTorchesText;
		private static LocalizedText categoryWallsText;
		private static LocalizedText categoryAccessoriesText;
		private static LocalizedText categoryWingsText;
		private static LocalizedText categoryAmmoText;
		private static LocalizedText categoryPotionsText;
		private static LocalizedText categoryHealthPotionsText;
		private static LocalizedText categoryManaPotionsText;
		private static LocalizedText categoryBuffPotionsText;
		private static LocalizedText categoryExpertText;
		private static LocalizedText categoryPetsText;
		private static LocalizedText categoryLightPetsText;
		private static LocalizedText categoryMountsText;
		private static LocalizedText categoryCartsText;
		private static LocalizedText categoryHooksText;
		private static LocalizedText categoryDyesText;
		private static LocalizedText categoryHairDyesText;
		private static LocalizedText categoryBossSummonsText;
		private static LocalizedText categoryConsumablesText;
		private static LocalizedText categoryCapturedNPCText;
		private static LocalizedText categoryFishingText;
		private static LocalizedText categoryPolesText;
		private static LocalizedText categoryBaitText;
		private static LocalizedText categoryQuestFishText;
		private static LocalizedText categoryExtractinatorText;
		private static LocalizedText categoryOtherText;
		private static LocalizedText sortPlaceTileText;
		private static LocalizedText sortAmmoTypeText;
		private static LocalizedText sortHealLifeText;
		private static LocalizedText sortHealManaText;
		private static LocalizedText sortGrappleRangeText;
		private static LocalizedText sortProgressionOrderText;
		private static LocalizedText sortPolePowerText;
		private static LocalizedText sortBaitPowerText;
        
		public SharedUI() {
			instance = this;

			mutuallyExclusiveFilterVanityText = Language.GetOrRegister( LocalizationKey + nameof(mutuallyExclusiveFilterVanityText) );
			mutuallyExclusiveFilterArmorText = Language.GetOrRegister( LocalizationKey + nameof(mutuallyExclusiveFilterArmorText) );
			cycleAmmoTypesText = Language.GetOrRegister( LocalizationKey + nameof(cycleAmmoTypesText) );
			cycleUsedAmmoTypesText = Language.GetOrRegister( LocalizationKey + nameof(cycleUsedAmmoTypesText) );
			categoryAllText = Language.GetOrRegister( LocalizationKey + nameof(categoryAllText) );
			categoryMeleeText = Language.GetOrRegister( LocalizationKey + nameof(categoryMeleeText) );
			categoryYoyoText = Language.GetOrRegister( LocalizationKey + nameof(categoryYoyoText) );
			categoryMagicText = Language.GetOrRegister( LocalizationKey + nameof(categoryMagicText) );
			categoryRangedText = Language.GetOrRegister( LocalizationKey + nameof(categoryRangedText) );
			sortUseAmmoTypeText = Language.GetOrRegister( LocalizationKey + nameof(sortUseAmmoTypeText) );
			categoryThrowingText = Language.GetOrRegister( LocalizationKey + nameof(categoryThrowingText) );
			categorySummonText = Language.GetOrRegister( LocalizationKey + nameof(categorySummonText) );
			categorySentryText = Language.GetOrRegister( LocalizationKey + nameof(categorySentryText) );
			sortDamageText = Language.GetOrRegister( LocalizationKey + nameof(sortDamageText) );
			categoryToolsText = Language.GetOrRegister( LocalizationKey + nameof(categoryToolsText) );
			categoryPickaxesText = Language.GetOrRegister( LocalizationKey + nameof(categoryPickaxesText) );
			categoryAxesText = Language.GetOrRegister( LocalizationKey + nameof(categoryAxesText) );
			categoryHammersText = Language.GetOrRegister( LocalizationKey + nameof(categoryHammersText) );
			categoryArmorText = Language.GetOrRegister( LocalizationKey + nameof(categoryArmorText) );
			categoryWeaponsText = Language.GetOrRegister( LocalizationKey + nameof(categoryWeaponsText) );
			sortValueText = Language.GetOrRegister( LocalizationKey + nameof(sortValueText) );
			sortAlphabeticalText = Language.GetOrRegister( LocalizationKey + nameof(sortAlphabeticalText) );
			sortRarityText = Language.GetOrRegister( LocalizationKey + nameof(sortRarityText) );
			sortTerrariaSortText = Language.GetOrRegister( LocalizationKey + nameof(sortTerrariaSortText) );
			filterMaterialsText = Language.GetOrRegister( LocalizationKey + nameof(filterMaterialsText) );
			sortPickPower = Language.GetOrRegister( LocalizationKey + nameof(sortPickPower) );
			sortAxePower = Language.GetOrRegister( LocalizationKey + nameof(sortAxePower) );
			sortHammerPower = Language.GetOrRegister( LocalizationKey + nameof(sortHammerPower) );
			categoryHead = Language.GetOrRegister( LocalizationKey + nameof(categoryHead) );
			categoryBody = Language.GetOrRegister( LocalizationKey + nameof(categoryBody) );
			categoryLegs = Language.GetOrRegister( LocalizationKey + nameof(categoryLegs) );
			sortDefense = Language.GetOrRegister( LocalizationKey + nameof(sortDefense) );
			categoryTiles = Language.GetOrRegister( LocalizationKey + nameof(categoryTiles) );
			categoryContainersText = Language.GetOrRegister( LocalizationKey + nameof(categoryContainersText) );
			categoryWiringText = Language.GetOrRegister( LocalizationKey + nameof(categoryWiringText) );
			categoryStatuesText = Language.GetOrRegister( LocalizationKey + nameof(categoryStatuesText) );
			categoryDoorsText = Language.GetOrRegister( LocalizationKey + nameof(categoryDoorsText) );
			categoryChairsText = Language.GetOrRegister( LocalizationKey + nameof(categoryChairsText) );
			categoryTablesText = Language.GetOrRegister( LocalizationKey + nameof(categoryTablesText) );
			categoryLightSourcesText = Language.GetOrRegister( LocalizationKey + nameof(categoryLightSourcesText) );
			categoryTorchesText = Language.GetOrRegister( LocalizationKey + nameof(categoryTorchesText) );
			categoryWallsText = Language.GetOrRegister( LocalizationKey + nameof(categoryWallsText) );
			categoryAccessoriesText = Language.GetOrRegister( LocalizationKey + nameof(categoryAccessoriesText) );
			categoryWingsText = Language.GetOrRegister( LocalizationKey + nameof(categoryWingsText) );
			categoryAmmoText = Language.GetOrRegister( LocalizationKey + nameof(categoryAmmoText) );
			categoryPotionsText = Language.GetOrRegister( LocalizationKey + nameof(categoryPotionsText) );
			categoryHealthPotionsText = Language.GetOrRegister( LocalizationKey + nameof(categoryHealthPotionsText) );
			categoryManaPotionsText = Language.GetOrRegister( LocalizationKey + nameof(categoryManaPotionsText) );
			categoryBuffPotionsText = Language.GetOrRegister( LocalizationKey + nameof(categoryBuffPotionsText) );
			categoryExpertText = Language.GetOrRegister( LocalizationKey + nameof(categoryExpertText) );
			categoryPetsText = Language.GetOrRegister( LocalizationKey + nameof(categoryPetsText) );
			categoryPetsText = Language.GetOrRegister( LocalizationKey + nameof(categoryPetsText) );
			categoryLightPetsText = Language.GetOrRegister( LocalizationKey + nameof(categoryLightPetsText) );
			categoryMountsText = Language.GetOrRegister( LocalizationKey + nameof(categoryMountsText) );
			categoryCartsText = Language.GetOrRegister( LocalizationKey + nameof(categoryCartsText) );
			categoryHooksText = Language.GetOrRegister( LocalizationKey + nameof(categoryHooksText) );
			categoryDyesText = Language.GetOrRegister( LocalizationKey + nameof(categoryDyesText) );
			categoryDyesText = Language.GetOrRegister( LocalizationKey + nameof(categoryDyesText) );
			categoryHairDyesText = Language.GetOrRegister( LocalizationKey + nameof(categoryHairDyesText) );
			categoryBossSummonsText = Language.GetOrRegister( LocalizationKey + nameof(categoryBossSummonsText) );
			categoryConsumablesText = Language.GetOrRegister( LocalizationKey + nameof(categoryConsumablesText) );
			categoryCapturedNPCText = Language.GetOrRegister( LocalizationKey + nameof(categoryCapturedNPCText) );
			categoryFishingText = Language.GetOrRegister( LocalizationKey + nameof(categoryFishingText) );
			categoryPolesText = Language.GetOrRegister( LocalizationKey + nameof(categoryPolesText) );
			categoryBaitText = Language.GetOrRegister( LocalizationKey + nameof(categoryBaitText) );
			categoryQuestFishText = Language.GetOrRegister( LocalizationKey + nameof(categoryQuestFishText) );
			categoryExtractinatorText = Language.GetOrRegister( LocalizationKey + nameof(categoryExtractinatorText) );
			categoryOtherText = Language.GetOrRegister( LocalizationKey + nameof(categoryOtherText) );
			sortItemIdText = Language.GetOrRegister( LocalizationKey + nameof(sortItemIdText) );
			sortPlaceTileText = Language.GetOrRegister( LocalizationKey + nameof(sortPlaceTileText) );
			sortAmmoTypeText = Language.GetOrRegister( LocalizationKey + nameof(sortAmmoTypeText) );
			sortHealLifeText = Language.GetOrRegister( LocalizationKey + nameof(sortHealLifeText) );
			sortHealManaText = Language.GetOrRegister( LocalizationKey + nameof(sortHealManaText) );
			sortGrappleRangeText = Language.GetOrRegister( LocalizationKey + nameof(sortGrappleRangeText) );
			sortProgressionOrderText = Language.GetOrRegister( LocalizationKey + nameof(sortProgressionOrderText) );
			sortPolePowerText = Language.GetOrRegister( LocalizationKey + nameof(sortPolePowerText) );
			sortBaitPowerText = Language.GetOrRegister( LocalizationKey + nameof(sortBaitPowerText) );
		}

		internal void Initialize() {
			// Sorts
			// Filters: Categories?
			// Craft and Loot Badges as well!
			// Hide with alt click?
			// show hidden toggle
			// Favorite: Only affects sort order?

			sortsAndFiltersPanel = new UIPanel();
			sortsAndFiltersPanel.SetPadding(6);
			sortsAndFiltersPanel.Top.Set(0, 0f);
			sortsAndFiltersPanel.Width.Set(-275, 1);
			sortsAndFiltersPanel.Height.Set(60, 0f);
			sortsAndFiltersPanel.BackgroundColor = Color.CornflowerBlue;//Color.LightSeaGreen;

			//sortsAndFiltersPanel.SetPadding(4);
			//mainPanel.Append(sortsAndFiltersPanel);
			//additionalDragTargets.Add(sortsAndFiltersPanel);
			//SetupSortsAndCategories();
			//PopulateSortsAndFiltersPanel();

			updateNeeded = true;
		}

		internal void Update() {
			if (!updateNeeded) { return; }
			updateNeeded = false;

			// Delay this so we can integrate mod categories.
			if (sorts == null) {
				SetupSortsAndCategories();
			}

			PopulateSortsAndFiltersPanel();
		}

		internal List<Filter> availableFilters;
		private void PopulateSortsAndFiltersPanel() {
			var availableSorts = new List<Sort>(sorts);
			availableFilters = new List<Filter>(filters);

			//sortsAndFiltersPanel.RemoveAllChildren();
			if (subCategorySortsFiltersGrid != null) {
				sortsAndFiltersPanel.RemoveChild(subCategorySortsFiltersGrid);
				sortsAndFiltersPanel.RemoveChild(lootGridScrollbar2);
			}

			if (categoriesGrid == null) {
				categoriesGrid = new UIHorizontalGrid();
				categoriesGrid.Width.Set(0, 1f);
				categoriesGrid.Height.Set(26, 0f);
				categoriesGrid.ListPadding = 2f;
				categoriesGrid.drawArrows = true;

				categoriesGridScrollbar = new InvisibleFixedUIHorizontalScrollbar(ItemChecklist.ItemChecklistInterface);
				categoriesGridScrollbar.SetView(100f, 1000f);
				categoriesGridScrollbar.Width.Set(0, 1f);
				categoriesGridScrollbar.Top.Set(0, 0f);
				sortsAndFiltersPanel.Append(categoriesGridScrollbar);
				categoriesGrid.SetScrollbar(categoriesGridScrollbar);
				sortsAndFiltersPanel.Append(categoriesGrid); // This is after so it gets the mouse events.
			}

			subCategorySortsFiltersGrid = new UIHorizontalGrid();
			subCategorySortsFiltersGrid.Width.Set(0, 1f);
			subCategorySortsFiltersGrid.Top.Set(26, 0f);
			subCategorySortsFiltersGrid.Height.Set(26, 0f);
			subCategorySortsFiltersGrid.ListPadding = 2f;
			subCategorySortsFiltersGrid.drawArrows = true;

			float oldRow2ViewPosition = lootGridScrollbar2?.ViewPosition ?? 0f;
			lootGridScrollbar2 = new InvisibleFixedUIHorizontalScrollbar(ItemChecklist.ItemChecklistInterface);
			lootGridScrollbar2.SetView(100f, 1000f);
			lootGridScrollbar2.Width.Set(0, 1f);
			lootGridScrollbar2.Top.Set(28, 0f);
			sortsAndFiltersPanel.Append(lootGridScrollbar2);
			subCategorySortsFiltersGrid.SetScrollbar(lootGridScrollbar2);
			sortsAndFiltersPanel.Append(subCategorySortsFiltersGrid);

			//sortsAndFiltersPanelGrid = new UIGrid();
			//sortsAndFiltersPanelGrid.Width.Set(0, 1);
			//sortsAndFiltersPanelGrid.Height.Set(0, 1);
			//sortsAndFiltersPanel.Append(sortsAndFiltersPanelGrid);

			//sortsAndFiltersPanelGrid2 = new UIGrid();
			//sortsAndFiltersPanelGrid2.Width.Set(0, 1);
			//sortsAndFiltersPanelGrid2.Height.Set(0, 1);
			//sortsAndFiltersPanel.Append(sortsAndFiltersPanelGrid2);

			int count = 0;

			var visibleCategories = new List<Category>();
			var visibleSubCategories = new List<Category>();
			int left = 0;
			foreach (var category in categories) {
				category.button.selected = false;
				visibleCategories.Add(category);
				bool meOrChildSelected = SelectedCategory == category;
				foreach (var subcategory in category.subCategories) {
					subcategory.button.selected = false;
					meOrChildSelected |= subcategory == SelectedCategory;
				}
				if (meOrChildSelected) {
					visibleSubCategories.AddRange(category.subCategories);
					category.button.selected = true;
				}
			}

			float oldTopRowViewPosition = categoriesGridScrollbar?.ViewPosition ?? 0f;
			categoriesGrid.Clear();
			foreach (var category in visibleCategories) {
				var container = new UISortableElement(++count);
				container.Width.Set(24, 0);
				container.Height.Set(24, 0);
				//category.button.Left.Pixels = left;
				//if (category.parent != null)
				//	container.OrderIndex
				//	category.button.Top.Pixels = 12;
				//sortsAndFiltersPanel.Append(category.button);
				container.Append(category.button);
				categoriesGrid.Add(container);
				left += 26;
			}

			//UISortableElement spacer = new UISortableElement(++count);
			//spacer.Width.Set(0, 1);
			//sortsAndFiltersPanelGrid2.Add(spacer);

			foreach (var category in visibleSubCategories) {
				var container = new UISortableElement(++count);
				container.Width.Set(24, 0);
				container.Height.Set(24, 0);
				container.Append(category.button);
				subCategorySortsFiltersGrid.Add(container);
				left += 26;
			}

			if (visibleSubCategories.Count > 0) {
				var container2 = new UISortableElement(++count);
				container2.Width.Set(24, 0);
				container2.Height.Set(24, 0);
				var image = new UIImage(ItemChecklist.instance.Assets.Request<Texture2D>("Images/spacer"));
				//image.Left.Set(6, 0);
				image.HAlign = 0.5f;
				container2.Append(image);
				subCategorySortsFiltersGrid.Add(container2);
			}

			// add to sorts and filters here
			if (SelectedCategory != null) {
				SelectedCategory.button.selected = true;
				SelectedCategory.ParentAddToSorts(availableSorts);
				SelectedCategory.ParentAddToFilters(availableFilters);
			}

			left = 0;
			foreach (var sort in availableSorts) {
				sort.button.selected = false;
				if (SelectedSort == sort) // TODO: SelectedSort no longwe valid
					sort.button.selected = true;
				//sort.button.Left.Pixels = left;
				//sort.button.Top.Pixels = 24;
				//sort.button.Width
				//grid.Add(sort.button);
				var container = new UISortableElement(++count);
				container.Width.Set(24, 0);
				container.Height.Set(24, 0);
				container.Append(sort.button);
				subCategorySortsFiltersGrid.Add(container);
				//sortsAndFiltersPanel.Append(sort.button);
				left += 26;
			}
			if (!availableSorts.Contains(SharedUI.instance.SelectedSort)) {
				availableSorts[0].button.selected = true;
				SharedUI.instance.SelectedSort = availableSorts[0];
				updateNeeded = false;
			}

			if (availableFilters.Count > 0) {
				var container2 = new UISortableElement(++count);
				container2.Width.Set(24, 0);
				container2.Height.Set(24, 0);
				var image = new UIImage(ItemChecklist.instance.Assets.Request<Texture2D>("Images/spacer"));
				image.HAlign = 0.5f;
				container2.Append(image);
				subCategorySortsFiltersGrid.Add(container2);

				foreach (var item in availableFilters) {
					var container = new UISortableElement(++count);
					container.Width.Set(24, 0);
					container.Height.Set(24, 0);
					container.Append(item.button);
					subCategorySortsFiltersGrid.Add(container);
				}
			}

			// Restore view position after CycleFilter changes current filters.
			subCategorySortsFiltersGrid.Recalculate();
			lootGridScrollbar2.ViewPosition = oldRow2ViewPosition;
			categoriesGrid.Recalculate();
			//categoriesGridScrollbar.ViewPosition = oldTopRowViewPosition; // And after category disappears, not really needed since only 1 will disappear, unlike 2nd row. Test more if more special categories are added
		}

		internal List<Category> categories;
		internal List<Filter> filters;
		internal List<Sort> sorts;

		// Items whose textures are resized used during setup
		// If they aren't loaded, some buttons doesn't have an icon
		// TODO: A better way to do this?
		private int[] itemTexturePreload =
		{
			ItemID.MetalDetector, ItemID.SpellTome, ItemID.IronAnvil, ItemID.MythrilAnvil, ItemID.Blindfold, ItemID.GoldBroadsword, ItemID.GoldenShower, ItemID.FlintlockPistol,
			ItemID.Shuriken, ItemID.SlimeStaff, ItemID.DD2LightningAuraT1Popper, ItemID.SilverHelmet, ItemID.SilverChainmail, ItemID.SilverGreaves,
			ItemID.BunnyHood, ItemID.HerosHat, ItemID.GoldHelmet, ItemID.Sign, ItemID.IronAnvil, ItemID.PearlstoneBrickWall, ItemID.EoCShield,
			ItemID.ZephyrFish, ItemID.FairyBell, ItemID.MechanicalSkull, ItemID.SlimySaddle, ItemID.AmethystHook, ItemID.OrangeDye, ItemID.BiomeHairDye,
			ItemID.FallenStarfish, ItemID.HermesBoots, ItemID.LeafWings, ItemID.Minecart, ItemID.HealingPotion, ItemID.ManaPotion, ItemID.RagePotion,
			ItemID.AlphabetStatueA, ItemID.GoldChest, ItemID.PaintingMartiaLisa, ItemID.HeartStatue, ItemID.Wire, ItemID.PurificationPowder,
			ItemID.Extractinator, ItemID.UnicornonaStick, ItemID.SilverHelmet, ItemID.BunnyHood, ItemID.ZephyrFish, ItemID.Sign, ItemID.FallenStarfish,
			ItemID.HealingPotion, ItemID.OrangeDye, ItemID.Candelabra, ItemID.WoodenDoor, ItemID.WoodenChair, ItemID.PalmWoodTable, ItemID.ChineseLantern,
			ItemID.RainbowTorch, ItemID.GoldBunny, ItemID.WoodenDoor, ItemID.WoodenChair, ItemID.PalmWoodTable, ItemID.ChineseLantern, ItemID.RainbowTorch
		};

		private void SetupSortsAndCategories() {
			foreach (int type in itemTexturePreload)
				Main.instance.LoadItem(type);

			Texture2D terrariaSort = ResizeImage(TextureAssets.InventorySort[0], 24, 24);
			Texture2D rarity = ResizeImage(TextureAssets.Item[ItemID.MetalDetector], 24, 24);

			// TODO: Implement Badge text as used in Item Checklist.
			sorts = new List<Sort>()
			{
				new Sort(sortItemIdText.Value, "Images/sortItemID", (x,y)=>x.type.CompareTo(y.type), x=>x.type.ToString()),
				new Sort(sortValueText.Value, "Images/sortValue", (x,y)=>x.value.CompareTo(y.value), x=>x.value.ToString()),
				new Sort(sortAlphabeticalText.Value, "Images/sortAZ", (x,y)=>x.Name.CompareTo(y.Name), x=>x.Name.ToString()),
				new Sort(sortRarityText.Value, rarity, (x,y)=> x.rare==y.rare ? x.value.CompareTo(y.value) : Math.Abs(x.rare).CompareTo(Math.Abs(y.rare)), x=>x.rare.ToString()),
				new Sort(sortTerrariaSortText.Value, terrariaSort, ByCreativeSortingId, ByCreativeSortingIdBadgeText),
			};

			Texture2D materialsIcon = Utilities.StackResizeImage(new[] { TextureAssets.Item[ItemID.SpellTome] }, 24, 24);
			filters = new List<Filter>()
			{
				new Filter(filterMaterialsText.Value, x=>ItemID.Sets.IsAMaterial[x.type], materialsIcon),
			};

			// TODOS: Vanity armor, grapple, cart, potions buffs
			// 24x24 pixels

			var yoyos = new List<int>();
			for (int i = 0; i < ItemID.Sets.Yoyo.Length; ++i) {
				if (ItemID.Sets.Yoyo[i]) {
					Main.instance.LoadItem(i);
					yoyos.Add(i);
				}
			}

			var useAmmoTypes = new Dictionary<int, int>();
			var ammoTypes = new Dictionary<int, int>();
			var testItem = new Item();
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				testItem.SetDefaults(i);
				if (testItem.useAmmo >= ItemLoader.ItemCount || testItem.ammo >= ItemLoader.ItemCount || testItem.useAmmo < 0 || testItem.ammo < 0)
					continue; // Some mods misuse useAmmo
				if (testItem.useAmmo > 0) {
					useAmmoTypes.TryGetValue(testItem.useAmmo, out var currentCount);
					useAmmoTypes[testItem.useAmmo] = currentCount + 1;
				}
				if (testItem.ammo > 0) {
					ammoTypes.TryGetValue(testItem.ammo, out var currentCount);
					ammoTypes[testItem.ammo] = currentCount + 1;
				}
			}
			var sortedUseAmmoTypes = from pair in useAmmoTypes orderby pair.Value descending select pair.Key;
			var sortedAmmoTypes = from pair in ammoTypes orderby pair.Value descending select pair.Key;

			foreach (int type in sortedUseAmmoTypes)
				Main.instance.LoadItem(type);
			foreach (int type in sortedAmmoTypes)
				Main.instance.LoadItem(type);

			var ammoFilters = sortedAmmoTypes.Select(ammoType => new Filter(Lang.GetItemNameValue(ammoType), x => x.ammo == ammoType, ResizeImage(TextureAssets.Item[ammoType], 24, 24))).ToList();
			var useAmmoFilters = sortedUseAmmoTypes.Select(ammoType => new Filter(Lang.GetItemNameValue(ammoType), x => x.useAmmo == ammoType, ResizeImage(TextureAssets.Item[ammoType], 24, 24))).ToList();

			var ammoFilter = new CycleFilter(cycleAmmoTypesText.Value, "Images/sortAmmo", ammoFilters);
			var useAmmoFilter = new CycleFilter(cycleUsedAmmoTypesText.Value, "Images/sortAmmo", useAmmoFilters);

			Texture2D smallMelee = ResizeImage(TextureAssets.Item[ItemID.GoldBroadsword], 24, 24);
			Texture2D smallYoyo = ResizeImage(TextureAssets.Item[Main.rand.Next(yoyos)], 24, 24); //Main.rand.Next(ItemID.Sets.Yoyo) ItemID.Yelets
			Texture2D smallMagic = ResizeImage(TextureAssets.Item[ItemID.GoldenShower], 24, 24);
			Texture2D smallRanged = ResizeImage(TextureAssets.Item[ItemID.FlintlockPistol], 24, 24);
			Texture2D smallThrown = ResizeImage(TextureAssets.Item[ItemID.Shuriken], 24, 24);
			Texture2D smallSummon = ResizeImage(TextureAssets.Item[ItemID.SlimeStaff], 24, 24);
			Texture2D smallSentry = ResizeImage(TextureAssets.Item[ItemID.DD2LightningAuraT1Popper], 24, 24);
			Texture2D smallHead = ResizeImage(TextureAssets.Item[ItemID.SilverHelmet], 24, 24);
			Texture2D smallBody = ResizeImage(TextureAssets.Item[ItemID.SilverChainmail], 24, 24);
			Texture2D smallLegs = ResizeImage(TextureAssets.Item[ItemID.SilverGreaves], 24, 24);
			Texture2D smallVanity = ResizeImage(TextureAssets.Item[ItemID.BunnyHood], 24, 24);
			//Texture2D smallVanity2 = ResizeImage(TextureAssets.Item[ItemID.HerosHat], 24, 24);
			Texture2D smallNonVanity = ResizeImage(TextureAssets.Item[ItemID.GoldHelmet], 24, 24);
			Texture2D smallTiles = ResizeImage(TextureAssets.Item[ItemID.Sign], 24, 24);
			Texture2D smallCraftingStation = ResizeImage(TextureAssets.Item[ItemID.IronAnvil], 24, 24);
			Texture2D smallWalls = ResizeImage(TextureAssets.Item[ItemID.PearlstoneBrickWall], 24, 24);
			Texture2D smallExpert = ResizeImage(TextureAssets.Item[ItemID.EoCShield], 24, 24);
			Texture2D smallPets = ResizeImage(TextureAssets.Item[ItemID.ZephyrFish], 24, 24);
			Texture2D smallLightPets = ResizeImage(TextureAssets.Item[ItemID.FairyBell], 24, 24);
			Texture2D smallBossSummon = ResizeImage(TextureAssets.Item[ItemID.MechanicalSkull], 24, 24);
			Texture2D smallMounts = ResizeImage(TextureAssets.Item[ItemID.SlimySaddle], 24, 24);
			Texture2D smallHooks = ResizeImage(TextureAssets.Item[ItemID.AmethystHook], 24, 24);
			Texture2D smallDyes = ResizeImage(TextureAssets.Item[ItemID.OrangeDye], 24, 24);
			Texture2D smallHairDye = ResizeImage(TextureAssets.Item[ItemID.BiomeHairDye], 24, 24);
			Texture2D smallQuestFish = ResizeImage(TextureAssets.Item[ItemID.FallenStarfish], 24, 24);
			Texture2D smallAccessories = ResizeImage(TextureAssets.Item[ItemID.HermesBoots], 24, 24);
			Texture2D smallWings = ResizeImage(TextureAssets.Item[ItemID.LeafWings], 24, 24);
			Texture2D smallCarts = ResizeImage(TextureAssets.Item[ItemID.Minecart], 24, 24);
			Texture2D smallHealth = ResizeImage(TextureAssets.Item[ItemID.HealingPotion], 24, 24);
			Texture2D smallMana = ResizeImage(TextureAssets.Item[ItemID.ManaPotion], 24, 24);
			Texture2D smallBuff = ResizeImage(TextureAssets.Item[ItemID.RagePotion], 24, 24);
			Texture2D smallAll = ResizeImage(TextureAssets.Item[ItemID.AlphabetStatueA], 24, 24);
			Texture2D smallContainer = ResizeImage(TextureAssets.Item[ItemID.GoldChest], 24, 24);
			Texture2D smallPaintings = ResizeImage(TextureAssets.Item[ItemID.PaintingMartiaLisa], 24, 24);
			Texture2D smallStatue = ResizeImage(TextureAssets.Item[ItemID.HeartStatue], 24, 24);
			Texture2D smallWiring = ResizeImage(TextureAssets.Item[ItemID.Wire], 24, 24);
			Texture2D smallConsumables = ResizeImage(TextureAssets.Item[ItemID.PurificationPowder], 24, 24);
			Texture2D smallExtractinator = ResizeImage(TextureAssets.Item[ItemID.Extractinator], 24, 24);
			Texture2D smallOther = ResizeImage(TextureAssets.Item[ItemID.UnicornonaStick], 24, 24);

			Texture2D smallArmor = StackResizeImage(new[] { TextureAssets.Item[ItemID.SilverHelmet], TextureAssets.Item[ItemID.SilverChainmail], TextureAssets.Item[ItemID.SilverGreaves] }, 24, 24);
			//Texture2D smallVanityFilterGroup = StackResizeImage2424(TextureAssets.Item[ItemID.BunnyHood], TextureAssets.Item[ItemID.GoldHelmet]);
			Texture2D smallPetsLightPets = StackResizeImage(new[] { TextureAssets.Item[ItemID.ZephyrFish], TextureAssets.Item[ItemID.FairyBell] }, 24, 24);
			Texture2D smallPlaceables = StackResizeImage(new[] { TextureAssets.Item[ItemID.Sign], TextureAssets.Item[ItemID.PearlstoneBrickWall] }, 24, 24);
			Texture2D smallWeapons = StackResizeImage(new[] { smallMelee, smallMagic, smallThrown }, 24, 24);
			Texture2D smallTools = StackResizeImage(new[] { ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortPick"), ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortAxe"), ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortHammer") }, 24, 24);
			Texture2D smallFishing = StackResizeImage(new[] { ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortFish"), ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortBait"), TextureAssets.Item[ItemID.FallenStarfish] }, 24, 24);
			Texture2D smallPotions = StackResizeImage(new[] { TextureAssets.Item[ItemID.HealingPotion], TextureAssets.Item[ItemID.ManaPotion], TextureAssets.Item[ItemID.RagePotion] }, 24, 24);
			Texture2D smallBothDyes = StackResizeImage(new[] { TextureAssets.Item[ItemID.OrangeDye], TextureAssets.Item[ItemID.BiomeHairDye] }, 24, 24);
			Texture2D smallSortTiles = StackResizeImage(new[] { TextureAssets.Item[ItemID.Candelabra], TextureAssets.Item[ItemID.GrandfatherClock] }, 24, 24);

			Texture2D StackResizeImage2424(params Asset<Texture2D>[] textures) => StackResizeImage(textures, 24, 24);
			Texture2D ResizeImage2424(Asset<Texture2D> texture) => ResizeImage(texture, 24, 24);

			// Potions, other?
			// should inherit children?
			// should have other category?
			if (GenVars.statueList == null)
				WorldGen.SetupStatueList();

			var vanity = new MutuallyExclusiveFilter(mutuallyExclusiveFilterVanityText.Value, x => x.vanity, smallVanity);
			var armor = new MutuallyExclusiveFilter(mutuallyExclusiveFilterArmorText.Value, x => !x.vanity, smallNonVanity);
			vanity.SetExclusions(new List<Filter>() { vanity, armor });
			armor.SetExclusions(new List<Filter>() { vanity, armor });

			categories = new List<Category>() {
				new Category(categoryAllText.Value, x=> true, smallAll),
				// TODO: Filter out tools from weapons. Separate belongs and doesn't belong predicates? How does inheriting work again? Other?
				new Category(categoryWeaponsText.Value/*, x=>x.damage>0*/, x=> false, smallWeapons) { //"Images/sortDamage"
					subCategories = new List<Category>() {
						new Category(categoryMeleeText.Value, x=>x.CountsAsClass(DamageClass.Melee) && !(x.pick>0 || x.axe>0 || x.hammer>0), smallMelee),
						new Category(categoryYoyoText.Value, x=>ItemID.Sets.Yoyo[x.type], smallYoyo),
						new Category(categoryMagicText.Value, x=>x.CountsAsClass(DamageClass.Magic), smallMagic),
						new Category(categoryRangedText.Value, x=>x.CountsAsClass(DamageClass.Ranged) && x.ammo == 0, smallRanged) // TODO and ammo no
						{
							sorts = new List<Sort>() { new Sort(sortUseAmmoTypeText.Value, "Images/sortAmmo", (x,y)=>x.useAmmo.CompareTo(y.useAmmo), x => x.useAmmo.ToString()), },
							filters = new List<Filter> { useAmmoFilter }
						},
						new Category(categoryThrowingText.Value, x=>x.CountsAsClass(DamageClass.Throwing), smallThrown),
						new Category(categorySummonText.Value, x=>x.CountsAsClass(DamageClass.Summon) && !x.sentry, smallSummon),
						new Category(categorySentryText.Value, x=>x.CountsAsClass(DamageClass.Summon) && x.sentry, smallSentry),
					},
					sorts = new List<Sort>() { new Sort(sortDamageText.Value, "Images/sortDamage", (x,y)=>x.damage.CompareTo(y.damage), x => x.damage.ToString()), },
				},
				new Category(categoryToolsText.Value/*,x=>x.pick>0||x.axe>0||x.hammer>0*/, x=>false, smallTools) {
					subCategories = new List<Category>() {
						new Category(categoryPickaxesText.Value, x=>x.pick>0, "Images/sortPick") { sorts = new List<Sort>() { new Sort(sortPickPower.Value, "Images/sortPick", (x,y)=>x.pick.CompareTo(y.pick), x => x.pick.ToString()), } },
						new Category(categoryAxesText.Value, x=>x.axe>0, "Images/sortAxe"){ sorts = new List<Sort>() { new Sort(sortAxePower.Value, "Images/sortAxe", (x,y)=>x.axe.CompareTo(y.axe), x => (x.axe*5).ToString()), } },
						new Category(categoryHammersText.Value, x=>x.hammer>0, "Images/sortHammer"){ sorts = new List<Sort>() { new Sort(sortHammerPower.Value, "Images/sortHammer", (x,y)=>x.hammer.CompareTo(y.hammer), x => x.hammer.ToString()), } },
					},
				},
				new Category(categoryArmorText.Value/*,  x=>x.headSlot!=-1||x.bodySlot!=-1||x.legSlot!=-1*/, x => false, smallArmor) {
					subCategories = new List<Category>() {
						new Category(categoryHead.Value, x=>x.headSlot!=-1, smallHead),
						new Category(categoryBody.Value, x=>x.bodySlot!=-1, smallBody),
						new Category(categoryLegs.Value, x=>x.legSlot!=-1, smallLegs),
					},
					sorts = new List<Sort>() { new Sort(sortDefense.Value, "Images/sortDefense", (x,y)=>x.defense.CompareTo(y.defense), x => x.defense.ToString()), },
					filters = new List<Filter> {
						//new Filter("Vanity", x=>x.vanity, RecipeBrowser.instance.Assets.Request<Texture2D>("Images/sortDefense")),
						// Prefer MutuallyExclusiveFilter for this, rather than CycleFilter since there are only 2 options.
						//new CycleFilter("Vanity/Armor", smallVanityFilterGroup, new List<Filter> {
						//	new Filter("Vanity", x=>x.vanity, smallVanity),
						//	new Filter("Armor", x=>!x.vanity, smallNonVanity),
						//}),
						vanity, armor,
						//new DoubleFilter("Vanity", "Armor", smallVanity2, x=>x.vanity),
					}
				},
				new Category(categoryTiles.Value, x=>x.createTile!=-1, smallTiles)
				{
					subCategories = new List<Category>()
					{
						new Category(categoryContainersText.Value, x=>x.createTile!=-1 && Main.tileContainer[x.createTile], smallContainer),
						new Category(categoryWiringText.Value, x=>ItemID.Sets.SortingPriorityWiring[x.type] > -1, smallWiring),
						new Category(categoryStatuesText.Value, x=>GenVars.statueList.Any(point => point.X == x.createTile && point.Y == x.placeStyle), smallStatue),
						new Category(categoryDoorsText.Value, x=> x.createTile > 0 && TileID.Sets.RoomNeeds.CountsAsDoor.Contains(x.createTile), ResizeImage2424(TextureAssets.Item[ItemID.WoodenDoor])),
						new Category(categoryChairsText.Value, x=> x.createTile > 0 && TileID.Sets.RoomNeeds.CountsAsChair.Contains(x.createTile), ResizeImage2424(TextureAssets.Item[ItemID.WoodenChair])),
						new Category(categoryTablesText.Value, x=> x.createTile > 0 && TileID.Sets.RoomNeeds.CountsAsTable.Contains(x.createTile), ResizeImage2424(TextureAssets.Item[ItemID.PalmWoodTable])),
						new Category(categoryLightSourcesText.Value, x=> x.createTile > 0 && TileID.Sets.RoomNeeds.CountsAsTorch.Contains(x.createTile), ResizeImage2424(TextureAssets.Item[ItemID.ChineseLantern])),
						new Category(categoryTorchesText.Value, x=> x.createTile > 0 && TileID.Sets.Torch[x.createTile], ResizeImage2424(TextureAssets.Item[ItemID.RainbowTorch])),
						// Banners => Banner Bonanza mod integration
						//TextureAssets.Item[Main.rand.Next(TileID.Sets.RoomNeeds.CountsAsTable)] doesn't work since those are tilesids. yoyo approach?
						// todo: music box
						//new Category("Paintings", x=>ItemID.Sets.SortingPriorityPainting[x.type] > -1, smallPaintings), // oops, this is painting tools not painting tiles
						//new Category("5x4", x=>{
						//	if(x.createTile!=-1)
						//	{
						//		var tod = Terraria.ObjectData.TileObjectData.GetTileData(x.createTile, x.placeStyle);
						//		return tod != null && tod.Width == 5 && tod.Height == 4;
						//	}
						//	return false;
						//} , smallContainer),
					},
					sorts = new List<Sort>() {
						new Sort(sortPlaceTileText.Value, smallSortTiles, (x,y)=> x.createTile == y.createTile ? x.placeStyle.CompareTo(y.placeStyle) : x.createTile.CompareTo(y.createTile), x=>$"{x.createTile},{x.placeStyle}"),
					}
				},
				new Category(categoryWallsText.Value, x=>x.createWall!=-1, smallWalls),
				new Category(categoryAccessoriesText.Value, x=>x.accessory, smallAccessories)
				{
					subCategories = new List<Category>()
					{
						new Category(categoryWingsText.Value, x=>x.wingSlot > 0, smallWings)
					}
				},
				new Category(categoryAmmoText.Value, x=>x.ammo!=0, "Images/sortAmmo")
				{
					sorts = new List<Sort>() {
						new Sort(sortAmmoTypeText.Value, "Images/sortAmmo", (x,y)=>x.ammo.CompareTo(y.ammo), x => $"{x.ammo}"),
						new Sort(sortDamageText.Value, "Images/sortDamage", (x,y)=>x.damage.CompareTo(y.damage), x => $"{x.damage}"),
					},
					filters = new List<Filter> { ammoFilter }
					// TODO: Filters/Subcategories for all ammo types? // each click cycles?
				},
				new Category(categoryPotionsText.Value, x=> (x.UseSound?.IsTheSameAs(SoundID.Item3) == true), smallPotions)
				{
					subCategories = new List<Category>() {
						new Category(categoryHealthPotionsText.Value, x=>x.healLife > 0, smallHealth) { sorts = new List<Sort>() { new Sort(sortHealLifeText.Value, smallHealth, (x,y)=>x.healLife.CompareTo(y.healLife), x => $"{x.healLife}"), } },
						new Category(categoryManaPotionsText.Value, x=>x.healMana > 0, smallMana) { sorts = new List<Sort>() { new Sort(sortHealManaText.Value, smallMana, (x,y)=>x.healMana.CompareTo(y.healMana), x => $"{x.healMana}"),   }},
						new Category(categoryBuffPotionsText.Value, x=>(x.UseSound?.IsTheSameAs(SoundID.Item3) == true) && x.buffType > 0, smallBuff),
						// Todo: Automatic other category?
					}
				},
				new Category(categoryExpertText.Value, x=>x.expert, smallExpert),
				new Category(categoryPetsText.Value/*, x=> x.buffType > 0 && (Main.vanityPet[x.buffType] || Main.lightPet[x.buffType])*/, x=>false, smallPetsLightPets){
					subCategories = new List<Category>() {
						new Category(categoryPetsText.Value, x=>Main.vanityPet[x.buffType], smallPets),
						new Category(categoryLightPetsText.Value, x=>Main.lightPet[x.buffType], smallLightPets),
					}
				},
				new Category(categoryMountsText.Value, x=>x.mountType != -1, smallMounts)
				{
					subCategories = new List<Category>()
					{
						new Category(categoryCartsText.Value, x=>x.mountType != -1 && MountID.Sets.Cart[x.mountType], smallCarts) // TODO: need mountType check? inherited parent logic or parent unions children?
					}
				},
				new Category(categoryHooksText.Value, x=> Main.projHook[x.shoot], smallHooks){
					sorts = new List<Sort>() {
						new Sort(sortGrappleRangeText.Value, smallHooks, (x,y)=> GrappleRange(x.shoot).CompareTo(GrappleRange(y.shoot)), x => $"{GrappleRange(x.shoot)}"),
					},
				},
				new Category(categoryDyesText.Value, x=>false, smallBothDyes)
				{
					subCategories = new List<Category>()
					{
						new Category(categoryDyesText.Value, x=>x.dye != 0, smallDyes),
						new Category(categoryHairDyesText.Value, x=>x.hairDye != -1, smallHairDye),
					}
				},
				new Category(categoryBossSummonsText.Value, x=>ItemID.Sets.SortingPriorityBossSpawns[x.type] != -1 && x.type != ItemID.LifeCrystal && x.type != ItemID.ManaCrystal && x.type != ItemID.CellPhone && x.type != ItemID.IceMirror && x.type != ItemID.MagicMirror && x.type != ItemID.LifeFruit && x.netID != ItemID.TreasureMap || x.netID == ItemID.PirateMap, smallBossSummon) { // vanilla bug.
					sorts = new List<Sort>() { new Sort(sortProgressionOrderText.Value, "Images/sortDamage", (x,y)=>ItemID.Sets.SortingPriorityBossSpawns[x.type].CompareTo(ItemID.Sets.SortingPriorityBossSpawns[y.type]), x => $"{ItemID.Sets.SortingPriorityBossSpawns[x.type]}"), }
				},
				new Category(categoryConsumablesText.Value, x=> !(x.createWall > 0 || x.createTile > -1) && !(x.ammo > 0 && !x.notAmmo) && x.consumable, smallConsumables){
					subCategories = new List<Category>() {
						new Category(categoryCapturedNPCText.Value, x=>x.makeNPC != 0, ResizeImage2424(TextureAssets.Item[ItemID.GoldBunny])),
					}
				},
				new Category(categoryFishingText.Value/*, x=> x.fishingPole > 0 || x.bait>0|| x.questItem*/, x=>false, smallFishing){
					subCategories = new List<Category>() {
						new Category(categoryPolesText.Value, x=>x.fishingPole > 0, "Images/sortFish") {sorts = new List<Sort>() { new Sort(sortPolePowerText.Value, "Images/sortFish", (x,y)=>x.fishingPole.CompareTo(y.fishingPole), x => $"{x.fishingPole}"), } },
						new Category(categoryBaitText.Value, x=>x.bait>0, "Images/sortBait") {sorts = new List<Sort>() { new Sort(sortBaitPowerText.Value, "Images/sortBait", (x,y)=>x.bait.CompareTo(y.bait), x => $"{x.bait}"), } },
						new Category(categoryQuestFishText.Value, x=>x.questItem, smallQuestFish),
					}
				},
				new Category(categoryExtractinatorText.Value, x=>ItemID.Sets.ExtractinatorMode[x.type] > -1, smallExtractinator),
				//modCategory,
				new Category(categoryOtherText.Value, x=>BelongsInOther(x), smallOther),
			};

			/* Think about this one.
			foreach (var modCategory in RecipeBrowser.instance.modCategories)
			{
				if (string.IsNullOrEmpty(modCategory.parent))
				{
					categories.Insert(categories.Count - 2, new Category(modCategory.name, modCategory.belongs, modCategory.icon));
				}
				else
				{
					foreach (var item in categories)
					{
						if (item.name == modCategory.parent)
						{
							item.subCategories.Add(new Category(modCategory.name, modCategory.belongs, modCategory.icon));
						}
					}
				}
			}
			foreach (var modCategory in RecipeBrowser.instance.modFilters)
			{
				filters.Add(new Filter(modCategory.name, modCategory.belongs, modCategory.icon));
			}
			*/

			foreach (var parent in categories) {
				foreach (var child in parent.subCategories) {
					child.parent = parent; // 3 levels?
				}
			}
			SelectedSort = sorts[0];
			SelectedCategory = categories[0];
		}

		private string ByCreativeSortingIdBadgeText(Item x) {
			return ItemChecklistUI.vanillaIDsInSortOrder[x.type].ToString();
		}

		private int ByCreativeSortingId(Item x, Item y) {
			return ItemChecklistUI.vanillaIDsInSortOrder[x.type].CompareTo(ItemChecklistUI.vanillaIDsInSortOrder[y.type]);

			ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup itemGroupAndOrderInGroup = ContentSamples.ItemCreativeSortingId[x.type];
			ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup itemGroupAndOrderInGroup2 = ContentSamples.ItemCreativeSortingId[y.type];
			int num = itemGroupAndOrderInGroup.Group.CompareTo(itemGroupAndOrderInGroup2.Group);
			if (num == 0)
				num = itemGroupAndOrderInGroup.OrderInGroup.CompareTo(itemGroupAndOrderInGroup2.OrderInGroup);

			return num;
		}

		// TODO: Update with new 1.4 values.
		Dictionary<int, float> vanillaGrappleRanges = new Dictionary<int, float>() {
			[13] = 300f,
			[32] = 400f,
			[73] = 440f,
			[74] = 440f,
			[165] = 250f,
			[256] = 350f,
			[315] = 500f,
			[322] = 550f,
			[13] = 300f,
			[331] = 400f,
			[332] = 550f,
			[372] = 400f,
			[396] = 300f,
			[446] = 500f,
			[652] = 600f,
			[646] = 550f,
			[647] = 550f,
			[648] = 550f,
			[649] = 550f,
			[486] = 480f,
			[487] = 480f,
			[488] = 480f,
			[489] = 480f,
			[230] = 300f,
			[231] = 330f,
			[232] = 360f,
			[233] = 390f,
			[234] = 420f,
			[235] = 450f,
		};

		private float GrappleRange(int type) {
			if (vanillaGrappleRanges.ContainsKey(type))
				return vanillaGrappleRanges[type];
			if (type > ProjectileID.Count)
				return ProjectileLoader.GetProjectile(type).GrappleRange();
			return 0;
		}

		private bool BelongsInOther(Item item) {
			var cats = categories.Skip(1).Take(categories.Count - 2);
			foreach (var category in cats) {
				if (category.BelongsRecursive(item))
					return false;
			}
			return true;
		}
	}

	internal class Filter
	{
		internal string name;
		internal Predicate<Item> belongs;
		internal List<Category> subCategories;
		internal List<Sort> sorts;
		internal UISilentImageButton button;
		internal Texture2D texture;
		//internal Category parent;

		public Filter(string name, Predicate<Item> belongs, Texture2D texture) {
			this.name = name;
			this.texture = texture;
			subCategories = new List<Category>();
			sorts = new List<Sort>();
			this.belongs = belongs;

			this.button = new UISilentImageButton(texture, name);
			button.OnLeftClick += (a, b) => {
				button.selected = !button.selected;
				ItemChecklistUI.instance.UpdateNeeded();
				//Main.NewText("clicked on " + button.hoverText);
			};
		}
	}

	internal class MutuallyExclusiveFilter : Filter
	{
		List<Filter> exclusives;

		public MutuallyExclusiveFilter(string name, Predicate<Item> belongs, Texture2D texture) : base(name, belongs, texture) {
			button.OnLeftClick += (a, b) => {
				if (button.selected) {
					foreach (var item in exclusives) {
						if (item != this)
							item.button.selected = false;
					}
				}
			};
		}

		internal void SetExclusions(List<Filter> exclusives) {
			this.exclusives = exclusives;
		}
	}

	// A bit confusing, don't use.
	internal class DoubleFilter : Filter
	{
		bool right;
		string other;
		public DoubleFilter(string name, string other, Texture2D texture, Predicate<Item> belongs) : base(name, belongs, texture) {
			this.other = other;
			this.belongs = (item) => {
				return belongs(item) ^ right;
			};
			button = new UIBadgedSilentImageButton(texture, name + " (RMB)");
			button.OnLeftClick += (a, b) => {
				button.selected = !button.selected;
				ItemChecklistUI.instance.UpdateNeeded();
				//Main.NewText("clicked on " + button.hoverText);
			};
			button.OnRightClick += (a, b) => {
				right = !right;
				(button as UIBadgedSilentImageButton).drawX = right;
				button.hoverText = (right ? other : name) + " (RMB)";
				ItemChecklistUI.instance.UpdateNeeded();
			};
		}
	}

	internal class CycleFilter : Filter
	{
		int index = 0; // different images? different backgrounds?
		List<Filter> filters;
		List<UISilentImageButton> buttons = new List<UISilentImageButton>();

		public CycleFilter(string name, string textureFileName, List<Filter> filters) :
			this(name, ItemChecklist.instance.Assets.Request<Texture2D>(textureFileName, AssetRequestMode.ImmediateLoad).Value, filters) {
		}

		public CycleFilter(string name, Texture2D texture, List<Filter> filters) : base(name, (item) => false, texture) {
			this.filters = filters;
			this.belongs = (item) => {
				return index == 0 ? true : filters[index - 1].belongs(item);
			};
			//CycleFilter needs SharedUI.instance.updateNeeded to update image, since each filter acts independently.

			var firstButton = new UISilentImageButton(texture, name);
			firstButton.OnLeftClick += (a, b) => ButtonBehavior(true);
			firstButton.OnRightClick += (a, b) => ButtonBehavior(false);

			buttons.Add(firstButton);

			for (int i = 0; i < filters.Count; i++) {
				var buttonOption = new UISilentImageButton(filters[i].texture, filters[i].name);
				buttonOption.OnLeftClick += (a, b) => ButtonBehavior(true);
				buttonOption.OnRightClick += (a, b) => ButtonBehavior(false);
				buttonOption.OnMiddleClick += (a, b) => ButtonBehavior(false, true);
				buttons.Add(buttonOption);
			}

			button = buttons[0];

			void ButtonBehavior(bool increment, bool zero = false) {
				button.selected = false;

				index = zero ? 0 : (increment ? (index + 1) % buttons.Count : (buttons.Count + index - 1) % buttons.Count);
				button = buttons[index];
				if (index != 0)
					button.selected = true;
				ItemChecklistUI.instance.UpdateNeeded();
				SharedUI.instance.updateNeeded = true;
			}
		}
	}

	internal class Sort
	{
		internal Func<Item, Item, int> sort;
		internal Func<Item, string> badge;
		internal UISilentImageButton button;

		public Sort(string hoverText, Texture2D texture, Func<Item, Item, int> sort, Func<Item, string> badge) {
			this.sort = sort;
			this.badge = badge;
			button = new UISilentImageButton(texture, hoverText);
			button.OnLeftClick += (a, b) => {
				SharedUI.instance.SelectedSort = this;
			};
		}

		public Sort(string hoverText, string textureFileName, Func<Item, Item, int> sort, Func<Item, string> badge) :
			this(hoverText, ItemChecklist.instance.Assets.Request<Texture2D>(textureFileName, AssetRequestMode.ImmediateLoad).Value, sort, badge) {
		}
	}

	// Represents a requested Category or Filter.
	internal class ModCategory
	{
		internal string name;
		internal string parent;
		internal Texture2D icon;
		internal Predicate<Item> belongs;
		public ModCategory(string name, string parent, Texture2D icon, Predicate<Item> belongs) {
			this.name = name;
			this.parent = parent;
			this.icon = icon;
			this.belongs = belongs;
		}
	}

	// Can belong to 2 Category? -> ??
	// Separate filter? => yes, but Separate conditional filters?
	// All children belong to parent -> yes.
	internal class Category // Filter
	{
		internal string name;
		internal Predicate<Item> belongs;
		internal List<Category> subCategories;
		internal List<Sort> sorts;
		internal List<Filter> filters;
		internal UISilentImageButton button;
		internal Category parent;

		public Category(string name, Predicate<Item> belongs, Texture2D texture = null) {
			if (texture == null)
				texture = ItemChecklist.instance.Assets.Request<Texture2D>("Images/sortAmmo", AssetRequestMode.ImmediateLoad).Value;
			this.name = name;
			subCategories = new List<Category>();
			sorts = new List<Sort>();
			filters = new List<Filter>();
			this.belongs = belongs;

			this.button = new UISilentImageButton(texture, name);
			button.OnLeftClick += (a, b) => {
				//Main.NewText("clicked on " + button.hoverText);
				SharedUI.instance.SelectedCategory = this;
			};
		}

		public Category(string name, Predicate<Item> belongs, string textureFileName) :
			this(name, belongs, ItemChecklist.instance.Assets.Request<Texture2D>(textureFileName, AssetRequestMode.ImmediateLoad).Value) {
		}

		internal bool BelongsRecursive(Item item) {
			if (belongs(item))
				return true;
			return subCategories.Any(x => x.belongs(item));
		}

		internal void ParentAddToSorts(List<Sort> availableSorts) {
			if (parent != null)
				parent.ParentAddToSorts(availableSorts);
			availableSorts.AddRange(sorts);
		}

		internal void ParentAddToFilters(List<Filter> availableFilters) {
			if (parent != null)
				parent.ParentAddToFilters(availableFilters);
			availableFilters.AddRange(filters);
		}
	}
}
