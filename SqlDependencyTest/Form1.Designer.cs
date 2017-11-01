using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Permissions;

namespace SqlDependencyTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        List<string> NameList = new List<string>();

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

        void GetNames()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;


            if (!DoesHavePermitions())
            {
                return;
            }

            SqlDependency.Stop(connectionString);
            SqlDependency.Start(connectionString);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select name, lastName From dbo.[Names]";

                    cmd.Notification = null;
                    NameList.Clear();
                    SqlDependency dep = new SqlDependency(cmd);

                    dep.OnChange += new OnChangeEventHandler(HandleOnChange);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           // Console.WriteLine(reader.GetString(0) + " " + reader.GetString(1));
                            var name = reader.GetString(0) + " " + reader.GetString(1);
                            NameList.Add(name);
                            Debug.WriteLine(reader.GetString(0) + " " + reader.GetString(1));
                        }
                    }


                }
            }

        }

        void PrintNames()
        {
            GetNames();

            if (NameList.Count == 0)
                Debug.WriteLine("List is empty");
           
            foreach (var item in NameList )
            {
                Debug.WriteLine(item);
            }

            Debug.WriteLine("-------------------");
        }
        void HandleOnChange(object sender, SqlNotificationEventArgs e)
        {
           
            PrintNames();
            SqlDependency dep = sender as SqlDependency;
            dep.OnChange -= new OnChangeEventHandler(HandleOnChange);
        }

        bool DoesHavePermitions()
        {
            try
            {
                SqlClientPermission clientPermition = new SqlClientPermission(PermissionState.Unrestricted);
                clientPermition.Demand();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(260, 173);
            this.listBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += Button1_Click;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            PrintNames();
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
    }
}

