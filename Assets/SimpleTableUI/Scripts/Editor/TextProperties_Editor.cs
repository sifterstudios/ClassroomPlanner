using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;

namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(TextProperties))]
    public class TextProperties_Editor : Editor
    {
        static readonly GUIContent k_RtlToggleLabel =
            new("Enable RTL Editor", "Reverses text direction and allows right to left editing.");

        static readonly GUIContent k_MainSettingsLabel = new("Main Settings");

        static readonly GUIContent k_FontAssetLabel = new("Font Asset",
            "The Font Asset containing the glyphs that can be rendered for this text.");

        static readonly GUIContent k_MaterialPresetLabel = new("Material Preset",
            "The material used for rendering. Only materials created from the Font Asset can be used.");

        static readonly GUIContent k_FontStyleLabel =
            new("Font Style", "Styles to apply to the text such as Bold or Italic.");

        static readonly GUIContent k_FontSizeLabel =
            new("Font Size", "The size the text will be rendered at in points.");

        static readonly GUIContent k_AutoSizeLabel =
            new("Auto Size", "Auto sizes the text to fit the available space.");

        static readonly GUIContent k_AutoSizeOptionsLabel = new("Auto Size Options");
        static readonly GUIContent k_MinLabel = new("Min", "The minimum font size.");
        static readonly GUIContent k_MaxLabel = new("Max", "The maximum font size.");

        static readonly GUIContent k_WdLabel = new("WD%",
            "Compresses character width up to this value before reducing font size.");

        static readonly GUIContent k_LineLabel = new("Line",
            "Negative value only. Compresses line height down to this value before reducing font size.");

        static readonly GUIContent k_BaseColorLabel = new("Vertex Color", "The base color of the text vertices.");

        static readonly GUIContent k_BoldLabel = new("B", "Bold");
        static readonly GUIContent k_ItalicLabel = new("I", "Italic");
        static readonly GUIContent k_UnderlineLabel = new("U", "Underline");
        static readonly GUIContent k_StrikethroughLabel = new("S", "Strikethrough");
        static readonly GUIContent k_LowercaseLabel = new("ab", "Lowercase");
        static readonly GUIContent k_UppercaseLabel = new("AB", "Uppercase");
        static readonly GUIContent k_SmallcapsLabel = new("SC", "Smallcaps");

        static readonly GUIContent k_AlignmentLabel =
            new("Alignment", "Horizontal and vertical aligment of the text within its container.");

        static readonly GUIContent k_WrapMixLabel = new("Wrap Mix (W <-> C)",
            "How much to favor words versus characters when distributing the text.");

        protected static readonly GUIContent k_ExtraSettingsLabel = new("Extra Settings");
        protected GUIContent[] m_MaterialPresetNames;


        protected Material[] m_MaterialPresets;
        protected int m_MaterialPresetSelectionIndex;


        Rect rect;
        SerializedProperty textAlignment;
        TextProperties tp;

        void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoEvent;
            if (tp == null)
                tp = target as TextProperties;
            if (tp == null)
                return;
            if (tp.FontAsset == null)
                tp.FontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

            m_MaterialPresetNames = GetMaterialPresets();

            textAlignment = serializedObject.FindProperty("_alignment");
        }


        void OnUndoRedoEvent()
        {
            Undo.undoRedoPerformed -= OnUndoRedoEvent;
        }

        public override void OnInspectorGUI()
        {
            Font();
            Color();
            Alignment();
        }

        void Font()
        {
            EditorGUI.BeginChangeCheck();
            var fontAsset =
                (TMP_FontAsset) EditorGUILayout.ObjectField(k_FontAssetLabel, tp.FontAsset, typeof(TMP_FontAsset),
                    true);

            if (EditorGUI.EndChangeCheck())
            {
                tp.textPropertiesUndoEvent();
                tp.FontAsset = fontAsset;
            }

            rect = EditorGUILayout.GetControlRect(false, 17);

            int v1, v2, v3, v4, v5, v6, v7;
            rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2f);
            EditorGUI.PrefixLabel(rect, k_FontStyleLabel);

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;

            rect.width = Mathf.Max(25f, rect.width / 7f);

            v1 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 1) == 1, k_BoldLabel,
                TMP_UIStyleManager.alignmentButtonLeft)
                ? 1
                : 0; // Bold
            rect.x += rect.width;
            v2 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 2) == 2, k_ItalicLabel,
                TMP_UIStyleManager.alignmentButtonMid)
                ? 2
                : 0; // Italics
            rect.x += rect.width;
            v3 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 4) == 4, k_UnderlineLabel,
                TMP_UIStyleManager.alignmentButtonMid)
                ? 4
                : 0; // Underline
            rect.x += rect.width;
            v7 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 64) == 64, k_StrikethroughLabel,
                TMP_UIStyleManager.alignmentButtonRight)
                ? 64
                : 0; // Strikethrough
            rect.x += rect.width;

            var selected = 0;


            EditorGUI.BeginChangeCheck();
            v4 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 8) == 8, k_LowercaseLabel,
                TMP_UIStyleManager.alignmentButtonLeft)
                ? 8
                : 0; // Lowercase
            if (EditorGUI.EndChangeCheck() && v4 > 0) selected = v4;
            rect.x += rect.width;
            EditorGUI.BeginChangeCheck();
            v5 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 16) == 16, k_UppercaseLabel,
                TMP_UIStyleManager.alignmentButtonMid)
                ? 16
                : 0; // Uppercase
            if (EditorGUI.EndChangeCheck() && v5 > 0) selected = v5;
            rect.x += rect.width;
            EditorGUI.BeginChangeCheck();
            v6 = TMP_EditorUtility.EditorToggle(rect, (tp.FontStyle & 32) == 32, k_SmallcapsLabel,
                TMP_UIStyleManager.alignmentButtonRight)
                ? 32
                : 0; // Smallcaps
            if (EditorGUI.EndChangeCheck() && v6 > 0) selected = v6;

            if (selected > 0)
            {
                v4 = selected == 8 ? 8 : 0;
                v5 = selected == 16 ? 16 : 0;
                v6 = selected == 32 ? 32 : 0;
            }

            var fontStyle = v1 + v2 + v3 + v4 + v5 + v6 + v7;

            if (fontStyle != tp.FontStyle)
            {
                tp.textPropertiesUndoEvent();
                tp.FontStyle = fontStyle;
            }


            EditorGUI.BeginDisabledGroup(tp.AutoSize);
            EditorGUI.BeginChangeCheck();
            var fontSize = EditorGUILayout.FloatField(k_FontSizeLabel, tp.FontSize,
                GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 50f));
            if (EditorGUI.EndChangeCheck())
            {
                tp.textPropertiesUndoEvent();
                tp.FontSize = fontSize;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel += 1;

            EditorGUI.BeginChangeCheck();
            var autoSize = EditorGUILayout.Toggle(k_AutoSizeLabel, tp.AutoSize);
            if (EditorGUI.EndChangeCheck())
            {
                tp.textPropertiesUndoEvent();
                tp.AutoSize = autoSize;
            }

            if (tp.AutoSize)
            {
                rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                EditorGUI.PrefixLabel(rect, k_AutoSizeOptionsLabel);
                var previousIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                rect.width = (rect.width - EditorGUIUtility.labelWidth) / 4f;
                rect.x += EditorGUIUtility.labelWidth;

                EditorGUI.BeginChangeCheck();
                EditorGUIUtility.labelWidth = 24;
                var fontSizeMin = EditorGUI.FloatField(rect, k_MinLabel, tp.FontSizeMin);
                rect.x += rect.width;
                EditorGUIUtility.labelWidth = 27;
                var fontSizeMax = EditorGUI.FloatField(rect, k_MaxLabel, tp.FontSizeMax);
                rect.x += rect.width;
                EditorGUIUtility.labelWidth = 36;
                var characterWidthAdjustment = EditorGUI.FloatField(rect, k_WdLabel, tp.CharacterWidthAdjustment);
                rect.x += rect.width;
                EditorGUIUtility.labelWidth = 28;
                var lineSpacingAdjustment = EditorGUI.FloatField(rect, k_LineLabel, tp.LineSpacingAdjustment);

                EditorGUIUtility.labelWidth = 0;
                EditorGUI.indentLevel = previousIndent;

                if (EditorGUI.EndChangeCheck())
                {
                    tp.textPropertiesUndoEvent();
                    tp.FontSizeMin = fontSizeMin;
                    tp.FontSizeMax = fontSizeMax;
                    tp.CharacterWidthAdjustment = characterWidthAdjustment;
                    tp.LineSpacingAdjustment = lineSpacingAdjustment;
                }
            }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }

        void Color()
        {
            EditorGUI.BeginChangeCheck();
            var vertexColor = EditorGUILayout.ColorField(k_BaseColorLabel, tp.VertexColor);
            if (EditorGUI.EndChangeCheck())
            {
                tp.textPropertiesUndoEvent();
                tp.VertexColor = vertexColor;
            }
        }

        void Alignment()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            EditorGUILayout.PropertyField(textAlignment, k_AlignmentLabel);
            if (EditorGUI.EndChangeCheck())
            {
                tp.textPropertiesUndoEvent();
                tp.Alignment = (TextAlignmentOptions) textAlignment.intValue;
                serializedObject.ApplyModifiedProperties();
            }

            if (((HorizontalAlignmentOptions) textAlignment.intValue & HorizontalAlignmentOptions.Justified) ==
                HorizontalAlignmentOptions.Justified ||
                ((HorizontalAlignmentOptions) textAlignment.intValue & HorizontalAlignmentOptions.Flush) ==
                HorizontalAlignmentOptions.Flush)
            {
                EditorGUI.BeginChangeCheck();
                var wrapMixWC = EditorGUILayout.Slider(k_WrapMixLabel, tp.WrapMixWC, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    tp.textPropertiesUndoEvent();
                    tp.WrapMixWC = wrapMixWC;
                }
            }
        }

        protected GUIContent[] GetMaterialPresets()
        {
            var fontAsset = tp.FontAsset;
            if (fontAsset == null) return null;

            m_MaterialPresets = TMP_EditorUtility.FindMaterialReferences(fontAsset);
            m_MaterialPresetNames = new GUIContent[m_MaterialPresets.Length];

            for (var i = 0; i < m_MaterialPresetNames.Length; i++)
                m_MaterialPresetNames[i] = new GUIContent(m_MaterialPresets[i].name);


            return m_MaterialPresetNames;
        }
    }
}