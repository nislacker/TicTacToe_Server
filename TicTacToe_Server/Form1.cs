using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TicTacToe_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpListener listener;
        TcpClient client;
        NetworkStream stream;
        BinaryReader br;
        BinaryWriter bw;
        int[,] a;
        bool my_turn = true;
        int scor1 = 0, scor2 = 0, total = 0;
        bool i_want = true;
        bool winner = false;

        private PictureBox GetPicByPos(int i, int j)
        {
            if (i == 1)
                switch (j)
                {
                    case 1: return pictureBox1;
                    case 2: return pictureBox2;
                    case 3: return pictureBox3;
                }
            if (i == 2)
                switch (j)
                {
                    case 1: return pictureBox4;
                    case 2: return pictureBox5;
                    case 3: return pictureBox6;
                }
            if (i == 3)
                switch (j)
                {
                    case 1: return pictureBox7;
                    case 2: return pictureBox8;
                    case 3: return pictureBox9;
                }
            return null;
        }
        private void Reset()
        {
            this.Invoke(new Action(() =>
            {
                for (int i = 1; i <= 3; ++i)
                {
                    for (int j = 1; j <= 3; ++j)
                    {
                        a[i, j] = 0;
                        GetPicByPos(i, j).Image = imageList1.Images[0];
                        GetPicByPos(i, j).Enabled = true;
                    }
                }
                //my_turn = !my_turn;
                i_want = false;
                if (my_turn)
                    label3.Text = "Ваша очередь ходить";
                else
                    label3.Text = "Ходит Ваш оппонент";
                winner = false;
            }));

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Handler_Click(3, 3);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Handler_Click(2, 3);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Handler_Click(3, 2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Handler_Click(1, 3);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Handler_Click(3, 1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Handler_Click(1, 2);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Handler_Click(2, 1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Handler_Click(1, 1);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Handler_Click(2, 2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            a = new int[10, 10];
            Reset();
            for (int i = 1; i <= 3; ++i)
                for (int j = 1; j <= 3; ++j)
                    GetPicByPos(i, j).Enabled = false;

        }
        private void Handler_Click(int i, int j)
        {
            if (my_turn && a[i, j] == 0)
            {
                GetPicByPos(i, j).Image = imageList1.Images[1];
                a[i, j] = 1;
                if (Winner())
                {
                    button2.Visible = true;
                    for (int ii = 1; ii <= 3; ++ii)
                        for (int jj = 1; jj <= 3; ++jj)
                            GetPicByPos(ii, jj).Enabled = false;
                    winner = true;
                }
                bw.Write(1);
                bw.Write(i * 10 + j);
                bw.Flush();
                my_turn = false;
                if (!winner)
                    label3.Text = "Ходит Ваш оппонент";
            }
        }
        private bool Winner()
        {
            // проверяем каждую строку       
            for (int i = 1; i <= 3; ++i)
            {
                if (a[i, 1] == a[i, 2] && a[i, 2] == a[i, 3] && a[i, 1] != 0)
                {
                    Handler_Winner(a[i, 1], 1, i);
                    return true;
                }
            }
            // столбец
            for (int j = 1; j <= 3; ++j)
            {
                if (a[1, j] == a[2, j] && a[2, j] == a[3, j] && a[1, j] != 0)
                {
                    Handler_Winner(a[1, j], 2, j);
                    return true;
                }
            }
            // главная диагональ
            if (a[1, 1] == a[2, 2] && a[2, 2] == a[3, 3] && a[1, 1] != 0)
            {
                Handler_Winner(a[1, 1], 3, 1);
                return true;
            }

            if (a[1, 3] == a[2, 2] && a[2, 2] == a[3, 1] && a[1, 3] != 0)
            {
                Handler_Winner(a[1, 3], 3, 2);
                return true;
            }
            // Ничья
            bool draw = true;
            for (int i = 1; i <= 3; ++i)
                for (int j = 1; j <= 3; ++j)
                    if (a[i, j] == 0)
                        draw = false;
            if (draw)
            {
                UpdateScor();
                this.Invoke(new Action(() => { label3.Text = "Ничья"; }));
                return true;
            }
            return false;
        }
        private void Handler_Winner(int nr, int tip, int v)
        {
            this.Invoke(new Action(() =>
            {
                if (nr == 2)
                    label3.Text = "Вы проиграли";
                else
                    label3.Text = "Вы выиграли!";
            }));
            nr += 2;
            if (tip == 1)
                for (int i = 1; i <= 3; ++i)
                    GetPicByPos(v, i).Image = imageList1.Images[nr];
            if (tip == 2)
                for (int i = 1; i <= 3; ++i)
                    GetPicByPos(i, v).Image = imageList1.Images[nr];
            if (tip == 3)
            {
                if (v == 1)
                    for (int i = 1; i <= 3; ++i)
                        GetPicByPos(i, i).Image = imageList1.Images[nr];
                else
                    for (int i = 1; i <= 3; ++i)
                        GetPicByPos(i, 4 - i).Image = imageList1.Images[nr];
            }
            nr -= 2;
            if (nr == 1)
                scor1++;
            else
                scor2++;
            UpdateScor();
        }
        private void UpdateScor()
        {
            total++;
            this.Invoke(new Action(() =>
            {
                label1.Text = "Игрок №1: " + scor1.ToString();
                label2.Text = "Игрок №2: " + scor2.ToString();
                label4.Text = "Всего: " + total.ToString();
            }));

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Thread SearchConnectionThread = new Thread(SearchConnection);
                SearchConnectionThread.IsBackground = true;
                SearchConnectionThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SearchConnection()
        {
            try
            {
                int port;
                int.TryParse(tbxPort.Text, out port);
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                this.Invoke(new Action(() =>
                {
                    button1.Enabled = false;
                    button1.Text = "Поиск...";
                }));
                client = listener.AcceptTcpClient();
                stream = client.GetStream();
                bw = new BinaryWriter(stream);
                br = new BinaryReader(stream);
                this.Invoke(new Action(() =>
                {
                    button1.Text = "Подключен";
                    label3.Visible = true;
                    for (int i = 1; i <= 3; ++i)
                        for (int j = 1; j <= 3; ++j)
                            GetPicByPos(i, j).Enabled = true;
                }));
                Thread HandlerThread = new Thread(Handler);
                HandlerThread.IsBackground = true;
                HandlerThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Handler()
        {
            int type = 0;
            int nr = 0;
            while (true)
            {
                type = br.ReadInt32();
                if (type == 1) // позиция
                {
                    nr = br.ReadInt32();
                    if (a[nr / 10, nr % 10] != 0)
                        MessageBox.Show("У Вас проблема");
                    GetPicByPos(nr / 10, nr % 10).Image = imageList1.Images[2];
                    a[nr / 10, nr % 10] = 2;
                    if (Winner())
                    {
                        this.Invoke(new Action(() =>
                        {
                            button2.Visible = true;
                            for (int ii = 1; ii <= 3; ++ii)
                                for (int jj = 1; jj <= 3; ++jj)
                                    GetPicByPos(ii, jj).Enabled = false;
                            winner = true;
                        }));
                    }
                    my_turn = true;
                    if (!winner)
                        this.Invoke(new Action(() => { label3.Text = "Ваша очередь ходить"; }));
                }
                if (type == 2)
                {
                    this.Invoke(new Action(() =>
                    {
                        label3.Text = "Новая игра";
                        if (i_want)
                        {
                            bw.Write(3);
                            bw.Flush();
                            Reset();
                            button2.Visible = false;
                        }
                    }));
                }
                if (type == 3)
                    Reset();
                //if ( type == 5 )
                //{
                //    this.Invoke(new Action(() =>
                //    {
                //        listBox1.Items.Clear();
                //        int lenght = br.ReadInt32();
                //        for ( int i = 0; i < lenght; ++i )
                //        {
                //            string s = br.ReadString();
                //            listBox1.Items.Add(s);
                //        }
                //    }));
                //}
                Thread.Sleep(10);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "Дождитесь\nподтверждения";
            i_want = true;
            bw.Write(2);
            bw.Flush();
            button2.Visible = false;
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    if ( textBox1.Text == " 4" )
        //    {
        //        bw.Write(4);
        //        bw.Write(textBox2.Text);
        //        bw.Flush();
        //        textBox1.Clear();
        //    }
        //    if ( textBox1.Text == " 5" )
        //    {
        //        bw.Write(5);
        //        bw.Write(textBox2.Text);
        //        bw.Flush();
        //    }
        //}
    }
}
