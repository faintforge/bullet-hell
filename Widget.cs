namespace BulletHell {
    [Flags]
    public enum WidgetFlags {
        None            = 0,
        FloatingX       = 1 << 0,
        FloatingY       = 1 << 1,
        ShowText        = 1 << 2,
        DrawBackground  = 1 << 3,
    }

    public enum WidgetSizeType {
        Pixels,
        TextContent,
        SumOfChildren,
        PercentOfParent,
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

    public enum WidgetTextAlignment {
        Left,
        Center,
        Right,
    }

    public struct WidgetSize {
        public WidgetSizeType Type { get; set; }
        public float Value { get; set; }
    }

    public struct WidgetSignal {
        public bool Hovered { get; set; }
    }

    public class Widget {
        internal Widget? Parent { get; private set; }
        internal string Id { get; private set; }
        internal string Text { get; private set; }
        internal Vector2 ComputedRelativePosition { get; set; }
        internal Vector2 ComputedAbsolutePosition { get; set; }
        internal Vector2 ComputedSize { get; set; }
        internal Box ComputedBox { get; set; }
        internal WidgetFlags Flags { get; private set; }
        internal List<Widget> Children { get; private set; } = new List<Widget>();
        // 0 = X-axis
        // 1 = Y-axis
        internal WidgetSize[] Sizes { get; private set; } = new WidgetSize[2];
        internal Font? Font { get; private set; }

        internal Color Bg { get; private set; } = Color.TRASNPARENT;
        internal Color Fg = Color.WHITE;
        internal WidgetFlow Flow { get; private set; } = WidgetFlow.Vertical;

        internal WidgetAlignment VerticalAlign { get; private set; }
        internal WidgetAlignment HorizontalAlign { get; private set; }
        internal WidgetTextAlignment TextAlign { get; private set; }
        internal int lastTouchFrame { get; set; } = 0;
        private UI ui;
        internal Action<Widget, Renderer>? RenderExt { get; private set; } = null;

        internal Widget(string text, Widget? parent, UI ui) {
            Parent = parent;
            Id = "";
            Text = "";
            this.ui = ui;
            HandleText(text);
        }

        internal void Reset(string displayText, Widget? parent) {
            Flags = 0;
            Children = new List<Widget>();
            Sizes = new WidgetSize[2];
            Bg = Color.TRASNPARENT;
            Fg = Color.WHITE;
            Flow = WidgetFlow.Vertical;
            VerticalAlign = WidgetAlignment.Top;
            HorizontalAlign = WidgetAlignment.Left;
            Parent = parent;
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
                    Id = text.Substring(i);
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
            Widget child = ui.MakeWidget(text, this);
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

        public Widget FixedWidth(float width) {
            Sizes[0] = new WidgetSize() {
                Type = WidgetSizeType.Pixels,
                 Value = width,
            };
            return this;
        }

        public Widget FixedHeight(float height) {
            Sizes[1] = new WidgetSize() {
                Type = WidgetSizeType.Pixels,
                 Value = height,
            };
            return this;
        }

        public Widget FixedSize(Vector2 size) {
            return this.FixedWidth(size.X).FixedHeight(size.Y);
        }

        public Widget Background(Color bg) {
            Bg = bg;
            Flags |= WidgetFlags.DrawBackground;
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

        public WidgetSignal Signal() {
            return ui.Signal(this);
        }

        public Widget AlignText(WidgetTextAlignment alignment) {
            TextAlign = alignment;
            return this;
        }

        public Widget RenderingExtension(Action<Widget, Renderer> ext) {
            RenderExt = ext;
            return this;
        }

        public Widget PercentOfParentWidth(float percent) {
            Sizes[0] = new WidgetSize() {
                Type = WidgetSizeType.PercentOfParent,
                Value = percent,
            };
            return this;
        }

        public Widget PercentOfParentHeight(float percent) {
            Sizes[0] = new WidgetSize() {
                Type = WidgetSizeType.PercentOfParent,
                Value = percent,
            };
            return this;
        }

        public Widget PercentOfParentSize(Vector2 percentages) {
            return this.PercentOfParentWidth(percentages.X)
                .PercentOfParentHeight(percentages.Y);
        }
    }
}
