namespace Laba6_2_
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtClientPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnRefreshFiles = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCurrentGroup = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtChatLog = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtClientLog = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lstServerFiles = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnCreateGroup = new System.Windows.Forms.Button();
            this.btnJoinGroup = new System.Windows.Forms.Button();
            this.btnLeaveGroup = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtClientPath
            // 
            this.txtClientPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtClientPath.Location = new System.Drawing.Point(214, 808);
            this.txtClientPath.Name = "txtClientPath";
            this.txtClientPath.Size = new System.Drawing.Size(369, 28);
            this.txtClientPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(13, 811);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Скачанные файлы:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(668, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "Имя пользователя:";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtUsername.Location = new System.Drawing.Point(860, 21);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(369, 28);
            this.txtUsername.TabIndex = 3;
            // 
            // txtServerIP
            // 
            this.txtServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtServerIP.Location = new System.Drawing.Point(860, 68);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(369, 28);
            this.txtServerIP.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(735, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 22);
            this.label3.TabIndex = 5;
            this.label3.Text = "IP сервера:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtServerPort.Location = new System.Drawing.Point(860, 116);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(369, 28);
            this.txtServerPort.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(784, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 22);
            this.label4.TabIndex = 7;
            this.label4.Text = "Порт:";
            // 
            // btnUpload
            // 
            this.btnUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUpload.Location = new System.Drawing.Point(13, 861);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(132, 67);
            this.btnUpload.TabIndex = 8;
            this.btnUpload.Text = "Отправка файла";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnRefreshFiles
            // 
            this.btnRefreshFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRefreshFiles.Location = new System.Drawing.Point(351, 852);
            this.btnRefreshFiles.Name = "btnRefreshFiles";
            this.btnRefreshFiles.Size = new System.Drawing.Size(132, 84);
            this.btnRefreshFiles.TabIndex = 9;
            this.btnRefreshFiles.Text = "Обновить список файлов";
            this.btnRefreshFiles.UseVisualStyleBackColor = true;
            this.btnRefreshFiles.Click += new System.EventHandler(this.btnRefreshFiles_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDownload.Location = new System.Drawing.Point(180, 861);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(132, 67);
            this.btnDownload.TabIndex = 10;
            this.btnDownload.Text = "Скачать файл";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // lstUsers
            // 
            this.lstUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.ItemHeight = 22;
            this.lstUsers.Location = new System.Drawing.Point(12, 59);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.ScrollAlwaysVisible = true;
            this.lstUsers.Size = new System.Drawing.Size(340, 136);
            this.lstUsers.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(74, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(199, 22);
            this.label5.TabIndex = 12;
            this.label5.Text = "Пользователи онлайн:";
            // 
            // lblCurrentGroup
            // 
            this.lblCurrentGroup.AutoSize = true;
            this.lblCurrentGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCurrentGroup.Location = new System.Drawing.Point(689, 163);
            this.lblCurrentGroup.Name = "lblCurrentGroup";
            this.lblCurrentGroup.Size = new System.Drawing.Size(151, 22);
            this.lblCurrentGroup.TabIndex = 13;
            this.lblCurrentGroup.Text = "Текущая группа:";
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtMessage.Location = new System.Drawing.Point(788, 874);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(441, 60);
            this.txtMessage.TabIndex = 14;
            // 
            // txtChatLog
            // 
            this.txtChatLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtChatLog.Location = new System.Drawing.Point(788, 288);
            this.txtChatLog.Multiline = true;
            this.txtChatLog.Name = "txtChatLog";
            this.txtChatLog.ReadOnly = true;
            this.txtChatLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChatLog.Size = new System.Drawing.Size(441, 561);
            this.txtChatLog.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(784, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 22);
            this.label6.TabIndex = 16;
            this.label6.Text = "Сообщения:";
            // 
            // txtClientLog
            // 
            this.txtClientLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtClientLog.Location = new System.Drawing.Point(16, 265);
            this.txtClientLog.Multiline = true;
            this.txtClientLog.Name = "txtClientLog";
            this.txtClientLog.ReadOnly = true;
            this.txtClientLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClientLog.Size = new System.Drawing.Size(336, 241);
            this.txtClientLog.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(105, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(127, 22);
            this.label7.TabIndex = 18;
            this.label7.Text = "Логи клиента:";
            // 
            // lstServerFiles
            // 
            this.lstServerFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lstServerFiles.FormattingEnabled = true;
            this.lstServerFiles.ItemHeight = 22;
            this.lstServerFiles.Location = new System.Drawing.Point(16, 577);
            this.lstServerFiles.Name = "lstServerFiles";
            this.lstServerFiles.ScrollAlwaysVisible = true;
            this.lstServerFiles.Size = new System.Drawing.Size(336, 180);
            this.lstServerFiles.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(131, 541);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 22);
            this.label8.TabIndex = 20;
            this.label8.Text = "Файлы:";
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSendMessage.Location = new System.Drawing.Point(620, 874);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(146, 60);
            this.btnSendMessage.TabIndex = 21;
            this.btnSendMessage.Text = "Отправить";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnCreateGroup
            // 
            this.btnCreateGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCreateGroup.Location = new System.Drawing.Point(375, 48);
            this.btnCreateGroup.Name = "btnCreateGroup";
            this.btnCreateGroup.Size = new System.Drawing.Size(119, 69);
            this.btnCreateGroup.TabIndex = 22;
            this.btnCreateGroup.Text = "Создать группу";
            this.btnCreateGroup.UseVisualStyleBackColor = true;
            this.btnCreateGroup.Click += new System.EventHandler(this.btnCreateGroup_Click);
            // 
            // btnJoinGroup
            // 
            this.btnJoinGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnJoinGroup.Location = new System.Drawing.Point(375, 140);
            this.btnJoinGroup.Name = "btnJoinGroup";
            this.btnJoinGroup.Size = new System.Drawing.Size(119, 69);
            this.btnJoinGroup.TabIndex = 23;
            this.btnJoinGroup.Text = "Вступить в группу";
            this.btnJoinGroup.UseVisualStyleBackColor = true;
            this.btnJoinGroup.Click += new System.EventHandler(this.btnJoinGroup_Click);
            // 
            // btnLeaveGroup
            // 
            this.btnLeaveGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLeaveGroup.Location = new System.Drawing.Point(510, 96);
            this.btnLeaveGroup.Name = "btnLeaveGroup";
            this.btnLeaveGroup.Size = new System.Drawing.Size(112, 69);
            this.btnLeaveGroup.TabIndex = 24;
            this.btnLeaveGroup.Text = "Покинуть группу";
            this.btnLeaveGroup.UseVisualStyleBackColor = true;
            this.btnLeaveGroup.Click += new System.EventHandler(this.btnLeaveGroup_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnConnect.Location = new System.Drawing.Point(942, 210);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(177, 60);
            this.btnConnect.TabIndex = 25;
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 953);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnLeaveGroup);
            this.Controls.Add(this.btnJoinGroup);
            this.Controls.Add(this.btnCreateGroup);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lstServerFiles);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtClientLog);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtChatLog);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblCurrentGroup);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lstUsers);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnRefreshFiles);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtClientPath);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtClientPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnRefreshFiles;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCurrentGroup;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtChatLog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtClientLog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstServerFiles;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnCreateGroup;
        private System.Windows.Forms.Button btnJoinGroup;
        private System.Windows.Forms.Button btnLeaveGroup;
        private System.Windows.Forms.Button btnConnect;
    }
}

