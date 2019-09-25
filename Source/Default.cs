using System;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using Microsoft.Xna.Framework;

namespace Apos.Gui {
    /// <summary>
    /// Predefined inputs for Mouse, Keyboard, Gamepad, Touchscreen.
    /// </summary>
    public static class Default {

        // Group: Public Variables

        /// <returns>Returns true when the mouse is inside the clip area of a component.</returns>
        public static Func<Component, bool> ConditionHoverMouse = (Component c) => c.IsInsideClip(GuiHelper.MouseToUI());
        /// <returns>Returns true when a component just got hovered.</returns>
        public static Func<Component, bool> ConditionGotHovered = (Component c) => !c.OldIsHovered && c.IsHovered;
        /// <returns>
        /// Returns true when gamepad 0's A button or space or enter or left mouse button are released.
        /// The left mouse button requires that the component is hovered.
        /// </returns>
        public static Func<Component, bool> ConditionInteraction = (Component c) =>
            c.HasFocus && (buttonReleased(GamePadButton.A, 0) ||
                           buttonReleased(Keys.Space) || buttonReleased(Keys.Enter)) ||
            c.IsHovered && buttonReleased(MouseButton.LeftButton);
        /// <returns>
        /// Returns true when gamepad 0's left thumbstick has just been made positive or the up arrow key is released.
        /// </returns>
        public static Func<bool> ConditionPreviousFocus = () =>
            InputHelper.OldGamePad[0].ThumbSticks.Left.Y <= 0 && InputHelper.NewGamePad[0].ThumbSticks.Left.Y > 0 ||
            buttonReleased(Keys.Up);
        /// <returns>
        /// Returns true when gamepad 0's left thumbstick has just been made negative or the down arrow key is released.
        /// </returns>
        public static Func<bool> ConditionNextFocus = () =>
            InputHelper.OldGamePad[0].ThumbSticks.Left.Y >= 0 && InputHelper.NewGamePad[0].ThumbSticks.Left.Y < 0 ||
            buttonReleased(Keys.Down);
        /// <returns>
        /// Returns true when gamepad 0's B button is released or the escape key is released.
        /// </returns>
        public static Func<bool> ConditionBackFocus = () =>
            buttonReleased(GamePadButton.B, 0) ||
            buttonReleased(Keys.Escape);
        /// <returns>
        /// Always returns true. This is useful when you want a condition to mark an input as used.
        /// When an input is marked as used, the component will request to be put in focus.
        /// </returns>
        public static Func<Component, bool> ConsumeCondition = (Component c) => true;
        /// <returns>Returns true when a component is hovered and the mouse wheel is being scrolled.</returns>
        public static Func<Component, bool> IsScrolled = (Component c) => {
            return c.IsHovered && GuiHelper.ScrollWheelDelta != 0;
        };
        /// <returns>This should be strictly used on a panel so that it can be scrolled vertically.</returns>
        /// <seealso cref="IsScrolled"/>
        public static Func<Component, bool> ScrollVertically = (Component c) => {
            Panel p = (Panel)c;
            int scrollWheelDelta = GuiHelper.ScrollWheelDelta;
            p.Offset = new Point(p.Offset.X, (int)Math.Min(Math.Max(p.Offset.Y + scrollWheelDelta, p.ClippingRect.Height - p.Size.Height), 0));

            return true;
        };
        /// <returns>This should be strictly used on a panel so that it can be scrolled horizontally.</returns>
        /// <seealso cref="IsScrolled"/>
        public static Func<Component, bool> ScrollHorizontally = (Component c) => {
            Panel p = (Panel)c;
            int scrollWheelDelta = GuiHelper.ScrollWheelDelta;
            p.Offset = new Point((int)Math.Min(Math.Max(p.Offset.X + scrollWheelDelta, p.ClippingRect.Width - p.Size.Width), 0), p.Offset.Y);

            return true;
        };

        // Group: Public Functions

        /// <summary>
        /// Creates a button with a label that becomes white on hover.
        /// The button can be interacted with using gamepad 0, keyboard and mouse.
        /// Adds a border of size 20 around the label.
        /// </summary>
        /// <param name="t">The string to use for the label.</param>
        /// <param name="operation">The action that the button does when interacted with.</param>
        /// <param name="grabFocus">A way for the component to request focus.</param>
        /// <returns>Returns the button that was created.</returns>
        public static Component CreateButton(string t, Func<Component, bool> operation, Action<Component> grabFocus) {
            Label l = new Label(t);
            l.ActiveColor = Color.White;
            l.NormalColor = new Color(150, 150, 150);

            return CreateButton(l, operation, grabFocus);
        }
        /// <summary>
        /// Creates a button with a dynamic label that becomes white on hover.
        /// The button can be interacted with using gamepad 0, keyboard and mouse.
        /// Adds a border of size 20 around the label.
        /// </summary>
        /// <param name="ld">A function that returns a string.</param>
        /// <param name="operation">The action that the button does when interacted with.</param>
        /// <param name="grabFocus">A way for the component to request focus.</param>
        /// <returns>Returns the button that was created.</returns>
        public static Component CreateButton(Func<string> ld, Func<Component, bool> operation, Action<Component> grabFocus) {
            LabelDynamic l = new LabelDynamic(ld);
            l.ActiveColor = Color.White;
            l.NormalColor = new Color(150, 150, 150);

            return CreateButton(l, operation, grabFocus);
        }
        /// <summary>
        /// Creates a button with a custom component.
        /// The button can be interacted with using gamepad 0, keyboard and mouse.
        /// Adds a border of size 20 around the component.
        /// </summary>
        /// <param name="c">The component to give to the button.</param>
        /// <param name="operation">The action that the button does when interacted with.</param>
        /// <param name="grabFocus">A way for the component to request focus.</param>
        /// <returns></returns>
        public static Component CreateButton(Component c, Func<Component, bool> operation, Action<Component> grabFocus) {
            Border border = new Border(c, 20, 20, 20, 20);
            Button b = new Button(border);
            b.ShowBox = false;
            b.GrabFocus = grabFocus;
            b.AddHoverCondition(ConditionHoverMouse);
            b.AddAction(ConditionInteraction, operation);
            b.AddAction(ConditionGotHovered, ConsumeCondition);

            return b;
        }

        // Group: Private Functions

        private static bool buttonReleased(Keys key) {
            return InputHelper.IsActive && ConditionKeyboard.Released(key);
        }
        private static bool buttonReleased(MouseButton button) {
            return InputHelper.IsActive && ConditionMouse.Released(button);
        }
        private static bool buttonReleased(GamePadButton button, int gamePadIndex) {
            return InputHelper.IsActive && ConditionGamePad.Released(button, gamePadIndex);
        }
    }
}