namespace BulletHell {
    public class TestUpgrade : Upgrade {
        public TestUpgrade() {
            Name = "Test Upgrade";
            Description = new string[] {
                "+1 something",
                "+20% health",
                "-1 something else"
            };
        }

        public override void Apply(Player player) {
            Console.WriteLine("Apply test upgrade");
        }
    }
}