using System.Windows;
using System.Windows.Input;
using DoAn.Pages; // Giả sử các trang con nằm trong folder Pages

namespace DoAn
{
    public partial class MainWindowPatient : Window
    {
        private string patientID;
        private string fullName;

        public MainWindowPatient(string id, string name)
        {
            InitializeComponent();
            this.patientID = id;
            this.fullName = name;

            // Hiển thị thông tin lên giao diện
            txtPatientName.Text = "Xin chào, " + name;
            txtPatientID.Text = "ID: " + id;

            // Mặc định load trang Profile (Trang có thể cập nhật)
            LoadPage("Profile");
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            LoadPage(tag);
        }

        private void LoadPage(string tag)
        {
            switch (tag)
            {
                case "Profile":
                    txtTitle.Text = "THÔNG TIN CÁ NHÂN";
                    // Truyền ID vào trang Profile để truy vấn và cập nhật
                    MainContainer.Content = new PagePatientProfile(patientID);
                    break;
                case "History":
                    txtTitle.Text = "HỒ SƠ BỆNH ÁN (CHỈ XEM)";
                    // Trang này bạn thiết lập IsReadOnly="True" cho DataGrid
                    MainContainer.Content = new PagePatientHistory(patientID);
                    break;
                case "Billing":
                    txtTitle.Text = "HÓA ĐƠN THANH TOÁN (CHỈ XEM)";
                    MainContainer.Content = new PagePatientBilling(patientID);
                    break;
                default:
                    txtTitle.Text = "TRANG CHỦ";
                    MainContainer.Content = null; 
                    break;
            }
        }

        // Các nút điều hướng hệ thống
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void Minimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Mở lại màn hình chọn vai trò hoặc Login
            // new LoginWindow().Show();
            this.Close();
        }
    }
}
