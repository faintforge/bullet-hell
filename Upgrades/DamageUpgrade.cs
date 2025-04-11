namespace BulletHell {
    public class DamageUpgrade : Upgrade {
        public DamageUpgrade() {
            Name = "More Ouch";
            Description = new string[] {
                "+25% damage modifier",
            };
        }

        public override void Apply(Player player) {
            player.DamageMod *= 1.25f;
        }
    }
}
