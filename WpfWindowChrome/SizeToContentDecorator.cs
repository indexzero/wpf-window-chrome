//-----------------------------------------------------------------------
// <copyright file="SizeToContentDecorator.cs" company="Charlie Robbins">
//     Copyright (c) Charlie Robbins.  All rights reserved.
// </copyright>
// <license>
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </license>
// <summary>Contains the SizeToContentDecorator class.</summary>
//-----------------------------------------------------------------------

namespace WpfWindowChrome
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A decorator the respects SizeToContent parameters
    /// </summary>
    public class SizeToContentDecorator : Decorator
    {
        #region Dependency Properties 

        /// <summary>
        /// Backing store for the SizeToContent property
        /// </summary>
        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register(
            "SizeToContent",
            typeof(SizeToContent),
            typeof(SizeToContentDecorator),
            new FrameworkPropertyMetadata(SizeToContent.Manual, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion Dependency Properties 

        #region Fields 

        /// <summary>
        /// Backing store for the DesiredSizeChanged event
        /// </summary>
        public static readonly RoutedEvent DesiredSizeChangedEvent = EventManager.RegisterRoutedEvent(
            "DesiredSizeChanged", 
            RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), 
            typeof(SizeToContentDecorator));

        #endregion Fields 

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeToContentDecorator"/> class.
        /// </summary>
        public SizeToContentDecorator()
            : base()
        {
        }

        #region Events 

        /// <summary>
        /// Adds or removes handlers to the DesiredSizedChanged event
        /// </summary>
        public event RoutedEventHandler DesiredSizeChanged
        {
            add { AddHandler(DesiredSizeChangedEvent, value); }
            remove { RemoveHandler(DesiredSizeChangedEvent, value); }
        }

        #endregion Events 

        #region Properties 

        /// <summary>
        /// Gets or sets the content of the size to.
        /// </summary>
        /// <value>The content of the size to.</value>
        public SizeToContent SizeToContent
        {
            get { return (SizeToContent)GetValue(SizeToContentProperty); }
            set { SetValue(SizeToContentProperty, value); }
        }

        #endregion Properties 

        #region Methods 

        /// <summary>
        /// Measures the child element of a <see cref="T:System.Windows.Controls.Decorator"/> to prepare for arranging it during the <see cref="M:System.Windows.Controls.Decorator.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The target <see cref="T:System.Windows.Size"/> of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size measureConstraint = constraint;

            switch (this.SizeToContent)
            {
                case SizeToContent.Height:
                    measureConstraint = new Size(constraint.Width, double.PositiveInfinity);
                    break;
                case SizeToContent.Width:
                    measureConstraint = new Size(double.PositiveInfinity, constraint.Height);
                    break;
                case SizeToContent.WidthAndHeight:
                    measureConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);
                    break;
            }

            return base.MeasureOverride(measureConstraint);
        }

        /// <summary>
        /// Supports layout behavior when a child element is resized.
        /// </summary>
        /// <param name="child">The child element that is being resized.</param>
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            this.RaiseDesiredSizedChangedEvent();
        }

        /// <summary>
        /// Raises the desired sized changed event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Consistency with existing WPF APIs and implementation for RoutedEvents.")]
        protected void RaiseDesiredSizedChangedEvent()
        {
            RoutedEventArgs desiredSizeEventArgs = new RoutedEventArgs(SizeToContentDecorator.DesiredSizeChangedEvent);
            this.RaiseEvent(desiredSizeEventArgs);
        }

        #endregion Methods 
    }
}
