using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VMware_VM_Generator
{
    // Custom icon display control with full alpha transparency and high-quality rendering support.
    public class CustomIconBox : Control
    {
        private Image iconImage;

        public CustomIconBox()
        {
            // Enable double buffering and transparent background support to prevent flickering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Size = new Size(32, 32);
        }

        public Image IconImage
        {
            get => iconImage;
            set
            {
                iconImage = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Set high-quality rendering modes for smooth image scaling and anti-aliasing
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Draw the image with full alpha channel support if assigned
            if (iconImage != null)
            {
                Rectangle rect = this.ClientRectangle;
                g.DrawImage(iconImage, rect);
            }
        }
    }
}