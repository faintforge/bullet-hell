namespace BulletHell {
    public class UI {
        private List<Widget> roots = new List<Widget>();

        public UI() {}

        public Widget MakeWidget(string text) {
            Widget widget = new Widget(text, null);
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

        private void BuildSumOfChildrenSizes(Widget widget) {
            foreach (Widget child in widget.Children) {
                BuildSumOfChildrenSizes(child);
            }

            Vector2 childSize = widget.ComputedSize;
            foreach (Widget child in widget.Children) {
                if (child.Parent == null) {
                    continue;
                }

                switch (child.Parent.Flow) {
                    case WidgetFlow.Horizontal:
                        if (widget.Sizes[0].Type == WidgetSizeType.SumOfChildren) {
                            childSize.X += child.ComputedSize.X;
                        }
                        if (widget.Sizes[1].Type == WidgetSizeType.SumOfChildren) {
                            childSize.Y = MathF.Max(childSize.Y, child.ComputedSize.Y);
                        }
                        break;
                    case WidgetFlow.Vertical:
                        if (widget.Sizes[0].Type == WidgetSizeType.SumOfChildren) {
                            childSize.X = MathF.Max(childSize.X, child.ComputedSize.X);
                        }
                        if (widget.Sizes[1].Type == WidgetSizeType.SumOfChildren) {
                            childSize.Y += child.ComputedSize.Y;
                        }
                        break;
                }
            }
            widget.ComputedSize = childSize;
        }

        private Vector2 BuildPositions(Widget widget, Vector2 relPosition) {
            Vector2 nextPosition = relPosition;
            widget.ComputedRelativePosition = relPosition;

            // X
            if (!widget.Flags.HasFlag(WidgetFlags.FloatingX) && widget.Parent != null) {
                Vector2 pos = widget.ComputedAbsolutePosition;
                pos.X = widget.Parent.ComputedAbsolutePosition.X + relPosition.X;
                widget.ComputedAbsolutePosition = pos;
                if (widget.Parent.Flow == WidgetFlow.Horizontal) {
                    nextPosition.X += widget.ComputedSize.X;
                }
            }

            // Y
            if (!widget.Flags.HasFlag(WidgetFlags.FloatingY) && widget.Parent != null) {
                Vector2 pos = widget.ComputedAbsolutePosition;
                pos.Y = widget.Parent.ComputedAbsolutePosition.Y + relPosition.Y;
                widget.ComputedAbsolutePosition = pos;
                if (widget.Parent.Flow == WidgetFlow.Vertical) {
                    nextPosition.Y += widget.ComputedSize.Y;
                }
            }

            Vector2 siblingPosition = nextPosition;
            nextPosition = new Vector2();
            foreach (Widget child in widget.Children) {
                nextPosition = BuildPositions(child, nextPosition);
            }

            return siblingPosition;
        }

        public void Begin() {
            foreach (Widget root in roots) {
                BuildFixedSizes(root);
                BuildSumOfChildrenSizes(root);
                BuildPositions(root, new Vector2());
            }
        }

        public void End() {
            roots = new List<Widget>();
        }

        private void DrawHelper(Widget widget, Renderer renderer) {
            renderer.Draw(new Box() {
                    Origin = new Vector2(-1.0f),
                    Pos = widget.ComputedAbsolutePosition,
                    Size = widget.ComputedSize,
                }, widget.Bg);

            if (widget.Flags.HasFlag(WidgetFlags.ShowText)) {
                if (widget.Font != null) {
                    renderer.DrawText(widget.Text, widget.Font, widget.ComputedAbsolutePosition, widget.Fg);
                }
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
    }
}
