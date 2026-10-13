using System;
using System.Drawing;
using System.Windows.Forms;

namespace VMware_VM_Generator
{
    public class CustomButton : Button
    {
        private bool isHovered = false;
        private bool isPressed = false;

        public CustomButton()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.FlatStyle = FlatStyle.Flat;
            this.UseVisualStyleBackColor = false;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.ForeColor = Color.FromArgb(30, 30, 30);
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);

            // Cursor remains standard default pointer as requested

            // Handle mouse events for clean modern visual states
            this.MouseEnter += (s, e) => { isHovered = true; Invalidate(); };
            this.MouseLeave += (s, e) => { isHovered = false; isPressed = false; Invalidate(); };
            this.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { isPressed = true; Invalidate(); } };
            this.MouseUp += (s, e) => { if (e.Button == MouseButtons.Left) { isPressed = false; Invalidate(); } };
            this.RightToLeftChanged += (s, e) => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            Rectangle rect = this.ClientRectangle;

            // Determine button colors based on state
            Color fillColor = this.BackColor;
            Color borderColor = this.FlatAppearance.BorderColor;

            if (!this.Enabled)
            {
                fillColor = Color.FromArgb(245, 245, 245);
                borderColor = Color.FromArgb(220, 220, 220);
            }
            else if (isPressed)
            {
                fillColor = Color.FromArgb(204, 228, 247); // Modern subtle blue accent on press
                borderColor = Color.FromArgb(0, 120, 215);
            }
            else if (isHovered)
            {
                fillColor = Color.FromArgb(229, 243, 251); // Modern subtle highlight on hover
                borderColor = Color.FromArgb(0, 120, 215);
            }

            // Draw clean flat background and border (no transparency artifacts)
            using (SolidBrush brush = new SolidBrush(fillColor))
            {
                g.FillRectangle(brush, rect);
            }
            using (Pen pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            // Always center the text inside the button regardless of LTR or RTL
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                    TextFormatFlags.VerticalCenter |
                                    TextFormatFlags.WordBreak |
                                    TextFormatFlags.EndEllipsis;

            Form parentForm = this.FindForm();
            bool isRtl = this.RightToLeft == RightToLeft.Yes ||
                         (parentForm != null && parentForm.RightToLeft == RightToLeft.Yes);

            if (isRtl)
            {
                flags |= TextFormatFlags.RightToLeft;
            }

            // Add a small padding so text doesn't touch the borders
            Rectangle textRect = rect;
            textRect.Inflate(-8, -4);

            Color textColor = this.Enabled ? this.ForeColor : SystemColors.GrayText;

            // Draw the text centrally aligned
            TextRenderer.DrawText(g, this.Text, this.Font, textRect, textColor, flags);
        }
    }
}