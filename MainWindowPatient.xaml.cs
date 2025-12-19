using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DoAn
{
    public partial class MainWindowPatient : Window
    {
        private string patientID;
        private string patientName;
        private Button currentActiveButton;

        public MainWindowPatient(string id, string name)
        {
            InitializeComponent();
            this.patientID = id;
            this.patientName = name;

            // Hiển thị thông tin người dùng lên Sidebar
            txtPatientName.Text = name;

            // Mặc định nạp trang Thông tin cá nhân khi vừa mở
            SetActiveButton(btnProfile);
            MainContent.Content = new PagePatientProfile(patientID);
        }

        // Xử lý sự kiện Click Menu chung
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn == currentActiveButton) return;

            SetActiveButton(btn);
            string tag = btn.Tag.ToString();

            switch (tag)
            {
                case "Home":
                    // MainContent.Content = new PageHome();
                    break;
                case "Profile":
                    MainContent.Content = new PagePatientProfile(patientID);
                    break;
                case "History":
                    MainContent.Content = new PagePatientHistory(patientID);
                    break;
                case "Billing":
                    MainContent.Content = new PagePatientBilling(patientID);
                    break;
            }
        }

        // Hiệu ứng đánh dấu nút đang chọn (IsEnabled = false để kích hoạt Trigger trong XAML)
        private void SetActiveButton(Button btn)
        {
            if (currentActiveButton != null)
                currentActiveButton.IsEnabled = true;

            currentActiveButton = btn;
            currentActiveButton.IsEnabled = false; 
        }

        // Các nút điều khiển cửa sổ
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void Minimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Mở lại cửa sổ chọn tài khoản (tùy thuộc vào tên class của bạn)
                // CheckAccountWindow login = new CheckAccountWindow();
                // login.Show();
                this.Close();
            }
        }
    }
}
