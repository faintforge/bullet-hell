namespace BulletHell {
    public class UI {
        private List<Widget> roots = new List<Widget>();
        private Dictionary<string, Widget> widgetTable = new Dictionary<string, Widget>();
        private int currentFrame = 0;
        private Vector2 mousePosition = new Vector2();

        public UI() {}

        private (string, string) HandleText(string text) {
            string id = text;
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

            string display = text.Substring(0, length);
            return (id, display);
        }

        internal Widget MakeWidget(string text, Widget? parent) {
            (string id, string display) = HandleText(text);
            Widget? widget;
            if (!widgetTable.TryGetValue(id, out widget)) {
                widget = new Widget(text, null, this);
                widgetTable.Add(id, widget);
            }

            // Widget has already been used this frame.
            if (widget.lastTouchFrame == currentFrame) {
                throw new Exception($"Duplicate widget ID:s ({id}) are not allowed!");
            }

            widget.lastTouchFrame = currentFrame;
            widget.Reset(display, parent);

            return widget;
        }

        public Widget MakeWidget(string text) {
            Widget widget = MakeWidget(text, null);
            roots.Add(widget);
            return widget;
        }

        private void BuildFixedSizes(Widget widget) {
            // X-axis
            {
                Vector2 newSize = widget.ComputedSize;
                switch (widget.Sizes[0].Type) {
                    case WidgetSizeType.Pixels:
                        newSize.X = widget.Sizes[0].Value;
                        break;
                    case WidgetSizeType.TextContent:
                        if (widget.Font == null) {
                            break;
                        }
                        newSize.X = widget.Font.MeasureText(widget.Text).X;
                        break;
                }
                widget.ComputedSize = newSize;
            }

            // Y-axis
            {
                Vector2 newSize = widget.ComputedSize;
                switch (widget.Sizes[1].Type) {
                    case WidgetSizeType.Pixels:
                        newSize.Y = widget.Sizes[1].Value;
                        break;
                    case WidgetSizeType.TextContent:
                        if (widget.Font == null) {
                            break;
                        }
                        newSize.Y = widget.Font.MeasureText(widget.Text).Y;
                        break;
                }
                widget.ComputedSize = newSize;
            }

            foreach (Widget child in widget.Children) {
                BuildFixedSizes(child);
            }
        }

        private Vector2 SumSizeOfChildren(Widget widget) {
            Vector2 childSize = new Vector2();
            foreach (Widget child in widget.Children) {
                if (child.Parent == null) {
                    continue;
                }

                switch (child.Parent.Flow) {
                    case WidgetFlow.Horizontal:
                        if (!child.Flags.HasFlag(WidgetFlags.FloatingX)) {
                            childSize.X += child.ComputedSize.X;
                        }
                        if (!child.Flags.HasFlag(WidgetFlags.FloatingY)) {
                            childSize.Y = MathF.Max(childSize.Y, child.ComputedSize.Y);
                        }
                        break;
                    case WidgetFlow.Vertical:
                        if (!child.Flags.HasFlag(WidgetFlags.FloatingX)) {
                            childSize.X = MathF.Max(childSize.X, child.ComputedSize.X);
                        }
                        if (!child.Flags.HasFlag(WidgetFlags.FloatingY)) {
                            childSize.Y += child.ComputedSize.Y;
                        }
                        break;
                }
            }
            return childSize;
        }

        private void BuildSumOfChildrenSizes(Widget widget) {
            foreach (Widget child in widget.Children) {
                BuildSumOfChildrenSizes(child);
            }
            Vector2 sum = SumSizeOfChildren(widget);
            if (widget.Sizes[0].Type == WidgetSizeType.SumOfChildren) {
                Vector2 newSize = widget.ComputedSize;
                newSize.X = sum.X;
                widget.ComputedSize = newSize;
            }
            if (widget.Sizes[1].Type == WidgetSizeType.SumOfChildren) {
                Vector2 newSize = widget.ComputedSize;
                newSize.Y = sum.Y;
                widget.ComputedSize = newSize;
            }
        }

        private void BuildPercentOfParentSizes(Widget widget) {
            if (widget.Parent != null) {
                if (widget.Sizes[0].Type == WidgetSizeType.PercentOfParent) {
                    Vector2 newSize = widget.ComputedSize;
                    newSize.X = widget.Parent.ComputedSize.X * widget.Sizes[0].Value;;
                    widget.ComputedSize = newSize;
                }
                if (widget.Sizes[1].Type == WidgetSizeType.PercentOfParent) {
                    Vector2 newSize = widget.ComputedSize;
                    newSize.Y = widget.Parent.ComputedSize.Y * widget.Sizes[1].Value;
                    widget.ComputedSize = newSize;
                }
            }

            foreach (Widget child in widget.Children) {
                BuildPercentOfParentSizes(child);
            }
        }

        private Vector2 BuildPositions(Widget widget, Vector2 relPosition) {
            Vector2 nextPosition = relPosition;
            widget.ComputedRelativePosition = relPosition;

            // X
            if (!widget.Flags.HasFlag(WidgetFlags.FloatingX) && widget.Parent != null) {
                Vector2 pos = widget.ComputedAbsolutePosition;
                pos.X = widget.Parent.ComputedAbsolutePosition.X + relPosition.X;
                if (widget.Parent.Flow == WidgetFlow.Horizontal) {
                    nextPosition.X += widget.ComputedSize.X;
                } else {
                    switch (widget.Parent.HorizontalAlign) {
                        case WidgetAlignment.Left:
                            break;
                        case WidgetAlignment.Center:
                            pos.X -= widget.ComputedSize.X / 2.0f;
                            break;
                        case WidgetAlignment.Right:
                            pos.X -= widget.ComputedSize.X;
                            break;
                    }
                }
                widget.ComputedAbsolutePosition = pos;
            }

            // Y
            if (!widget.Flags.HasFlag(WidgetFlags.FloatingY) && widget.Parent != null) {
                Vector2 pos = widget.ComputedAbsolutePosition;
                pos.Y = widget.Parent.ComputedAbsolutePosition.Y + relPosition.Y;
                if (widget.Parent.Flow == WidgetFlow.Vertical) {
                    nextPosition.Y += widget.ComputedSize.Y;
                } else {
                    switch (widget.Parent.VerticalAlign) {
                        case WidgetAlignment.Top:
                            break;
                        case WidgetAlignment.Center:
                            pos.Y -= widget.ComputedSize.Y / 2.0f;
                            break;
                        case WidgetAlignment.Bottom:
                            pos.Y -= widget.ComputedSize.Y;
                            break;
                    }
                }
                widget.ComputedAbsolutePosition = pos;
            }

            Vector2 siblingPosition = nextPosition;

            Vector2 childSum = SumSizeOfChildren(widget);
            switch (widget.HorizontalAlign) {
                case WidgetAlignment.Left:
                    nextPosition.X = 0.0f;
                    break;
                case WidgetAlignment.Center:
                    if(widget.Flow == WidgetFlow.Vertical) {
                        nextPosition.X = widget.ComputedSize.X / 2.0f;
                    } else {
                        nextPosition.X = (widget.ComputedSize.X - childSum.X) / 2.0f;
                    }
                    break;
                case WidgetAlignment.Right:
                    if(widget.Flow == WidgetFlow.Vertical) {
                        nextPosition.X = widget.ComputedSize.X;
                    } else {
                        nextPosition.X = widget.ComputedSize.X - childSum.X;
                    }
                    break;
            }
            switch (widget.VerticalAlign) {
                case WidgetAlignment.Top:
                    nextPosition.Y = 0.0f;
                    break;
                case WidgetAlignment.Center:
                    if (widget.Flow == WidgetFlow.Horizontal) {
                        nextPosition.Y = widget.ComputedSize.Y / 2.0f;
                    } else {
                        nextPosition.Y = (widget.ComputedSize.Y - childSum.Y) / 2.0f;
                    }
                    break;
                case WidgetAlignment.Bottom:
                    if (widget.Flow == WidgetFlow.Horizontal) {
                        nextPosition.Y = widget.ComputedSize.Y;
                    } else {
                        nextPosition.Y = widget.ComputedSize.Y - childSum.Y;
                    }
                    break;
            }
            foreach (Widget child in widget.Children) {
                nextPosition = BuildPositions(child, nextPosition);
            }

            return siblingPosition;
        }

        private void BuildBoxes(Widget widget) {
            widget.ComputedBox = new Box() {
                Origin = new Vector2(-1.0f),
                Pos = widget.ComputedAbsolutePosition,
                Size = widget.ComputedSize,
            };

            foreach (Widget child in widget.Children) {
                BuildBoxes(child);
            }
        }

        public void Begin(Vector2 mousePosition) {
            roots = new List<Widget>();
            currentFrame++;
            this.mousePosition = mousePosition;
        }

        public void End() {
            foreach (Widget root in roots) {
                BuildFixedSizes(root);
                BuildSumOfChildrenSizes(root);
                BuildPercentOfParentSizes(root);
                BuildPositions(root, new Vector2());
                BuildBoxes(root);
            }
        }

        private void DrawHelper(Widget widget, Renderer renderer) {
            if (widget.Flags.HasFlag(WidgetFlags.DrawBackground)) {
                renderer.Draw(widget.ComputedBox, widget.Bg);
            }

            if (widget.Flags.HasFlag(WidgetFlags.ShowText)) {
                if (widget.Font != null) {
                    Vector2 textSize = widget.Font.MeasureText(widget.Text);
                    Vector2 pos = widget.ComputedAbsolutePosition;
                    // Center vertically.
                    pos.Y += widget.ComputedSize.Y / 2.0f;
                    pos.Y -= textSize.Y / 2.0f;
                    switch (widget.TextAlign) {
                        case WidgetTextAlignment.Left:
                            break;
                        case WidgetTextAlignment.Center:
                            pos.X += widget.ComputedSize.X / 2.0f;
                            pos.X -= textSize.X / 2.0f;
                            break;
                        case WidgetTextAlignment.Right:
                            pos.X += widget.ComputedSize.X;
                            pos.X -= textSize.X;
                            break;
                    }
                    renderer.DrawText(widget.Text, widget.Font, pos, widget.Fg);
                }
            }

            if (widget.RenderExt != null) {
                widget.RenderExt(widget, renderer);
            }

            foreach (Widget child in widget.Children) {
                DrawHelper(child, renderer);
            }
        }

        public void Draw(Renderer renderer, Vector2 windowSize) {
            Camera uiCam = new Camera(windowSize, windowSize / 2.0f, windowSize.Y, true);
            renderer.BeginFrame(uiCam);
            foreach (Widget root in roots) {
                DrawHelper(root, renderer);
            }
            renderer.EndFrame();
        }

        internal WidgetSignal Signal(Widget widget) {
            return new WidgetSignal() {
                Hovered = new Box() {
                    Origin = new Vector2(-1.0f),
                    Pos = widget.ComputedAbsolutePosition,
                    Size = widget.ComputedSize,
                }.IntersectsPoint(mousePosition)
            };
        }
    }
}
