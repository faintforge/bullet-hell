using System.Diagnostics;

namespace BulletHell {
    public class Profile {
        /// <summary>
        /// Name identifier of profile.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Average duration of a single call.
        /// </summary>
        public double AverageDuration { get; private set; }
        /// <summary>
        /// Total time spent executing this profile.
        /// </summary>
        public double TotalDuration { get; private set; }
        /// <summary>
        /// Total times this profile has been called.
        /// </summary>
        public int CallCount { get; private set; }

        internal Profile? Parent { get; }
        private Stopwatch stopwatch = new Stopwatch();
        internal Dictionary<string, Profile> ChildProfiles = new Dictionary<string, Profile>();

        internal Profile(Profile? parent, string name) {
            Parent = parent;
            Name = name;
        }

        internal void Start() {
            CallCount++;
            stopwatch = Stopwatch.StartNew();
        }

        internal void End() {
            stopwatch.Stop();
            TotalDuration += stopwatch.Elapsed.TotalMilliseconds;
            AverageDuration = TotalDuration / CallCount;
        }
    }

    public class Profiler {
        private static Profiler? instance;
        /// <summary>
        /// Singelton instance of this class.
        /// </summary>
        public static Profiler Instance {
            get {
                if (instance == null) {
                    instance = new Profiler();
                }
                return instance;
            }
        }

        /// <summary>
        /// All profiles stored with their name as their key.
        /// </summary>
        public Dictionary<string, Profile> Profiles { get; private set; } = new Dictionary<string, Profile>();
        private Profile? currentProfile = null;

        private Profiler() {}

        /// <summary>
        /// Start scope of a profile.
        /// </summary>
        /// <param name="name">Name of profile.</param>
        [Conditional("DEBUG")]
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

        /// <summary>
        /// End scope of a profile.
        /// </summary>
        [Conditional("DEBUG")]
        public void End() {
            System.Diagnostics.Debug.Assert(currentProfile != null);
            currentProfile.End();
            currentProfile = currentProfile.Parent;
        }

        /// <summary>
        /// Reset the profiler for a new frame of profiling.
        /// </summary>
        [Conditional("DEBUG")]
        public void Reset() {
            Profiles = new Dictionary<string, Profile>();
        }
    }
}
