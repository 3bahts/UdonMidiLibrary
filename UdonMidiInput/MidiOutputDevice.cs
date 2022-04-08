using System;
using System.Runtime.InteropServices;

namespace UdonMidiOutput
{
    /// <summary>
    /// MIDI出力デバイス操作クラス。
    /// </summary>
    public class MidiOutputDeviece : IDisposable
    {
        /// <summary>
        /// MIDI出力デバイスの名前一覧を取得する。
        /// </summary>
        /// <returns>MIDI出力デバイスの名前一覧。</returns>
        public static string[] GetDeviceName()
        {
            int count = NativeMethods.GetDeviceNum();
            string[] names = new string[count];

            for (int id = 0; id < count; id++) {
                names[id] = GetDeviceName(id);
            }

            return names;
        }

        /// <summary>
        /// デフォルトコンストラクタ。
        /// </summary>
        public MidiOutputDeviece()
        {
            this.handle = NativeMethods.Open(GetDeviceName(0));
        }

        /// <summary>
        /// デバイス名指定コンストラクタ。
        /// </summary>
        /// <param name="deviceName">MIDI出力デバイスの名前。</param>
        public MidiOutputDeviece(string deviceName)
        {
            this.handle = NativeMethods.Open(deviceName);
        }

        /// <summary>
        /// 接続が有効かどうか。
        /// </summary>
        public bool IsEnable
        {
            get { return this.handle != IntPtr.Zero; }
        }

        /// <summary>
        /// NoteOnメッセージを送信する。
        /// </summary>
        /// <param name="channel">チャネル番号。0～15。</param>
        /// <param name="number">ノート番号。0～127。</param>
        /// <param name="velocity">ベロシティ。0～127。</param>
        /// <returns>true:成功。 / false:失敗。</returns>
        public bool NoteOn(int channel, int number, int velocity)
        {
            return SendMessage(MessageStatus.NoteOn, channel, number, velocity);
        }

        /// <summary>
        /// NoteOffメッセージを送信する。
        /// </summary>
        /// <param name="channel">チャネル番号。0～15。</param>
        /// <param name="number">ノート番号。0～127。</param>
        /// <param name="velocity">ベロシティ。0～127。</param>
        /// <returns>true:成功。 / false:失敗。</returns>
        public bool NoteOff(int channel, int number, int velocity)
        {
            return SendMessage(MessageStatus.NoteOff, channel, number, velocity);
        }

        /// <summary>
        /// ControlChangeメッセージを送信する。
        /// </summary>
        /// <param name="channel">チャネル番号。0～15。</param>
        /// <param name="number">ノート番号。0～127。</param>
        /// <param name="value">データ値。0～127。</param>
        /// <returns>true:成功。 / false:失敗。</returns>
        public bool ControlChange(int channel, int number, int value)
        {
            return SendMessage(MessageStatus.ControlChange, channel, number, value);
        }

        /// <summary>
        /// MIDI出力デバイスのハンドル。
        /// </summary>
        private readonly IntPtr handle;

        /// <summary>
        /// デバイス名のバッファサイズ。MAXPNAMELEN。
        /// </summary>
        private const int NameLength = 32;

        /// <summary>
        /// MIDI出力デバイスの名前を取得する。
        /// </summary>
        /// <param name="id">デバイスのID。0～。</param>
        /// <returns>MIDI出力デバイスの名前。</returns>
        private static string GetDeviceName(int id)
        {
            IntPtr buf = Marshal.AllocHGlobal(NameLength);
            int length = NativeMethods.GetDeviceName(id, buf, NameLength);
            string name = (length > 0) ? Marshal.PtrToStringAnsi(buf) : String.Empty;
            Marshal.FreeHGlobal(buf);
            return name;
        }

        /// <summary>
        /// MIDIメッセージのサイズ。単位バイト。
        /// </summary>
        private const int MessageLength = 3;

        /// <summary>
        /// MIDIメッセージのステータスバイト。
        /// </summary>
        private enum MessageStatus : byte
        {
            NoteOff = 0x80,
            NoteOn = 0x90,
            ControlChange = 0xB0,
        }

        /// <summary>
        /// MIDIメッセージを送信する。
        /// </summary>
        /// <param name="status">ステータス。</param>
        /// <param name="channel">チャネル番号。0～15。</param>
        /// <param name="number">ノート番号。0～127。</param>
        /// <param name="velocity">ベロシティ。0～127。</param>
        /// <returns>true:成功。 / false:失敗。</returns>
        private bool SendMessage(MessageStatus status, int channel, int number, int velocity)
        {
            if (this.handle == IntPtr.Zero) return false;
            IntPtr message = MakeMessage(status, channel, number, velocity);
            if (message == IntPtr.Zero) return false;
            int length = NativeMethods.PutMIDIMessage(this.handle, message, MessageLength);
            Marshal.FreeHGlobal(message);
            return (length == MessageLength);
        }

        /// <summary>
        /// 指定可能なチャネル番号の最大値。
        /// </summary>
        public const int ChannelMax = 15;
        /// <summary>
        /// 指定可能なノート番号の最大値。
        /// </summary>
        public const int NoteNumberMax = 127;
        /// <summary>
        /// 指定可能なベロシティの最大値。
        /// </summary>
        public const int VelocityMax = 127;

        /// <summary>
        /// MIDIメッセージを生成する。
        /// </summary>
        /// <param name="status">ステータス。</param>
        /// <param name="channel">チャネル番号。0～15。</param>
        /// <param name="number">ノート番号。0～127。</param>
        /// <param name="velocity">ベロシティ。0～127。</param>
        /// <returns>MIDIメッセージ。</returns>
        private static IntPtr MakeMessage(MessageStatus status, int channel, int number, int velocity)
        {
            if ((channel < 0) || (ChannelMax < channel)) return IntPtr.Zero;
            if ((number < 0) || (NoteNumberMax < number)) return IntPtr.Zero;
            if ((velocity < 0) || (VelocityMax < velocity)) return IntPtr.Zero;

            byte[] message = new byte[MessageLength];
            message[0] = (byte)channel;
            message[0] |= (byte)status;
            message[1] = (byte)number;
            message[2] = (byte)velocity;

            IntPtr buf = Marshal.AllocHGlobal(message.Length);
            Marshal.Copy(message, 0, buf, message.Length);
            return buf;
        }

        /// <summary>
        /// MIDIIO.dllの公開メソッド。
        /// </summary>
        private class NativeMethods
        {
            /// <summary>
            /// MIDI出力デバイスの数を調べる。
            /// </summary>
            /// <returns>MIDI出力デバイスの数。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_GetDeviceNum")]
            public static extern int GetDeviceNum();

            /// <summary>
            /// MIDI出力デバイスの名前を調べる。
            /// </summary>
            /// <param name="id">デバイスのID。0～。</param>
            /// <param name="deviceName">デバイスの名前。</param>
            /// <param name="nameLength"><paramref name="deviceName"/>の文字数。終端文字を含む。</param>
            /// <returns>実際に取得したデバイスの名前の文字数。終端文字を含まない。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_GetDeviceName")]
            public static extern int GetDeviceName(int id, IntPtr deviceName, int nameLength);

            /// <summary>
            /// MIDI出力デバイスを開く。
            /// </summary>
            /// <param name="deviceName">デバイスの名前。</param>
            /// <returns>開いたデバイスに対応したMIDI構造体のポインタ。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_Open", CharSet = CharSet.Ansi)]
            public static extern IntPtr Open(string deviceName);

            /// <summary>
            /// MIDI出力デバイスを閉じる。
            /// </summary>
            /// <param name="midi">閉じるMIDI構造体のポインタ。</param>
            /// <returns>エラーコード。MMSYSERR。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_Close")]
            public static extern int Close(IntPtr midi);

            /// <summary>
            /// MIDI出力デバイスをリセットする。
            /// </summary>
            /// <param name="midi">リセットするMIDI構造体のポインタ。</param>
            /// <returns>エラーコード。MMSYSERR。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_Reset")]
            public static extern int Reset(IntPtr midi);

            /// <summary>
            /// MIDIメッセージを送信する。
            /// </summary>
            /// <param name="midi">送信先のMIDI構造体のポインタ。</param>
            /// <param name="message">送信するメッセージ。</param>
            /// <param name="messageLength">送信するメッセージのサイズ。単位バイト。</param>
            /// <returns>送信したメッセージのサイズ。単位バイト。</returns>
            [DllImport("MIDIIO.dll", EntryPoint = "MIDIOut_PutMIDIMessage")]
            public static extern int PutMIDIMessage(IntPtr midi, IntPtr message, int messageLength);
        }

        /// <summary>
        /// リソースを破棄済みかどうか。
        /// </summary>
        private bool isDisposed = false;
        /// <summary>
        /// リソースを破棄する。
        /// </summary>
        /// <param name="disposing">true:Disposeからの呼び出し。 / false:デストラクタからの呼び出し。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed) return;
            this.isDisposed = true;
            if (disposing) {
                // マネージドリソースは無い。
            }
            NativeMethods.Close(this.handle);
        }
        /// <summary>
        /// リソースを破棄する。
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// デストラクタ。
        /// </summary>
        ~MidiOutputDeviece()
        {
            Dispose(disposing: false);
        }
    }
}
