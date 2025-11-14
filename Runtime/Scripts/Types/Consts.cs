using UnityEngine;

namespace Types
{
    public class Consts
    {
        #region FilePaths
        
        public const string PackagePath = "Packages/com.comereal.gamejamtoolkit/";

        public const string MaterialFolderPath = PackagePath + "Materials/";
        public const string SDFOutlineMatPath = MaterialFolderPath + "sdf_outlinemat.mat";
        
        /// <summary>
        /// This variable requires the use of <see cref="UnityEditor.AssetDatabase"/>.<see cref="UnityEditor.AssetDatabase.GetBuiltinExtraResource"/> to retrieve the sprite.
        /// </summary>
        public const string UnityUISpritesPath = "UI/Skin/";

        /// <summary>
        /// <inheritdoc cref="UnityUISpritesPath"/>
        /// </summary>
        public const string BackgroundSpritePath = UnityUISpritesPath + "Background.psd";
        
        /// <summary>
        /// <inheritdoc cref="UnityUISpritesPath"/>
        /// </summary>
        public const string CheckmarkSpritePath = UnityUISpritesPath + "Checkmark.psd";
        
        /// <summary>
        /// <inheritdoc cref="UnityUISpritesPath"/>
        /// </summary>
        public const string KnobSpritePath = UnityUISpritesPath + "Knob.psd";

        /// <summary>
        /// <inheritdoc cref="UnityUISpritesPath"/>
        /// </summary>
        public const string UISpriteSpritePath = UnityUISpritesPath + "UISprite.psd";

        /// <summary>
        /// <inheritdoc cref="UnityUISpritesPath"/>
        /// </summary>
        public const string DropdownArrowSpritePath = UnityUISpritesPath + "DropdownArrow.psd";
        
        #endregion

        #region PresetUIInfo
        
        public const float PropWidth = 300f;
        
        #region Vectors
        
        public static Vector2 ButtonSize { get; } = new(160f, 30f);
        public static Vector2 DropdownSize => ButtonSize;
        public static Vector2 InputFieldSize => ButtonSize;
        public static Vector2 ToggleSize { get; } = new(160f, 20f);
        public static Vector2 SliderSize => ToggleSize;

        public static Vector2 ButtonScale { get; } = new(PropWidth / ButtonSize.x, PropWidth / ButtonSize.x);
        public static Vector2 DropdownScale { get; } = new(PropWidth / DropdownSize.x, PropWidth / DropdownSize.x);
        public static Vector2 InputFieldScale { get; } = new(PropWidth / InputFieldSize.x, PropWidth / InputFieldSize.x);
        public static Vector2 ToggleScale { get; } = new(PropWidth / ToggleSize.x, PropWidth / ToggleSize.x);
        public static Vector2 SliderScale { get; } = new(PropWidth / SliderSize.x, PropWidth / SliderSize.x);

        #endregion

        #endregion
    }
}