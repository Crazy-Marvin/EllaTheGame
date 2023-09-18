using MonKey.Extensions;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Commands
{
    public static class UIUtilities
    {

        [Command("UI Anchor To Corners", "Moves the anchors of the UI elements to its corners",
            QuickName = "UAC",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "AnchorsToCorners", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "UI")]
        public static void AnchorsToCorners()
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
                Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

                t.anchorMin = newAnchorsMin;
                t.anchorMax = newAnchorsMax;
                t.offsetMin = t.offsetMax = new Vector2(0, 0);
            }
        }

        [Command("UI Corners To Anchor", "Moves the corners of the UI elements to its anchors",
            QuickName = "UCA",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "CornersToAnchors", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "UI")]
        public static void CornersToAnchors()
        {
            int undoGroup = MonkeyEditorUtils.CreateUndoGroup("Corners To Anchor");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                Undo.RecordObject(t, "anchor move");

                if (t == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                t.offsetMin = t.offsetMax = new Vector2(0, 0);
            }
            Undo.CollapseUndoOperations(undoGroup);
        }

        [Command("UI Mirror Horizontally", "Mirrors the rect transform relative to its parent",
            QuickName = "UMH",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "MirrorHorizontally", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "UI")]
        public static void MirrorHorizontally(
            [CommandParameter(Help = "If true, the anchor will be mirrored as well," +
                                     " otherwise just the offsets")]
            bool mirrorAnchors=false)
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
                    t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
                }

                Vector2 oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
                t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
            }
        }

        [Command("UI Mirror Vertically", "Mirrors the rect transform relative to its parent",
            QuickName = "UMV",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "MirrorVertically", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "UI")]
        public static void MirrorVertically(
            [CommandParameter(Help = "If true, the anchor will be mirrored as well," +
                                     " otherwise just the offsets")]
            bool mirrorAnchors=false)
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
                    t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
                }

                Vector2 oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
                t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);

                t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
            }
        }

        [Command("2D Scale Flip",
            "Flips the scale of the selected objects so they point in the opposite direction",
            QuickName = "2SF",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void Scale2DFlip(bool horizontal = true)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("scale flip");
            foreach (GameObject o in Selection.gameObjects)
            {
                Undo.RecordObject(o.transform, "scale");

                o.transform.localScale = new Vector3(horizontal ?
                        -o.transform.localScale.x : o.transform.localScale.x,
                    !horizontal ? -o.transform.localScale.y : o.transform.localScale.y,
                    o.transform.localScale.z);
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Flip Rotation",
            QuickName = "FR",
            Help = "Flips the rotations on the z axis of the selected objects so they point in the opposite direction",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void Rotation2DFlip(Axis axis = Axis.Y)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("rotation flip");
            foreach (GameObject o in Selection.gameObjects)
            {
                Undo.RecordObject(o.transform, "rotation");
                Vector3 axisVector = o.transform.AxisToVector(axis, true);
                o.transform.Rotate(axisVector, 180, Space.Self);
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Toggle Screen UI Visibility", "Shows or hides the screen UIs", QuickName = "TU",
            MenuItemLink = "ToggleScreenUIVisibility", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "UI")]
        public static void ToggleScreenUIElements()
        {
            Canvas[] uiObjects = Resources.FindObjectsOfTypeAll<Canvas>()
                .Where(_ => (_.hideFlags & HideFlags.NotEditable) == 0)
                .Where(_ => _.gameObject.scene.IsValid()).ToArray();
            int undoID = MonkeyEditorUtils.CreateUndoGroup("toggleUI");
            foreach (Canvas uiObject in uiObjects)
            {
                if (!uiObject.isRootCanvas || uiObject.renderMode == RenderMode.WorldSpace)
                {
                    continue;
                }

                Undo.RecordObject(uiObject.gameObject, "Toggle Active");
                uiObject.gameObject.SetActive(!uiObject.gameObject.activeSelf);
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Expand Anchors", "Expands the anchors to cover the parent", QuickName = "UE",
            MenuItemLink = "ExpandAnchors", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void ExpandAnchors()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {

                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Undo.RecordObject(t, "Expanding");
                    Undo.RecordObject(transform, "Expanding t");

                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = Vector2.zero;
                    t.anchorMax = Vector2.one;

                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Expand Transform", "Expands the anchors and the corners to cover the parent",
            QuickName = "UET",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void ExpandUITransform()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("expand all");
            ExpandAnchors();
            CornersToAnchors();
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Mirror Anchors Horizontally",
            "Mirrors the rect transform anchors relative to its parent",
            QuickName = "MAH",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void MirrorAnchorsHorizontally()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Mirror Anchors");

            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                Vector2 oldAnchorMin = t.anchorMin;
                t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
                t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
            }
            Undo.CollapseUndoOperations(undoID);

        }

        [Command("UI Mirror Anchors Vertically",
            "Mirrors the rect transform anchors relative to its parent",
            QuickName = "MAV",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void MirrorAnchorsVertically()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Mirror Anchors");

            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                Vector2 oldAnchorMin = t.anchorMin;
                t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
                t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
            }
            Undo.CollapseUndoOperations(undoID);

        }

        [Command("UI Center Anchors",
            "Centers Anchors given a reference anchor so that the distances to the parent corners are uniform",
            QuickName = "UCE", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void CenterAnchor(RectCorner corner = RectCorner.UP_LEFT)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Mirror Anchors");

            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                RectTransform pt = t != null ? t.parent as RectTransform : null;

                if (t == null || pt == null)
                {
                    return;
                }

                Undo.RecordObject(t, "Mirror");
                Undo.RecordObject(transform, "Mirror t");

                Vector2 previousMax = t.anchorMax;
                switch (corner)
                {
                    case RectCorner.UP_RIGHT:
                        t.anchorMin = Vector2.one - t.anchorMax;
                        break;
                    case RectCorner.UP_LEFT:
                        t.anchorMax = new Vector2(1 - t.anchorMin.x, t.anchorMax.y);
                        t.anchorMin = new Vector2(t.anchorMin.x, 1 - previousMax.y);
                        break;
                    case RectCorner.BOTTOM_RIGHT:
                        t.anchorMax = new Vector2(t.anchorMax.x, 1 - t.anchorMin.y);
                        t.anchorMin = new Vector2(1 - previousMax.x, t.anchorMin.y);
                        break;
                    case RectCorner.BOTTOM_LEFT:
                        t.anchorMax = Vector2.one - t.anchorMin;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Anchor Scale Position", "Scale the anchors of a rect transform" +
            " so that the position is not changes but the anchors are moved.", QuickName = "UAS",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void ScaleAnchor(
            [CommandParameter("The amount of scaling to apply on the anchor position")]
            float scaleFactor = 0.5f)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("scale anchor");
            foreach (var go in Selection.gameObjects)
            {
                RectTransform t = go.GetComponent<RectTransform>();
                if (t)
                {
                    Vector2 anchorCenter = new Vector2((t.anchorMax.x + t.anchorMin.x) / 2,
                        (t.anchorMax.y + t.anchorMin.y) / 2);

                    Undo.RecordObject(t, "update transform");

                    t.anchorMin = anchorCenter - new Vector2((anchorCenter.x - t.anchorMin.x) * scaleFactor,
                      (anchorCenter.y - t.anchorMin.y) * scaleFactor);
                    t.anchorMax = anchorCenter + new Vector2((t.anchorMax.x - anchorCenter.x) * scaleFactor,
                        (t.anchorMax.y - anchorCenter.y) * scaleFactor);
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Expand Anchors Width", "Expands the anchors width to cover the parent",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "UEW",
            Category = "UI")]
        public static void ExpandAnchorsWidth()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Undo.RecordObject(t, "Expanding");
                    Undo.RecordObject(transform, "Expanding t");

                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = new Vector2(0, t.anchorMin.y);
                    t.anchorMax = new Vector2(1, t.anchorMax.y);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;

                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Expand Anchors Height", "Expands the anchors width to cover the parent",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "UEH",
            Category = "UI")]
        public static void ExpandAnchorsHeight()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Undo.RecordObject(t, "Expanding");
                    Undo.RecordObject(transform, "Expanding t");
                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = new Vector2(t.anchorMin.x, 0);
                    t.anchorMax = new Vector2(t.anchorMax.x, 1);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Collapse Anchors", "Collapses the anchors to center them relative to the parent",
            QuickName = "UC", MenuItemLink = "CollapseAnchors", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "UI")]
        public static void CollapseAnchors()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Undo.RecordObject(t, "Collapsing");
                    Undo.RecordObject(transform, "Collapsing t");

                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = new Vector2(.5f, .5f);
                    t.anchorMax = new Vector2(.5f, .5f);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Collapse Anchors Width", "Collapse the anchors width to the center",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "UCW",
            Category = "UI")]
        public static void CollapseAnchorsWidth()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Undo.RecordObject(t, "Collapsing");
                    Undo.RecordObject(transform, "Collapsing t");

                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = new Vector2(.5f, t.anchorMin.y);
                    t.anchorMax = new Vector2(.5f, t.anchorMax.y);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("UI Collapse Anchors Height", "Collapse the anchors width to the center",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "UCH",
            Category = "UI")]
        public static void CollapseAnchorsHeight()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Expand Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform t = transform as RectTransform;
                if (t != null)
                {
                    Rect rect = t.rect;
                    Vector3 position = t.position;
                    t.anchorMin = new Vector2(t.anchorMin.x, .5f);
                    t.anchorMax = new Vector2(t.anchorMax.x, .5f);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                    t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                    t.position = position;
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }
    }
}
