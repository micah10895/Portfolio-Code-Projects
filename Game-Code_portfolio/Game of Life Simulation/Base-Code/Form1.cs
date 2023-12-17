using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace McMillian_GOL_Final_Project
{
    public partial class Form1 : Form
    {
        #region fields
        Brush HUDbrush = new SolidBrush(Color.FromArgb(150, Color.Orange));
        // The universe array
        bool[,] universe = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];  // x = number of columns (ie width)
        //McMillian: Scratchpad array
        bool[,] scratch = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];   // y = number of rows (ie height)

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color x10color = Color.Black;

        int _seed = 2022;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        //McMillian: Living cells count
        int livecells = 0;
        //McMillian: Placeholder for the runto dialog box
        int runToTemp = 0;
        //McMillian: Another field related to the run to dialog box
        bool isRunning = false;

        ModalDialog seed = new ModalDialog();
        ModalDialog runto = new ModalDialog();
        //McMillian: options dialog box
        Universe_Editor _universe = new Universe_Editor();
        //McMillian: View options
        bool NeighborCount = true;
        bool showgrid = true;
        bool showHUD = true;
        bool isToroidal = true;
        string boundarytype_s = "Toroidal";
        string HUDinfo = "";
        //McMillian: Cell's neighbor count fields
        Font font = new Font("Arial", 10f);
        Font forHUD = new Font("Arial", 15f);
        StringFormat format = new StringFormat();

        #endregion
        public Form1()
        {
            InitializeComponent();
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            cellColor = Properties.Settings.Default.cellColor;
            gridColor = Properties.Settings.Default.gridColor;
            x10color = Properties.Settings.Default.x10color;
            // Setup the timer
            timer.Interval = Properties.Settings.Default.Timer; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            //McMillian: GOL rules logic
            #region GOL rule logic
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    if (isToroidal)
                    {
                        CountNeighborsToroidal(j, i, out int count);
                        if (universe[j, i] == false)
                        {
                            if (count == 3)
                            {
                                scratch[j, i] = true;
                            }
                            else
                            {
                                scratch[j, i] = false;
                            }
                        }
                        else
                        {
                            if (count < 2 || count > 3)
                            {
                                scratch[j, i] = false;
                            }
                            else
                            {
                                scratch[j, i] = true;
                            }
                        }
                    }
                    else
                    {
                        CountNeighborsFinite(j, i, out int count);
                        if (universe[j, i] == false)
                        {
                            if (count == 3)
                            {
                                scratch[j, i] = true;
                            }
                            else
                            {
                                scratch[j, i] = false;
                            }
                        }
                        else
                        {
                            if (count < 2 || count > 3)
                            {
                                scratch[j, i] = false;
                            }
                            else
                            {
                                scratch[j, i] = true;
                            }
                        }
                    }
                }
            }
            #endregion
            //McMillian: Array Swap
            bool[,] temp = universe;
            universe = scratch;
            scratch = temp;
            //McMillian: clearing the scratchpad for new input
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    scratch[j, i] = false;
                }
            }
            
            // Increment generation count
            generations++;
            Live();
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }
        //McMillian: count neighbors method (finite and toroidal)
        #region Count Neighbors (toroidal and finite)
        private void CountNeighborsToroidal(int x, int y, out int count)
        {
            count = 0;
            int xLen = universe.GetLength(0) - 1;
            int yLen = universe.GetLength(1) - 1;
            for (int i = -1; i <= 1; i++) //McMillian: error corrected i/j must be <= 1 to include 1
            {
                for (int j = -1; j <= 1; j++)
                {
                    int xcheck = x + j;
                    int ycheck = y + i;
                    if (xcheck > xLen)
                    {
                        xcheck = 0;
                    }
                    else if (xcheck < 0)
                    {
                        xcheck = xLen;
                    }
                    if (ycheck > yLen)
                    {
                        ycheck = 0;
                    }
                    else if (ycheck < 0)
                    {
                        ycheck = yLen;
                    }
                    //this if check is to ensure that the input cell is not counted as a neighbor itself
                    if (universe[xcheck, ycheck] == true)
                    {
                        if (j == 0 && i == 0)
                        {

                        }
                        else
                        {
                            count++;
                        }

                    }
                }
            }
        }
        private void CountNeighborsFinite(int x, int y, out int count)
        {
            count = 0;
            int xLen = universe.GetLength(0) - 1;
            int yLen = universe.GetLength(1) - 1;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int xcheck = x + j;
                    int ycheck = y + i;
                    if (xcheck > xLen)
                    {
                        continue;
                    }
                    else if (xcheck < 0)
                    {
                        continue;
                    }
                    if (ycheck > yLen)
                    {
                        continue;
                    }
                    else if (ycheck < 0)
                    {
                        continue;
                    }
                    //this if check is to ensure that the input cell is not counted as a neighbor itself
                    if (universe[xcheck, ycheck] == true)
                    {
                        if (j == 0 && i == 0)
                        {

                        }
                        else
                        {
                            count++;
                        }

                    }
                }
            }
        }
        #endregion
        //McMillian: Live cell count method
        private void Live()
        {
            livecells = 0;
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    if (universe[j, i] == true)
                    {
                        livecells = livecells + 1;
                    }
                    else
                    {

                    }

                }
            }
            Live_Cells.Text = "Live Cells = " + livecells.ToString();
        }
        #region Timer tick method
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            //McMillian: repaint every next generation
            graphicsPanel1.Invalidate();
            Live();
            //McMillian: this block of code will determine if the run to dialog was clicked and edited when the timer starts
            //McMillian: and once the generation count reaches the chosen number it wil stop the clock and reset the bool
            if (isRunning)
            {
                if (generations == runToTemp)
                {
                    timer.Enabled = false;
                    isRunning = false;
                }
            }
        }
        #endregion

        #region Panel paint and mouseclick events
        //McMillian: panel events
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            //McMillian: a pen to draw the x10 grid
            Pen grid10 = new Pen(x10color, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    if (showgrid) //McMillian: determines if the entire grid is showing
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    //McMillian: this is where the neighbor count for each cell will be printed in the center of the cell
                    if (NeighborCount)
                    {
                        if (isToroidal)
                        {
                            CountNeighborsToroidal(x, y, out int count);
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Center;
                            if (count == 3)
                            {
                                e.Graphics.DrawString(count.ToString(), font, Brushes.Green, cellRect, format);
                            }
                            else if (count == 2)
                            {
                                if (universe[x, y] == false)
                                {
                                    e.Graphics.DrawString(count.ToString(), font, Brushes.Green, cellRect, format);
                                }
                                else
                                {
                                    e.Graphics.DrawString(count.ToString(), font, Brushes.Red, cellRect, format);
                                }
                            }
                            else if (count == 0)
                            {
                                //McMillian: do nothing
                            }
                            else
                            {
                                e.Graphics.DrawString(count.ToString(), font, Brushes.Red, cellRect, format);
                            }
                        }
                        else
                        {
                            CountNeighborsFinite(x, y, out int count);
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Center;
                            if (count == 3)
                            {
                                e.Graphics.DrawString(count.ToString(), font, Brushes.Green, cellRect, format);
                            }
                            else if (count == 2)
                            {
                                if (universe[x, y] == false)
                                {
                                    e.Graphics.DrawString(count.ToString(), font, Brushes.Green, cellRect, format);
                                }
                                else
                                {
                                    e.Graphics.DrawString(count.ToString(), font, Brushes.Red, cellRect, format);
                                }
                            }
                            else if (count == 0)
                            {
                                //McMillian: do nothing
                            }
                            else
                            {
                                e.Graphics.DrawString(count.ToString(), font, Brushes.Red, cellRect, format);
                            }
                        } //McMillian: Finite
                    }

                }
            }
            //McMillian: this is where the HUD will be drawn
            if (showHUD)
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Far;
                if (isToroidal) //McMillian: this if check is to determine the boundary type
                {
                    HUDinfo = $"Generations: {generations.ToString()}\n" +
                        $"Cell Count: {livecells.ToString()}\n" +
                        $"Boundary Type: {boundarytype_s}\n" +
                        "Universe SizeL: {" +
                        $"Width={universe.GetLength(0)}, Height={universe.GetLength(1)}" +
                        "}";
                }
                else
                {
                    HUDinfo = $"Generations: {generations.ToString()}\n" +
                        $"Cell Count: {livecells.ToString()}\n" +
                        $"Boundary Type: {boundarytype_s}\n" +
                        "Universe SizeL: {" +
                        $"Width={universe.GetLength(0)}, Height={universe.GetLength(1)}" +
                        "}";
                }
                e.Graphics.DrawString(HUDinfo, forHUD, HUDbrush, graphicsPanel1.ClientRectangle, format);
            }
            //McMillian: Grid x10
            if (showgrid) //McMillian: determines if the entire grid is showing
            {
                for (int i = 0; i < universe.GetLength(0); i++)
                {
                    if (i % 10 == 0)
                    {
                        e.Graphics.DrawLine(grid10, (cellWidth * i), 0, (cellWidth * i), graphicsPanel1.ClientSize.Height);
                    }
                }
                for (int i = 0; i < universe.GetLength(1); i++)
                {
                    if (i % 10 == 0)
                    {
                        e.Graphics.DrawLine(grid10, 0, (cellHeight * i), graphicsPanel1.ClientSize.Width, (cellHeight * i));
                    }
                }
            }


            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            toolStripStatusInterval.Text = "Interval = " + timer.Interval;
            toolStripStatusSeed.Text = "Seed = " + seed.Number;
        }
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
                Live();
            }
            else if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, e.X, e.Y); //McMillian: context menu
            }
        }
        #endregion

        #region Play/Pause button (code for nextgen button will not appear here)
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        #endregion
        //New/Clear button
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    universe[j, i] = false;
                }
            }
            generations = 0;
            timer.Enabled = false;
            graphicsPanel1.Invalidate();
            Live();
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }
        #region view menu buttons
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showHUD)
            {
                showHUD = false;
                hUDToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else
            {
                showHUD = true;
                hUDToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showgrid)
            {
                showgrid = false;
                gridToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else
            {
                showgrid = true;
                gridToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NeighborCount)
            {
                NeighborCount = false;
                neighborCountToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else
            {
                NeighborCount = true;
                neighborCountToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isToroidal)
            {
                isToroidal = true;
                boundarytype_s = "Toroidal";
                finiteToolStripMenuItem.Checked = false;
                toroidalToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isToroidal)
            {
                isToroidal = false;
                boundarytype_s = "Finite";
                finiteToolStripMenuItem.Checked = true;
                toroidalToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
        }

        #endregion
        //McMillian: 'Run to' button
        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            runto.Text = "Run To Dialog";
            runto.dialogText = "Run To Generation";
            runto.RandomButton = false;
            runto.Number = generations + 1;
            runto.NumMinimum = generations;
            if (DialogResult.OK == runto.ShowDialog())
            {
                timer.Enabled = true;
                runToTemp = runto.Number;
                isRunning = true;
            }
        }

        #region color dialog boxes for background, cells, and the grid
        //McMillian: Background color dialog box (change background color)
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog BackGround = new ColorDialog();
            BackGround.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == BackGround.ShowDialog())
            {
                graphicsPanel1.BackColor = BackGround.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //McMillian: Cell color dialog box (change cell color)
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog GridColor = new ColorDialog();
            GridColor.Color = gridColor;
            if (DialogResult.OK == GridColor.ShowDialog())
            {
                gridColor = GridColor.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //McMillian: Grid color dialog box (change grid color)
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog CellColor = new ColorDialog();
            CellColor.Color = cellColor;
            if (DialogResult.OK == CellColor.ShowDialog())
            {
                cellColor = CellColor.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //McMillian: x10 Grid color dialog box (change x10 grid color)
        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog x10GridColor = new ColorDialog();
            x10GridColor.Color = x10color;
            if (DialogResult.OK == x10GridColor.ShowDialog())
            {
                x10color = x10GridColor.Color;
                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        #region seed code blocks
        //McMillian: randomize seed dialog box
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seed.Text = "Seed Dialog";
            seed.Number = _seed;
            if (DialogResult.OK == seed.ShowDialog())
            {
                _seed = seed.Number;
                graphicsPanel1.Invalidate();
                Random rand = new Random(_seed);
                for (int i = 0; i < universe.GetLength(1); i++)
                {
                    for (int j = 0; j < universe.GetLength(0); j++)
                    {
                        if (rand.Next() % 2 == 1)
                        {
                            universe[j, i] = true;
                        }
                        else
                        {
                            universe[j, i] = false;

                        }
                    }
                }
                graphicsPanel1.Invalidate();
                Live();
            }
        }
        //McMillian: randomize from current seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rand = new Random(_seed);
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    if (rand.Next() % 2 == 1)
                    {
                        universe[j, i] = true;
                    }
                    else
                    {
                        universe[j, i] = false;

                    }
                }
            }
            graphicsPanel1.Invalidate();
            Live();
        }
        //McMillian: randomize from time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random _rand = new Random();
            _seed = _rand.Next(int.MinValue, int.MaxValue);
            seed.Number = _seed;
            Random rand = new Random(_seed);
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    if (rand.Next() % 2 == 1)
                    {
                        universe[j, i] = true;
                    }
                    else
                    {
                        universe[j, i] = false;

                    }
                }
            }
            graphicsPanel1.Invalidate();
            Live();
        }

        #endregion

        //McMillian: Options Dialog box to edit the size of the universe and the time in milliseconds
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _universe.Text = "Options Dialog";
            _universe._TimeU = timer.Interval;
            _universe._HeightU = universe.GetLength(1);
            _universe._WidthU = universe.GetLength(0);
            if (DialogResult.OK == _universe.ShowDialog())
            {
                timer.Interval = _universe._TimeU;
                if ((_universe._HeightU != universe.GetLength(1)) || (_universe._WidthU != universe.GetLength(0)) )
                {
                    bool[,] Un2 = new bool[_universe._WidthU, _universe._HeightU];
                    bool[,] temp2 = universe;
                    universe = Un2;
                    Un2 = temp2;
                    bool[,] scratch2 = new bool[_universe._WidthU, _universe._HeightU];
                    bool[,] temp3 = scratch;
                    scratch = scratch2;
                    scratch2 = temp3;
                    generations = 0;
                    timer.Enabled = false;
                    graphicsPanel1.Invalidate();

                }
                graphicsPanel1.Invalidate();
            }
        }
        //McMillian: Settings saver created
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.cellColor = cellColor;
            Properties.Settings.Default.gridColor = gridColor;
            Properties.Settings.Default.Timer = timer.Interval;
            Properties.Settings.Default.Height = universe.GetLength(0);
            Properties.Settings.Default.Width = universe.GetLength(1);
            Properties.Settings.Default.x10color = x10color;
            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            cellColor = Properties.Settings.Default.cellColor;
            gridColor = Properties.Settings.Default.gridColor;
            timer.Interval = Properties.Settings.Default.Timer;
            x10color = Properties.Settings.Default.x10color;
            bool[,] newsize = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            bool[,] temp = universe;
            universe = newsize;
            newsize = temp;
            graphicsPanel1.Invalidate();

        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            cellColor = Properties.Settings.Default.cellColor;
            gridColor = Properties.Settings.Default.gridColor;
            timer.Interval = Properties.Settings.Default.Timer;
            x10color = Properties.Settings.Default.x10color;
            bool[,] newsize = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            bool[,] temp = universe;
            universe = newsize;
            newsize = temp;
            graphicsPanel1.Invalidate();
        }
        //McMillian: Open/Save files created
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savebox = new SaveFileDialog();
            savebox.Filter = "All Files|*.*|Cells|*.cells";
            savebox.FilterIndex = 2;
            savebox.DefaultExt = "cells";
            if (DialogResult.OK == savebox.ShowDialog())
            {
                StreamWriter save = new StreamWriter(savebox.FileName);
                save.WriteLine($"!App created by Micah L. McMillian {DateTime.Now}");
                for (int i = 0; i < universe.GetLength(1); i++)
                {
                    String currentrow = String.Empty;
                    for (int j = 0; j < universe.GetLength(0); j++)
                    {
                        if (universe[j, i] == true)
                        {
                            currentrow += "O";
                        }
                        else
                        {
                            currentrow += ".";
                        }
                    }
                    save.WriteLine(currentrow);
                }
                Live();
                save.WriteLine($"!Cell count = {livecells}");
                save.Close();
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openbox = new OpenFileDialog();
            openbox.Filter = "All Files|*.*|Cells|*.cells";
            openbox.FilterIndex = 2;
            if (DialogResult.OK == openbox.ShowDialog())
            {
                StreamReader open = new StreamReader(openbox.FileName);
                int width = 0;
                int height = 0;
                while (!open.EndOfStream)
                {
                    string row = open.ReadLine();
                    if (row[0] == '!')
                    {
                        continue;
                    }
                    else
                    {
                        height++;
                        width = row.Length;
                    }
                }
                universe = new bool[width, height];
                scratch = new bool[width, height];
                open.BaseStream.Seek(0, SeekOrigin.Begin);
                height = 0;
                while (!open.EndOfStream)
                {
                    string row = open.ReadLine();
                    if (row[0] == '!')
                    {
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < row.Length; i++)
                        {
                            if (row[i] == 'O')
                            {
                                universe[i, height] = true;
                            }
                            else if (row[i] == '.')
                            {
                                universe[i, height] = false;
                            }
                        }
                        height++;
                    }
                }
                graphicsPanel1.Invalidate();
                Live();
                open.Close();
            }
        }


    }
}
