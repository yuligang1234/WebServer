using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WebServer
{
    public partial class DevServerProcess : Form
    {
        private IContainer _components;

        private Label _label1;

        public DevServerProcess()
        {
            InitializeComponent();
        }

        private void closeTimer_Tick(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
            ((Timer)sender).Dispose();
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _components != null)
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _label1 = new Label();
            SuspendLayout();
            _label1.AutoSize = true;
            _label1.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _label1.Location = new Point(26, 26);
            _label1.Name = "_label1";
            _label1.Size = new Size(0x205, 20);
            _label1.TabIndex = 0;
            _label1.Text = "请稍等,正在启动web服务器...";
            _label1.UseWaitCursor = true;
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            CausesValidation = false;
            ClientSize = new Size(0x23f, 70);
            Controls.Add(this._label1);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "加载中...";
            UseWaitCursor = true;
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Timer timer = new Timer();
            timer.Interval = 0xfa0;
            timer.Tick += new EventHandler(this.closeTimer_Tick);
            timer.Start();
        }
    }
}
