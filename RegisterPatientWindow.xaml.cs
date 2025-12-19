using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoAn
{
    public partial class MainWindowPatient : Window
    {
        private string patientID;
        private Button currentBtn;

        public MainWindowPatient(string id, string name)
        {
            InitializeComponent();
            this.patientID = id;
            this.txtDisplayName.Text = name;

            // Tự động nạp trang Thông tin cá nhân khi khởi động
            NavigateToPage(btnProfile, new PagePatientProfile(id));
        }

        // Xử lý điều hướng Menu
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn == currentBtn) return;

            string tag = btn.Tag.ToString();

            switch (tag)
            {
                case "Profile":
                    NavigateToPage(btn, new PagePatientProfile(patientID));
                    break;
                case "History":
                    NavigateToPage(btn, new PagePatientHistory(patientID));
                    break;
                case "Bill":
                    // MainFrame.Navigate(new PagePatientBill(patientID));
                    break;
                case "Home":
                    // MainFrame.Navigate(new PageHome());
                    break;
            }
        }

        private void NavigateToPage(Button btn, Page page)
        {
            if (currentBtn != null) currentBtn.IsEnabled = true;
            
            currentBtn = btn;
            currentBtn.IsEnabled = false; // Vô hiệu hóa nút đang chọn để highlight
            MainFrame.Navigate(page);
        }

        // Điều khiển cửa sổ
        private void Minimize_Click(object sender, RoutedEventArgs e) 
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                btnMax.Content = "▢";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                btnMax.Content = "❐";
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) 
        {
            Application.Current.Shutdown();
        }

        // Logic Đăng xuất trả về LoginWindow
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var login = new LoginWindow(); // Khởi tạo lại màn hình Login
                login.Show();
                this.Close(); // Đóng cửa sổ hiện tại
            }
        }

        // Hỗ trợ kéo di chuyển cửa sổ khi nhấn vào vùng Header xanh
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed) this.DragMove();
        }
    }
}
