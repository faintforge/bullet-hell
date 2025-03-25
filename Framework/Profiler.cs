using System.Diagnostics;

namespace BulletHell {
    public class Profile {
        public string Name { get; }
        public double AverageDuration { get; private set; }
        public double TotalDuration { get; private set; }
        public int CallCount { get; private set; }
        public Profile? Parent { get; }
        private Stopwatch stopwatch = new Stopwatch();

        public Dictionary<string, Profile> ChildProfiles = new Dictionary<string, Profile>();

        public Profile(Profile? parent, string name) {
            Parent = parent;
            Name = name;
        }

        public void Start() {
            CallCount++;
            stopwatch = Stopwatch.StartNew();
        }

        public void End() {
            stopwatch.Stop();
            TotalDuration += stopwatch.Elapsed.TotalMilliseconds;
            AverageDuration = TotalDuration / CallCount;
        }
    }

    public class Profiler {
        private static Profiler? instance;
        public static Profiler Instance {
            get {
                if (instance == null) {
                    instance = new Profiler();
                }
                return instance;
            }
        }

        public Dictionary<string, Profile> Profiles { get; private set; } = new Dictionary<string, Profile>();
        private Profile? currentProfile = null;

        private Profiler() {}

        // [Conditional("DEBUG")]
        public void Start(string name) {
            Profile? profile;
            if (currentProfile == null) {
                if (!Profiles.TryGetValue(name, out profile)) {
                    profile = new Profile(currentProfile, name);
                    Profiles.Add(name, profile);
                }
            } else {
                if (!currentProfile.ChildProfiles.TryGetValue(name, out profile)) {
                    profile = new Profile(currentProfile, name);
                    currentProfile.ChildProfiles.Add(name, profile);
                }
            }
            profile.Start();
            currentProfile = profile;
        }

        // [Conditional("DEBUG")]
        public void End() {
            System.Diagnostics.Debug.Assert(currentProfile != null);
            currentProfile.End();
            currentProfile = currentProfile.Parent;
        }

        // [Conditional("DEBUG")]
        public void Reset() {
            Profiles = new Dictionary<string, Profile>();
        }
    }
}
