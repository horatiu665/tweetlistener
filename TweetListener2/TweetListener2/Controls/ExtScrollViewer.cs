using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TweetListener2.Controls
{
    /// <summary>
    /// scroll viewer which can be parented under other scroll viewers and scroll each of them as their scrolling completes in each direction
    /// </summary>
    public class ExtScrollViewer : ScrollViewer
    {
        private ScrollBar verticalScrollbar;
        private ScrollBar horizontalScrollbar;

        /// <summary>
        /// use excess to prevent unwanted scrolling losing control
        /// </summary>
        private int horizontalExcess = 0, verticalExcess = 0;

        // this should ideally be exposed in xaml, to be able to set them in xaml
        private int horizontalExcessMax = 5 * 120;
        public int HorizontalExcessMax
        {
            get
            {
                return horizontalExcessMax;
            }

            set
            {
                horizontalExcessMax = value;
            }
        }

        // this should ideally be exposed in xaml, to be able to set them in xaml
        private int verticalExcessMax = 5 * 120;
        public int VerticalExcessMax
        {
            get
            {
                return verticalExcessMax;
            }

            set
            {
                verticalExcessMax = value;
            }
        }

        public override void OnApplyTemplate()
        {
            // Call base class
            base.OnApplyTemplate();

            // Obtain the vertical scrollbar
            this.verticalScrollbar = this.GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
            this.horizontalScrollbar = this.GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            bool scrollSelf = false;

            var verticalScrollbarExists =
                ((this.verticalScrollbar != null)
                && (this.verticalScrollbar.Visibility == System.Windows.Visibility.Visible)
                && this.verticalScrollbar.IsEnabled);
            var horizontalScrollbarExists =
                ((this.horizontalScrollbar != null)
                && (this.horizontalScrollbar.Visibility == System.Windows.Visibility.Visible)
                && this.horizontalScrollbar.IsEnabled);

            if (horizontalScrollbarExists || verticalScrollbarExists) {
                var verticalHitTop = (VerticalOffset <= 0 && e.Delta > 0);
                var verticalHitBottom = (VerticalOffset >= ExtentHeight - ViewportHeight && e.Delta < 0);
                var horizontalHitLeft = (HorizontalOffset <= 0 && e.Delta > 0);
                var horizontalHitRight = (HorizontalOffset >= ExtentWidth - ViewportWidth && e.Delta < 0);

                // if limits are hit
                if ((verticalScrollbarExists && (verticalHitTop || verticalHitBottom)) ||
                    (horizontalScrollbarExists && (horizontalHitLeft || horizontalHitRight))) {
                    // limits are hit, but we should still "scroll" (block) this element until excess is scrolled
                    if (horizontalScrollbarExists) {
                        if (horizontalExcess > -HorizontalExcessMax && e.Delta < 0) {
                            horizontalExcess += e.Delta;
                            scrollSelf = true;
                        } else if (horizontalExcess < HorizontalExcessMax && e.Delta > 0) {
                            horizontalExcess += e.Delta;
                            scrollSelf = true;
                        }
                    }
                    if (verticalScrollbarExists) {
                        if (verticalExcess > -VerticalExcessMax && e.Delta < 0) {
                            verticalExcess += e.Delta;
                            scrollSelf = true;
                        } else if (verticalExcess > VerticalExcessMax && e.Delta > 0) {
                            verticalExcess -= e.Delta;
                            scrollSelf = true;
                        }
                    }

                } else {
                    // limits not hit => scroll this element.
                    horizontalExcess = 0;
                    verticalExcess = 0;
                    scrollSelf = true;
                }
            }

            if (scrollSelf) {
                if (horizontalScrollbarExists && !verticalScrollbarExists) {
                    ScrollToHorizontalOffset(HorizontalOffset - e.Delta);
                }
                base.OnMouseWheel(e);
            }
        }
    }
}