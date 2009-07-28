/**************************************************************
 * Copyright (c) 2009 Charlie Robbins
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
**************************************************************/

namespace WpfWindowChrome
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// A better Window that supports custom chrome, title bars, hierarchical data context management by default.
    /// </summary>
    public class CustomChromeWindow : Window
    {
        #region Dependency Properties 

        /// <summary>
        /// Backing store for the ActiveWindowBarBrush Property
        /// </summary>
        public static readonly DependencyProperty ActiveWindowBarBrushProperty = DependencyProperty.Register(
            "ActiveWindowBarBrush",
            typeof(Brush),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Backing store for the CloseButtonStyle Property
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
            "CloseButtonStyle",
            typeof(Style),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Backing store for the HeightAdjustment property
        /// </summary>
        public static readonly DependencyProperty HeightAdjustmentProperty = DependencyProperty.Register(
            "HeightAdjustment",
            typeof(double),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(36.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Backing store for the InactiveWindowBarBrush Property
        /// </summary>
        public static readonly DependencyProperty InactiveWindowBarBrushProperty = DependencyProperty.Register(
            "InactiveWindowBarBrush",
            typeof(Brush),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightBlue), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Backing store for the MaximizeButtonStyle Property
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonStyleProperty = DependencyProperty.Register(
            "MaximizeButtonStyle",
            typeof(Style),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Backing store for the MinimizeButtonStyle Property
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonStyleProperty = DependencyProperty.Register(
            "MinimizeButtonStyle",
            typeof(Style),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Backing store for the WindowBarHeight Property
        /// </summary>
        public static readonly DependencyProperty WindowBarHeightProperty = DependencyProperty.Register(
            "WindowBarHeight",
            typeof(int),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(25));

        /// <summary>
        /// Backing store for the WindowBarStyle Property
        /// </summary>
        public static readonly DependencyProperty WindowBarStyleProperty = DependencyProperty.Register(
            "WindowBarStyle",
            typeof(Style),
            typeof(CustomChromeWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Dependency Properties 

        #region Fields 

        /// <summary>
        /// The name for the close button template part
        /// </summary>
        private const string CloseButtonName = "PART_CloseButton";

        /// <summary>
        /// The name for the maximize button template part
        /// </summary>
        private const string MaximizeButtonName = "PART_MaximizeButton";

        /// <summary>
        /// The name for the minimize button template part
        /// </summary>
        private const string MinimizeButtonName = "PART_MinimizeButton";

        /// <summary>
        /// The name for the sizing decorator template part
        /// </summary>
        private const string SizingDecoratorName = "PART_SizingDecorator";

        /// <summary>
        /// The name for the window bar thumb template part
        /// </summary>
        private const string WindowBarThumbName = "PART_WindowBarThumb";

        /// <summary>
        /// The name for the window content template part
        /// </summary>
        private const string WindowContentName = "PART_WindowContent";

        /// <summary>
        /// The close button template part
        /// </summary>
        private Button closeButton;

        /// <summary>
        /// The maximize button template part
        /// </summary>
        private Button maximizeButton;

        /// <summary>
        /// The minimize button template part
        /// </summary>
        private Button minimizeButton;

        /// <summary>
        /// The sizing decorator template part
        /// </summary>
        private SizeToContentDecorator sizingDecorator;

        /// <summary>
        /// The window bar template part
        /// </summary>
        private System.Windows.Controls.Primitives.Thumb windowBar;

        #endregion Fields 

        /// <summary>
        /// Initializes static members of the <see cref="CustomChromeWindow"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Consistency with existing WPF APIs")]
        static CustomChromeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomChromeWindow), new FrameworkPropertyMetadata(typeof(CustomChromeWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomChromeWindow"/> class.
        /// </summary>
        public CustomChromeWindow()
        {
        }

        #region Properties 

        /// <summary>
        /// Gets or sets the active window bar brush.
        /// </summary>
        /// <value>The active window bar brush.</value>
        public Brush ActiveWindowBarBrush
        {
            get { return (Brush)GetValue(ActiveWindowBarBrushProperty); }
            set { SetValue(ActiveWindowBarBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the close button style.
        /// </summary>
        /// <value>The close button style.</value>
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height adjustment.
        /// </summary>
        /// <value>The height adjustment, a value used for tweaking margins / padding in the custom chrome.</value>
        public double HeightAdjustment
        {
            get { return (double)GetValue(HeightAdjustmentProperty); }
            set { SetValue(HeightAdjustmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the inactive window bar brush.
        /// </summary>
        /// <value>The inactive window bar brush.</value>
        public Brush InactiveWindowBarBrush
        {
            get { return (Brush)GetValue(InactiveWindowBarBrushProperty); }
            set { SetValue(InactiveWindowBarBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximize button style.
        /// </summary>
        /// <value>The maximize button style.</value>
        public Style MaximizeButtonStyle
        {
            get { return (Style)GetValue(MaximizeButtonStyleProperty); }
            set { SetValue(MaximizeButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimize button style.
        /// </summary>
        /// <value>The minimize button style.</value>
        public Style MinimizeButtonStyle
        {
            get { return (Style)GetValue(MinimizeButtonStyleProperty); }
            set { SetValue(MinimizeButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the window bar.
        /// </summary>
        /// <value>The height of the window bar.</value>
        public int WindowBarHeight
        {
            get { return (int)GetValue(WindowBarHeightProperty); }
            set { SetValue(WindowBarHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the window bar style.
        /// </summary>
        /// <value>The window bar style.</value>
        public Style WindowBarStyle
        {
            get { return (Style)GetValue(WindowBarStyleProperty); }
            set { SetValue(WindowBarStyleProperty, value); }
        }

        #endregion Properties 

        #region Methods 

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            System.Windows.Controls.Primitives.Thumb oldWindowBar = this.windowBar;
            this.windowBar = this.GetTemplateChild(WindowBarThumbName) as System.Windows.Controls.Primitives.Thumb;

            Button oldMinimizeButton = this.minimizeButton;
            this.minimizeButton = this.GetTemplateChild(MinimizeButtonName) as Button;

            Button oldMaximizeButton = this.maximizeButton;
            this.maximizeButton = this.GetTemplateChild(MaximizeButtonName) as Button;

            Button oldCloseButton = this.closeButton;
            this.closeButton = this.GetTemplateChild(CloseButtonName) as Button;

            SizeToContentDecorator oldSizingDecorator = this.sizingDecorator;
            this.sizingDecorator = this.GetTemplateChild(SizingDecoratorName) as SizeToContentDecorator;

            if (!object.ReferenceEquals(oldSizingDecorator, this.sizingDecorator))
            {
                if (oldSizingDecorator != null)
                {
                    oldSizingDecorator.DesiredSizeChanged -= this.OnContentDesiredSizeChanged;
                }

                if (this.sizingDecorator != null)
                {
                    this.sizingDecorator.DesiredSizeChanged += this.OnContentDesiredSizeChanged;
                }
            }

            // Add and Remove the Event Handlers for the WindowBar
            if (!object.ReferenceEquals(oldWindowBar, this.windowBar))
            {
                if (oldWindowBar != null)
                {
                    oldWindowBar.DragDelta -= this.DoDragWindow;
                    oldWindowBar.MouseDoubleClick -= this.MaximizeWindow;
                }

                if (this.windowBar != null)
                {
                    this.windowBar.DragDelta += this.DoDragWindow;
                    this.windowBar.MouseDoubleClick += this.MaximizeWindow;
                }
            }

            // Add and Remove the Event Handlers for the MinimizeButton
            if (!object.ReferenceEquals(oldMinimizeButton, this.minimizeButton))
            {
                if (oldMinimizeButton != null)
                {
                    oldMinimizeButton.Click -= this.MinimizeWindow;
                }

                if (this.minimizeButton != null)
                {
                    this.minimizeButton.Click += this.MinimizeWindow;
                }
            }

            // Add and Remove the Event Handlers for the MaximizeButton
            if (!object.ReferenceEquals(oldMaximizeButton, this.maximizeButton))
            {
                if (oldMaximizeButton != null)
                {
                    oldMaximizeButton.Click -= this.MaximizeWindow;
                }

                if (this.maximizeButton != null)
                {
                    this.maximizeButton.Click += this.MaximizeWindow;
                }
            }

            // Add and Remove the Event Handlers for the CloseButton
            if (!object.ReferenceEquals(oldCloseButton, this.closeButton))
            {
                if (oldCloseButton != null)
                {
                    oldCloseButton.Click -= this.CloseWindow;
                }

                if (this.closeButton != null)
                {
                    this.closeButton.Click += this.CloseWindow;
                }
            }
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Should able to be overriden by child types")]
        protected virtual void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.VerifyAccess();

            this.Close();
        }

        /// <summary>
        /// Performs a drag operation for the window for a single DragDelta event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Should able to be overriden by child types")]
        protected virtual void DoDragWindow(object sender, DragDeltaEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.Top += e.VerticalChange;
                this.Left += e.HorizontalChange;
            }
        }

        /// <summary>
        /// Finds the parent window of this instance if it exists.
        /// </summary>
        /// <returns>The parent window of this instance if it exists.</returns>
        protected Window FindParentWindow()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            Window parentWindow = parent as Window;

            while (parent != null && parentWindow == null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                parentWindow = parent as Window;
            }

            return parentWindow;
        }

        /// <summary>
        /// Maximizes the window or returns it to it's previous state depending on the current state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Should able to be overriden by child types")]
        protected virtual void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            this.VerifyAccess();

            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Override this method to measure the size of a window.
        /// </summary>
        /// <param name="availableSize">A <see cref="T:System.Windows.Size"/> that reflects the available size that this window can give to the child. Infinity can be given as a value to indicate that the window will size to whatever content is available.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that reflects the size that this window determines it needs during layout, based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size baseSize = base.MeasureOverride(availableSize);

            FrameworkElement child = this.sizingDecorator != null 
                ? this.sizingDecorator.Child as FrameworkElement 
                : this.Content as FrameworkElement;

            if (child != null)
            {
                switch (this.SizeToContent)
                {
                    case SizeToContent.Height:
                        return new Size(baseSize.Width, child.DesiredSize.Height + this.HeightAdjustment);
                    case SizeToContent.Width:
                        return new Size(child.DesiredSize.Width, baseSize.Height);
                    case SizeToContent.WidthAndHeight:
                        return new Size(child.DesiredSize.Width, child.DesiredSize.Height + this.HeightAdjustment);
                }
            }

            return baseSize;
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Should able to be overriden by child types")]
        protected virtual void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.VerifyAccess();

            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        /// Called when the Content's Desired Size Changes 
        /// </summary>
        protected virtual void OnContentDesiredSizeChanged()
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Called when the Content's Desired Size Changes 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnContentDesiredSizeChanged(object sender, RoutedEventArgs args)
        {
            this.OnContentDesiredSizeChanged();
        }

        #endregion Methods 
    }
}
