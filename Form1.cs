using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace SelectedNumbers
{
    public class ColorComp : IComparer<Color>
    {
        // Compares by Height, Length, and Width.
        public int Compare(Color x, Color y)
        {
            if (x.GetBrightness() > y.GetBrightness())
                return 1;
            else if (x.GetBrightness() < y.GetBrightness())
                return -1;
            else
                return 0;
        }
    }
    public partial class Form1 : Form
    {
        (int, int) prevOpenPos;
        (int, int) nowOpenPos;
        (int, int) forDelete;

        int completed = 0;
        int openingAttemps = 0;
        ImageList imageList;
        int[,] imagesPositionsArray;
        int[] numberArray;
        Color[] colorArray;
        bool gameStarted = false;
        int mode = 0;
        int difficulty = 4;
        Random rand;
        Button[,] btnArray;
        ProgressBar progress;
        ProgressBar progressMemory;
        ComboBox difficultyBox;
        ComboBox gameModeBox;
        Button newGameBtn;
        ListBox valuesHistory;
        Label historyLabel;
        Label difficultyLabel;
        Label gameModeLabel;
        GroupBox upDownBox;
        GroupBox upDownBoxMemory;
        Font boldFont;
        NumericUpDown gameDurationNumeric;
        NumericUpDown memorisingTime;
        System.Windows.Forms.Timer timer;
        System.Windows.Forms.Timer memorisingTimer;
        System.Windows.Forms.Timer chekTimer;

        public Form1()
        {
            InitializeComponent();
            CreateForm(4, 4);
            ClientSize = new Size(510, 300);
         
         
        }
        private void ClienSizeChanger()
        {
            switch (difficulty)
            {
                case 4:
                    if (mode == 2)
                    {
                        MemoryModeEstablishment();
                        ClientSize = new Size(510, 390);
                    }
                    else
                    {
                        CleanMemoryMode();
                        ClientSize = new Size(510, 300);
                    }
                    break;
                case 5:
                    if (mode == 2)
                    {
                        MemoryModeEstablishment();
                        ClientSize = new Size(570, 450);
                    }
                    else
                    {
                        CleanMemoryMode();
                        ClientSize = new Size(570, 360);
                    }
                    break;

                case 6:
                    if (mode == 2)
                    {
                        MemoryModeEstablishment();
                        ClientSize = new Size(630, 510);
                    }
                    else
                    {
                        CleanMemoryMode();
                        ClientSize = new Size(630, 420);
                    }
                    break;
            }
        }
        private void DifficultyValChanged(object sender, EventArgs e)
        {
            if (!gameStarted)
                switch (difficultyBox.SelectedIndex)
                {
                    case 0:
                        Controls.Clear();
                        CreateForm(6, 6);
                        difficulty = 6;

                        break;
                    case 1:
                        Controls.Clear();
                        CreateForm(5, 5);
                        difficulty = 5;

                        break;
                    case 2:
                        Controls.Clear();
                        CreateForm(4, 4);
                        difficulty = 4;

                        break;
                }
            gameModeBox.SelectedIndex = mode;
            ClienSizeChanger();
        }


        private void CleanMemoryMode()
        {
            Controls.Remove(progressMemory);
            Controls.Remove(upDownBoxMemory);
            Controls.Remove(memorisingTime);
        }
        private void MemoryModeEstablishment()
        {

            progressMemory.Bounds = new Rectangle(0, progress.Location.Y + 90, btnArray[0, 0].Size.Width * difficulty, 50);
            Controls.Add(progressMemory);
            Controls.Add(upDownBoxMemory);
        }


        private void ModesChanged(object sender, EventArgs e)
        {
            if (!gameStarted)
                switch (gameModeBox.SelectedIndex)
                {
                    case 0:
                        gameModeBox.Text = "NUMBERS";
                        mode = 0;
                        break;
                    case 1:
                        gameModeBox.Text = "COLORS";
                        mode = 1;
                        break;
                    case 2:
                        gameModeBox.Text = "MEMORY";
                        MemoryModeEstablishment();
                        mode = 2;
                        break;
                }
            ClienSizeChanger();
        }
        private void StartButtonClicked(object sender, EventArgs e)
        {
            if (gameDurationNumeric.Value == 0 || gameStarted)
                return;
            if ((string)difficultyBox.SelectedItem == "MEDIUM" && mode == 2)
            {
                MessageBox.Show("Unfortunately such difficulty is unavailable for this mode", "RESTRICTION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            switch (mode)
            {
                case 0:
                    {
                        numberArray = new int[difficulty * difficulty];
                        int tmp = 0;
                        for (int i = 0; i < difficulty * difficulty; i++)
                        {
                            do
                            {
                                tmp = rand.Next(0, 150);
                            } while (numberArray.Contains(tmp));
                            numberArray[i] = tmp;
                            btnArray[i / difficulty, i % difficulty].Text = numberArray[i].ToString();
                        }
                        Array.Sort(numberArray);
                        timer.Start();
                    }
                    break;
                case 1:
                    {
                        colorArray = new Color[difficulty * difficulty];
                        Color tmp = Color.Empty;
                        for (int i = 0; i < difficulty * difficulty; i++)
                        {
                            do
                            {
                                int r = rand.Next(256);
                                int g = rand.Next(256);
                                int b = rand.Next(256);
                                tmp = Color.FromArgb(r, g, b);
                            } while (colorArray.Contains(tmp));
                            colorArray[i] = tmp;
                            btnArray[i / difficulty, i % difficulty].BackColor = colorArray[i];
                            btnArray[i / difficulty, i % difficulty].Text = $"{colorArray[i].GetBrightness().ToString("N1")}";

                        }
                        Array.Sort(colorArray, new ColorComp());
                        MessageBox.Show("On each button you will see number.\nThis number is a beggining of a full brightness coefficient for each color.\nChoose colors from lowest brightness to the highest.\nGood luck!", "TIPS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        timer.Start();
                    }
                    break;
                case 2:
                    {
                        imageList = new ImageList();
                        imageList.ImageSize = new Size(60, 60);
                        for (int i = 0; i < ((difficulty * difficulty) / 2); i++)
                        {
                            imageList.Images.Add(Image.FromFile($"{(i + 1)}.jpg"));
                        }

                        imagesPositionsArray = new int[difficulty, difficulty];
                        int[] usedIndex = new int[((difficulty * difficulty) / 2)];
                        usedIndex = usedIndex.Select((a) => a = -1).ToArray();
                        int ind = 0;
                        for (int i = 0, attemp = 0; i < ((difficulty * difficulty) / 2); i++)
                        {
                            attemp = 0;
                            do
                            {
                                ind = rand.Next(0, ((difficulty * difficulty) / 2));
                            } while (usedIndex.Contains(ind));
                            usedIndex[i] = ind;
                            do
                            {
                                int row = rand.Next(0, difficulty);
                                int column = rand.Next(0, difficulty);
                                if (btnArray[row, column].Image == null)
                                {
                                    btnArray[row, column].Image = imageList.Images[ind];
                                    imagesPositionsArray[row, column] = ind;
                                    attemp++;
                                }
                            } while (attemp != 2);
                        }

                        memorisingTimer = new System.Windows.Forms.Timer();
                        memorisingTimer.Interval = 1000;
                        memorisingTimer.Tick += (s, ee) =>
                        {
                            progressMemory.Value += 1;
                            if (progressMemory.Value == progressMemory.Maximum)
                            {
                                memorisingTimer.Stop();
                                for (int i = 0; i < difficulty * difficulty; i++)
                                {
                                    btnArray[i / difficulty, i % difficulty].Image = null;
                                    btnArray[i / difficulty, i % difficulty].Enabled = true;
                                }
                                timer.Start();
                            }
                        };
                        memorisingTime.Enabled = false;
                        gameDurationNumeric.Enabled = false;
                        memorisingTimer.Start();
                        chekTimer.Start();
                    }
                    break;
            }
            gameDurationNumeric.Enabled = false;
            gameStarted = true;
            gameModeBox.Enabled = false;
            difficultyBox.Enabled = false;
        }
        private void SelectedItemChangedDifficulty(object sender, EventArgs e)
        {
            try
            {
                difficultyBox.Text = difficultyBox.SelectedItem.ToString();
            }
            catch (Exception g)
            {

            }
        }
        private void ButtonWereClicked(object sender, EventArgs e)
        {
            if (!gameStarted)
                return;
            if (gameModeBox.SelectedIndex == 0)
                for (int i = 0; i < difficulty * difficulty; i++)
                {
                    if (btnArray[i / difficulty, i % difficulty].ContainsFocus)
                        if (btnArray[i / difficulty, i % difficulty].Text == numberArray[0].ToString())
                        {
                            btnArray[i / difficulty, i % difficulty].Enabled = false;
                            valuesHistory.Items.Add(numberArray[0]);
                            if (numberArray.Length == 1)
                            {
                                timer.Stop();
                                DialogResult dR = MessageBox.Show("VICTORY\nPRESS OK TO TRY AGAIN", "YOU WON", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                RecreateAfterVorL(dR);
                                return;
                            }
                            int[] tmpArr = numberArray;
                            numberArray = new int[numberArray.Length - 1];
                            Array.Copy(tmpArr, 1, numberArray, 0, numberArray.Length);
                            return;
                        }
                }
            else
            if (gameModeBox.SelectedIndex == 1)
            {
                for (int i = 0; i < difficulty * difficulty; i++)
                {
                    if (btnArray[i / difficulty, i % difficulty].ContainsFocus)
                        if (btnArray[i / difficulty, i % difficulty].BackColor.GetBrightness() == colorArray[0].GetBrightness())
                        {
                            btnArray[i / difficulty, i % difficulty].Enabled = false;
                            btnArray[i / difficulty, i % difficulty].BackColor = Color.Empty;
                            valuesHistory.Items.Add(colorArray[0].Name);
                            if (colorArray.Length == 1)
                            {
                                timer.Stop();
                                DialogResult dR = MessageBox.Show("VICTORY\nPRESS OK TO TRY AGAIN", "YOU WON", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                RecreateAfterVorL(dR);
                                return;
                            }
                            Color[] tmpArr = colorArray;
                            colorArray = new Color[colorArray.Length - 1];
                            Array.Copy(tmpArr, 1, colorArray, 0, colorArray.Length);
                            return;
                        }
                }
            }
            else if (gameModeBox.SelectedIndex == 2)
            {
                openingAttemps++;
                for (int i = 0; i < difficulty * difficulty; i++)
                {
                    if (btnArray[i / difficulty, i % difficulty].ContainsFocus)
                    {
                        if (openingAttemps % 2 != 0)
                            forDelete = (i / difficulty, i % difficulty);

                        nowOpenPos = (i / difficulty, i % difficulty);
                        btnArray[i / difficulty, i % difficulty].Image = imageList.Images[imagesPositionsArray[i / difficulty, i % difficulty]];
                    }
                }

                if (openingAttemps == 2)
                {
                    if ((imagesPositionsArray[nowOpenPos.Item1, nowOpenPos.Item2] == imagesPositionsArray[prevOpenPos.Item1, prevOpenPos.Item2]) && !(nowOpenPos.Item1 == prevOpenPos.Item1 && nowOpenPos.Item2 == prevOpenPos.Item2))
                    {
                        btnArray[nowOpenPos.Item1, nowOpenPos.Item2].Enabled = false;
                        btnArray[prevOpenPos.Item1, prevOpenPos.Item2].Enabled = false;
                        completed++;

                    }
                }
                if (completed == ((difficulty * difficulty) / 2))
                {
                    completed = 0;
                    openingAttemps = 0;
                    timer.Stop();
                    chekTimer.Stop();
                    DialogResult dR = MessageBox.Show("VICTORY\nPRESS OK TO TRY AGAIN", "YOU WON", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    RecreateAfterVorL(dR);
                    return;
                }

                prevOpenPos = nowOpenPos;
            }
        }


        private void RecreateAfterVorL(DialogResult dR)
        {
            if (dR == DialogResult.Cancel)
                Close();
            else
            {
                timer.Stop();
                switch (difficulty)
                {
                    case 6:
                        Controls.Clear();
                        CreateForm(6, 6);
                        difficulty = 6;
                        difficultyBox.SelectedIndex = 0;
                        break;

                    case 5:
                        Controls.Clear();
                        CreateForm(5, 5);
                        difficulty = 5;
                        difficultyBox.SelectedIndex = 1;
                        break;
                    case 4:
                        Controls.Clear();
                        CreateForm(4, 4);
                        difficulty = 4;
                        difficultyBox.SelectedIndex = 2;
                        break;
                }
                ClienSizeChanger();
                gameModeBox.SelectedIndex = mode;
                gameStarted = false;
            }
        }
        private void CreateForm(int rows, int columns)
        {
            CenterToScreen();
            rand = new Random();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                progress.Value += 1;
                if (progress.Value == progress.Maximum)
                {
                    timer.Stop();
                    DialogResult dR = MessageBox.Show("TIME OUT\nPRESS OK TO TRY AGAIN", "YOU LOST", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    RecreateAfterVorL(dR);
                }
            };

            chekTimer = new System.Windows.Forms.Timer();
            chekTimer.Interval = 250;
            chekTimer.Tick += (s, e) =>
            {
                if (openingAttemps == 2)
                {
                    if ((imagesPositionsArray[nowOpenPos.Item1, nowOpenPos.Item2] != imagesPositionsArray[forDelete.Item1, forDelete.Item2]) || (nowOpenPos.Item1 == forDelete.Item1 && nowOpenPos.Item2 == forDelete.Item2))
                    {
                        btnArray[nowOpenPos.Item1, nowOpenPos.Item2].Image = null;
                        btnArray[forDelete.Item1, forDelete.Item2].Image = null;
                    }
                    openingAttemps = 0;
                }

            };


            boldFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
            btnArray = new Button[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    btnArray[i, j] = new Button();
                    btnArray[i, j].Size = new Size(60, 60);
                    btnArray[i, j].Location = new Point(j * 60, i * 60);
                    btnArray[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    btnArray[i, j].Click += ButtonWereClicked;
                    btnArray[i, j].Font = boldFont;
                    Controls.Add(btnArray[i, j]);
                }
            }
            progress = new ProgressBar();
            progress.Bounds = new Rectangle(0, btnArray[rows - 1, columns - 1].Location.Y + 65, btnArray[0, 0].Size.Width * rows, 50);
            progress.Style = ProgressBarStyle.Continuous;
            progress.Maximum = 60;
            Controls.Add(progress);

            historyLabel = new Label();
            historyLabel.Bounds = new Rectangle(btnArray[rows - 1, columns - 1].Location.X + 65, 0, 130, 25);
            historyLabel.BorderStyle = BorderStyle.Fixed3D;
            historyLabel.Font = boldFont;
            historyLabel.Text = "Chosen Values";
            historyLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(historyLabel);

            valuesHistory = new ListBox();
            valuesHistory.HorizontalScrollbar = true;
            valuesHistory.Bounds = new Rectangle(btnArray[rows - 1, columns - 1].Location.X + 65, historyLabel.Location.Y + historyLabel.Size.Height, 130, 120);
            valuesHistory.Font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Italic);
            Controls.Add(valuesHistory);

            difficultyLabel = new Label();
            difficultyLabel.Bounds = new Rectangle(valuesHistory.Location.X, valuesHistory.Location.Y + valuesHistory.Size.Height, 130, 25);
            difficultyLabel.BorderStyle = BorderStyle.Fixed3D;
            difficultyLabel.Font = boldFont;
            difficultyLabel.Text = "Difficulties";
            difficultyLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(difficultyLabel);

            difficultyBox = new ComboBox();
            difficultyBox.Items.AddRange(new string[] { "HARD", "MEDIUM", "EASY" });
            switch (rows)
            {
                case 4:
                    difficultyBox.Text = "EASY";
                    break;
                case 5:
                    difficultyBox.Text = "MEDIUM";
                    break;
                case 6:
                    difficultyBox.Text = "HARD";
                    break;
            }
            difficultyBox.SelectedIndexChanged += SelectedItemChangedDifficulty;
            difficultyBox.Font = boldFont;
            difficultyBox.Bounds = new Rectangle(valuesHistory.Location.X, difficultyLabel.Location.Y + difficultyLabel.Size.Height, 130, 30);
            Controls.Add(difficultyBox);


            gameModeLabel = new Label();
            gameModeLabel.Bounds = new Rectangle(valuesHistory.Location.X + difficultyBox.Size.Width, valuesHistory.Location.Y + valuesHistory.Size.Height, 130, 25);
            gameModeLabel.BorderStyle = BorderStyle.Fixed3D;
            gameModeLabel.Font = boldFont;

            gameModeLabel.Text = "Game Modes";
            gameModeLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(gameModeLabel);

            gameModeBox = new ComboBox();
            switch (mode)
            {
                case 0:
                    gameModeBox.Text = "NUMBERS";
                    break;
                case 1:
                    gameModeBox.Text = "COLORS";
                    break;
                case 2:
                    gameModeBox.Text = "MEMORY";
                    break;
            }
            gameModeBox.Items.AddRange(new string[] { "NUMBERS", "COLORS", "MEMORY" });
            gameModeBox.Font = boldFont;
            gameModeBox.Bounds = new Rectangle(valuesHistory.Location.X + difficultyBox.Size.Width, gameModeLabel.Location.Y + gameModeLabel.Size.Height, 130, 30);
            Controls.Add(gameModeBox);

            gameDurationNumeric = new NumericUpDown();
            gameDurationNumeric.Font = boldFont;
            gameDurationNumeric.Bounds = new Rectangle(12, 20, 60, 40);
            gameDurationNumeric.ValueChanged += (s, e) => { progress.Maximum = (int)gameDurationNumeric.Value; };

            upDownBox = new GroupBox();
            upDownBox.Font = boldFont;
            upDownBox.Text = "Duration";
            upDownBox.Bounds = new Rectangle(progress.Location.X + progress.Size.Width, progress.Location.Y - 8, difficultyBox.Size.Width + 4, 60);
            upDownBox.Controls.Add(gameDurationNumeric);
            Controls.Add(upDownBox);

            newGameBtn = new Button();
            newGameBtn.Font = boldFont;
            newGameBtn.Text = "New Game";
            newGameBtn.Bounds = new Rectangle(upDownBox.Location.X + upDownBox.Size.Width, progress.Location.Y, difficultyBox.Size.Width + 4, 50);
            newGameBtn.Click += StartButtonClicked;
            Controls.Add(newGameBtn);

            progressMemory = new ProgressBar();
            progressMemory.Style = ProgressBarStyle.Continuous;
            progressMemory.Minimum = 0;
            progressMemory.Maximum = 5;

            memorisingTime = new NumericUpDown();
            memorisingTime.Font = boldFont;
            memorisingTime.Bounds = new Rectangle(gameDurationNumeric.Location.X, 20, 60, 40);
            memorisingTime.ValueChanged += (s, ee) => { progressMemory.Maximum = (int)memorisingTime.Value; };
            memorisingTime.Value = 5;

            upDownBoxMemory = new GroupBox();
            upDownBoxMemory.Font = boldFont;
            upDownBoxMemory.Text = "Memorising Time";
            upDownBoxMemory.Bounds = new Rectangle(upDownBox.Location.X, progress.Location.Y + 90 - 8, upDownBox.Size.Width * 2, 60);
            upDownBoxMemory.Controls.Add(memorisingTime);

            difficultyBox.SelectedIndexChanged += DifficultyValChanged;
            gameModeBox.SelectedIndexChanged += ModesChanged;
            gameModeBox.SelectedIndex = 0;
        }

    }
}
