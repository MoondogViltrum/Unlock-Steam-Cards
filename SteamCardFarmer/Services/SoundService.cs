using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SteamCardFarmer.Services
{
    public static class SoundService
    {
        public static readonly List<(string Name, string Key)> AvailableSounds = new()
        {
            ("🔔 Notification douce", "soft"),
            ("⚡ Double alerte",      "alert"),
            ("🎵 Fanfare",           "fanfare"),
        };

        public static bool Enabled { get; set; } = true;
        public static string CurrentSound { get; set; } = "soft";
        public static int Volume { get; set; } = 100; // 0-100

        public static void Play()
        {
            if (!Enabled) return;
            Task.Run(() =>
            {
                try
                {
                    switch (CurrentSound)
                    {
                        case "soft":    PlayTones(new[] { (880, 150), (1100, 200) }); break;
                        case "alert":   PlayTones(new[] { (1200, 80), (1200, 80), (1500, 200) }, gap: 50); break;
                        case "fanfare": PlayTones(new[] { (523, 100), (659, 100), (784, 100), (1047, 300) }); break;
                        default:        PlayTones(new[] { (880, 150), (1100, 200) }); break;
                    }
                }
                catch { }
            });
        }

        private static void PlayTones((int freq, int ms)[] tones, int gap = 0)
        {
            foreach (var (freq, ms) in tones)
            {
                using var stream = GenerateWav(freq, ms, Volume);
                using var player = new SoundPlayer(stream);
                player.PlaySync();
                if (gap > 0) System.Threading.Thread.Sleep(gap);
            }
        }

        /// <summary>Génère un WAV PCM 16-bit mono avec amplitude proportionnelle au volume.</summary>
        private static MemoryStream GenerateWav(int frequency, int durationMs, int volumePercent)
        {
            const int sampleRate = 44100;
            int samples = sampleRate * durationMs / 1000;
            double amplitude = short.MaxValue * (volumePercent / 100.0);

            var ms = new MemoryStream();
            using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                // WAV header
                int dataSize = samples * 2;
                bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(36 + dataSize);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16); bw.Write((short)1); bw.Write((short)1);
                bw.Write(sampleRate); bw.Write(sampleRate * 2);
                bw.Write((short)2); bw.Write((short)16);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                bw.Write(dataSize);

                // PCM samples — sine wave avec fade out pour éviter le clic
                for (int i = 0; i < samples; i++)
                {
                    double t = (double)i / sampleRate;
                    double fade = i > samples * 0.8 ? (samples - i) / (samples * 0.2) : 1.0;
                    short sample = (short)(amplitude * fade * Math.Sin(2 * Math.PI * frequency * t));
                    bw.Write(sample);
                }
            }

            ms.Position = 0;
            return ms;
        }
    }
}
