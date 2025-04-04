namespace BulletHell {
    public abstract class Upgrade
    {
        public string Name { get; protected set; } = "Dummy";
        public string[] Description { get; protected set; } = new string[] {
            "No stats"
        };
        public bool Selected { get; private set; }

        public void DrawHUD(Widget container, int uiID) {
            Widget card = container.MakeWidget($"##card{uiID}-wieuhkjsdbkjcbv")
                .Background(Color.HexRGB(0x151d28))
                .AlignChildren(WidgetAlignment.Top, WidgetAlignment.Center)
                .FixedSize(new Vector2(64.0f * 4.0f, 64.0f * 6));

            if (card.Signal().Hovered) {
                card.Background(Color.HexRGB(0x241527))
                    .FixedSize(new Vector2(64.0f * 4.5f, 64.0f * 6.5f));
                if (Input.Instance.GetButtonOnDown(MouseButton.Left)) {
                    Selected = true;
                }
            }

            card.MakeWidget($"##spacer{uiID}-oiwerwioeurh")
                .FixedHeight(16.0f);
            card.MakeWidget($"{Name}##cardName{uiID}")
                .FitText()
                .ShowText(AssetManager.Instance.GetFont("lato24"), Color.WHITE);
            card.MakeWidget($"##spacer{uiID}-oiwerwioeuuhiwuerhiwrh")
                .FixedHeight(16.0f);

            foreach (string desc in Description) {
                Widget modContainer = card.MakeWidget($"##{Name}{desc}modContainer{uiID}")
                    .FitChildrenHeight()
                    .PercentOfParentWidth(0.8f);

                modContainer.MakeWidget($"{desc}##oasmd{uiID}")
                    .FitText()
                    .ShowText(AssetManager.Instance.GetFont("roboto_mono"), Color.WHITE);
            }
        }

        public virtual void Apply(Player player) {}
    }
}