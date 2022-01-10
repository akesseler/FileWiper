/*
 * Copyright (C)  2013  Axel Kesseler
 * 
 * This software is free and you can use it for any purpose. Furthermore, 
 * you are free to copy, to modify and/or to redistribute this software.
 * 
 * In addition, this software is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace plexdata.Controls
{
    public class ProgressBar3D : Control
    {
        // Below find the link to an owner-drawn progress bar example 
        // which can be taken as source for inspiration: 
        // http://www.codeproject.com/Articles/19309/Vista-Style-Progress-Bar-in-C

        #region Public property change event declaration section.

        [Category("Progress")]
        public event EventHandler<EventArgs> ValueChanged;
        [Category("Progress")]
        public event EventHandler<EventArgs> StepsChanged;
        [Category("Progress")]
        public event EventHandler<EventArgs> MaximumChanged;
        [Category("Progress")]
        public event EventHandler<EventArgs> DigitsChanged;
        [Category("Appearance")]
        public event EventHandler<EventArgs> TextEnabledChanged;
        [Category("Appearance")]
        public event EventHandler<EventArgs> NullTextEnabledChanged;
        [Category("Appearance")]
        public event EventHandler<EventArgs> VerticalChanged;
        [Category("Appearance")]
        public event EventHandler<EventArgs> TextVerticalChanged;
        [Category("Appearance")]
        public event EventHandler<EventArgs> PercentageChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> ForeColorDarkChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> ForeColorLightChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> BackColorDarkChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> BackColorLightChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> TextColorDarkChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> TextColorLightChanged;
        [Category("Appearance.Colors")]
        public event EventHandler<EventArgs> BorderColorChanged;

        #endregion //Public property change event declaration section.

        private StringFormat horzFormat;
        private StringFormat vertFormat;

        public ProgressBar3D()
            : base()
        {
            // It's too easy if you know how...
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;

            // Setup private values.
            this.horzFormat = new StringFormat();
            this.horzFormat.Alignment = StringAlignment.Center;
            this.horzFormat.LineAlignment = StringAlignment.Center;

            this.vertFormat = new StringFormat(StringFormatFlags.DirectionVertical);
            this.vertFormat.Alignment = StringAlignment.Center;
            this.vertFormat.LineAlignment = StringAlignment.Center;

            // Setup Progress properties.
            this.Value = 33D;
            this.Steps = 1D;
            this.Maximum = 100D;
            this.Digits = 0;

            // Setup Appearance properties.
            this.TextEnabled = true;
            this.NullTextEnabled = true;
            this.Vertical = false;
            this.TextVertical = false;
            this.Percentage = true;

            // Setup Appearance.Colors properties.
            this.ForeColorDark = Color.DarkBlue;
            this.ForeColorLight = Color.LightBlue;
            this.BackColorDark = SystemColors.ControlDark;
            this.BackColorLight = SystemColors.ControlLight;
            this.TextColorDark = Color.DarkBlue;
            this.TextColorLight = Color.White;
            this.BorderColor = SystemColors.ControlDarkDark;
            this.RedirectHitTest = false;
        }

        #region Overwritten property implementation section.

        public override Rectangle DisplayRectangle
        {
            get
            {
                // Bounds size is equal to client area.
                // client area - padding is equal to display area.
                // But anyway the client area is moved by one pixel
                // and x and y is not correct...
                return new Rectangle(
                    this.ClientRectangle.X - 1 + this.Padding.Left,
                    this.ClientRectangle.Y - 1 + this.Padding.Top,
                    this.ClientRectangle.Width + 1 - this.Padding.Horizontal,
                    this.ClientRectangle.Height + 1 - this.Padding.Vertical
                );
            }
        }

        [DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [DefaultValue("")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                if (this.TextEnabled)
                {
                    if (!this.NullTextEnabled && this.Value == 0)
                    {
                        return String.Empty;
                    }

                    if (this.Percentage)
                    {
                        // Avoid division by zero...
                        double divisor = this.Maximum != 0 ? this.Maximum : 1;
                        double result = Math.Round(
                            (this.Value * 100 / divisor),
                            this.Digits, MidpointRounding.AwayFromZero);

                        return result.ToString() + "%";
                    }
                    else
                    {
                        string format = "F" + this.Digits.ToString();
                        return this.Value.ToString(format, NumberFormatInfo.CurrentInfo);
                    }
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                // Not supported yet.
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(120, 21);
            }
        }

        #endregion // Overwritten property implementation section.

        #region Public property implementation section.

        private double value = 33D;
        [DefaultValue(33D)]
        [Category("Progress")]
        [RefreshProperties(RefreshProperties.All)]
        public double Value
        {
            get { return this.value; }
            set
            {
                if (value < 0) { value = 0; }
                if (value > this.Maximum && this.Percentage) { value = this.Maximum; }

                if (this.value != value)
                {
                    this.value = value;
                    this.Invalidate();

                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private double steps = 1D;
        [DefaultValue(1D)]
        [Category("Progress")]
        [RefreshProperties(RefreshProperties.All)]
        public double Steps
        {
            get { return this.steps; }
            set
            {
                if (this.steps != value)
                {
                    this.steps = value;
                    this.Invalidate();
                    if (this.StepsChanged != null)
                    {
                        this.StepsChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private double maximum = 100D;
        [DefaultValue(100D)]
        [Category("Progress")]
        [RefreshProperties(RefreshProperties.All)]
        public double Maximum
        {
            get { return this.maximum; }
            set
            {
                if (value < 0) { value = 0; }
                if (value < this.Value) { value = this.Value; }
                if (this.maximum != value)
                {
                    this.maximum = value;
                    this.Invalidate();
                    if (this.MaximumChanged != null)
                    {
                        this.MaximumChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private int digits = 0;
        [DefaultValue(0)]
        [Category("Progress")]
        [RefreshProperties(RefreshProperties.All)]
        public int Digits
        {
            get { return this.digits; }
            set
            {
                // Math.Round() causes an ArgumentOutOfRangeException if digits 
                // is less than 0 or greater than 15. See Text property!
                value = Math.Max(Math.Min(value, 15), 0);
                if (this.digits != value)
                {
                    this.digits = value;
                    this.Invalidate();
                    if (this.DigitsChanged != null)
                    {
                        this.DigitsChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool textEnabled;
        [DefaultValue(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool TextEnabled
        {
            get { return this.textEnabled; }
            set
            {
                if (this.textEnabled != value)
                {
                    this.textEnabled = value;
                    this.Invalidate();
                    if (this.TextEnabledChanged != null)
                    {
                        this.TextEnabledChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool nullTextEnabled;
        [DefaultValue(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool NullTextEnabled
        {
            get { return this.nullTextEnabled; }
            set
            {
                if (this.nullTextEnabled != value)
                {
                    this.nullTextEnabled = value;
                    this.Invalidate();
                    if (this.NullTextEnabledChanged != null)
                    {
                        this.NullTextEnabledChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool vertical;
        [DefaultValue(false)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool Vertical
        {
            get { return this.vertical; }
            set
            {
                if (this.vertical != value)
                {
                    this.vertical = value;
                    this.Invalidate();
                    if (this.VerticalChanged != null)
                    {
                        this.VerticalChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool textVertical;
        [DefaultValue(false)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool TextVertical
        {
            get { return this.textVertical; }
            set
            {
                if (this.textVertical != value)
                {
                    this.textVertical = value;
                    this.Invalidate();
                    if (this.TextVerticalChanged != null)
                    {
                        this.TextVerticalChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool percentage;
        [DefaultValue(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public bool Percentage
        {
            get { return this.percentage; }
            set
            {
                if (this.percentage != value)
                {
                    this.percentage = value;
                    this.Invalidate();
                    if (this.PercentageChanged != null)
                    {
                        this.PercentageChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color foreColorDark;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        [RefreshProperties(RefreshProperties.All)]
        public Color ForeColorDark
        {
            get { return this.foreColorDark; }
            set
            {
                if (this.foreColorDark != value)
                {
                    this.foreColorDark = value;
                    this.Invalidate();
                    if (this.ForeColorDarkChanged != null)
                    {
                        this.ForeColorDarkChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color foreColorLight;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "LightBlue")]
        [RefreshProperties(RefreshProperties.All)]
        public Color ForeColorLight
        {
            get { return this.foreColorLight; }
            set
            {
                if (this.foreColorLight != value)
                {
                    this.foreColorLight = value;
                    this.Invalidate();
                    if (this.ForeColorLightChanged != null)
                    {
                        this.ForeColorLightChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color backColorDark;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "ControlDark")]
        [RefreshProperties(RefreshProperties.All)]
        public Color BackColorDark
        {
            get { return this.backColorDark; }
            set
            {
                if (this.backColorDark != value)
                {
                    this.backColorDark = value;
                    this.Invalidate();
                    if (this.BackColorDarkChanged != null)
                    {
                        this.BackColorDarkChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color backColorLight;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "ControlLight")]
        [RefreshProperties(RefreshProperties.All)]
        public Color BackColorLight
        {
            get { return this.backColorLight; }
            set
            {
                if (this.backColorLight != value)
                {
                    this.backColorLight = value;
                    this.Invalidate();
                    if (this.BackColorLightChanged != null)
                    {
                        this.BackColorLightChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color textColorDark;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        [RefreshProperties(RefreshProperties.All)]
        public Color TextColorDark
        {
            get { return this.textColorDark; }
            set
            {
                if (this.textColorDark != value)
                {
                    this.textColorDark = value;
                    this.Invalidate();
                    if (this.TextColorDarkChanged != null)
                    {
                        this.TextColorDarkChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color textColorLight;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "White")]
        [RefreshProperties(RefreshProperties.All)]
        public Color TextColorLight
        {
            get { return this.textColorLight; }
            set
            {
                if (this.textColorLight != value)
                {
                    this.textColorLight = value;
                    this.Invalidate();
                    if (this.TextColorLightChanged != null)
                    {
                        this.TextColorLightChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Color borderColor;
        [Category("Appearance.Colors")]
        [DefaultValue(typeof(Color), "ControlDarkDark")]
        [RefreshProperties(RefreshProperties.All)]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set
            {
                if (this.borderColor != value)
                {
                    this.borderColor = value;
                    this.Invalidate();
                    if (this.BorderColorChanged != null)
                    {
                        this.BorderColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool redirectHitTest;
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Enable this property to forward all non-client hit test messages to the control's parent control.")]
        public bool RedirectHitTest
        {
            get { return this.redirectHitTest; }
            set
            {
                if (this.redirectHitTest != value)
                {
                    this.redirectHitTest = value;
                }
            }
        }

        #endregion // Public property implementation section.

        #region Overwritten method implementation section.

        public override string ToString()
        {
            return
                "Value: " + this.Value.ToString("N0", NumberFormatInfo.CurrentInfo) +
                " Maximum: " + this.Maximum.ToString("N0", NumberFormatInfo.CurrentInfo) +
                " Steps: " + this.Steps.ToString("N0", NumberFormatInfo.CurrentInfo);
        }

        #endregion // Overwritten method implementation section.

        #region Public method implementation section.

        public void Increment()
        {
            this.Increment(this.Steps);
        }

        public void Increment(double steps)
        {
            this.Value += steps;
        }

        #endregion // Public method implementation section.

        #region Overwritten event handler implementation section.

        protected override void OnPaint(PaintEventArgs args)
        {
            // TODO: Make radius configurable, calculate inset from radius and darw foreground using rounded edges.
            const int inset = 2; // A single inset value to control the sizing of the inner rect.
            const int radius = 0;

            try
            {
                if (this.CanDrawProgress(radius))
                {
                    using (Bitmap offImage = new Bitmap(this.Width, this.Height))
                    using (Graphics offScreen = Graphics.FromImage(offImage))
                    {
                        offScreen.SmoothingMode = SmoothingMode.HighQuality;
                        offScreen.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        this.DrawBackground(offScreen, this.DisplayRectangle, radius, 0.3f);
                        this.DrawForeground(offScreen, this.DisplayRectangle, inset, 0.3f);

                        args.Graphics.DrawImageUnscaled(offImage, this.ClientRectangle);
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        protected override void OnSizeChanged(EventArgs args)
        {
            this.Invalidate();
            base.OnSizeChanged(args);
        }

        protected override void OnFontChanged(EventArgs args)
        {
            this.Invalidate();
            base.OnFontChanged(args);
        }

        protected override void OnBackColorChanged(EventArgs args)
        {
            this.Invalidate();
            base.OnBackColorChanged(args);
        }

        protected override void OnForeColorChanged(EventArgs args)
        {
            this.Invalidate();
            base.OnForeColorChanged(args);
        }

        protected override void WndProc(ref Message message)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = (-1);

            if (message.Msg == WM_NCHITTEST && this.RedirectHitTest)
            {
                message.Result = (IntPtr)HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref message);
            }
        }

        #endregion // Overwritten event handler implementation section.

        #region Private method implementation section.

        private void DrawBackground(Graphics graphics, Rectangle bounds, int radius, float sigma)
        {
            // Vertical direction means the opposite gradient mode!
            LinearGradientMode gradient =
                this.Vertical ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical;

            // Fill rounded rectangle.
            using (LinearGradientBrush bkbrush = new LinearGradientBrush(
                bounds, this.BackColorDark, this.BackColorLight, gradient))
            using (Pen pen = new Pen(this.BorderColor))
            using (GraphicsPath path = new GraphicsPath())
            {
                // Use a bell-shaped brush with the peak 
                // at the center of the drawing area.
                bkbrush.SetSigmaBellShape(sigma);

                pen.EndCap = LineCap.Round;
                pen.StartCap = LineCap.Round;

                int offset = Convert.ToInt32(Math.Ceiling(pen.Width));
                Rectangle rect = Rectangle.Inflate(bounds, -offset, -offset);

                if (radius > 0)
                {
                    path.StartFigure();
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();

                    graphics.FillPath(bkbrush, path);
                    graphics.DrawPath(pen, path);
                }
                else
                {
                    graphics.FillRectangle(bkbrush, bounds);
                    graphics.DrawRectangle(pen, Rectangle.Inflate(bounds, -1, -1));
                }
            }

            // Text on background uses a different color!
            string label = this.Text;
            if (!String.IsNullOrEmpty(label))
            {
                StringFormat format = this.horzFormat;
                if (this.TextVertical) { format = this.vertFormat; }

                using (Brush txbrush = new SolidBrush(this.TextColorDark))
                {
                    graphics.DrawString(label, this.Font, txbrush, bounds, format);
                }
            }
        }

        private void DrawForeground(Graphics graphics, Rectangle bounds, int inset, float sigma)
        {
            // Vertical direction means the opposite gradient mode!
            LinearGradientMode gradient =
                this.Vertical ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical;

            double val = this.Value;
            double div = this.Maximum > 0 ? this.Maximum : 1;

            // Deflate inner rect.
            Rectangle filled = Rectangle.Inflate(bounds, -inset, -inset);

            if (this.Vertical)
            {
                filled.Height = (int)(filled.Height * val / div);
                filled.Y = bounds.Height - (filled.Height + inset);
                if (filled.Height == 0 && this.Value != 0) { filled.Height = 1; }
            }
            else
            {
                filled.Width = (int)(filled.Width * val / div);
                if (filled.Width == 0 && this.Value != 0) { filled.Width = 1; }
            }

            if (filled.Height != 0 && filled.Width != 0)
            {
                using (LinearGradientBrush fgbrush = new LinearGradientBrush(
                    filled, this.ForeColorDark, this.ForeColorLight, gradient))
                {
                    fgbrush.SetSigmaBellShape(sigma);
                    graphics.FillRectangle(fgbrush, filled);
                }
            }

            string label = this.Text;
            if (!String.IsNullOrEmpty(label))
            {
                StringFormat format = this.horzFormat;
                if (this.textVertical) { format = this.vertFormat; }

                using (Brush txbrush = new SolidBrush(this.TextColorLight))
                {
                    Region region = new Region(bounds);
                    region.Intersect(filled);
                    graphics.Clip = region;
                    graphics.DrawString(label, this.Font, txbrush, bounds, format);
                }
            }
        }

        private bool CanDrawProgress(int radius)
        {
            // BUG: Text size not yet included in calculation. 
            // If text is enabled then current text width (size) 
            // is not yet included in the calculation.
            if (this.Width > 0 && this.Height > 0)
            {
                Rectangle rectangle = this.DisplayRectangle;

                if (rectangle.Width - radius > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion // Private method implementation section.
    }
}
