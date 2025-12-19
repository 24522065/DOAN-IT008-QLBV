using System;
using System.Data; // Thêm namespace này
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using DoAn.ClassData;

namespace DoAn.Windows
{
    public partial class AddEmployeeWindow : Window
    {
        Database db = new Database();

        public AddEmployeeWindow()
        {
            InitializeComponent();

            // 1. Load danh sách khoa từ Database
            LoadDepartments();

            // Set mặc định
            dpDOB.SelectedDate = DateTime.Now.AddYears(-20);
            dpStartDate.SelectedDate = DateTime.Now;
            cbType.SelectedIndex = 0;
        }

        private void LoadDepartments()
        {
            try
            {
                // Lấy Mã và Tên khoa
                DataTable dt = db.GetData("SELECT Depart_ID, Depart_Name FROM DEPARTMENT");

                // Gán vào ComboBox
                cbDept.ItemsSource = dt.DefaultView;

                // Hiển thị Tên Khoa, nhưng giá trị lấy về là Mã Khoa
                cbDept.DisplayMemberPath = "Depart_Name";
                cbDept.SelectedValuePath = "Depart_ID";

                // Chọn item đầu tiên nếu có dữ liệu
                if (cbDept.Items.Count > 0) cbDept.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khoa: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCID.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ tên và CCCD!");
                return;
            }

            try
            {
                // Lấy thông tin
                string empName = txtName.Text.Trim();
                string cid = txtCID.Text.Trim();
                string gender = cbGender.Text;
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();
                string email = txtEmail.Text.Trim();
                DateTime dob = dpDOB.SelectedDate ?? DateTime.Now;
                DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Now;

                // Lấy Mã Khoa từ SelectedValue (Do đã cài SelectedValuePath="Depart_ID")
                string deptID = cbDept.SelectedValue?.ToString();

                string empType = ((cbType.SelectedItem as ComboBoxItem).Tag).ToString();
                decimal salary = 0;
                decimal.TryParse(txtSalary.Text, out salary);

                // Tạo ID tự động
                Random r = new Random();
                string empID = "EMP" + r.Next(100, 999).ToString();

                string sql = @"INSERT INTO EMPLOYEE 
                               (Emp_ID, Emp_Name, Emp_Type, Gender, Day_Of_Birth, Start_Date, Phone, CID, Address, Email, Salary, Depart_ID)
                               VALUES 
                               (@id, @name, @type, @gender, @dob, @start, @phone, @cid, @addr, @email, @salary, @dept)";

                SqlParameter[] paramsList = new SqlParameter[]
                {
                    new SqlParameter("@id", empID),
                    new SqlParameter("@name", empName),
                    new SqlParameter("@type", empType),
                    new SqlParameter("@gender", gender),
                    new SqlParameter("@dob", dob),
                    new SqlParameter("@start", startDate),
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@cid", cid),
                    new SqlParameter("@addr", address),
                    new SqlParameter("@email", email),
                    new SqlParameter("@salary", salary),
                    new SqlParameter("@dept", deptID) // Insert đúng mã khoa
                };

                bool result = db.Execute(sql, paramsList);

                if (result)
                {
                    MessageBox.Show($"Thêm thành công!");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
