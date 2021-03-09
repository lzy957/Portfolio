namespace WindowsFormsApplication1.Forms
{
    partial class AttributeQueryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxField = new System.Windows.Forms.ListBox();
            this.listBoxValue = new System.Windows.Forms.ListBox();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnempty = new System.Windows.Forms.Button();
            this.btnspace = new System.Windows.Forms.Button();
            this.btnbetween = new System.Windows.Forms.Button();
            this.btncharacter = new System.Windows.Forms.Button();
            this.btnpercent = new System.Windows.Forms.Button();
            this.btnunderline = new System.Windows.Forms.Button();
            this.Btnless = new System.Windows.Forms.Button();
            this.btnmore = new System.Windows.Forms.Button();
            this.btnin = new System.Windows.Forms.Button();
            this.btnand = new System.Windows.Forms.Button();
            this.btnnot = new System.Windows.Forms.Button();
            this.btnnull = new System.Windows.Forms.Button();
            this.btnor = new System.Windows.Forms.Button();
            this.btnloe = new System.Windows.Forms.Button();
            this.btnmoe = new System.Windows.Forms.Button();
            this.btnlike = new System.Windows.Forms.Button();
            this.btnis = new System.Windows.Forms.Button();
            this.btnunequal = new System.Windows.Forms.Button();
            this.Btnequal = new System.Windows.Forms.Button();
            this.textBoxSql = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxField
            // 
            this.listBoxField.FormattingEnabled = true;
            this.listBoxField.ItemHeight = 12;
            this.listBoxField.Location = new System.Drawing.Point(32, 76);
            this.listBoxField.Name = "listBoxField";
            this.listBoxField.Size = new System.Drawing.Size(132, 148);
            this.listBoxField.TabIndex = 0;
            this.listBoxField.SelectedIndexChanged += new System.EventHandler(this.listBoxField_SelectedIndexChanged);
            this.listBoxField.DoubleClick += new System.EventHandler(this.listBoxField_DoubleClick);
            // 
            // listBoxValue
            // 
            this.listBoxValue.FormattingEnabled = true;
            this.listBoxValue.ItemHeight = 12;
            this.listBoxValue.Location = new System.Drawing.Point(180, 76);
            this.listBoxValue.Name = "listBoxValue";
            this.listBoxValue.Size = new System.Drawing.Size(134, 148);
            this.listBoxValue.TabIndex = 1;
            this.listBoxValue.DoubleClick += new System.EventHandler(this.listBoxValue_DoubleClick);
            // 
            // cboLayer
            // 
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(92, 18);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(222, 20);
            this.cboLayer.TabIndex = 2;
            this.cboLayer.SelectedIndexChanged += new System.EventHandler(this.cboLayer_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnempty);
            this.groupBox1.Controls.Add(this.btnspace);
            this.groupBox1.Controls.Add(this.btnbetween);
            this.groupBox1.Controls.Add(this.btncharacter);
            this.groupBox1.Controls.Add(this.btnpercent);
            this.groupBox1.Controls.Add(this.btnunderline);
            this.groupBox1.Controls.Add(this.Btnless);
            this.groupBox1.Controls.Add(this.btnmore);
            this.groupBox1.Controls.Add(this.btnin);
            this.groupBox1.Controls.Add(this.btnand);
            this.groupBox1.Controls.Add(this.btnnot);
            this.groupBox1.Controls.Add(this.btnnull);
            this.groupBox1.Controls.Add(this.btnor);
            this.groupBox1.Controls.Add(this.btnloe);
            this.groupBox1.Controls.Add(this.btnmoe);
            this.groupBox1.Controls.Add(this.btnlike);
            this.groupBox1.Controls.Add(this.btnis);
            this.groupBox1.Controls.Add(this.btnunequal);
            this.groupBox1.Controls.Add(this.Btnequal);
            this.groupBox1.Location = new System.Drawing.Point(32, 237);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 143);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "表达式";
            // 
            // btnempty
            // 
            this.btnempty.Location = new System.Drawing.Point(214, 107);
            this.btnempty.Name = "btnempty";
            this.btnempty.Size = new System.Drawing.Size(68, 23);
            this.btnempty.TabIndex = 18;
            this.btnempty.Text = "清空";
            this.btnempty.UseVisualStyleBackColor = true;
            this.btnempty.Click += new System.EventHandler(this.btnempty_Click);
            // 
            // btnspace
            // 
            this.btnspace.Location = new System.Drawing.Point(138, 107);
            this.btnspace.Name = "btnspace";
            this.btnspace.Size = new System.Drawing.Size(68, 23);
            this.btnspace.TabIndex = 17;
            this.btnspace.Text = "空格";
            this.btnspace.UseVisualStyleBackColor = true;
            this.btnspace.Click += new System.EventHandler(this.btnspace_Click);
            // 
            // btnbetween
            // 
            this.btnbetween.Location = new System.Drawing.Point(64, 107);
            this.btnbetween.Name = "btnbetween";
            this.btnbetween.Size = new System.Drawing.Size(68, 23);
            this.btnbetween.TabIndex = 16;
            this.btnbetween.Text = "Between";
            this.btnbetween.UseVisualStyleBackColor = true;
            this.btnbetween.Click += new System.EventHandler(this.btnbetween_Click);
            // 
            // btncharacter
            // 
            this.btncharacter.Location = new System.Drawing.Point(8, 107);
            this.btncharacter.Name = "btncharacter";
            this.btncharacter.Size = new System.Drawing.Size(50, 23);
            this.btncharacter.TabIndex = 15;
            this.btncharacter.Text = "\' \'";
            this.btncharacter.UseVisualStyleBackColor = true;
            this.btncharacter.Click += new System.EventHandler(this.btncharacter_Click);
            // 
            // btnpercent
            // 
            this.btnpercent.Location = new System.Drawing.Point(232, 78);
            this.btnpercent.Name = "btnpercent";
            this.btnpercent.Size = new System.Drawing.Size(50, 23);
            this.btnpercent.TabIndex = 14;
            this.btnpercent.Text = "%";
            this.btnpercent.UseVisualStyleBackColor = true;
            this.btnpercent.Click += new System.EventHandler(this.btnpercent_Click);
            // 
            // btnunderline
            // 
            this.btnunderline.Location = new System.Drawing.Point(176, 78);
            this.btnunderline.Name = "btnunderline";
            this.btnunderline.Size = new System.Drawing.Size(50, 23);
            this.btnunderline.TabIndex = 13;
            this.btnunderline.Text = "_";
            this.btnunderline.UseVisualStyleBackColor = true;
            this.btnunderline.Click += new System.EventHandler(this.btnunderline_Click);
            // 
            // Btnless
            // 
            this.Btnless.Location = new System.Drawing.Point(8, 49);
            this.Btnless.Name = "Btnless";
            this.Btnless.Size = new System.Drawing.Size(50, 23);
            this.Btnless.TabIndex = 5;
            this.Btnless.Text = "<";
            this.Btnless.UseVisualStyleBackColor = true;
            this.Btnless.Click += new System.EventHandler(this.Btnless_Click);
            // 
            // btnmore
            // 
            this.btnmore.Location = new System.Drawing.Point(232, 20);
            this.btnmore.Name = "btnmore";
            this.btnmore.Size = new System.Drawing.Size(50, 23);
            this.btnmore.TabIndex = 4;
            this.btnmore.Text = ">";
            this.btnmore.UseVisualStyleBackColor = true;
            this.btnmore.Click += new System.EventHandler(this.btnmore_Click);
            // 
            // btnin
            // 
            this.btnin.Location = new System.Drawing.Point(120, 78);
            this.btnin.Name = "btnin";
            this.btnin.Size = new System.Drawing.Size(50, 23);
            this.btnin.TabIndex = 12;
            this.btnin.Text = "In";
            this.btnin.UseVisualStyleBackColor = true;
            this.btnin.Click += new System.EventHandler(this.btnin_Click);
            // 
            // btnand
            // 
            this.btnand.Location = new System.Drawing.Point(64, 78);
            this.btnand.Name = "btnand";
            this.btnand.Size = new System.Drawing.Size(50, 23);
            this.btnand.TabIndex = 11;
            this.btnand.Text = "And";
            this.btnand.UseVisualStyleBackColor = true;
            this.btnand.Click += new System.EventHandler(this.btnand_Click);
            // 
            // btnnot
            // 
            this.btnnot.Location = new System.Drawing.Point(8, 78);
            this.btnnot.Name = "btnnot";
            this.btnnot.Size = new System.Drawing.Size(50, 23);
            this.btnnot.TabIndex = 10;
            this.btnnot.Text = "Not";
            this.btnnot.UseVisualStyleBackColor = true;
            this.btnnot.Click += new System.EventHandler(this.btnnot_Click);
            // 
            // btnnull
            // 
            this.btnnull.Location = new System.Drawing.Point(232, 49);
            this.btnnull.Name = "btnnull";
            this.btnnull.Size = new System.Drawing.Size(50, 23);
            this.btnnull.TabIndex = 9;
            this.btnnull.Text = "Null";
            this.btnnull.UseVisualStyleBackColor = true;
            this.btnnull.Click += new System.EventHandler(this.btnnull_Click);
            // 
            // btnor
            // 
            this.btnor.Location = new System.Drawing.Point(176, 49);
            this.btnor.Name = "btnor";
            this.btnor.Size = new System.Drawing.Size(50, 23);
            this.btnor.TabIndex = 8;
            this.btnor.Text = "Or";
            this.btnor.UseVisualStyleBackColor = true;
            this.btnor.Click += new System.EventHandler(this.btnor_Click);
            // 
            // btnloe
            // 
            this.btnloe.Location = new System.Drawing.Point(120, 49);
            this.btnloe.Name = "btnloe";
            this.btnloe.Size = new System.Drawing.Size(50, 23);
            this.btnloe.TabIndex = 7;
            this.btnloe.Text = "<=";
            this.btnloe.UseVisualStyleBackColor = true;
            this.btnloe.Click += new System.EventHandler(this.btnloe_Click);
            // 
            // btnmoe
            // 
            this.btnmoe.Location = new System.Drawing.Point(64, 49);
            this.btnmoe.Name = "btnmoe";
            this.btnmoe.Size = new System.Drawing.Size(50, 23);
            this.btnmoe.TabIndex = 6;
            this.btnmoe.Text = ">=";
            this.btnmoe.UseVisualStyleBackColor = true;
            this.btnmoe.Click += new System.EventHandler(this.btnmoe_Click);
            // 
            // btnlike
            // 
            this.btnlike.Location = new System.Drawing.Point(176, 20);
            this.btnlike.Name = "btnlike";
            this.btnlike.Size = new System.Drawing.Size(50, 23);
            this.btnlike.TabIndex = 3;
            this.btnlike.Text = "like";
            this.btnlike.UseVisualStyleBackColor = true;
            this.btnlike.Click += new System.EventHandler(this.btnlike_Click);
            // 
            // btnis
            // 
            this.btnis.Location = new System.Drawing.Point(120, 20);
            this.btnis.Name = "btnis";
            this.btnis.Size = new System.Drawing.Size(50, 23);
            this.btnis.TabIndex = 2;
            this.btnis.Text = "is";
            this.btnis.UseVisualStyleBackColor = true;
            this.btnis.Click += new System.EventHandler(this.btnis_Click);
            // 
            // btnunequal
            // 
            this.btnunequal.Location = new System.Drawing.Point(64, 20);
            this.btnunequal.Name = "btnunequal";
            this.btnunequal.Size = new System.Drawing.Size(50, 23);
            this.btnunequal.TabIndex = 1;
            this.btnunequal.Text = "!=";
            this.btnunequal.UseVisualStyleBackColor = true;
            this.btnunequal.Click += new System.EventHandler(this.btnunequal_Click);
            // 
            // Btnequal
            // 
            this.Btnequal.Location = new System.Drawing.Point(8, 20);
            this.Btnequal.Name = "Btnequal";
            this.Btnequal.Size = new System.Drawing.Size(50, 23);
            this.Btnequal.TabIndex = 0;
            this.Btnequal.Text = "=";
            this.Btnequal.UseVisualStyleBackColor = true;
            this.Btnequal.Click += new System.EventHandler(this.btnequal_Click);
            // 
            // textBoxSql
            // 
            this.textBoxSql.Location = new System.Drawing.Point(32, 412);
            this.textBoxSql.Name = "textBoxSql";
            this.textBoxSql.Size = new System.Drawing.Size(281, 21);
            this.textBoxSql.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(32, 460);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(246, 460);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 20;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10F);
            this.label1.Location = new System.Drawing.Point(30, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 21;
            this.label1.Text = "图层：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "字段：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(178, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "取值：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 397);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "Selesct * From Table Where";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(32, 438);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(198, 16);
            this.checkBox1.TabIndex = 25;
            this.checkBox1.Text = "Only Display Choosen Features";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // AttributeQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 495);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.textBoxSql);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cboLayer);
            this.Controls.Add(this.listBoxValue);
            this.Controls.Add(this.listBoxField);
            this.Name = "AttributeQueryForm";
            this.Text = "属性选择";
            this.Load += new System.EventHandler(this.AttributeQueryForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxField;
        private System.Windows.Forms.ListBox listBoxValue;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxSql;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnempty;
        private System.Windows.Forms.Button btnspace;
        private System.Windows.Forms.Button btnbetween;
        private System.Windows.Forms.Button btncharacter;
        private System.Windows.Forms.Button btnpercent;
        private System.Windows.Forms.Button btnunderline;
        private System.Windows.Forms.Button btnin;
        private System.Windows.Forms.Button btnand;
        private System.Windows.Forms.Button btnnot;
        private System.Windows.Forms.Button btnnull;
        private System.Windows.Forms.Button btnor;
        private System.Windows.Forms.Button btnloe;
        private System.Windows.Forms.Button btnmoe;
        private System.Windows.Forms.Button Btnless;
        private System.Windows.Forms.Button btnmore;
        private System.Windows.Forms.Button btnlike;
        private System.Windows.Forms.Button btnis;
        private System.Windows.Forms.Button btnunequal;
        private System.Windows.Forms.Button Btnequal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}