using System;

namespace UdonMidiOutput
{
    /// <summary>
    /// MIDIで文字列をUDONに送信する。
    /// </summary>
    public class UdonTextOutput : IDisposable
    {
        /// <summary>
        /// MIDI出力デバイス操作クラス。
        /// </summary>
        private readonly MidiOutputDeviece midi = null;

        /// <summary>
        /// デフォルトコンストラクタ。
        /// </summary>
        public UdonTextOutput()
        {
            this.midi = new MidiOutputDeviece();
        }

        /// <summary>
        /// デバイス名指定コンストラクタ。
        /// </summary>
        /// <param name="deviceName">MIDI出力デバイスの名前。</param>
        public UdonTextOutput(string deviceName)
        {
            this.midi = new MidiOutputDeviece(deviceName);
        }

        /// <summary>
        /// 接続が有効化どうか。
        /// </summary>
        public bool IsEnable
        {
            get { return this.midi.IsEnable; }
        }

        /// <summary>
        /// MIDIメッセージを使用して文字列を送信する。
        /// </summary>
        /// <param name="channel">送信先のチャネル。0～15。</param>
        /// <param name="text">送信する文字列。</param>
        /// <returns>成功時は送信した文字列を返す。失敗時は空の文字列を返す。</returns>
        public string SendText(int channel, string text)
        {
            // NoteOnで文字列送信開始を通知。
            if (!this.midi.NoteOn(channel, 0, 0)) return String.Empty;
            foreach (var binary in StringOnMidi.StringToBinary(text)) {
                // 文字列を1バイトずつ分解してControlChangeで送信。
                var (number, value) = StringOnMidi.ByteToMidi(binary);
                if (!this.midi.ControlChange(channel, number, value)) return String.Empty;
            }
            // NoteOffで文字列送信完了を通知。
            if (!this.midi.NoteOff(channel, 0, 0)) return String.Empty;
            return text;
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
            if (!disposing) return;

            this.midi.Dispose();
        }
        /// <summary>
        /// リソースを破棄する。
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
