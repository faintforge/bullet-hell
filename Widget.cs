namespace BulletHell {
    [Flags]
    public enum WidgetFlags {
        None      = 0,
        FloatingX = 1 << 0,
        FloatingY = 1 << 1,
        ShowText  = 1 << 2,
    }

    public enum WidgetSizeType {
        Pixels,
        TextContent,
        SumOfChildren,
    }

    public enum WidgetFlow {
        Horizontal,
        Vertical,
    }

    public enum WidgetAlignment {
        Left,
        Top = Left,
        Center,
        Right,
        Bottom = Right,
    }

    public struct WidgetSize {
        public WidgetSizeType Type { get; set; }
        public float Value { get; set; }
    }

    public class Widget {
        public Widget? Parent { get; private set; }
        private string id;
        public string Text { get; private set; }
        public Vector2 ComputedRelativePosition { get; set; }
        public Vector2 ComputedAbsolutePosition { get; set; }
        public Vector2 ComputedSize { get; set; }
        public WidgetFlags Flags { get; private set; }
        public List<Widget> Children { get; private set; } = new List<Widget>();
        // 0 = X-axis
        // 1 = Y-axis
        public WidgetSize[] Sizes { get; private set; } = new WidgetSize[2];
        public Font? Font { get; private set; }

        public Color Bg { get; private set; } = Color.TRASNPARENT;
        public Color Fg = Color.WHITE;
        public WidgetFlow Flow { get; private set; } = WidgetFlow.Vertical;

        public WidgetAlignment VerticalAlign { get; private set; }
        public WidgetAlignment HorizontalAlign { get; private set; }

        internal Widget(string text, Widget? parent) {
            Parent = parent;
            id = "";
            Text = "";
            HandleText(text);
        }

        private void HandleText(string text) {
            // Find id part
            for (int i = 0; i < text.Length; i++) {
                if (i + 2 == text.Length) {
                    break;
                }

                if (text[i] == '#' &&
                    text[i + 1] == '#' &&
                    text[i + 2] == '#' ) {
                    id = text.Substring(i);
                    break;
                }
            }

            // Find visible text part
            int length = text.Length;
            for (int i = 0; i < text.Length; i++) {
                if (i + 1 == text.Length) {
                    break;
                }

                if (text[i] == '#' &&
                    text[i + 1] == '#') {
                    length = i;
                    break;
                }
            }
            Text = text.Substring(0, length);
        }

        public Widget MakeWidget(string text) {
            Widget child = new Widget(text, this);
            Children.Add(child);
            return child;
        }

        public Widget FloatX(float x) {
            Vector2 newPos = ComputedAbsolutePosition;
            newPos.X = x;
            ComputedAbsolutePosition = newPos;
            Flags |= WidgetFlags.FloatingX;
            return this;
        }

        public Widget FloatY(float y) {
            Vector2 newPos = ComputedAbsolutePosition;
            newPos.Y = y;
            ComputedAbsolutePosition = newPos;
            Flags |= WidgetFlags.FloatingY;
            return this;
        }

        public Widget Floating(Vector2 position) {
            return this.FloatX(position.X).FloatY(position.Y);
        }

        public Widget FixedSize(Vector2 size) {
            Sizes[0] = new WidgetSize() {
                Type = WidgetSizeType.Pixels,
                Value = size.X,
            };
            Sizes[1] = new WidgetSize() {
                Type = WidgetSizeType.Pixels,
                Value = size.Y,
            };
            return this;
        }

        public Widget Background(Color bg) {
            Bg = bg;
            return this;
        }

        public Widget ShowText(Font font, Color color) {
            Font = font;
            Flags |= WidgetFlags.ShowText;
            Fg = color;
            return this;
        }

        public Widget FitTextWidth() {
            Sizes[0] = new WidgetSize() { Type = WidgetSizeType.TextContent};
            return this;
        }

        public Widget FitTextHeight() {
            Sizes[1] = new WidgetSize() { Type = WidgetSizeType.TextContent };
            return this;
        }

        public Widget FitText() {
            return this.FitTextWidth().FitTextHeight();
        }

        public Widget FitChildrenWidth() {
            Sizes[0] = new WidgetSize() { Type = WidgetSizeType.SumOfChildren };
            return this;
        }

        public Widget FitChildrenHeight() {
            Sizes[1] = new WidgetSize() { Type = WidgetSizeType.SumOfChildren };
            return this;
        }

        public Widget FitChildren() {
            return this.FitChildrenWidth().FitChildrenHeight();
        }

        public Widget FlowHorizontal() {
            Flow = WidgetFlow.Horizontal;
            return this;
        }
        public Widget FlowVertical() {
            Flow = WidgetFlow.Vertical;
            return this;
        }

        public Widget AlignChildren(WidgetAlignment vertical, WidgetAlignment horizontal) {
            VerticalAlign = vertical;
            HorizontalAlign = horizontal;
            return this;
        }
    }
}
