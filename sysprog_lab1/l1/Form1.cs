using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using Syste   m.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;


namespace l1
{
    
    public partial class Form1 : Form
    {
        public int thread_id;
        Process Child = null;
        EventWaitHandle evStart = new EventWaitHandle(false, EventResetMode.ManualReset, "EventStart");
        EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.ManualReset, "EventStop");

        
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


        private void button1_Click(object sender, EventArgs e)
        {
            int text = (int)numericUpDown1.Value;
            if ((Child == null) || (Child.HasExited))
            {
                Child = Process.Start("..\\..\\lab1.1(1)\\LAB1\\Debug\\L1c.exe");
                comboBox1.Items.Add(" Main thread" + "\n");
                comboBox1.Items.Add(" All  thread" + "\n");
                Child.EnableRaisingEvents = true;
                Child.Exited += new System.EventHandler(ChildExited);
            }
            else
            {

                for (int i = 0; i < text; i++)
                {
                //Thread thread = new Thread(new ThreadStart(i));
                //thread.Start();
                    comboBox1.Items.Add($"id {thread_id}\n");
                    thread_id++;
                    evStart.Set();
                  //evStart.Reset();
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (thread_id > 0)
            {
                comboBox1.Items.RemoveAt(thread_id);
                thread_id--;
                evStop.Reset();
                evStop.Set();

            }
            //else
            //{
            //    comboBox1.Items.Clear();
            //}
            
            evStop.Set();
            evStop.Reset();
           
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Child?.Kill();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
