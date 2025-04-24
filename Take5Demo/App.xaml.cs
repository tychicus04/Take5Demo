using Microsoft.Maui.Platform;

namespace Take5Demo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
#if ANDROID
            Microsoft.Maui.Handlers
                .EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                h.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });
            Microsoft.Maui.Handlers
                .PickerHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                  h.PlatformView.BackgroundTintList =
                      Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}