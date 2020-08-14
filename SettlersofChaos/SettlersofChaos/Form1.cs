﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SettlersofChaos
{
    public partial class FormGame : Form
    {
        private List<Hexagon> hexagons = new List<Hexagon>();
        private Artilltery[] redblocks = new Artilltery[20];
        public int speed = 10;
        string shellmove;
        int MouseX;
        int MouseY;
        public int PlayerOneDefense = 0;
        public int PlayerTwoDefense = 0;
        bool gamestart = false;
        public bool left, right;
        public int Difficulty = 12;
        ArtilleryShell artilleryShell = new ArtilleryShell();
        ArtilleryTarget artilleryTarget = new ArtilleryTarget();
        GameHotbar gamehotbar = new GameHotbar();
        PlayerOne plrone = new PlayerOne();
        PlayerTwo plrtwo = new PlayerTwo();
        Backsplash backsplash = new Backsplash();
        public FormGame()
        {
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, PnlBackSplash, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, PnlFight, new object[] { true });
            var random = new Random();
            var RedAxis = new Random();
            var shapeback = new Backsplash {
                BackRow = 1,
                BackColumn = 1,
                BackRadius = 50,
            };

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    if (j - i <= 2 && i - j <= 2)
                    {
                        var hexagon = new Hexagon
                        {
                            Row = i,
                            Column = j,
                            Radius = 50,
                        };
                        hexagons.Add(hexagon);
                    }
                }
            }
            for (int i = 0; i < 20; i++)
            {
                redblocks[i] = new Artilltery(random);
            }
        }

        private void PnlGame_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var size = PnlBackSplash.Size;
            var center = new PointF(size.Width / 2f, size.Height / 2f);
            if (gamestart == false)
            {
                foreach (var hexagon in hexagons)
                    hexagon.Draw(g, center);
            }
        }

        private void PnlFight_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            foreach (var block in redblocks)
            {
                block.Draw(g);
            }
            artilleryShell.DrawShell(g);
            artilleryTarget.Draw(g);

        }

        public void ArtilleryTicks_Tick(object sender, EventArgs e)
        {
            foreach (var block in redblocks)
            {
                block.RedPosX -= speed;
            }
            artilleryTarget.TargetPosX -= speed;
            PnlFight.Invalidate();
        }

        private void FormGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up) { left = true; }
            if (e.KeyData == Keys.Down) { right = true; }
        }

        private void TmrShellMove_Tick(object sender, EventArgs e)
        {
            if (CollidesWithRedBlock())
            {
                //reset planet[i] back to top of panel
                YouMissedLBL.Visible = true;
                TmrArtilleryTicks.Enabled = false;
                TmrShellMove.Enabled = false;
                ArtilleryGameExit();

            }
            if (CollidesWithTarget())
            {
                LblTargetHit.Visible = true;
                TmrArtilleryTicks.Enabled = false;
                TmrShellMove.Enabled = false;
                PlayerTwoDefense = PlayerTwoDefense -5;
                ArtilleryGameExit();

            }
            else
            {
                if (right) // if right arrow key pressed
                {
                    shellmove = "up";
                    artilleryShell.MoveShell(shellmove);
                }
                if (left) // if left arrow key pressed
                {
                    shellmove = "down";
                    artilleryShell.MoveShell(shellmove);
                }
            }
        }

        private bool CollidesWithRedBlock()
        {
            foreach (var block in redblocks)
            {
                if (artilleryShell.ShellRec.IntersectsWith(new Rectangle(block.Position, block.Size)))
                {
                    return true;
                }

            }
            return false;
        }

        private bool CollidesWithTarget()
        {
            if (artilleryShell.ShellRec.IntersectsWith(new Rectangle(artilleryTarget.TargetPosition, artilleryTarget.TargetSize)))
            {
                return true;
            }
            return false;
        }

        private void FormGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up) { left = false; }
            if (e.KeyData == Keys.Down) { right = false; }
        }

        public void ArtilleryGameTriggered()
        {
            PnlFight.Location = new Point(-3, 20);
            GameBoot();
            TmrArtilleryTicks.Enabled = true;
            TmrShellMove.Enabled = true;
            PnlFight.Visible = true;
            PlayerOneDefense = PlayerOneDefense - 3;
        }

        public void ArtilleryGameExit() {
            LblPlayerOne.Text = Convert.ToString(PlayerOneDefense);
            LblPlayerTwo.Text = Convert.ToString(PlayerTwoDefense);
            LblPlayerOne.Visible = true;
            LblPlayerTwo.Visible = true;
            BtnHelp.Visible = true;
            BtnExit.Visible = true;
            PnlFight.Visible = false;
            BtnArtillery.Visible = true;
            BtnShoot.Visible = true;
            BtnFortify.Visible = true;
        }

        private void PnlGame_MouseDown(object sender, MouseEventArgs e)
        {
            MouseX = Cursor.Position.X;
            MouseY = Cursor.Position.Y;

        }

        public void GameBoot()
        {
            LblTargetHit.Location = new Point(-3, 20);
            YouMissedLBL.Location = new Point(-3, 20);
        }

        private void PnlMenu_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var size = PnlMenu.Size;
            var center = new PointF(size.Width / 2f, size.Height / 2f);
        }

        public void HexagonSelect() {
            float[] distances = new float[hexagons.Count];
            foreach (var hexagon in hexagons)
            {
                //distances[i] = hexagon.getdistanceto(MouseX, MouseY);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gamestart();
        }

        private void PnlHome_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            gamehotbar.Draw(g);
            plrone.Draw(g);
            plrtwo.Draw(g);
        }

        private void BtnArtillery_Click(object sender, EventArgs e)
        {
            ArtilleryGameTriggered();
            PnlFight.Visible = true;
            BtnFortify.Visible = false;
            BtnHelp.Visible = false;
            BtnShoot.Visible = false;
            BtnArtillery.Visible = false;
            BtnExit.Visible = false;
            LblPlayerOne.Visible = false;
            LblPlayerTwo.Visible = false;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            GameEnd();
        }

        public void GameEnd()
        {
            gamestart = false;
            BtnSettings.Visible = true;
            BtnStart.Visible = true;
            PnlHome.Visible = false;
            BtnTutorial.Visible = true;
            LblTitle.Visible = true;

        }

        private void BtnShoot_Click(object sender, EventArgs e)
        {
            PlayerTwoDefense = -30;
            LblPlayerOne.Text = Convert.ToString(PlayerOneDefense);
            LblPlayerTwo.Text = Convert.ToString(PlayerTwoDefense);
        }

        public void Gamestart() {
            gamestart = true;
            BtnSettings.Visible = false;
            BtnStart.Visible = false;
            BtnTutorial.Visible = false;
            LblTitle.Visible = false;
            PnlBackSplash.Invalidate();
            PnlHome.Visible = true;
            PlayerOneDefense = Difficulty;
            PlayerTwoDefense = Difficulty;
            LblPlayerOne.Text = Convert.ToString(PlayerOneDefense);
            LblPlayerTwo.Text = Convert.ToString(PlayerTwoDefense);
           }



    }
}

