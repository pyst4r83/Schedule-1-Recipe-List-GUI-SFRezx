using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.StationFramework;
using MelonLoader;
using MelonLoader.Utils;
using System.Collections.Generic;
using UnityEngine;
[assembly: MelonInfo(typeof(RecipeListGui.RecipeListGuiClass), "Recipe List", "1.0.7", "Rezx, Community Updates By: ispa (Translation), pyst4r (effect colors)")]

namespace RecipeListGui
{
    public class RecipeListGuiClass : MelonMod
    {
        private static Dictionary<string, string> effectColors = new()
        {
            { "Anti-Gravity", "#235BCD" },
            { "Athletic", "#75C8FD" },
            { "Balding", "#C79232" },
            { "Bright-Eyed", "#BEF7FD" },
            { "Calming", "#FED09B" },
            { "Calorie-Dense", "#FE84F4" },
            { "Cyclopean", "#FEC174" },
            { "Disorienting", "#D16546" },
            { "Electrifying", "#55C8FD" },
            { "Energizing", "#9AFE6D" },
            { "Euphoric", "#FEEA74" },
            { "Explosive", "#FE4B40" },
            { "Focused", "#75F1FD" },
            { "Foggy", "#B0B0AF" },
            { "Gingeritis", "#FE8829" },
            { "Glowing", "#85E459" },
            { "Jennerising", "#FE8DF8" },
            { "Laxative", "#763C25" },
            { "Lethal", "#AB2232" },
            { "Long faced", "#FED961" },
            { "Munchies", "#C96E57" },
            { "Paranoia", "#C46762" },
            { "Refreshing", "#B2FE98" },
            { "Schizophrenic", "#645AFD" },
            { "Sedating", "#6B5FD8" },
            { "Seizure-Inducing", "#FEE900" },
            { "Shrinking", "#B6FEDA" },
            { "Slippery", "#A2DFFD" },
            { "Smelly", "#7DBC31" },
            { "Sneaky", "#7B7B7B" },
            { "Spicy", "#FE6B4C" },
            { "Thought-Provoking", "#FEA0CB" },
            { "Toxic", "#5F9A31" },
            { "Tropic Thunder", "#FE9F47" },
            { "Zombifying", "#71AB5D" }
        };

        private class DataForFullIngredentsList
        {
            public string Name { get; set; } = string.Empty;
            public int Qnt { get; set; }
        }

        private static Dictionary<string, string> _translationDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static string Translate(string englishText)
        {
            if (string.IsNullOrEmpty(englishText))
                return englishText;

            if (_translationDictionary.TryGetValue(englishText, out string? translation))
                return translation ?? englishText;

            return englishText;
        }

        private static void LoadTranslations()
        {
            string translationFilePath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "Translations", "RecipeListGUI_translations.txt");

            Melon<RecipeListGuiClass>.Logger.Msg($"Trying to load translations from: {translationFilePath}");

            if (File.Exists(translationFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(translationFilePath);
                    Melon<RecipeListGuiClass>.Logger.Msg($"Read {lines.Length} lines from translation file");

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                            continue;

                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            _translationDictionary[key] = value;
                        }
                    }
                    Melon<RecipeListGuiClass>.Logger.Msg($"Loaded {_translationDictionary.Count} translations");
                }
                catch (Exception ex)
                {
                    Melon<RecipeListGuiClass>.Logger.Error($"Error loading translations: {ex.Message}");
                }
            }
            else
            {
                string translationDir = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "Translations");
                if (!Directory.Exists(translationDir))
                {
                    try
                    {
                        Directory.CreateDirectory(translationDir);
                        Melon<RecipeListGuiClass>.Logger.Msg("Translation directory created. Please add RecipeListGUI_translations.txt file to the folder Mods/Translations/");
                    }
                    catch (Exception ex)
                    {
                        Melon<RecipeListGuiClass>.Logger.Error($"Failed to create directory for translations: {ex.Message}");
                    }
                }
                else
                {
                    Melon<RecipeListGuiClass>.Logger.Msg("Translation file not found. Please add RecipeListGUI_translations.txt Mods/Translations/");
                }
            }
        }

        private static MelonPreferences_Entry<float> _guiScale = null!;
        private static MelonPreferences_Entry<KeyCode> _toggleKeyCode = null!;
        private static MelonPreferences_Entry<KeyCode> _resetKeyCode = null!;
        private static MelonPreferences_Entry<float> _transparency = null!;
        private static MelonPreferences_Entry<Color> _pageColor = null!;
        public override void OnInitializeMelon()
        {
            LoadTranslations();
            MelonPreferences_Category melonCfgCategory = MelonPreferences.CreateCategory("RecipeListGUI");
            _guiScale = melonCfgCategory.CreateEntry<float>("GUI_Scale", 1f);
            _toggleKeyCode = melonCfgCategory.CreateEntry<KeyCode>("Open_And_Close_Button", KeyCode.F5);
            _resetKeyCode = melonCfgCategory.CreateEntry<KeyCode>("Reset_Button", KeyCode.F6);
            _transparency = melonCfgCategory.CreateEntry<float>("Transparency", 0.56f);
            _pageColor = melonCfgCategory.CreateEntry<Color>("Page_Color", Color.gray);

            string configPath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "RecipeGUI.cfg");
            melonCfgCategory.SetFilePath(configPath);

            if (!File.Exists(configPath))
            {
                melonCfgCategory.SaveToFile();
                Melon<RecipeListGuiClass>.Logger.Msg("Config file created");
            }
            else
            {
                melonCfgCategory.LoadFromFile();
                Melon<RecipeListGuiClass>.Logger.Msg("Config file loaded");
            }
            Melon<RecipeListGuiClass>.Logger.Msg($"{_toggleKeyCode.Value} to open");
            Melon<RecipeListGuiClass>.Logger.Msg($"{_resetKeyCode.Value} while gui is open to reset gui location");
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(_toggleKeyCode.Value))
            {
                ToggleMenu();
            }
            if (Input.GetKeyDown(_resetKeyCode.Value) && _guiShowen)
            {
                MelonLogger.Msg("F6 pressed reset gui location");
                _productListPageRect = new Rect(100, 20, 295, 300);
                _shouldMinimizeProductListPage = false;
                _favsListPageRect = new Rect(100, 325, 295, 300);
                _shouldMinimizeFavListPage = false;
                _recipeResultPageRect = new Rect(600, 20, 600, 600);
            }
        }

        private static bool _guiShowen;
        private static void ToggleMenu()
        {
            _guiShowen = !_guiShowen;
            if (_guiShowen)
            {
                MelonEvents.OnGUI.Subscribe(DrawPages, 50);
            }
            else
            {
                _listOfCreatedProducts = null;
                MelonEvents.OnGUI.Unsubscribe(DrawPages);
            }
        }

        private static void DrawPages()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(_guiScale.Value, _guiScale.Value, 1));
            GUIStyle style = CreateCustomWindowStyle(_pageColor.Value, _transparency.Value);

            _productListPageRect = GUI.Window(651, _productListPageRect, (GUI.WindowFunction)ProductListPage, "<b>" + Translate("Product List") + "</b>", style);
            _favsListPageRect = GUI.Window(652, _favsListPageRect, (GUI.WindowFunction)FavListPage, "<b>" + Translate("Favorite List") + "</b>", style);

            if (_hasSelectedBud)
            {
                _recipeResultPageRect = GUI.Window(653, _recipeResultPageRect, (GUI.WindowFunction)RecipePage, "<b>" + Translate("Recipe") + "</b>", style);
            }
        }

        private static GUIStyle CreateCustomWindowStyle(Color backgroundColor, float transparency)
        {
            Texture2D bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, transparency));
            bgTex.Apply();

            GUIStyle style = new GUIStyle(GUI.skin.window);

            style.normal.background = bgTex;
            style.onNormal.background = bgTex;
            style.hover.background = bgTex;
            style.active.background = bgTex;
            style.focused.background = bgTex;

            style.alignment = TextAnchor.UpperCenter;
            style.normal.textColor = Color.white;
            style.onNormal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.focused.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.border = new RectOffset(0, 0, 0, 0);

            return style;
        }

        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? GetlistOfCreatedProducts()
        {
            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                return new Il2CppSystem.Collections.Generic.List<ProductDefinition>();
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                return new Il2CppSystem.Collections.Generic.List<ProductDefinition>();
            }
            return productManagerComp.AllProducts;
        }

        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? GetlistOf_FavProducts()
        {
            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                return new Il2CppSystem.Collections.Generic.List<ProductDefinition>();
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                return new Il2CppSystem.Collections.Generic.List<ProductDefinition>();
            }
            Il2CppSystem.Collections.Generic.List<ProductDefinition> favourites = ProductManager.FavouritedProducts;

            return favourites ?? new Il2CppSystem.Collections.Generic.List<ProductDefinition>();
        }

        private static float _costToMake;
        static Il2CppSystem.Collections.Generic.List<string> GetIngredientList(ProductDefinition product, int selectedProductRecipe)
        {
            Il2CppSystem.Collections.Generic.List<string> outputLines = new();
            List<DataForFullIngredentsList> ingredientListTemp = new();

            _costToMake = 0;
            ProcessProduct(product, outputLines, selectedProductRecipe, true, ingredientListTemp);
            outputLines.Add("-----------------------------------------------------------------------------------");
            outputLines.Add($"{Translate("Market Price")} {product.MarketValue}, {Translate("Addictiv")}: {Math.Round(product.GetAddictiveness())}, {Translate("Cost")}: {_costToMake}, {Translate("After Cost")}: {product.MarketValue - _costToMake} ({Translate("base ingredients not included")})");
            outputLines.Add("-----------------------------------------------------------------------------------");

            foreach (var ingredient in ingredientListTemp)
            {
                outputLines.Add($"{ingredient.Qnt}x {Translate(ingredient.Name)} ");
            }
            return outputLines;
        }

        static int _currentStep = 0; // Neue Klassenvariable für die Schritt-Zählung

        static void ProcessProduct(ProductDefinition product, Il2CppSystem.Collections.Generic.List<string> outputLines, int recipeToUse, bool isSelectedProductRecipe, System.Collections.Generic.List<DataForFullIngredentsList> ingredientListTemp, bool isRoot = true)
        {
            if (!isSelectedProductRecipe && product == _selectedBud)
            {
                return;
            }

            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                return;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                return;
            }

            if (product.Recipes.Count >= 1 && !productManagerComp.DefaultKnownProducts.Contains(product))
            {
                var recipes = product.Recipes;
                StationRecipe recipe = recipes.ToArray().ToList()[recipeToUse];

                // Zurücksetzen der Zählung für neuen Durchlauf
                if (isRoot && isSelectedProductRecipe)
                {
                    _currentStep = 0;
                }

                var currentProductOutput = new Il2CppSystem.Collections.Generic.List<string>();
                var currentProductIngredients = new System.Collections.Generic.List<DataForFullIngredentsList>();
                var nestedProducts = new Il2CppSystem.Collections.Generic.List<ProductDefinition>();

                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredient.Item.Category.ToString() == "Product")
                    {
                        ProductDefinition? drug = ingredient.Item.TryCast<ProductDefinition>();
                        if (drug != null)
                        {
                            currentProductOutput.Add($"{ingredient.Quantity}x {Translate(drug.Name)}");

                            if (drug.Recipes.Count >= 1)
                            {
                                nestedProducts.Add(drug);
                            }
                        }
                    }
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    string cat = ingredient.Item.Category.ToString();
                    if (cat == "Consumable" || cat == "Ingredient")
                    {
                        StorableItemDefinition? prop = ingredient.Item.TryCast<StorableItemDefinition>();
                        if (prop != null)
                        {
                            currentProductOutput.Add($"{ingredient.Quantity}x {Translate(prop.Name)} {prop.BasePurchasePrice}$");

                            var existingIngredient = currentProductIngredients.FirstOrDefault(i => i.Name == prop.Name);
                            if (existingIngredient != null)
                            {
                                existingIngredient.Qnt += ingredient.Quantity;
                            }
                            else
                            {
                                currentProductIngredients.Add(new DataForFullIngredentsList() { Name = prop.Name, Qnt = ingredient.Quantity });
                            }

                            _costToMake += prop.BasePurchasePrice * ingredient.Quantity;
                            if (!ingredientIcons.ContainsKey(prop.Name))
                            {
                                ingredientIcons.Add(prop.Name, prop.Icon);
                            }
                        }
                    }
                }

                if (isRoot && isSelectedProductRecipe)
                {
                    outputLines.Add($"<b>{Translate("Recipe for")} <i>{Translate(recipe.RecipeTitle)}</i></b>");

                    var effectsBuilder = new System.Text.StringBuilder();
                    int effectCounter = 0;
                    foreach (var effect in product.Properties)
                    {
                        string colorHex = effectColors.ContainsKey(effect.Name) ? effectColors[effect.Name] : "#FFFFFF";
                        string coloredEffect = $"<b><color={colorHex}>{Translate(effect.Name)}</color></b>";

                        if (effectCounter > 0) effectsBuilder.Append(", ");
                        effectsBuilder.Append(coloredEffect);
                        effectCounter++;

                        if (effectCounter % 4 == 0 && effectCounter != product.Properties.Count)
                        {
                            outputLines.Add(effectsBuilder.ToString());
                            effectsBuilder.Clear();
                            effectCounter = 0;
                        }
                    }
                    if (effectsBuilder.Length > 0) outputLines.Add(effectsBuilder.ToString());
                    outputLines.Add($"-----------------------------------------------------------------------------------");
                }

                foreach (var nestedProduct in nestedProducts)
                {
                    ProcessProduct(nestedProduct, outputLines, 0, false, ingredientListTemp, false);
                }

                if (!isRoot || !isSelectedProductRecipe)
                {
                    _currentStep++;
                }

                if (!isRoot || !isSelectedProductRecipe)
                {
                    outputLines.Add($"<b>Zwischenprodukt {_currentStep}: <i>{Translate(recipe.RecipeTitle)}</i></b>");
                }
                else
                {
                    outputLines.Add($"<b>Endprodukt: <i>{Translate(recipe.RecipeTitle)}</i></b>");
                }

                foreach (var line in currentProductOutput)
                {
                    outputLines.Add(line);
                }

                ingredientListTemp.AddRange(currentProductIngredients);
            }
        }

        private static Il2CppSystem.Collections.Generic.List<string> _ingredientListRecipePage = new Il2CppSystem.Collections.Generic.List<string>();
        private static Vector2 _recipeResultPageScrollViewVector = Vector2.zero;
        private static Rect _recipeResultPageRect = new Rect(600, 20, 600, 600);
        private static bool _hasSelectedProductRecipe;
        private static int _selectedProductRecipeIndex;
        private static ProductDefinition _lastSelectedBud = null!;
        private static Dictionary<string, Sprite> ingredientIcons = new();

        static void RecipePage(int windowId)
        {
            Rect resizeHandleRect = new Rect(_recipeResultPageRect.width - 50, _recipeResultPageRect.height - 50, 25, 25);
            GUI.Box(resizeHandleRect, "");

            if (GUI.Button(new Rect(_recipeResultPageRect.width - 45, 20, 30, 30), "X"))
            {
                _selectedBud = null!;
                _hasSelectedBud = false;
                return;
            }

            if (_selectedBud.Recipes.Count > 1)
            {
                if (_hasSelectedProductRecipe)
                {
                    if (GUI.Button(new Rect(_recipeResultPageRect.width - 85, 20, 30, 30), Translate("B")))
                    {
                        _hasSelectedProductRecipe = false;
                        _ingredientListRecipePage = null!;
                    }
                }

                if (!_hasSelectedProductRecipe)
                {
                    for (int i = 0; i < _selectedBud.Recipes.Count; i++)
                    {
                        if (GUI.Button(new Rect(_recipeResultPageRect.width / 3, 50 + 20 * i, 200, 20), $"{Translate("Recipe")} {i + 1}"))
                        {
                            _hasSelectedProductRecipe = true;
                            _selectedProductRecipeIndex = i;
                        }
                    }
                    ProcessResize(resizeHandleRect);
                    GUI.DragWindow(new Rect(0, 0, _recipeResultPageRect.width, _recipeResultPageRect.height - 30));
                    return;
                }

            }

            if (_selectedBud != null)
            {
                if (_ingredientListRecipePage == null || _selectedBud != _lastSelectedBud)
                {
                    _ingredientListRecipePage = GetIngredientList(_selectedBud, _selectedProductRecipeIndex);
                    _lastSelectedBud = _selectedBud;
                }
                _recipeResultPageScrollViewVector = GUI.BeginScrollView(new Rect(55, 20, _recipeResultPageRect.width - 55, _recipeResultPageRect.height), _recipeResultPageScrollViewVector, new Rect(0, 0, _recipeResultPageRect.width - 55, _ingredientListRecipePage.Count * 30));
                var ingredientListTemp = _ingredientListRecipePage.ToArray().ToList();
                for (int i = 0; i < _ingredientListRecipePage.Count; i++)
                {
                    string currentingredient = ingredientListTemp[i];
                    if (!currentingredient.StartsWith(" ") && !currentingredient.StartsWith("Price") && !currentingredient.StartsWith("Recipe") && !currentingredient.StartsWith("----"))
                    {
                        string[] currentingredientSplit = currentingredient.Split(' ');
                        string ingredientFromSplit = "";
                        string lastWord = currentingredientSplit.Last();

                        if (lastWord.Contains("$"))
                        {
                            ingredientFromSplit = string.Join(" ", currentingredientSplit.Skip(1).Take(currentingredientSplit.Length - 2));
                        }
                        else
                        {
                            ingredientFromSplit = string.Join(" ", currentingredientSplit.Skip(1));
                        }

                        ingredientFromSplit = ingredientFromSplit.Trim();


                        Sprite? icon = ingredientIcons.ContainsKey(ingredientFromSplit) ? ingredientIcons[ingredientFromSplit] : null;
                        if (icon != null)
                        {
                            GUI.DrawTexture(new Rect(8, 40 + (20 * i), 22, 22), icon.texture);
                        }
                    }
                    GUI.Label(new Rect(50, 40 + (20 * i), _recipeResultPageRect.width - 75, 20), ingredientListTemp[i]);
                }
                GUI.EndScrollView();
            }
            ProcessResize(resizeHandleRect);
            GUI.DragWindow(new Rect(0, 0, _recipeResultPageRect.width, _recipeResultPageRect.height - 30));
        }

        private static Vector2 _productListPageScrollViewVector = Vector2.zero;
        private static Rect _productListPageRect = new Rect(100, 20, 295, 55);
        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? _listOfCreatedProducts;
        private static bool _hasSelectedProductType;
        private static string _typeOfDrugToFilter = "";
        private static bool _hasSelectedBud;
        private static ProductDefinition _selectedBud = null!;
        private static bool _shouldMinimizeProductListPage = true;
        private static bool _sortProductListPageByPrice = false;
        
        static void ProductListPage(int windowID)
        {
            if (_shouldMinimizeProductListPage)
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 25, 2, 18, 17), "+"))
                {
                    _shouldMinimizeProductListPage = false;
                    _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, 295, 300);
                }
                GUI.DragWindow(new Rect(20, 10, 500, 500));
                return;
            }
            else
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 25, 2, 17, 17), "-"))
                {
                    _shouldMinimizeProductListPage = true;
                    _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, 295, 55);
                    return;
                }
            }

            _listOfCreatedProducts ??= GetlistOfCreatedProducts();
            if (_listOfCreatedProducts == null)
            {
                return;
            }

            if (!_hasSelectedProductType)
            {
                if (GUI.Button(new Rect(50, 20, 200, 20), Translate("Marijuana")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Marijuana";
                }
                if (GUI.Button(new Rect(50, 40, 200, 20), Translate("Methamphetamine")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Methamphetamine";
                }
                if (GUI.Button(new Rect(50, 60, 200, 20), Translate("Cocaine")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Cocaine";
                }
                if (GUI.Button(new Rect(50, 80, 200, 20), Translate("All Products")))
                {
                    _hasSelectedProductType = true;
                }

                GUI.DragWindow(new Rect(40, 10, 500, 500));

                if (!_hasSelectedProductType)
                {
                    return;
                }
            }
            else
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 37, 20, 29, 27), "B"))
                {
                    _hasSelectedProductType = false;
                    _typeOfDrugToFilter = "";
                    _productListPageScrollViewVector = Vector2.zero;
                }

                if (GUI.Button(new Rect(_productListPageRect.width - 37, 50, 29, 27), "$"))
                {
                    _sortProductListPageByPrice = !_sortProductListPageByPrice;
                    _productListPageScrollViewVector = Vector2.zero;
                }
            }

            var sortedProducts = _listOfCreatedProducts.ToArray().ToList();
            if (_typeOfDrugToFilter != "")
            {
                sortedProducts = sortedProducts.Where(product => product.DrugType.ToString() == _typeOfDrugToFilter).ToList();
            }

            if (_sortProductListPageByPrice)
            {
                sortedProducts = sortedProducts.OrderByDescending(product => product.MarketValue).ToList();
            }

            _productListPageScrollViewVector = GUI.BeginScrollView(new Rect(55, 20, 300, 300), _productListPageScrollViewVector, new Rect(0, 0, 300, sortedProducts.Count * 20 + 10));
            int spacer = 0;
            foreach (var createdProduct in sortedProducts)
            {
                if (GUI.Button(new Rect(0, 20 * spacer, 160, 20), Translate(createdProduct.name)))
                {
                    _selectedBud = createdProduct;
                    _hasSelectedBud = true;
                    _ingredientListRecipePage = new Il2CppSystem.Collections.Generic.List<string>();
                    _hasSelectedProductRecipe = false;
                    _selectedProductRecipeIndex = 0;
                }
                GUI.Label(new Rect(165, 20 * spacer, 50, 20), $"${createdProduct.MarketValue}");

                spacer++;
            }
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(20, 10, 500, 500));
        }

        private static Vector2 _favsListPageScrollViewVector = Vector2.zero;
        private static Rect _favsListPageRect = new Rect(100, 325, 295, 55);
        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? _listOf_FavsProducts;
        private static bool _shouldMinimizeFavListPage = true;
        private static bool _sortFavListPageByPrice = false;

        static void FavListPage(int windowID)
        {
            if (_shouldMinimizeFavListPage)
            {
                if (GUI.Button(new Rect(_favsListPageRect.width - 25, 2, 18, 17), "+"))
                {
                    _shouldMinimizeFavListPage = false;
                    _favsListPageRect = new Rect(_favsListPageRect.x, _favsListPageRect.y, 295, 300);
                    return;
                }
                GUI.DragWindow(new Rect(20, 10, 500, 500));
                return;
            }
            else
            {
                if (GUI.Button(new Rect(_favsListPageRect.width - 25, 2, 17, 17), "-"))
                {
                    _shouldMinimizeFavListPage = true;
                    _favsListPageRect = new Rect(_favsListPageRect.x, _favsListPageRect.y, 295, 55);
                    return;
                }
            }
            
            if (GUI.Button(new Rect(_favsListPageRect.width - 37, 20, 29, 27), "$"))
            {
                _sortFavListPageByPrice = !_sortFavListPageByPrice;
                _favsListPageScrollViewVector = Vector2.zero;
            }
            
            _listOf_FavsProducts ??= GetlistOf_FavProducts();
            if (_listOf_FavsProducts == null)
            {
                return;
            }

            var filteredFavProducts = _listOf_FavsProducts.ToArray().ToList();
            
            if (_sortFavListPageByPrice)
            {
                filteredFavProducts = filteredFavProducts.OrderByDescending(product => product.MarketValue).ToList();
            }

            _favsListPageScrollViewVector = GUI.BeginScrollView(new Rect(55, 20, 300, 300), _favsListPageScrollViewVector, new Rect(0, 0, 300, filteredFavProducts.Count * 20 + 10));

            int spacer = 0;
            foreach (var favProduct in filteredFavProducts)
            {
                if (GUI.Button(new Rect(0, 20 * spacer, 160, 20), Translate(favProduct.name)))
                {
                    _selectedBud = favProduct;
                    _hasSelectedBud = true;
                    _ingredientListRecipePage = new Il2CppSystem.Collections.Generic.List<string>();
                    _hasSelectedProductRecipe = false;
                    _selectedProductRecipeIndex = 0;
                }
                GUI.Label(new Rect(165, 20 * spacer, 50, 20), $"${favProduct.MarketValue}");
                spacer++;
            }
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(20, 10, 500, 500));
        }

        private static bool _isResizing;
        private static Vector2 _initialMousePosition;
        private static Rect _initialWindowRect;
        private static Vector2 _minWindowSize = new Vector2(375, 400);
        static void ProcessResize(Rect handleRect)
        {
            Event currentEvent = Event.current;
            Vector2 mousePos = currentEvent.mousePosition;
            Vector2 screenMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (handleRect.Contains(mousePos))
                    {
                        _isResizing = true;
                        _initialMousePosition = screenMousePos;
                        _initialWindowRect = _recipeResultPageRect;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isResizing)
                    {
                        Vector2 currentMousePos = screenMousePos;
                        float newWidth = Mathf.Max(_minWindowSize.x, _initialWindowRect.width + (currentMousePos.x - _initialMousePosition.x));
                        float newHeight = Mathf.Max(_minWindowSize.y, _initialWindowRect.height + (currentMousePos.y - _initialMousePosition.y));

                        _recipeResultPageRect.width = newWidth;
                        _recipeResultPageRect.height = newHeight;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_isResizing)
                    {
                        _isResizing = false;
                        currentEvent.Use();
                    }
                    break;
            }
        }
    }
}
