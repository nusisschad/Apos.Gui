using System.Collections.Generic;
using System.Linq;
using Optional;

namespace Apos.Gui {
    /// <summary>
    /// Goal: A Switch component that works like a tab.
    /// </summary>
    public class Switcher<T> : Component {

        // Group: Constructors

        public Switcher() { }

        // Group: Public Variables

        public Option<T> Key {
            get {
                if (_children.Count > 0) {
                    return _key.Or(() => _children.Keys.First());
                }
                return Option.None<T>();
            }
            set {
                value.MatchSome(key => {
                    if (_children.ContainsKey(key)) {
                        _key = value;
                    }
                });
            }
        }
        public override bool OldIsHovered {
            get => base.OldIsHovered;
            set {
                base.OldIsHovered = value;
                Key.MatchSome(key => {
                    _children[key].OldIsHovered = value;
                });
            }
        }
        public override bool IsHovered {
            get => base.IsHovered;
            set {
                base.IsHovered = value;
                Key.MatchSome(key => {
                    _children[key].IsHovered = value;
                });
            }
        }
        //public override bool IsFocusable => Key.Map(key => _children[key].IsFocusable).ValueOr(false);

        public override bool HasFocus {
            get => base.HasFocus;
            set {
                base.HasFocus = value;
                Key.MatchSome(key => {
                    _children[key].HasFocus = value;
                });
            }
        }
        public override int PrefWidth => Key.Map(key => _children[key].PrefWidth).ValueOr(Width);
        public override int PrefHeight => Key.Map(key => _children[key].PrefHeight).ValueOr(Height);

        // Group: Public Functions

        public void Add(T key, Component c) {
            _children.Add(key, c);
            c.Parent = Option.Some((Component)this);
        }
        public override Component GetPrev(Component c) {
            return Key.Map(key => _children[key]).ValueOr(() => Parent.Map(parent => parent.GetPrev(this)).ValueOr(this));
        }
        public override Component GetNext(Component c) {
            return Key.Map(key => _children[key]).ValueOr(() => Parent.Map(parent => parent.GetNext(this)).ValueOr(this));
        }
        public override Component GetFinal() {
            return Key.Map(key => _children[key]).ValueOr(this);
        }
        public override Component GetFinalInverse() {
            return Key.Map(key => _children[key]).ValueOr(this);
        }
        public override void UpdateSetup() {
            base.UpdateSetup();

            Key.MatchSome(key => {
                Component c = _children[key];
                c.Width = Width;
                c.Height = Height;
                c.Position = Position;
                c.ClippingRect = ClippingRect;

                c.UpdateSetup();
            });
        }
        public override bool UpdateInput() {
            bool isUsed = false;

            Key.MatchSome(key => {
                isUsed = _children[key].UpdateInput();
            });

            if (!isUsed) {
                isUsed = base.UpdateInput();
            }

            return isUsed;
        }
        public override void Update() {
            base.Update();
            Key.MatchSome(key => {
                _children[key].Update();
            });
        }
        public override void Draw() {
            Key.MatchSome(key => {
                _children[key].Draw();
            });
        }

        // Group: Private Variables

        protected Option<T> _key = Option.None<T>();
        protected Dictionary<T, Component> _children = new Dictionary<T, Component>();
    }
}