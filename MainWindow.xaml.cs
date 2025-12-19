using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Data;
using Microsoft.Data.SqlClient;
using DoAn.ClassData;
using DoAn.Windows;

namespace DoAn
{
    public partial class MainWindow : Window
    {
        private readonly Database db = new Database();

        private string currentRole = "";

        public MainWindow(string username, string roleCode)
        {
            InitializeComponent();
            currentRole = roleCode; // Lưu lại quyền

            // Thiết lập trạng thái mặc định
            btnHome.IsChecked = true;
            dgData.Visibility = Visibility.Collapsed;
            dgDept.Visibility = Visibility.Collapsed;

            // Gọi hàm phân quyền
            ApplyAuthorization();
        }

        private void ApplyAuthorization()
        {
            // Mặc định ẩn các nút quản lý nhạy cảm
            btnNhanVien.Visibility = Visibility.Collapsed;
            btnBacSi.Visibility = Visibility.Collapsed;
            btnKhoa.Visibility = Visibility.Collapsed;

            switch (currentRole)
            {
                case "AD004": // Admin: Thấy tất cả
                    btnNhanVien.Visibility = Visibility.Visible;
                    btnBacSi.Visibility = Visibility.Visible;
                    btnKhoa.Visibility = Visibility.Visible;
                    break;
                case "DC002": // Bác sĩ: Chỉ xem đồng nghiệp và Khoa
                    btnBacSi.Visibility = Visibility.Visible;
                    btnKhoa.Visibility = Visibility.Visible;
                    break;
                case "EC003": // Nhân viên: Thấy danh sách NV và Khoa
                    btnNhanVien.Visibility = Visibility.Visible;
                    btnKhoa.Visibility = Visibility.Visible;
                    break;
                case "EM001": // Bệnh nhân: Chỉ xem trang chủ
                    break;
            }
        }
        // THÊM ĐOẠN NÀY VÀO MainWindow.xaml.cs
        public MainWindow()
        {
            InitializeComponent();

            // Thiết lập các trạng thái mặc định giống như constructor kia
            btnHome.IsChecked = true;
            dgData.Visibility = Visibility.Collapsed;
            dgDept.Visibility = Visibility.Collapsed;

            // Nếu muốn hiển thị tên mặc định (vì không qua Login)
            // txtHomeMessage.Text = "Chào mừng Admin (Chế độ Test)"; 
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton clicked = sender as ToggleButton;
            if (clicked == null) return;

            // Reset Toggle
            btnHome.IsChecked = false;
            btnNhanVien.IsChecked = false;
            btnBacSi.IsChecked = false;
            btnKhoa.IsChecked = false;
            clicked.IsChecked = true;

            // Ẩn tất cả view
            txtHomeMessage.Visibility = Visibility.Collapsed;
            dgData.Visibility = Visibility.Collapsed;
            dgDept.Visibility = Visibility.Collapsed;
            toolbarNhanSu.Visibility = Visibility.Collapsed;

            // Xử lý logic
            if (clicked == btnHome)
            {
                txtHomeMessage.Visibility = Visibility.Visible;
            }
            else if (clicked == btnNhanVien)
            {
                LoadData("SELECT * FROM EMPLOYEE WHERE Emp_Type = 'EC003'", dgData);
                toolbarNhanSu.Visibility = Visibility.Visible;
                btnAddEmployee.Visibility = Visibility.Visible; // Hiện nút thêm
            }
            else if (clicked == btnBacSi)
            {
                LoadData("SELECT * FROM EMPLOYEE WHERE Emp_Type = 'DC002'", dgData);
                toolbarNhanSu.Visibility = Visibility.Visible;
                btnAddEmployee.Visibility = Visibility.Visible;
            }
            else if (clicked == btnKhoa)
            {
                // Load bảng DEPARTMENT vào DataGrid Khoa
                LoadData("SELECT * FROM DEPARTMENT", dgDept);
                // Với Khoa, có thể bạn không muốn hiện nút "Thêm nhân viên", hoặc tạo nút "Thêm Khoa" riêng
                toolbarNhanSu.Visibility = Visibility.Visible;
                btnAddEmployee.Visibility = Visibility.Collapsed; // Tạm ẩn nút thêm nhân viên khi xem khoa
            }
        }

        private void LoadData(string query, DataGrid grid)
        {
            try
            {
                DataTable dt = db.GetData(query);
                grid.ItemsSource = dt.DefaultView;
                grid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (btnKhoa.IsChecked == true)
            {
                // Tìm kiếm Khoa
                string sql = "SELECT * FROM DEPARTMENT WHERE Depart_ID LIKE @key OR Depart_Name LIKE @key";
                try
                {
                    dgDept.ItemsSource = db.GetData(sql, new SqlParameter[] {
                new SqlParameter("@key", "%" + keyword + "%")
            }).DefaultView;
                }
                catch { }
            }
            else
            {
                // Tìm kiếm Nhân viên
                string type = (btnNhanVien.IsChecked == true) ? "EC003" : "DC002";
                string sql = "SELECT * FROM EMPLOYEE WHERE Emp_Type = @type AND Emp_ID LIKE @id";
                try
                {
                    dgData.ItemsSource = db.GetData(sql, new SqlParameter[] {
                        new SqlParameter("@type", type),
                        new SqlParameter("@id", "%" + keyword + "%")
                    }).DefaultView;
                }
                catch { }
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddEmployeeWindow();
            if (w.ShowDialog() == true)
            {
                // Refresh lại dữ liệu sau khi thêm
                if (btnNhanVien.IsChecked == true)
                    LoadData("SELECT * FROM EMPLOYEE WHERE Emp_Type = 'EC003'", dgData);
                else if (btnBacSi.IsChecked == true)
                    LoadData("SELECT * FROM EMPLOYEE WHERE Emp_Type = 'DC002'", dgData);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn đăng xuất không?",
        "Xác nhận",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Mở lại màn hình đăng nhập
                LoginWindow login = new LoginWindow();
                login.Show();

                // Đóng màn hình chính
                this.Close();
            }
        }
        private void Min_Click(object sender, RoutedEventArgs e) { this.WindowState = WindowState.Minimized; }
        private void Close_Click(object sender, RoutedEventArgs e) { this.Close(); }
    }
}
