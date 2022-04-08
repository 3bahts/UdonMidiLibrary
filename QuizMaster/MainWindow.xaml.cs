using System;
using System.Threading.Tasks;
using System.Windows;
using UdonMidiOutput;

namespace QuizMaster
{
    /// <summary>
    /// メインウィンドウ。
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// 問題文の送信先。
        /// </summary>
        private UdonTextOutput? udon = null;

        /// <summary>
        /// 問題文モデル。
        /// </summary>
        private readonly QuestionModel? model = null;

        /// <summary>
        /// データ送信タスク。
        /// </summary>
        private Task? sendTask = null;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.model = this.DataContext as QuestionModel;
        }

        /// <summary>
        /// 送信ボタンの有効状態を判定する。
        /// </summary>
        /// <returns>true:有効。 / false:無効。</returns>
        private bool IsSendEnable()
        {
            bool isSending = !(this.sendTask?.IsCompleted ?? true);
            if (isSending) return false;
            return this.udon?.IsEnable ?? false;
        }

        /// <summary>
        /// 送信ボタンの有効状態を更新する。
        /// </summary>
        private void UpdateSendEnable()
        {
            if (this.model == null) return;
            this.model.EnableSend = IsSendEnable();
        }

        /// <summary>
        /// 接続ボタン押下イベントハンドラ。
        /// </summary>
        /// <param name="sender">接続ボタン。</param>
        /// <param name="e">イベント情報。</param>
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.model == null) return;

            string deviceName = this.model.SelectedDevice ?? String.Empty;
            if (String.IsNullOrEmpty(deviceName)) return;
            this.udon?.Dispose();
            this.udon = new UdonTextOutput(deviceName);

            UpdateSendEnable();
        }

        /// <summary>
        /// 送信ボタン押下イベントハンドラ。
        /// </summary>
        /// <param name="sender">接続ボタン。</param>
        /// <param name="e">イベント情報。</param>
        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            if (this.model == null || this.udon == null) return;
            if (this.sendTask != null && !this.sendTask.IsCompleted) return;

            this.sendTask?.Dispose();
            this.sendTask = Task.Run(() =>
            {
                this.udon?.SendText((int)QuestionModel.TextType.Title, this.model.Title);
                this.udon?.SendText((int)QuestionModel.TextType.Question, this.model.Question);
                this.udon?.SendText((int)QuestionModel.TextType.Answer, this.model.Answer);
            });
            this.sendTask.ContinueWith(task => UpdateSendEnable());
            UpdateSendEnable();
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

            this.udon?.Dispose();
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
