using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace VRC_Favourite_Manager
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;
        public MainWindow()
        {
            this.InitializeComponent();

            GetAppWindowAndPresenter();
            _presenter.IsResizable = false;
        }

        public void GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;

            var appWindow = this.AppWindow;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWin = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
            appWin.SetIcon("Assets/vrchat_2023_11_04_08_55_35_417_7680x4320_RmH_icon.ico");
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

            appWindow.Title = "VRC Worlds Manager";
        }
    }
}
