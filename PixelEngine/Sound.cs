using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WaveFormatEx = PixelEngine.Windows.WaveFormatEx;
using WaveHdr = PixelEngine.Windows.WaveHdr;
using WaveDelegate = PixelEngine.Windows.WaveDelegate;

namespace PixelEngine {
	/// <summary> Class holding a sound sample to play. Holds logic to load the sound from disk. </summary>
	public class Sound {
		
		/// <summary> Should this sound loop? </summary>
		public bool Loop { get; set; }

		/// <summary> Header information </summary>
		internal WaveFormatEx WavHeader;
		/// <summary> Sound waveform sample data </summary>
		internal short[] Samples = null;
		/// <summary> Number of samples </summary>
		internal long SampleCount = 0;
		/// <summary> Number of channels </summary>
		internal int Channels = 0;
		/// <summary> True if successfully loaded, false otherwise. </summary>
		internal bool Valid = false;
		
		/// <summary> Read a sound from a file. Currently only supports .wav and .mp3 files. </summary>
		/// <param name="file"> File to load. </param>
		internal Sound(string file) {
			using (Stream stream = File.OpenRead(file)) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					if (file.ToLower().EndsWith(".wav") && LoadFromWav(reader)) { Valid = true; }
					if (file.ToLower().EndsWith(".mp3") && LoadFromMp3(reader, file)) { Valid = true; }
				}
			}
		}

		/// <summary> Loads a wave file </summary>
		/// <param name="reader"> Stream reader. </param>
		/// <param name="isFromMp3"> Flag if converted from mp3 </param>
		/// <returns> True if success, false if failed </returns>
		private bool LoadFromWav(BinaryReader reader, bool isFromMp3 = false) {
			const string Riff = "RIFF";
			const string Wave = "WAVE";
			const string Fmt = "fmt ";

			char[] dump;

			dump = reader.ReadChars(4); // RIFF"
			if (string.Compare(string.Concat(dump), Riff) != 0) { return false; }

			dump = reader.ReadChars(4); // Ignore

			dump = reader.ReadChars(4); // "WAVE"
			if (string.Compare(string.Concat(dump), Wave) != 0) { return false; }

			dump = reader.ReadChars(4); // "fmt "
			if (string.Compare(string.Concat(dump), Fmt) != 0) { return false; }

			dump = reader.ReadChars(4); // Ignore

			WavHeader = new WaveFormatEx() {
				FormatTag = reader.ReadInt16(),
				Channels = reader.ReadInt16(),
				SamplesPerSec = reader.ReadInt32(),
				AvgBytesPerSec = reader.ReadInt32(),
				BlockAlign = reader.ReadInt16(),
				BitsPerSample = reader.ReadInt16()
			};
			WavHeader.Size = (short)Marshal.SizeOf(WavHeader);

			if (WavHeader.SamplesPerSec != 44100) { return false; }

			const string Data = "data";

			// Offset 2 characters to reach data.
			// There are 2 extra chars while converting from mp3 for some reason ???
			if (isFromMp3) { reader.ReadChars(2); }


			dump = reader.ReadChars(4); // Chunk header
			long chunkSize = reader.ReadUInt32();
			while (string.Compare(string.Concat(dump), Data) != 0) {
				reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
				dump = reader.ReadChars(4);
				chunkSize = reader.ReadUInt32();
			}

			SampleCount = chunkSize / (WavHeader.Channels * (WavHeader.BitsPerSample >> 3));
			Channels = WavHeader.Channels;

			Samples = new short[SampleCount * Channels];

			const float Mult8Bit = (float)short.MaxValue / byte.MaxValue;
			const float Mult24Bit = (float)short.MaxValue / (1 << 24);
			const float Mult32Bit = (float)short.MaxValue / int.MaxValue;

			for (long l = 0; l < Samples.Length; l++) {
				// Divide by 8 to convert to bits to bytes 
				switch (WavHeader.BitsPerSample / 8) {
					case 1: // 8-bits
						byte b = reader.ReadByte();
						Samples[l] = (short)(b * Mult8Bit);
						break;

					case 2: // 16-bits
						short s = reader.ReadInt16();
						Samples[l] = s;
						break;

					case 3: // 24-bits
						byte[] bs = reader.ReadBytes(3);
						int n = bs[0] | (bs[1] << 8) | (bs[2] << 16);
						Samples[l] = (short)(n * Mult24Bit);
						break;

					case 4: // 32-bits
						int i = reader.ReadInt32();
						Samples[l] = (byte)(i * Mult32Bit);
						break;
				}
			}

			return true;
		}
		/// <summary> Loads a sound from an MP3 file. </summary>
		/// <param name="reader"> Stream reader </param>
		/// <param name="file"> File to read </param>
		/// <returns> true if successful, false if failed. </returns>
		private bool LoadFromMp3(BinaryReader reader, string file) {
			string fileName = Path.GetFileNameWithoutExtension(file);
			string newFile = Path.Combine(Windows.TempPath, $"{fileName}.wav");

			Windows.ConvertToMp3(file, newFile);

			using (Stream str = File.OpenRead(newFile)) {
				using (BinaryReader br = new BinaryReader(str)) {
					return LoadFromWav(br, true);
				}
			}
		}
	}

	/// <summary> Struct to track progress of playing a sound </summary>
	internal struct PlayingSample {
		/// <summary> Sample being played </summary>
		public Sound AudioSample { get; set; }
		/// <summary> Current position in samples </summary>
		public long SamplePosition { get; set; }
		/// <summary> Did it finish by itself? </summary>
		public bool Finished { get; set; }
		/// <summary> Should it loop? </summary>
		public bool Loop { get; set; }
	}

	/// <summary> Class holding actual audio engine logic </summary>
	internal class AudioEngine {
		/// <summary> Function to process sounds. </summary>
		public Func<int, float, float, float> OnSoundCreate { get; set; }
		/// <summary> Function to filter sounds. </summary>
		public Func<int, float, float, float> OnSoundFilter { get; set; }

		/// <summary> Is the sound engine currently active? </summary>
		public bool Active { get; private set; }

		/// <summary> Current global sound playback time </summary>
		public float GlobalTime { get; private set; }

		/// <summary> Current sound system volume </summary>
		internal float Volume = 1;

		/// <summary> All sound samples loaded through the system. Keys are file names, values are sounds loaded from file.  </summary>
		private Dictionary<string, Sound> samples;
		/// <summary> Samples that are currently playing. </summary>
		private List<PlayingSample> playingSamples;


		/// <summary> Callback for Windows when sound system updates.  </summary>
		private static WaveDelegate waveProc;

		/// <summary> Sample rate of sound system </summary>
		private uint sampleRate;
		/// <summary> Channels in sound system </summary>
		private uint channels;
		/// <summary> Number of blocks in buffer </summary>
		private uint blockCount;
		/// <summary> Number samples per block </summary>
		private uint blockSamples;
		/// <summary> Current block index </summary>
		private uint blockCurrent;

		/// <summary> Blocks of samples </summary>
		private short[] blockMemory = null;
		/// <summary> Headers of sounds </summary>
		private WaveHdr[] waveHeaders = null;
		/// <summary> Native pointer to sound device </summary>
		private IntPtr device = IntPtr.Zero;
		/// <summary> Thread handling audio updates </summary>
		private Thread audioThread;
		/// <summary> Free blocks </summary>
		private uint blockFree = 0;

		/// <summary> delay between sound thread updates. </summary>
		private const int SoundInterval = 10;

		/// <summary> Loads a sound from a file. </summary>
		/// <param name="file"> Filename to load </param>
		/// <returns> <see cref="Sound"/> object representing sound if load is successful, or null if it fails. </returns>
		public Sound LoadSound(string file) {
			if (samples == null) { samples = new Dictionary<string, Sound>(); }
			if (samples.ContainsKey(file)) { return samples[file]; }

			FileInfo fi = new FileInfo(file);
			Sound s = new Sound(fi.FullName);
			if (s.Valid) {
				samples[file] = s;
				return s;
			} 
			
			samples[file] = null;
			return null;
		}

		/// <summary> Play a given sound object. </summary>
		/// <param name="s"> Sound to play </param>
		public void PlaySound(Sound s) {
			if (s == null) { return; }
			if (playingSamples == null) { playingSamples = new List<PlayingSample>(); }

			PlayingSample ps = new PlayingSample {
				AudioSample = s,
				SamplePosition = 0,
				Finished = false,
				Loop = s.Loop
			};

			playingSamples.Add(ps);
		}

		/// <summary> Stop the first instance of the given sound. </summary>
		/// <param name="s"> Sound to stop </param>
		public void StopSound(Sound s) {
			if (s == null) { return; }
			
			bool Match(PlayingSample p) { return !p.Finished && p.AudioSample == s; }

			if (playingSamples != null && playingSamples.Exists(Match)) {
				int index = playingSamples.FindIndex(Match);
				PlayingSample ps = playingSamples[index];
				ps.Finished = true;
				playingSamples[index] = ps;
			}
		}

		/// <summary> Initializes the audio system with given settings </summary>
		/// <param name="sampleRate"> Sample rate (default is 44.1kHz) </param>
		/// <param name="channels"> Number of channels (default is 1) </param>
		/// <param name="blocks"> Number of blocks in buffer (default is 8) </param>
		/// <param name="blockSamples"> Number of samples per block (default is 512) </param>
		public void CreateAudio(uint sampleRate = 44100, uint channels = 1, uint blocks = 8, uint blockSamples = 512) {
			Active = false;
			this.sampleRate = sampleRate;
			this.channels = channels;
			blockCount = blocks;
			this.blockSamples = blockSamples;
			blockFree = blockCount;
			blockCurrent = 0;
			blockMemory = null;
			waveHeaders = null;

			WaveFormatEx waveFormat = new WaveFormatEx {
				FormatTag = Windows.WaveFormatPcm,
				SamplesPerSec = (int)sampleRate,
				BitsPerSample = sizeof(short) * 8,
				Channels = (short)channels,
			};
			waveFormat.BlockAlign = (short)((waveFormat.BitsPerSample / 8) * waveFormat.Channels);
			waveFormat.AvgBytesPerSec = waveFormat.SamplesPerSec * waveFormat.BlockAlign;
			waveFormat.Size = (short)Marshal.SizeOf(waveFormat);

			waveProc = WaveOutProc;

			if (Windows.WaveOutOpen(out device, Windows.WaveMapper, waveFormat, waveProc, 0, Windows.CallbackFunction) != 0) {
				DestroyAudio();
			}
			
			blockMemory = new short[blockCount * blockSamples];
			waveHeaders = new WaveHdr[blockCount];

			unsafe {
				fixed (short* mem = blockMemory) {
					for (uint n = 0; n < blockCount; n++) {
						waveHeaders[n].BufferLength = (int)(blockSamples * sizeof(short));
						waveHeaders[n].Data = (IntPtr)(mem + (n * blockSamples));
					}
				}
			}

			Active = true;
			audioThread = new Thread(AudioThread);
			audioThread.Start();
		}

		/// <summary> Stops audio system, and makes audio thread exit sometime in future. </summary>
		public void DestroyAudio() { Active = false; }

		/// <summary> Hook for <see cref="Windows.WaveDelegate"/> to find when sounds are finished. </summary>
		private void WaveOutProc(IntPtr hWaveOut, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2) {
			if (uMsg != Windows.WomDone) { return; }

			blockFree++;
		}
		/// <summary> Sound-mixer function </summary>
		/// <param name="channel"> Channel to mix </param>
		/// <param name="globalTime"> Global timestamp </param>
		/// <param name="timeStep"> Time between updates </param>
		/// <returns> Mixed audio output </returns>
		private float GetMixerOutput(int channel, float globalTime, float timeStep) {
			const float MaxValue = 1f / short.MaxValue;

			float mixerSample = 0.0f;

			if (playingSamples != null) {
				for (int i = 0; i < playingSamples.Count; i++) {
					PlayingSample ps = playingSamples[i];

					float increment = ps.AudioSample.WavHeader.SamplesPerSec * timeStep;
					ps.SamplePosition += (long)Math.Ceiling(increment);

					if (ps.SamplePosition < ps.AudioSample.SampleCount) {
						mixerSample += ps.AudioSample.Samples[(ps.SamplePosition * ps.AudioSample.Channels) + channel] * MaxValue;
					} else {
						if (ps.Loop) { ps.SamplePosition = 0; }
						else { ps.Finished = true; }
					}

					playingSamples[i] = ps;
					playingSamples.RemoveAll(s => s.Finished);
				}
			}

			mixerSample += OnSoundCreate(channel, globalTime, timeStep);
			mixerSample = OnSoundFilter(channel, globalTime, mixerSample);
			mixerSample *= Volume;

			return mixerSample;
		}

		/// <summary> Audio thread loop. Pipes waveform to windows for playback. </summary>
		private void AudioThread() {
			float Clip(float sample, float max) {
				if (sample >= 0) { return Math.Min(sample, max); } 
				else { return Math.Max(sample, -max); }
			}

			int whdrSize = Marshal.SizeOf(waveHeaders[blockCurrent]);

			GlobalTime = 0.0f;
			float timeStep = 1.0f / sampleRate;

			float maxSample = (float)(Math.Pow(2, (sizeof(short) * 8) - 1) - 1);

			while (Active) {
				if (blockFree == 0) {
					while (blockFree == 0) {
						Thread.Sleep(SoundInterval);
					}
				}
				blockFree--;

				short newSample = 0;
				int currentBlock = (int)(blockCurrent * blockSamples);

				if ((waveHeaders[blockCurrent].Flags & Windows.WHdrPrepared) != 0) {
					Windows.WaveOutUnprepareHeader(device, ref waveHeaders[blockCurrent], whdrSize);
				}


				for (uint n = 0; n < blockSamples; n += channels) {
					for (int c = 0; c < channels; c++) {
						newSample = (short)(Clip(GetMixerOutput(c, GlobalTime, timeStep), 1.0f) * maxSample);
						blockMemory[currentBlock + n + c] = newSample;
					}

					GlobalTime += timeStep;
				}

				Windows.WaveOutPrepareHeader(device, ref waveHeaders[blockCurrent], whdrSize);
				Windows.WaveOutWrite(device, ref waveHeaders[blockCurrent], whdrSize);

				blockCurrent++;
				blockCurrent %= blockCount;
			}
		}
	}
}
