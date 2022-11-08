using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using NReco;


namespace SearchPathCustomCaspar
{
    public partial class Graphics : Form
    {
        System.Net.Sockets.TcpClient casparClient = new System.Net.Sockets.TcpClient();
        Point startPos;
        Point currentPos;
        bool drawing;
        List<Rectangle> myRectangles = new List<Rectangle>();
        string currentImgPath;
        string currentFilename;
        string mediapath;
        Point previousLocation;
        Control activeControl;
        List<Panel> panelsList = new List<Panel>();
        ImageList thumbnailList = new ImageList();
        ImageList thumbnailListRundown = new ImageList();
        List<string> thumbnailPaths = new List<string>();
        List<TextBoxAnimation> textBoxes = new List<TextBoxAnimation>(); 

        ListViewItem heldDownItem;
        Point heldDownPoint;

        public Graphics()
        {
            InitializeComponent();
            FillComboBoxWithFonts();
            panelsList.Add(panel1);
            panelsList.Add(panel2);
            panelsList[0].BringToFront();
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                casparClient.Connect(txtCasparServer.Text, int.Parse(txtCasparPort.Text));
                if (casparClient.Connected)
                {
                    /*lblStatus.Text = "CONNECTED";
                    lblStatus.ForeColor = Color.Green;
                    txtConsole.Text += Environment.NewLine + "Caspar server connected! " + Environment.NewLine;
                */
                }
                else
                {
                    //lblStatus.Text = "NOT CONNECTED";
                    //lblStatus.ForeColor = Color.Green;
                    txtConsole.Text += Environment.NewLine + "Unable to connect to caspar server! " + Environment.NewLine;
                }
            }
            catch (Exception)
            {
                txtConsole.Text += Environment.NewLine + "Unable to connect to caspar server, server refused! " + Environment.NewLine;
            }
        }



        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            try
            {
                var reader = new StreamReader(casparClient.GetStream());
                var writer = new StreamWriter(casparClient.GetStream());

                writer.WriteLine(txtCommand.Text);
                writer.Flush();

                var reply = reader.ReadLine();

                txtConsole.Text += reply + Environment.NewLine;

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    txtConsole.Text += reply + Environment.NewLine;
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        txtConsole.Text += reply + Environment.NewLine;
                    }
                }
            }
            catch (Exception)
            {
                txtConsole.Text += "Please make sure the connection is connected. " + Environment.NewLine;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "CG 1 INVOKE 1 Clear";
                var reader = new StreamReader(casparClient.GetStream());
                var writer = new StreamWriter(casparClient.GetStream());


                writer.WriteLine(command);
                writer.Flush();
                var reply = reader.ReadLine();

                txtConsole.Text += reply + Environment.NewLine;

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    txtConsole.Text += reply + Environment.NewLine;
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        txtConsole.Text += reply + Environment.NewLine;
                    }
                }
            }
            catch (Exception)
            {

                txtConsole.Text += "Please make sure the connection is connected. " + Environment.NewLine;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (preview.Controls.Count == 0 && preview.BackgroundImage == null)
            {

            }
            else
            {
                AnimateSend(1);
            }
        }

        private void AnimateSend(int channel)
        {
            try
            {
                var reader = new StreamReader(casparClient.GetStream());
                var writer = new StreamWriter(casparClient.GetStream());
                var backgroundcolor = HexConverter(txtChosenColor.BackColor);
                var command = "CG " + channel + " ADD 1 \"SIMPLE-LOWER-THIRD/SIMPLE-LOWER-THIRD\" 1 ";
                command += "\"";
                command += "{";

                if (preview.Controls.Count > 0)
                {
                    int n = 1;
                    foreach (var item in preview.Controls)
                    {
                        TextBox t = item as TextBox;
                        var name = "p" + n;
                        command += "\\\"pname" + n + "\\\":\\\"" + name + "\\\",";
                        command += "\\\"xpos" + n + "\\\":\\\"" + t.Location.X * 2 + "\\\",";
                        command += "\\\"ypos" + n + "\\\":\\\"" + t.Location.Y * 2 + "\\\",";
                        command += "\\\"width" + n + "\\\":\\\"" + t.Width * 2 + "\\\",";
                        command += "\\\"height" + n + "\\\":\\\"" + t.Height * 2 + "\\\",";
                        command += "\\\"text" + n + "\\\":\\\"" + t.Text + "\\\",";
                        n++;

                    }

                    command += "\\\"tbCount\\\":\\\"" + preview.Controls.Count.ToString() + "\\\",";
                }
                if (!string.IsNullOrEmpty(cbAnimation.Text) || cbAnimation.SelectedIndex > -1)
                {
                    command += "\\\"animation\\\":\\\"" + cbAnimation.SelectedItem.ToString() + "\\\",";
                }
                if (cbOutAnimation.SelectedItem != null)
                {
                    command += "\\\"outanimation\\\":\\\"" + cbOutAnimation.SelectedItem.ToString() + "\\\",";
                }
                if (tbFSize.Text != null)
                {
                    command += "\\\"font_size\\\":\\\"" + tbFSize.Text + "\\\",";
                }
                if (currentFilename != null)
                {
                    command += "\\\"background_img\\\":\\\"" + currentFilename + "\\\",";
                }
                if (mediapath != null)
                {
                    command += "\\\"mediapath\\\":\\\"" + mediapath + "\\\",";
                }
                if (comboBoxFonts.SelectedItem.ToString() != null)
                {
                    command += "\\\"font\\\":\\\"" + comboBoxFonts.SelectedItem.ToString() + "\\\",";
                }
                if (listBoxFontWeight.SelectedItem.ToString() != null)
                {
                    command += "\\\"font_weight\\\":\\\"" + listBoxFontWeight.SelectedItem.ToString() + "\\\",";
                }
                //if (backgroundcolor == null)                                                                // Sätter defaultfärg till svart
                //{
                //    command += "\\\"color\\\":\\\"black\\\"\\\"}";
                //}
                if (backgroundcolor != null)
                {
                    command += "\\\"color\\\":\\\"" + backgroundcolor + "\\\"}";
                }



                writer.WriteLine(command);
                writer.Flush();

                var reply = reader.ReadLine();

                txtConsole.Text += reply + Environment.NewLine;

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    txtConsole.Text += reply + Environment.NewLine;
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        txtConsole.Text += reply + Environment.NewLine;
                    }
                }
            }
            catch (Exception)
            {
                txtConsole.Text += "Please make sure the connection is connected. " + Environment.NewLine;
            }
        }
        private void btnAnimateOut_Click(object sender, EventArgs e)
        {
            AnimateOut(1);
        }

        private void AnimateOut(int channel)
        {
            try
            {
                string command = "";

                var reader = new StreamReader(casparClient.GetStream());
                var writer = new StreamWriter(casparClient.GetStream());

                if (cbOutAnimation.Text == "Out left")
                {
                    command = "CG " + channel + " INVOKE 1 OutLeft";
                }
                if (cbOutAnimation.Text == "Down")
                {
                    command = "CG " + channel + " INVOKE 1 Down";
                }
                if (cbOutAnimation.Text == "Shrink")
                {
                    command = "CG " + channel + " INVOKE 1 Shrink";
                }
                if (cbOutAnimation.Text == "Fade out")
                {
                    command = "CG " + channel + " INVOKE 1 FadeOut";
                }
                writer.WriteLine(command);
                writer.Flush();
                var reply = reader.ReadLine();

                txtConsole.Text += reply + Environment.NewLine;

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    txtConsole.Text += reply + Environment.NewLine;
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        txtConsole.Text += reply + Environment.NewLine;
                    }
                }
            }
            catch (Exception)
            {

                txtConsole.Text += "Please make sure the connection is connected. " + Environment.NewLine;
            }
        }

        private string RectangleToText(TextBox item)
        {
            string returner = "";

            returner += "\\\"xpos\\\":\\\"" + item.Location.X * 2 + "\\\",";
            returner += "\\\"ypos\\\":\\\"" + item.Location.Y * 2 + "\\\",";
            returner += "\\\"width\\\":\\\"" + item.Width * 2 + "\\\",";
            returner += "\\\"height\\\":\\\"" + item.Height * 2 + "\\\",";

            return returner;
        }

        #region Draw Rectangle
        private Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
        }
        private void preview_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                currentPos = startPos = e.Location;
                drawing = true;
            }
        }

        private void preview_MouseMove(object sender, MouseEventArgs e)
        {
            currentPos = e.Location;
            if (drawing) preview.Invalidate();

        }

        private void preview_Paint(object sender, PaintEventArgs e)
        {
            if (drawing)
            {
                e.Graphics.DrawRectangle(Pens.Red, getRectangle());
            }
        }
        private void preview_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                drawing = false;
                var rect = getRectangle();
                TextBox testTb = new TextBox();
                var upperLeftX = rect.X;
                var width = rect.Width;
                var height = rect.Height;
                testTb.Location = new Point(rect.X, rect.Y);
                testTb.Width = width;
                testTb.Height = height;
                testTb.Enabled = true;
                testTb.Multiline = true;
                if (comboBoxFonts.SelectedItem != null && tbFSize.Text != "")
                {
                    Font font = new Font(comboBoxFonts.SelectedItem.ToString(), int.Parse(tbFSize.Text));
                    testTb.Font = font;
                }
                preview.Controls.Add(testTb);
                testTb.MouseDown += new MouseEventHandler(MoveTB);
                testTb.MouseMove += new MouseEventHandler(DragTB);
                testTb.MouseUp += new MouseEventHandler(DropTB);
                CreateTextBoxAni(textBoxes.Count, rect, testTb);
                Debug.WriteLine(textBoxes.Count); 
                preview.Invalidate();
            }

        }

        private void CreateTextBoxAni(int index, Rectangle rec, TextBox tb)
        {
            TextBoxAnimation textBoxAni = new TextBoxAnimation();
            textBoxAni.rec = rec;
            textBoxAni.index = index;
            textBoxAni.tb = tb;
            textBoxes.Add(textBoxAni); 
        }

        void MarkTB(object sender, PaintEventArgs e)
        {
            var tb = sender as Control;

            if (tb.Name == "") //Markerar en TB genom att rita en rektabgel inne i den och sätta namn till Marked.
            {
                tb.Name = "Marked";
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Indigo, 2, ButtonBorderStyle.Solid, Color.Indigo, 2, ButtonBorderStyle.Solid, Color.Indigo, 2, ButtonBorderStyle.Solid, Color.Indigo, 2, ButtonBorderStyle.Solid);
            }
            else // Ritar vitt över den ritade rektangeln och sätter namn till "".
            {
                tb.Name = "";
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.White, 2, ButtonBorderStyle.Solid, Color.White, 2, ButtonBorderStyle.Solid, Color.White, 2, ButtonBorderStyle.Solid, Color.White, 2, ButtonBorderStyle.Solid);
            }

        }
        #region MoveTextBoxes
        void MoveTB(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//Högerklick raderar textboxen.
            {
                var tb = sender as TextBox;
                foreach (var item in preview.Controls)
                {
                    if (item == tb)
                    {
                        tb.Dispose();

                    }
                }
            }
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                Control ctrl = (Control)sender;
                var rect = ctrl.ClientRectangle;
                System.Drawing.Graphics grafs = ctrl.CreateGraphics();
                var g = new PaintEventArgs(grafs, rect);
                MarkTB(ctrl, g);
            }
            else
            {
                activeControl = sender as Control;
                previousLocation = e.Location;
                Cursor = Cursors.Hand;
            }

        }
        void DragTB(object sender, MouseEventArgs e)
        {
            if (activeControl == null || activeControl != sender)
            {
                return;
            }
            else
            {
                var location = activeControl.Location;
                location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
                activeControl.Location = location;
            }
        }
        void DropTB(object sender, MouseEventArgs e)
        {
            activeControl = null;
            Cursor = Cursors.Default;
        }

        #endregion

        private void btnReset_Click(object sender, EventArgs e)
        {
            preview.BackgroundImage = null;
            myRectangles.Clear();
            preview.Invalidate();
            preview.BringToFront();

            preview.Controls.Clear();

        }
        private void btnRemoveTextboxes_Click(object sender, EventArgs e)
        {
            preview.BackgroundImage = null;
            myRectangles.Clear();
            preview.Invalidate();
            preview.BringToFront();

            preview.Controls.Clear();
            if (currentImgPath != null)
            {
                var pic2 = Image.FromFile(currentImgPath.ToString());
                preview.BackgroundImage = pic2;
            }

        }
        #endregion
        #region Text and color
        void TxtConsoleTextChanged(object sender, EventArgs e)
        {
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.ScrollToCaret();
        }

        private void FillComboBoxWithFonts()
        {
            List<string> fonts = new List<string>();
            int selected = 0;
            foreach (FontFamily font in FontFamily.Families)
            {

                fonts.Add(font.Name);
                if (font.Name == "Arial")
                {
                    selected = fonts.Count();
                }

            }
            comboBoxFonts.DataSource = fonts;
            this.DoubleBuffered = true;
            comboBoxFonts.SelectedIndex = selected;
        }

        private void SelectedFont(object sender, EventArgs e)
        {
            Font font = new Font(comboBoxFonts.SelectedItem.ToString(), 12);
            comboBoxFonts.Font = font;
        }
        #endregion
        private void btnChoosePicture_Click(object sender, EventArgs e) // Välj bild till "preview"-boxen
        {
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "All Files | *.*|Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp|Video files(*.mov;*.avi;*.wmv;*.mts;)|*.mov;*.avi; *wmv;*.mts";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box 
                var pic = open.FileName;

                currentFilename = open.SafeFileName;
                if (pic.ToLower().EndsWith(".mov"))
                {
                    var ffMpeg = new NReco.VideoInfo.FFProbe();
                    var info = ffMpeg.GetMediaInfo(pic);
                    double totalSeconds = info.Duration.TotalSeconds;
                    float fRate = GetFramerate(info);
                    var frameCount = GetFramecount(totalSeconds, fRate); // Räknar ut antalet frames dynamiskt men avrundar uppåt. TODO fixa det. Sen ar tanken att lägga till en label som visar vilken frame man är på när man trycker på pause. Sedan kan vi göra en 1920/1080 (/2? lika stor som "preview"
                                                                         // och sätta som bakgrund i preview och preview.BringToFront() på det så kanske vi löser problemet med att det inte går att rita texter på en Windows Media Player.
                                                                         // När vi har en label som visar currentFrame vid pause så kan vi få till genom att "pausa" två gånger. Kanske spara värdet. 
                                                                         //PLAY 1-1 IMG_2652 SEEK 1000  ---- Spelar ett klipp från Frame 1000 till slutet
                                                                         //PLAY 1 - 1 IMG_2652 LENGTH 300---- Spelar ett klipp från Frame 1 till Frame 300
                                                                         //Alltså tyyyp svaret på om en .MOV innehåller upp och ner aniamtioner
                    lblSecondCount.Text = "0 / " + totalSeconds;
                    lblFrameCount.Text = "1 / " + frameCount;
                    preview.SendToBack();
                    axWindowsMediaPlayer1.Visible = true;
                    axWindowsMediaPlayer1.URL = pic;
                    btnPlayPauseVideo.Text = "Pause";
                    prBar1.Maximum = Convert.ToInt32(totalSeconds); 
                    
                }
                else
                {
                    var pic2 = Image.FromFile(pic.ToString());
                    preview.BackgroundImage = pic2;
                    currentImgPath = open.FileName;
                }

            }
        }
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 2)//Playstate 2= PAUSE
            {
                var currentVideo = axWindowsMediaPlayer1.currentMedia.sourceURL;
                var infoffMpeg = new NReco.VideoInfo.FFProbe();
                var info = infoffMpeg.GetMediaInfo(currentVideo);
                var totS = info.Duration.TotalSeconds;
                var frate = GetFramerate(info);
                var currentPos = Convert.ToSingle(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                if (!File.Exists(@"C:\Temp\TestCasparThumbs"))
                {
                    Directory.CreateDirectory((@"C:\Temp\TestCasparThumbs\")); 
                }
                if (File.Exists(@"C:\Temp\TestCasparThumbs\currentThumb.jpg")) // --------------------TODO: Lägg till kod som skapar mappen "CasparThumbs" om den INTE finns. DONE
                {
                    if (preview.BackgroundImage != null)
                    {
                        preview.BackgroundImage.Dispose();
                    }

                    File.Delete(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
                }
                ffMpeg.GetVideoThumbnail(currentVideo, @"C:\Temp\TestCasparThumbs\currentThumb.jpg", currentPos);
                preview.BackgroundImage = Image.FromFile(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
                preview.BringToFront();
                var frameCount = GetFramecount(totS, frate);
                var currentframe = GetCurrentFrame(frate, currentPos);
                lblSecondCount.Text = currentPos + " / " + totS;
                lblFrameCount.Text = currentframe + " / " + frameCount;
                

            }
            if (e.newState == 3)
            {
                axWindowsMediaPlayer1.BringToFront();
            }

        }
        private void btnPlayPauseVideo_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState.ToString() == "wmppsPlaying")
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                btnPlayPauseVideo.Text = "Play";
            }
            else if (axWindowsMediaPlayer1.playState.ToString() == "wmppsPaused" || axWindowsMediaPlayer1.playState.ToString() == "wmppsStopped")
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
                btnPlayPauseVideo.Text = "Pause";
            }

        }
        private void btnStopVideo_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            btnPlayPauseVideo.Text = "Play";
        }
        private void btnFrameback_Click(object sender, EventArgs e)
        {
            var currentVideo = axWindowsMediaPlayer1.currentMedia.sourceURL;
            var infoffMpeg = new NReco.VideoInfo.FFProbe();
            var info = infoffMpeg.GetMediaInfo(currentVideo);
            var totS = info.Duration.TotalSeconds;
            var frate = GetFramerate(info);
            var currentPos = Convert.ToSingle(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
            var currentframe = GetCurrentFrame(frate, currentPos);
            var frameCount = GetFramecount(totS, frate);
            lblFrameCount.Text = (currentframe -1)+ " / " + frameCount;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= (1 / frate);
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            if (!File.Exists(@"C:\Temp\TestCasparThumbs"))
            {
                Directory.CreateDirectory((@"C:\Temp\TestCasparThumbs\"));
            }
            if (File.Exists(@"C:\Temp\TestCasparThumbs\currentThumb.jpg")) // --------------------TODO: Lägg till kod som skapar mappen "CasparThumbs" om den INTE finns. DONE
            {
                if (preview.BackgroundImage != null)
                {
                    preview.BackgroundImage.Dispose();
                }

                File.Delete(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
            }
            ffMpeg.GetVideoThumbnail(currentVideo, @"C:\Temp\TestCasparThumbs\currentThumb.jpg", Convert.ToSingle(axWindowsMediaPlayer1.Ctlcontrols.currentPosition));
            preview.BackgroundImage = Image.FromFile(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
            preview.BringToFront();

        }

        private void btnFrameForward_Click(object sender, EventArgs e)
        {
            var currentVideo = axWindowsMediaPlayer1.currentMedia.sourceURL;
            var infoffMpeg = new NReco.VideoInfo.FFProbe();
            var info = infoffMpeg.GetMediaInfo(currentVideo);
            var totS = info.Duration.TotalSeconds;
            var frate = GetFramerate(info);
            var currentPos = Convert.ToSingle(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
            var currentframe = GetCurrentFrame(frate, currentPos);
            var frameCount = GetFramecount(totS, frate);
            lblFrameCount.Text = (currentframe + 1) + " / " + frameCount;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition += (1 / frate);
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            if (!File.Exists(@"C:\Temp\TestCasparThumbs"))
            {
                Directory.CreateDirectory((@"C:\Temp\TestCasparThumbs\"));
            }
            if (File.Exists(@"C:\Temp\TestCasparThumbs\currentThumb.jpg")) // --------------------TODO: Lägg till kod som skapar mappen "CasparThumbs" om den INTE finns.
            {
                if (preview.BackgroundImage != null)
                {
                    preview.BackgroundImage.Dispose();
                }

                File.Delete(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
            }
            ffMpeg.GetVideoThumbnail(currentVideo, @"C:\Temp\TestCasparThumbs\currentThumb.jpg", Convert.ToSingle(axWindowsMediaPlayer1.Ctlcontrols.currentPosition));
            preview.BackgroundImage = Image.FromFile(@"C:\Temp\TestCasparThumbs\currentThumb.jpg");
            preview.BringToFront();
        }

        public int GetFramecount(double totalSeconds, float frameRate)
        {
            double s = totalSeconds * frameRate;            
            int fCount = 0;
            fCount = Convert.ToInt32(totalSeconds * frameRate);
            int b = s.CompareTo(fCount);
            if (b == -1)
            {
                return fCount - 1; 
            }
            else
            {
                return fCount;
            }
                    
        }
        public float GetFramerate(NReco.VideoInfo.MediaInfo info)
        {
            float fRate = 0;
            for (int i = 0; i < info.Streams.Length; i++)
            {
                if (info.Streams[i].FrameRate > -1)
                {
                    fRate = info.Streams[i].FrameRate;
                    break;
                }
            }
            return fRate;
        }

        public int GetCurrentFrame(float framerate, float currentPos)
        {

            var currFrame = Convert.ToInt32(currentPos * framerate);
            return currFrame;
        }
        private void btnPickcolor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            //MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            MyDialog.Color = txtChosenColor.ForeColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                var hex = HexConverter(MyDialog.Color);
                var rgb = RGBConverter(MyDialog.Color);
                txtChosenColor.BackColor = MyDialog.Color;
                ChangeTextBoxFont();
            }

        }
        private void ChangeTextBoxFont()
        {
            foreach (var item in preview.Controls)
            {
                var t = item as TextBox;
                t.ForeColor = txtChosenColor.BackColor;
            }
        }
        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private static String RGBConverter(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            casparClient.Close();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //capture up arrow key
            if (keyData == Keys.Up)
            {
                MoveFocusedTesxtBoxes("up");
                return true;
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                MoveFocusedTesxtBoxes("down");
                return true;
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                MoveFocusedTesxtBoxes("left");
                return true;
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                MoveFocusedTesxtBoxes("right");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveFocusedTesxtBoxes(string v)
        {
            foreach (var item in preview.Controls)
            {
                var t = item as TextBox;
                if (t.Name == "Marked")                                 // Flyytar bara TBs som heter Marked.
                {
                    switch (v)
                    {
                        case "up":
                            t.Location = new Point(t.Location.X, t.Location.Y - 1);
                            break;
                        case "down":
                            t.Location = new Point(t.Location.X, t.Location.Y + 1);
                            break;
                        case "left":
                            t.Location = new Point(t.Location.X - 1, t.Location.Y);
                            break;
                        case "right":
                            t.Location = new Point(t.Location.X + 1, t.Location.Y);
                            break;
                    }
                }
            }
        }

        private void previewBtnSend_Click(object sender, EventArgs e)
        {
            AnimateSend(2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelsList[0].BringToFront();
            panel3.BringToFront();
            if (listFiles.Items.Count > 0)
            {
                listFiles.Items.Clear();
            }
        }

        private void btnPanel2_Click(object sender, EventArgs e)
        {
            if (listFiles.Items.Count > 0)
            {
                listFiles.Items.Clear();
            }

            panelsList[1].BringToFront();
            panel3.BringToFront();
            //GetFiles();

        }
        static Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 40;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        private void btnFilerSearcher_Click(object sender, EventArgs e) // Det här är Refresh Button
        {
            if (listFiles.Items.Count > 0)
            {
                listFiles.Items.Clear();
            }
            GetFiles();
        }

        public void GetFiles()
        {
            if (thumbnailList.Images.Count > 0)
            {
                thumbnailList.Images.Clear();
            }
            thumbnailList.ImageSize = new Size(87, 49);
            if (Directory.Exists(serverPathTB.Text + "\\media"))
            {
                string[] filePaths = Directory.GetFiles(serverPathTB.Text + "\\media");

                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (Path.GetExtension(filePaths[i]) == ".mov" || Path.GetExtension(filePaths[i]) == ".MOV" || Path.GetExtension(filePaths[i]) == ".mp4" || Path.GetExtension(filePaths[i]) == ".MP4")
                    {
                        thumbnailList.Images.Add(GetThumbnail(filePaths[i], i));
                        thumbnailList.Images.SetKeyName(i, Path.GetFileName(filePaths[i]));
                    }
                    else
                    {
                        Image image = Image.FromFile(filePaths[i]);
                        Size thumbnailSize = GetThumbnailSize(image);
                        Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);
                        thumbnailList.Images.Add(thumbnail);
                        thumbnailList.Images.SetKeyName(i, Path.GetFileName(filePaths[i]));
                    }

                    listFiles.Items.Add(Path.GetFileName(filePaths[i]), i);
                }
                thumbnailListRundown = thumbnailList;
                listFiles.SmallImageList = thumbnailList;
            }
            else
            {

            }

        }

        public Image GetThumbnail(string path, int i)
        {
            if (thumbnailPaths.Contains(i + "_" + "video_thumbnail.jpg"))
            {
                return Image.FromFile(i + "_" + "video_thumbnail.jpg");
            }
            else
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                ffMpeg.GetVideoThumbnail(path, (i + "_" + "video_thumbnail.jpg"), 1);
                thumbnailPaths.Add(i + "_" + "video_thumbnail.jpg");
                return Image.FromFile(i + "_" + "video_thumbnail.jpg");
            }
        }

        private void listFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<ListViewItem> lvi = new List<ListViewItem>();
            lvi.Add((ListViewItem)e.Item);
            foreach (ListViewItem item in listFiles.SelectedItems)
            {
                if (!lvi.Contains(item))
                {
                    lvi.Add(item);

                }
            }
            foreach (var item in lvi)
            {
                if (thumbnailList.Images.ContainsKey(item.Text))
                {
                    thumbnailListRundown.Images.Add(thumbnailList.Images[item.Text]);
                }
            }
            listFiles.DoDragDrop(lvi, DragDropEffects.Copy);
        }

        private void rundownList_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void rundownList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                var items = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                // move to dest LV
                foreach (ListViewItem lvi in items)
                {
                    // LVI obj can only belong to one LVI, remove
                    rundownList.Items.Add((ListViewItem)lvi.Clone());

                }
                thumbnailListRundown.ImageSize = new Size(87, 49);
                rundownList.SmallImageList = thumbnailListRundown;
            }

        }

        private void rundownList_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2 && listFiles.SelectedItems.Count == 1)
            {
                rundownList.Items.Add((ListViewItem)listFiles.SelectedItems[0].Clone());


                if (thumbnailList.Images.ContainsKey(listFiles.SelectedItems[0].Text))
                {
                    thumbnailListRundown.Images.Add(thumbnailList.Images[listFiles.SelectedItems[0].Text]);
                }
                thumbnailListRundown.ImageSize = new Size(87, 49);
                rundownList.SmallImageList = thumbnailListRundown;
            }
        }

        private void rundownList_MouseDown(object sender, MouseEventArgs e)
        {
            heldDownItem = rundownList.GetItemAt(e.X, e.Y);
            if (heldDownItem != null)
            {
                heldDownPoint = new Point(e.X - heldDownItem.Position.X, e.Y - heldDownItem.Position.Y);
            }
        }

        private void rundownList_MouseMove(object sender, MouseEventArgs e)
        {
            if (heldDownItem == null)
                return;

            if (heldDownItem != null)
            {
                heldDownItem.Position = new Point(e.Location.X - heldDownPoint.X,
                                                  e.Location.Y - heldDownPoint.Y);
            }
            Cursor = Cursors.Hand;
        }

        private void rundownList_MouseUp(object sender, MouseEventArgs e)
        {
            if (heldDownItem == null)
                return;


            ListViewItem itemOver = rundownList.GetItemAt(0, e.Y);
            if (itemOver == null)
            {
                itemOver = rundownList.Items[rundownList.Items.Count - 1];
            }

            Rectangle rc = itemOver.GetBounds(ItemBoundsPortion.Entire);

            // find out if we insert before or after the item the mouse is over
            bool insertBefore;
            if (e.Y < rc.Top + (rc.Height / 2))
                insertBefore = true;
            else
                insertBefore = false;

            if (heldDownItem != itemOver)
            // if we dropped the item on itself, nothing is to be done
            {
                if (insertBefore)
                {
                    rundownList.Items.Remove(heldDownItem);
                    rundownList.Items.Insert(itemOver.Index, heldDownItem);
                }
                else
                {
                    rundownList.Items.Remove(heldDownItem);
                    rundownList.Items.Insert(itemOver.Index + 1, heldDownItem);
                }

                Cursor = Cursors.Default;
            }
        }

        private void rundownList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back && rundownList.SelectedItems.Count != 0)
            {
                for (int i = 0; i < rundownList.SelectedItems.Count; i++)
                {
                    rundownList.Items.Remove(rundownList.SelectedItems[i]);
                }
            }
        }
        private void PlayMedia(string path)
        {
            if (casparClient.Connected)
            {
                try
                {
                    string command = "";
                    var reader = new StreamReader(casparClient.GetStream());
                    var writer = new StreamWriter(casparClient.GetStream());
                    command = "PLAY 1 " + path;

                    writer.WriteLine(command);
                    writer.Flush();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                label1.Text = "Connect to Caspar Server";
            }
        }

        private void playMediaBtn_Click(object sender, EventArgs e)
        {
            if (rundownList.SelectedItems.Count > 0)
            {
                if (rundownList.SelectedItems[0].Text != null)
                {
                    PlayMedia(rundownList.SelectedItems[0].Text);
                }
            }
        }

        private void rundownList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F1" && rundownList.SelectedItems.Count > 0)
            {
                PlayMedia(rundownList.SelectedItems[0].Text);
            }
            else if (e.KeyCode.ToString() == "F2")
            {
                StopMedia();
            }
        }

        private void stopMediaBtn_Click(object sender, EventArgs e)
        {
            StopMedia();
        }
        private void StopMedia()
        {
            if (casparClient.Connected)
            {
                try
                {
                    string command = "";
                    var reader = new StreamReader(casparClient.GetStream());
                    var writer = new StreamWriter(casparClient.GetStream());
                    command = "STOP 1";

                    writer.WriteLine(command);
                    writer.Flush();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                label1.Text = "Connect to Caspar Server";
            }
        }

        private void comboBoxFonts_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            if (int.TryParse(tbFSize.Text, out int s) && listBoxFontWeight.Text != null)
            {
                FontStyle style = new FontStyle();
                if (int.TryParse(listBoxFontWeight.Text, out int result))
                {
                    if (int.Parse(listBoxFontWeight.Text) > 400)
                    {
                        style = FontStyle.Bold;
                    }
                    else
                    {
                        style = FontStyle.Regular;
                    }
                }
                Font font = new Font(comboBoxFonts.SelectedItem.ToString(), int.Parse(tbFSize.Text), style);
                foreach (TextBox item in preview.Controls)
                {
                    item.Font = font;

                }
            }
            else
            {
                Font font = new Font(comboBoxFonts.SelectedItem.ToString(), 12);
                foreach (TextBox item in preview.Controls)
                {
                    item.Font = font;
                }
            }
        }

        private void tbFSize_TextChanged_1(object sender, EventArgs e)
        {
            comboBoxFonts_SelectedIndexChanged_1(sender, e);
        }

        private void listBoxFontWeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxFonts_SelectedIndexChanged_1(sender, e);
        }

        private void previewStopBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "CG 2 INVOKE 1 Clear";
                var reader = new StreamReader(casparClient.GetStream());
                var writer = new StreamWriter(casparClient.GetStream());


                writer.WriteLine(command);
                writer.Flush();
                var reply = reader.ReadLine();

                txtConsole.Text += reply + Environment.NewLine;

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    txtConsole.Text += reply + Environment.NewLine;
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        txtConsole.Text += reply + Environment.NewLine;
                    }
                }
            }
            catch (Exception)
            {

                txtConsole.Text += "Please make sure the connection is connected. " + Environment.NewLine;
            }
        }

        private void btnAnimateOutPreview_Click(object sender, EventArgs e)
        {
            AnimateOut(2);
        }

        private void serverPathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                serverPathTB.Text = folderDialog.SelectedPath;
                mediapath = ChangeSlashDirection(folderDialog.SelectedPath);
            }
        }

        private string ChangeSlashDirection(string selectedPath)
        {
            string newpath = "";
            foreach (var item in selectedPath)
            {
                if (item.ToString() == @"\")
                {
                    newpath += "/";
                }
                else
                {
                    newpath += item.ToString();
                }

            }
            return newpath + "/media/";
        }

        private void optionsPanelBtn_Click(object sender, EventArgs e)
        {
            panel2.SendToBack();
            panel3.BringToFront();
        }

        private void changeColorBtn_Click(object sender, EventArgs e)
        {
            ChangeColor("btn");
        }

        private void ChangeColorTextfield_Click(object sender, EventArgs e)
        {
            ChangeColor("textfield");
        }

        private void ChangeColor(string tag)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            //MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            MyDialog.Color = txtChosenColor.ForeColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                var hex = HexConverter(MyDialog.Color);
                var rgb = RGBConverter(MyDialog.Color);

            }
            foreach (Panel panel in panelsList)
            {
                foreach (Control btn in panel.Controls)
                {
                    if (btn.Tag != null && btn.Tag.Equals(tag))
                    {
                        (btn as Button).BackColor = MyDialog.Color;
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string path = serverPathTB.Text + "\\media\\" + rundownList.SelectedItems[0].Text;

            var ffMpeg = new NReco.VideoInfo.FFProbe();

            var info = ffMpeg.GetMediaInfo(path);

            GetTime(path, info);
        }

        private void GetTime(string path, NReco.VideoInfo.MediaInfo info)
        {
            int stream = 0;

            for (int i = 0; i < info.Streams.Length; i++)
            {
                if (info.Streams[i].FrameRate > 1)
                {
                    stream = i;
                }
            }

            label7.Text = info.Duration.ToString() + " Seconds:" + info.Duration.Seconds.ToString() + "Framerate: " + info.Streams[stream].FrameRate.ToString();

            label8.Text = ((float.Parse(info.Duration.Seconds.ToString()) + float.Parse(info.Duration.Minutes.ToString()) * 60) * float.Parse(info.Streams[stream].FrameRate.ToString())).ToString();

        }


    }
}
