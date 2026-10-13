using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace VMware_VM_Generator
{
    // Chameleon-style CustomTrackBar control with fixed hover/focus state handling.
    public class CustomTrackBar : Control
    {
        private int minimum = 0;
        private int maximum = 1000;
        private int value = 0;

        private bool dragging = false;
        private bool hovered = false;

        // Short timer to keep Hot visual briefly after mouse release.
        private System.Windows.Forms.Timer hoverTimer;

        public CustomTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Height = 45;
            TabStop = true;

            hoverTimer = new System.Windows.Forms.Timer();
            hoverTimer.Interval = 220; // milliseconds
            hoverTimer.Tick += (s, e) =>
            {
                try
                {
                    hoverTimer.Stop();
                    // Only remain hovered if the mouse is actually over the control
                    hovered = ClientRectangle.Contains(PointToClient(Cursor.Position));
                    Invalidate();
                }
                catch { }
            };
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hovered = false;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { hoverTimer?.Dispose(); } catch { }
            }

            base.Dispose(disposing);
        }

        // Compatibility property for tick style
        [Browsable(true)]
        [DefaultValue(TickStyle.None)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TickStyle TickStyle { get; set; } = TickStyle.None;

        // Tick values array
        private int[] tickValues = Array.Empty<int>();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] TickValues
        {
            get => tickValues;
            set
            {
                tickValues = value ?? Array.Empty<int>();
                Invalidate();
            }
        }

        // Recommended markers
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecommendedMin { get; set; } = 0;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecommendedDefault { get; set; } = 0;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecommendedMax { get; set; } = 0;

        // Minimum value property
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
        {
            get => minimum;
            set
            {
                if (value >= maximum)
                    throw new ArgumentOutOfRangeException(nameof(Minimum));

                minimum = value;
                if (this.value < minimum)
                    Value = minimum;

                Invalidate();
            }
        }

        // Maximum value property
        [DefaultValue(1000)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get => maximum;
            set
            {
                if (value <= minimum)
                    throw new ArgumentOutOfRangeException(nameof(Maximum));

                maximum = value;
                if (this.value > maximum)
                    Value = maximum;

                Invalidate();
            }
        }

        // Current slider value property
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get => value;
            set
            {
                int newVal = Math.Max(minimum, Math.Min(maximum, value));
                if (newVal == this.value)
                    return;

                this.value = newVal;
                Invalidate();
                OnScroll(EventArgs.Empty);
            }
        }

        public event EventHandler Scroll;

        protected virtual void OnScroll(EventArgs e)
        {
            Scroll?.Invoke(this, e);
        }

        // Windows Style toggle property (kept for compatibility)
        private bool windowsStyle = false;

        [DefaultValue(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool WindowsStyle
        {
            get => windowsStyle;
            set
            {
                if (windowsStyle == value) return;
                windowsStyle = value;
                Invalidate();
            }
        }

        private int TrackPadding => 14;

        private Rectangle ThumbRectFromValue(int val)
        {
            int left = TrackPadding;
            int right = Math.Max(TrackPadding + 2, Width - TrackPadding);
            int centerY = Height / 2;
            double t = (maximum == minimum) ? 0.0 : (val - minimum) / (double)(maximum - minimum);
            int thumbX = left + (int)((right - left) * t);

            // Chameleon-styled thumb size
            int size = 16;
            return new Rectangle(thumbX - size / 2, centerY - size / 2, size, size);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        // Helper method to sample parent background color dynamically (Chameleon effect)
        private Color GetEffectiveParentBackColor()
        {
            Control p = this.Parent;
            while (p != null)
            {
                if (p.BackColor != Color.Transparent)
                    return p.BackColor;
                p = p.Parent;
            }
            return SystemColors.Control;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int left = TrackPadding;
            int right = Math.Max(TrackPadding + 2, Width - TrackPadding);
            int centerY = Height / 2;
            int trackHeight = 6;

            Rectangle trackRect = new Rectangle(left, centerY - trackHeight / 2, right - left, trackHeight);
            Rectangle thumbRect = ThumbRectFromValue(this.value);
            int thumbX = thumbRect.Left + thumbRect.Width / 2;

            // Chameleon color adaptation based on parent background luminance
            Color parentBg = GetEffectiveParentBackColor();
            bool isDarkTheme = (parentBg.R * 0.299 + parentBg.G * 0.587 + parentBg.B * 0.114) < 128;

            Color trackColor = isDarkTheme ? ControlPaint.Light(parentBg, 0.2f) : ControlPaint.Dark(parentBg, 0.2f);
            Color activeColor = SystemColors.Highlight;

            // 1. Draw background track adapted to parent environment
            using (var trackPath = GetRoundedRectPath(trackRect, trackHeight / 2))
            {
                using (var bgBrush = new SolidBrush(trackColor))
                {
                    g.FillPath(bgBrush, trackPath);
                }
            }

            // 2. Draw active filled track portion using system highlight color
            if (thumbX > left)
            {
                Rectangle filledRect = new Rectangle(left, centerY - trackHeight / 2, thumbX - left, trackHeight);
                using (var filledPath = GetRoundedRectPath(filledRect, trackHeight / 2))
                {
                    using (var fillBrush = new SolidBrush(activeColor))
                    {
                        g.FillPath(fillBrush, filledPath);
                    }
                }
            }

            // 3. Determine thumb visual state strictly based on mouse interaction (hovered/dragging)
            TrackBarThumbState state;
            if (dragging) state = TrackBarThumbState.Pressed;
            else if (hovered) state = TrackBarThumbState.Hot; // Removed this.Focused dependency so it only triggers on actual mouse hover
            else state = TrackBarThumbState.Normal;

            Color thumbFill;
            Color thumbBorder;

            switch (state)
            {
                case TrackBarThumbState.Pressed:
                    thumbFill = trackColor;
                    thumbBorder = activeColor;
                    break;
                case TrackBarThumbState.Hot:
                    thumbFill = isDarkTheme ? ControlPaint.Light(parentBg, 0.4f) : Color.White;
                    thumbBorder = activeColor;
                    break;
                default:
                    thumbFill = parentBg;
                    thumbBorder = trackColor;
                    break;
            }

            // Draw chameleon thumb matching surrounding skin tones
            using (var thumbBrush = new SolidBrush(thumbFill))
            using (var borderPen = new Pen(thumbBorder, 1.5f))
            {
                g.FillEllipse(thumbBrush, thumbRect);
                g.DrawEllipse(borderPen, thumbRect);
            }

            // Inner core detail dot adapting to highlight/border state
            using (var coreBrush = new SolidBrush(activeColor))
            {
                g.FillEllipse(coreBrush, thumbX - 2, centerY - 2, 4, 4);
            }
        }

        // Helper method to create rounded rectangle path for pill shapes
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Converts mouse X coordinate to slider value
        private int PositionToValue(int x)
        {
            int left = TrackPadding;
            int right = Math.Max(TrackPadding + 2, Width - TrackPadding);

            if (x <= left) return minimum;
            if (x >= right) return maximum;

            double frac = (x - left) / (double)(right - left);
            return minimum + (int)Math.Round(frac * (maximum - minimum));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                Focus();
                dragging = true;
                Capture = true;
                Value = PositionToValue(e.X);

                try
                {
                    Rectangle r = ThumbRectFromValue(this.value);
                    r.Inflate(4, 4);
                    Invalidate(r);
                }
                catch { Invalidate(); }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (dragging)
                Value = PositionToValue(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (dragging && e.Button == MouseButtons.Left)
            {
                dragging = false;
                Capture = false;
                Value = PositionToValue(e.X);

                try
                {
                    hovered = ClientRectangle.Contains(PointToClient(Cursor.Position));
                    Focus();
                    Rectangle r = ThumbRectFromValue(this.value);
                    r.Inflate(4, 4);
                    Invalidate(r);

                    hoverTimer?.Stop();
                    hoverTimer?.Start();
                }
                catch { Invalidate(); }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Right ||
                keyData == Keys.Home || keyData == Keys.End ||
                keyData == Keys.PageUp || keyData == Keys.PageDown)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            int span = Math.Max(1, (Maximum - Minimum) / 100);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    Value = Math.Max(Minimum, Value - span);
                    break;

                case Keys.Right:
                    Value = Math.Min(Maximum, Value + span);
                    break;

                case Keys.Home:
                    Value = Minimum;
                    break;

                case Keys.End:
                    Value = Maximum;
                    break;

                case Keys.PageUp:
                    Value = Math.Min(Maximum, Value + span * 5);
                    break;

                case Keys.PageDown:
                    Value = Math.Max(Minimum, Value - span * 5);
                    break;
            }
        }
    }
}