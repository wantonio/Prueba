using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Odotograma
{
    public partial class diente : UserControl
    {
        private System.Drawing.Drawing2D.GraphicsPath _pathData;
        private int _activeIndex = -1;
        private ArrayList _pathsArray;
        //private ToolTip _toolTip;
        private Graphics g;

        public Color [] colores_partes { get; set; }

        public delegate void RegionClickDelegate(int index, string key);
        [Category("Action")]
        public event RegionClickDelegate RegionClick;

        public diente()
        {
            InitializeComponent();

            colores_partes = new Color[5];
            colores_partes[0] = colores_partes[1] = colores_partes[2] = colores_partes[3] = colores_partes[4] = Color.White;

            this._pathsArray = new ArrayList();
            this._pathData = new System.Drawing.Drawing2D.GraphicsPath();
            this._pathData.FillMode = System.Drawing.Drawing2D.FillMode.Winding;

            Dibujar_Diente();

            Annadir_Circulo("Centro", 11.5f, 11.5f, 17f, 17f);
            Annadir_Pie("Superior", 0, 0, 40, 40, 225, 90);
            Annadir_Pie("Derecha", 0, 0, 40, 40, -45, 90);
            Annadir_Pie("Inferior", 0, 0, 40, 40, 45, 90);
            Annadir_Pie("Izquierda", 0, 0, 40, 40, 135, 90);
        }

        public void Dibujar_Diente()
        {
            Bitmap area_de_dibujo = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(area_de_dibujo);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.FillPie(new SolidBrush(colores_partes[1]), 0, 0, 40, 40, 225, 90);
            g.FillPie(new SolidBrush(colores_partes[2]), 0, 0, 40, 40, -45, 90);
            g.FillPie(new SolidBrush(colores_partes[3]), 0, 0, 40, 40, 45, 90);
            g.FillPie(new SolidBrush(colores_partes[4]), 0, 0, 40, 40, 135, 90);

            Pen p = new Pen(Color.Black);
            
            g.DrawPie(p, 0, 0, 40, 40, 225, 90);
            g.DrawPie(p, 0, 0, 40, 40, -45, 90);
            g.DrawPie(p, 0, 0, 40, 40, 45, 90);
            g.DrawPie(p, 0, 0, 40, 40, 135, 90);

            g.FillEllipse(new SolidBrush(colores_partes[0]), 11.5f, 11.5f, 17f, 17f);
            g.DrawEllipse(Pens.Black, 11.5f, 11.5f, 17.0f, 17.0f);

            
            pictureBox.Image = area_de_dibujo;
        }

        private int Annadir_Pie(string key, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (this._pathsArray.Count > 0)
                this._pathData.SetMarkers();
            this._pathData.AddPie(x, y, width, height, startAngle, sweepAngle);

            return this._pathsArray.Add(key);
        }

        private int Annadir_Circulo(string key, float x, float y, float width, float height)
        {
			if(this._pathsArray.Count > 0)
				this._pathData.SetMarkers();
			this._pathData.AddEllipse(x, y, width, height);
			return this._pathsArray.Add(key);
        }

        private int getActiveIndexAtPoint(Point point)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Drawing2D.GraphicsPathIterator iterator = new System.Drawing.Drawing2D.GraphicsPathIterator(_pathData);
            iterator.Rewind();
            for (int current = 0; current < iterator.SubpathCount; current++)
            {
                iterator.NextMarker(path);
                if (path.IsVisible(point, this.g))
                    return current;
            }
            return -1;
        }

        private void _MouseMove(object sender, MouseEventArgs e)
        {
            int newIndex = this.getActiveIndexAtPoint(new Point(e.X, e.Y));
            if (newIndex > -1)
            {
                this.Cursor = Cursors.Hand;
                //if (this._activeIndex != newIndex)
                //    this._toolTip.SetToolTip(this.pictureBox, this._pathsArray[newIndex].ToString());
            }
            else
            {
                this.Cursor = Cursors.Default;
                //this._toolTip.RemoveAll();
            }
            this._activeIndex = newIndex;
        }

        private void _MouseLeave(object sender, EventArgs e)
        {
            this._activeIndex = -1;
            this.Cursor = Cursors.Default;
        }

        private void _MouseClick(object sender, MouseEventArgs e)
        {
            Point p = this.PointToClient(Cursor.Position);
            if (this._activeIndex == -1)
                this.getActiveIndexAtPoint(p);
            if (this._activeIndex > -1 && this.RegionClick != null)
                this.RegionClick(this._activeIndex, this._pathsArray[this._activeIndex].ToString());
        }
    }
}
