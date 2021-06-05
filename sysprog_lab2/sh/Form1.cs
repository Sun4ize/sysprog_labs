﻿using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;


namespace l1
{
    public partial class Form1 : Form
    {
        public int thread_id = 1;
        Process Child = null;
        EventWaitHandle evStart = new EventWaitHandle(false, EventResetMode.ManualReset, "EventStart");
        EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.AutoReset, "EventStop");
        EventWaitHandle evSubmit = new EventWaitHandle(false, EventResetMode.ManualReset, "ThreadSubmit");

        [DllImport("..\\Message.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool writeMap(StringBuilder s, int threadI=0);

        [DllImport("..\\Message.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void closeMap();

        public Form1()
        {
            InitializeComponent();
        }

        private void ChildExited(object sender, EventArgs e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => comboBox1.Items.Clear()));
            else
                comboBox1.Items.Clear();
            thread_id = 1;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            
            int thread_number = (int)thread_count.Value;
            if ((Child == null) || (Child.HasExited))
            {
                Child = Process.Start("..\\lab2cpp.exe");
                comboBox1.Items.Add("All Threads\n");
                comboBox1.Items.Add("Main Thread\n");
                Child.EnableRaisingEvents = true;
                Child.Exited += new System.EventHandler(ChildExited);
            }
            else
            {
                for (int i = 0; i < thread_number; i++)
                {
                    //Thread thread = new Thread(new ThreadStart(i));
                    //thread.Start();
                    comboBox1.Items.Add($"id {thread_id}\n");
                    thread_id++;
                    evStart.Set();
                    //evStart.Reset();
                }
                //for (int i = 0; i < thread_number; i++)
                //{
                //    evStart.Set();
                //    if (evSubmit.WaitOne(10000))
                //    {
                //        comboBox1.Items.Add("id " + thread_id++.ToString() + "\n");
                //        evSubmit.Reset();
                //    }
                //    else
                //        MessageBox.Show("Smth wrong...");
                //}

            }
        }

        private void Stop_click(object sender, EventArgs e)
        {
            if (!(Child?.HasExited == false))
                return;
            evStop.Set();
            if (evSubmit.WaitOne(1000))
            {
                if (thread_id > 1)
                    comboBox1.Items.RemoveAt(thread_id-- );
                else
                {
                    Child = null;
                    comboBox1.Items.Clear();
                }
                evSubmit.Reset();
            }
        }


        private void SendButton_Click(object sender, EventArgs e)
        {
            StringBuilder fileText = new StringBuilder(textBox1.Text, Encoding.Unicode.GetBytes(textBox1.Text).Length);
            if (!(comboBox1.SelectedIndex >= 0)) 
            {
                label1.Text = "You have to choose thread\n";
                label1.Visible = true;
            }
            else
            {
                label1.Visible = false;
                if (comboBox1.SelectedItem.ToString() == "All Threads\n")
                {
                    for (int i = 1; i < thread_id + 1; i++)
                    {
                        var index = (int)comboBox1.SelectedIndex - 1;
                        var msgText = "NewMessage" + i;
                        EventWaitHandle Message = new EventWaitHandle(false, EventResetMode.ManualReset, msgText);
                        writeMap(fileText, index);
                        Message.Set();
                        closeMap();
                    }
                }
                else if (comboBox1.SelectedItem.ToString() == "Main Thread\n")
                {
                    EventWaitHandle Message = new EventWaitHandle(false, EventResetMode.ManualReset, "evMain");
                    writeMap(fileText);
                    Message.Set();
                    closeMap();
                }
                else
                {
                    var index = (int)comboBox1.SelectedIndex - 1;
                    var msgText = "NewMessage" + index;
                    EventWaitHandle Message = new EventWaitHandle(false, EventResetMode.ManualReset, msgText);
                    writeMap(fileText, index);
                    Message.Set();
                    closeMap();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             Child?.Kill();
        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void thread_count_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
} 