namespace BulletHell {
    public class PenetrationUpgrade : Upgrade {
        public PenetrationUpgrade() {
            Name = "Penetration";
            Description = new string[] {
                "+1 pierce",
            };
        }

        public override void Apply(Player player) {
            player.Pierce += 1;
        }
    }
}
