﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AposGui {
    /// <summary>
    /// Goal: Stacks components on top of each others.
    /// </summary>
    public class LayoutVertical : Layout {
        public LayoutVertical() { }
        public override void RecomputeChildren(List<Component> childs) {
            //Tell each children their position and size.
            Point position = Panel.Position;
            int width = Panel.Width;
            int height = Panel.Height;

            int offsetY = position.Y;
            foreach (Component c in childs) {
                int cHeight = c.PrefHeight;
                c.Width = width;
                c.Height = cHeight;
                c.Position = new Point(position.X, offsetY) + Panel.Offset;
                offsetY += cHeight;
                c.ClippingRect = Panel.ClippingRect;
            }
            Panel.Size = new Size2(width, offsetY);
        }
    }
}