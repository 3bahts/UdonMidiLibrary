using System;
using System.Text;

namespace UdonMidiOutput
{
    /// <summary>
    /// MIDIメッセージに乗せるデータと文字列を変換する。
    /// </summary>
    public static class StringOnMidi
    {
        /// <summary>
        /// MIDIメッセージをバイトデータに変換する。
        /// </summary>
        /// <param name="number">ノート番号番号。</param>
        /// <param name="value">データ値。</param>
        /// <returns>MIDIメッセージから変換したバイトデータ。</returns>
        public static byte MidiToByte(int number, int value)
        {
            byte hi = (byte)((number & 0x01) << 7);
            byte lo = (byte)((value & 0x7F) << 0);
            return (byte)(hi | lo);
        }

        /// <summary>
        /// バイトデータをMIDIメッセージに変換する。
        /// </summary>
        /// <param name="data">バイトデータ。</param>
        /// <returns>バイトデータから変換したMIDIメッセージ。</returns>
        public static (int number, int value) ByteToMidi(byte data)
        {
            int number = (data >> 7) & 0x01;
            int velocity = (data >> 0) & 0x7F;
            return (number, velocity);
        }

        /// <summary>
        /// 文字列のエンコード。UTF-16。
        /// </summary>
        private static readonly Encoding StringEncoding = Encoding.Unicode;

        /// <summary>
        /// バイトデータの配列を文字列に変換する。
        /// </summary>
        /// <param name="binary">バイトデータの配列。</param>
        /// <returns>バイトデータから変換した文字列。</returns>
        public static string BinaryToString(byte[] binary)
        {
            if (binary == null) return String.Empty;
            return StringEncoding.GetString(binary);
        }

        /// <summary>
        /// 文字列をバイトデータの配列に変換する。
        /// </summary>
        /// <param name="text">文字列。</param>
        /// <returns>文字列から変換したバイトデータの配列。</returns>
        public static byte[] StringToBinary(string text)
        {
            if (text == null) return new byte[0];
            return StringEncoding.GetBytes(text);
        }
    }
}
