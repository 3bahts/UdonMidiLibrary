using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UdonMidiOutput;

namespace QuizMaster
{
    /// <summary>
    /// 問題文モデル。
    /// </summary>
    internal class QuestionModel : INotifyPropertyChanged
    {
        /// <summary>
        /// MIDI出力デバイス。
        /// </summary>
        public static string[] MidiDevices
        {
            get {
                return MidiOutputDeviece.GetDeviceName();
            }
        }
        /// <summary>
        /// 選択されたMIDI出力デバイス。
        /// </summary>
        public string SelectedDevice { get; set; } = String.Empty;

        /// <summary>
        /// プロパティ変更通知イベント。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティ変更を通知する。
        /// </summary>
        /// <param name="propertyName"></param>
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// MIDI出力デバイスに送信できるかどうか。
        /// </summary>
        private bool enableSend = false;
        /// <summary>
        /// MIDI出力デバイスに送信できるかどうか。
        /// </summary>
        public bool EnableSend
        {
            get { return this.enableSend; }
            set { SetProperty(ref this.enableSend, value); }
        }

        /// <summary>
        /// 問題テキスト種別。
        /// </summary>
        public enum TextType : int
        {
            Title,
            Question,
            Answer,
        }

        /// <summary>
        /// タイトル。
        /// </summary>
        private string title = String.Empty;
        /// <summary>
        /// タイトル。
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// 問題文。
        /// </summary>
        private string question = String.Empty;
        /// <summary>
        /// 問題文。
        /// </summary>
        public string Question
        {
            get { return this.question; }
            set { SetProperty(ref this.question, value); }
        }

        /// <summary>
        /// 解答。
        /// </summary>
        private string answer = String.Empty;
        /// <summary>
        /// 解答。
        /// </summary>
        public string Answer
        {
            get { return this.answer; }
            set { SetProperty(ref this.answer, value); }
        }
    }
}
