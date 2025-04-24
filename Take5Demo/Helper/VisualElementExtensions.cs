namespace Take5Demo.Helper
{
    public static class VisualElementExtensions
    {
        public static T FindAncestorOfType<T>(this VisualElement element) where T : VisualElement
        {
            var parent = element.Parent;
            while (parent != null)
            {
                if (parent is T typedParent)
                {
                    return typedParent;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
