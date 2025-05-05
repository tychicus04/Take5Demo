using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace Take5Demo.Control
{
    public class CustomSignaturePadView : DrawingView
    {
        public bool SignatureEnabled
        {
            get => base.IsEnabled;
            set
            {
                base.IsEnabled = value;
                Console.WriteLine($"SignaturePad {ClassId} IsEnabled set to {value}");
            }
        }

        protected override void ChangeVisualState()
        {
            base.ChangeVisualState();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            // Force refresh after handler is set
            if (Handler != null)
            {
                InvalidateMeasure();

                // Debug output to check handler type
                Console.WriteLine($"Handler type: {Handler.GetType().Name}");
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            Console.WriteLine($"SignaturePad parent set to: {Parent?.GetType().Name}, IsVisible: {IsVisible}");
        }

        protected override Size ArrangeOverride(Rect bounds)
        {
            Console.WriteLine($"SignaturePad arranging with bounds: {bounds}");

            // If bounds have zero dimension, create new bounds with minimum size
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                double width = WidthRequest > 0 ? WidthRequest : 200;
                double height = HeightRequest > 0 ? HeightRequest : 100;

                bounds = new Rect(bounds.X, bounds.Y, width, height);
                Console.WriteLine($"Modified bounds to: {bounds}");
            }

            var result = base.ArrangeOverride(bounds);
            Console.WriteLine($"SignaturePad arranged result: {result}");
            return result;
        }

        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            Console.WriteLine($"SignaturePad measuring with constraints: w:{widthConstraint}, h:{heightConstraint}");

            // Call base implementation
            var baseSize = base.MeasureOverride(widthConstraint, heightConstraint);

            Console.WriteLine($"Base measure returned: w:{baseSize.Width}, h:{baseSize.Height}");

            // Enforce minimum size if base returns zero
            if (baseSize.Width <= 0 || baseSize.Height <= 0)
            {
                // Use requested size or default minimum
                double width = WidthRequest > 0 ? WidthRequest : 200;
                double height = HeightRequest > 0 ? HeightRequest : 100;

                return new Size(width, height);
            }

            return baseSize;
        }
    }
}
