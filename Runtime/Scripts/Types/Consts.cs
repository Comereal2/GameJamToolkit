namespace Types
{
    public class Consts
    {
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
    }
}