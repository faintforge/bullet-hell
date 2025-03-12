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

        private Vector2 BuildPositions(Widget widget, Vector2 position) {
            Vector2 nextPosition = position;
            if (!widget.Flags.HasFlag(WidgetFlags.Floating) && widget.Parent != null) {
                widget.ComputedPosition = position;
                nextPosition.Y += widget.ComputedSize.Y;
            }

            foreach (Widget child in widget.Children) {
                nextPosition = BuildPositions(child, nextPosition);
            }

            return nextPosition;
        }

        public void Begin() {
            foreach (Widget root in roots) {
                BuildFixedSizes(root);
                BuildPositions(root, root.ComputedPosition);
            }
        }

        public void End() {
            roots = new List<Widget>();
        }

        private void DrawHelper(Widget widget, Renderer renderer) {
            renderer.Draw(new Box() {
                    Origin = new Vector2(-1.0f),
                    Pos = widget.ComputedPosition,
                    Size = widget.ComputedSize,
                }, widget.Bg);

            if (widget.Flags.HasFlag(WidgetFlags.ShowText)) {
                if (widget.Font != null) {
                    renderer.DrawText(widget.Text, widget.Font, widget.ComputedPosition, widget.Fg);
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
