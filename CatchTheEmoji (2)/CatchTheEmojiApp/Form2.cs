using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatchTheEmojiApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            initialWidth = Width;
            initialHeight = Height;
            pictureBoxes.Add(pictureBox1);
            pictureBoxes.Add(pictureBox2);
            pictureBoxes.Add(pictureBox3);
        }

        // Wellcome, UserName!
        public string UserName
        {
            get
            {
                return labelName.Text;
            }
            set
            {
                labelName.Text = value;
            }
        }



        private Random random = new Random();

        const int clicksPerLevel = 10;          // the number of the caught emojis for current level
        private int score = 0;                  // total number of the caught emojis for all levels in the current game

        private int defaultTime = 0;            // base time for choosen difficulty
        private int maxBonusTime = 0;           // max bonus time for choosen difficulty
        private int currentLevelTime = 0;       // default + random bonus time for current level
        private int timeTicks = 0;              // time ticks since current level start
        private int level = 0;                  // level from 0 to maxlevel
        private const int maxLevel = 4;
        private int pictureIndex = 0;           // random picture index
        private int pictureSize;                // picture size for current level
        private int initialWidth;               // initial form width for level 0, initialized in constructor
        private int initialHeight;              // initial form height for level 0, initialized in constructor
        private int initialPictureSize  = 100;  // initial picture size for level 0

        private List<PictureBox> pictureBoxes = new List<PictureBox>(); // all 3 pictures on the form

        private void gameStart()
        {
            // disable game settings controls
            buttonStart.Enabled = groupBoxDifficulty.Enabled = false;

            // hide game over label
            labelGameOver.Hide();

            // set initial score and level
            changeScore(0);
            changeLevel(0);

            // set random initial picture
            pictureIndex = random.Next(0, pictureBoxes.Count - 1);

            // move selected picture to random position
            movePicture();

            // start timer
            timer1.Start();
        }

        private void gameOver()
        {
            // stop timer
            timer1.Stop();

            // hide all pictures
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();

            // show score
            labelGameOver.Text = $"Game Over\nScore: {score}";
            labelGameOver.Show();

            // enable game settings controls
            buttonStart.Enabled = groupBoxDifficulty.Enabled = true;
        }

        private void changeLevel(int value)
        {
            // set level not greater then maxLevel
            level = Math.Min(value, maxLevel);

            // calculate bonus time and time for current level
            var bonusTime = random.Next(maxBonusTime + 1);
            currentLevelTime = defaultTime + bonusTime;

            // increase form size for level 5 to 150% of initial value for level 0
            this.Width = Convert.ToInt32((double)initialWidth * (1.0 + 0.5 * level / maxLevel));
            this.Height = Convert.ToInt32((double)initialHeight * (1.0 + 0.5 * level / maxLevel));

            // decrease picture size for level 5 to 70% of initial value for level 0
            pictureSize = Convert.ToInt32((double)initialPictureSize * (1.0 - 0.3 * level / maxLevel));

            // choose random picture
            changePicture();

            timeTicks = 0;
        }

        private void changePicture()
        {
            pictureBoxes[pictureIndex].Hide();

            // calculate next random picture - the next or next after next - to guaranteed change picture for each new level
            pictureIndex = (pictureIndex + 1 + random.Next(pictureBoxes.Count - 1)) % pictureBoxes.Count;
        }

        private void movePicture() // move current picture to random position on the form
        {
            pictureBoxes[pictureIndex].Hide();
            pictureBoxes[pictureIndex].Width = pictureSize;
            pictureBoxes[pictureIndex].Height = pictureSize;
            pictureBoxes[pictureIndex].Left = random.Next(panelGame.Width - pictureSize);
            pictureBoxes[pictureIndex].Top = random.Next(panelGame.Height - pictureSize);
            pictureBoxes[pictureIndex].Show();
        }

        private void changeScore(int value)
        {
            score = value;
            labelScore.Text = value.ToString();
        }

        private bool changeDifficulty()
        {
            if (radioButtonEasy.Checked)
            {
                defaultTime = 12;
                maxBonusTime = 5;
            }
            else if (radioButtonMedium.Checked)
            {
                defaultTime = 10;
                maxBonusTime = 3;
            }
            else if (radioButtonHard.Checked)
            {
                defaultTime = 7;
                maxBonusTime = 1;
            }
            else
            {
                MessageBox.Show("Choose difficulty");
                return false;
            }
            return true;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!changeDifficulty())
            {
                return;
            }
            gameStart();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            changeScore(score + 1);
            if (score % clicksPerLevel == 0)
            {
                changeLevel(level + 1);
            }
            movePicture();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            const double timeUnitsPerSecond = 1000; // timer interval is specified in milliseconds, 1000 milliseconds = 1 second
            var remainingTime = (currentLevelTime * timeUnitsPerSecond - timer1.Interval * (++timeTicks)) / timeUnitsPerSecond;
            labelTime.Text = remainingTime.ToString();
            if (remainingTime <= 0)
            {
                gameOver();
            }
        }
    }
}
