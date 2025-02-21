using System.Diagnostics;

namespace BulletHell {
    public struct ProfileData {
        public string Name { get; }
        public double Start { get; }
        public double Duration { get; }

        public ProfileData(string name, double start, double duration) {
            Name = name;
            Start = start;
            Duration = duration;
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

        private Stack<(string, double, Stopwatch)> timerStack = new Stack<(string, double, Stopwatch)>();
        public List<ProfileData> Profiles { get; private set; } = new List<ProfileData>();
        private Stopwatch stopwatch = Stopwatch.StartNew();

        private Profiler() {}

        // [Conditional("DEBUG")]
        public void Start(string name) {
            stopwatch.Stop();
            timerStack.Push((name, stopwatch.Elapsed.TotalMicroseconds, Stopwatch.StartNew()));
            stopwatch.Start();
        }

        // [Conditional("DEBUG")]
        public void End() {
            (string name, double start, Stopwatch timer) = timerStack.Pop();
            timer.Stop();
            double duration = timer.Elapsed.TotalMicroseconds;
            Profiles.Add(new ProfileData(name, start, duration));
        }

        // [Conditional("DEBUG")]
        public void Reset() {
            Profiles = new List<ProfileData>();
        }

        // [Conditional("DEBUG")]
        public void WriteJson(string filepath) {
            StreamWriter stream = new StreamWriter(filepath);

            // Header
            stream.Write("{\"otherData\": {},\"traceEvents\": [");

            int profilesLeft = Profiles.Count;
            foreach (ProfileData profile in Profiles) {
                stream.Write("{");
                stream.Write($"\"cat\": \"function\",");
                stream.Write($"\"dur\": {profile.Duration},");
                stream.Write($"\"name\": \"{profile.Name}\",");
                stream.Write($"\"ph\": \"X\",");
                stream.Write($"\"ts\": {profile.Start},");
                stream.Write($"\"pid\": 0");
                stream.Write("}");
                if (profilesLeft > 1) {
                    stream.Write(",");
                }
                profilesLeft--;
            }

            // Footer
            stream.Write("]}");

            stream.Close();
        }
    }
}
