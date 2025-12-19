using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Data.SqlClient;
using DoAn.ClassData;

namespace DoAn
{
    public partial class MainWindowHR : Window
    {
        private readonly Database hrDb = new Database();
        private string hrRole = "";

        public MainWindowHR(string user, string role)
        {
            InitializeComponent();
            this.hrRole = role;
            txtHRUserName.Text = user;
            ApplyHRAuthorization();
        }

        private void ApplyHRAuthorization()
        {
            // Logic ẩn hiện nút dựa trên role AD004, DC002...
            btnHRNhanVien.Visibility = (hrRole == "AD004" || hrRole == "EC003") ? Visibility.Visible : Visibility.Collapsed;
            btnHRBacSi.Visibility = (hrRole == "AD004" || hrRole == "DC002") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void HRMenu_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton clicked = sender as ToggleButton;
            if (clicked == null) return;

            // Reset trạng thái các nút HR
            btnHRHome.IsChecked = btnHRNhanVien.IsChecked = btnHRBacSi.IsChecked = btnHRKhoa.IsChecked = false;
            clicked.IsChecked = true;

            // Reset hiển thị
            txtHRWelcome.Visibility = Visibility.Collapsed;
            dgHRMain.Visibility = Visibility.Collapsed;
            dgHRDept.Visibility = Visibility.Collapsed;
            toolbarHR.Visibility = Visibility.Visible;

            if (clicked == btnHRHome) {
                txtHRWelcome.Visibility = Visibility.Visible;
                toolbarHR.Visibility = Visibility.Collapsed;
            }
            else if (clicked == btnHRNhanVien) {
                LoadHRData("SELECT Emp_ID, Emp_Name, Phone, Emp_Type FROM EMPLOYEE WHERE Emp_Type = 'EC003'", dgHRMain);
            }
            else if (clicked == btnHRBacSi) {
                LoadHRData("SELECT Emp_ID, Emp_Name, Phone, Depart_ID FROM EMPLOYEE WHERE Emp_Type = 'DC002'", dgHRMain);
            }
            else if (clicked == btnHRKhoa) {
                LoadHRData("SELECT * FROM DEPARTMENT", dgHRDept);
            }
        }

        private void LoadHRData(string query, DataGrid grid)
        {
            try {
                grid.ItemsSource = hrDb.GetData(query).DefaultView;
                grid.Visibility = Visibility.Visible;
            } catch (Exception ex) { MessageBox.Show("HR Error: " + ex.Message); }
        }

        private void btnHRSearch_Click(object sender, RoutedEventArgs e)
        {
            // Lấy từ khóa tìm kiếm từ TextBox (Giả sử tên là txtHRSearch)
            string keyword = txtHRSearch.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // Nếu để trống, load lại toàn bộ dữ liệu của Tab hiện tại
                RefreshCurrentGrid();
                return;
            }

            try
            {
                if (btnHRNhanVien.IsChecked == true)
                {
                    // Tìm nhân viên theo Tên hoặc ID
                    string sql = "SELECT Emp_ID, Emp_Name, Phone, Emp_Type FROM EMPLOYEE " +
                                 "WHERE Emp_Type = 'EC003' AND (Emp_Name LIKE @key OR Emp_ID LIKE @key)";
                    dgHRMain.ItemsSource = hrDb.GetData(sql, new SqlParameter("@key", "%" + keyword + "%")).DefaultView;
                }
                else if (btnHRBacSi.IsChecked == true)
                {
                    // Tìm bác sĩ theo Tên hoặc ID
                    string sql = "SELECT Emp_ID, Emp_Name, Phone, Depart_ID FROM EMPLOYEE " +
                                 "WHERE Emp_Type = 'DC002' AND (Emp_Name LIKE @key OR Emp_ID LIKE @key)";
                    dgHRMain.ItemsSource = hrDb.GetData(sql, new SqlParameter("@key", "%" + keyword + "%")).DefaultView;
                }
                else if (btnHRKhoa.IsChecked == true)
                {
                    // Tìm khoa theo Tên khoa
                    string sql = "SELECT * FROM DEPARTMENT WHERE Depart_Name LIKE @key OR Depart_ID LIKE @key";
                    dgHRDept.ItemsSource = hrDb.GetData(sql, new SqlParameter("@key", "%" + keyword + "%")).DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }
        private void btnHRAddNew_Click(object sender, RoutedEventArgs e) {
            DoAn.Windows.AddEmployeeWindow addWin = new DoAn.Windows.AddEmployeeWindow();
            addWin.Owner = this; // Đặt MainWindow làm chủ để căn giữa

            // 2. Hiện cửa sổ dưới dạng Dialog (buộc người dùng xử lý xong mới quay lại main)
            if (addWin.ShowDialog() == true)
            {
                // 3. Nếu thêm thành công (DialogResult = true), load lại dữ liệu
                RefreshCurrentGrid();
                MessageBox.Show("Dữ liệu đã được cập nhật!");
            }
        }

        private void RefreshCurrentGrid()
        {
            if (btnHRNhanVien.IsChecked == true)
                LoadHRData("SELECT Emp_ID, Emp_Name, Phone, Emp_Type FROM EMPLOYEE WHERE Emp_Type = 'EC003'", dgHRMain);
            else if (btnHRBacSi.IsChecked == true)
                LoadHRData("SELECT Emp_ID, Emp_Name, Phone, Depart_ID FROM EMPLOYEE WHERE Emp_Type = 'DC002'", dgHRMain);
            else if (btnHRKhoa.IsChecked == true)
                LoadHRData("SELECT * FROM DEPARTMENT", dgHRDept);
        }
        private void HRLogOut_Click(object sender, RoutedEventArgs e) { this.Close(); }
    }
}
