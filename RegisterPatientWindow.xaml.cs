using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using DoAn.ClassData;

namespace DoAn
{
    public partial class RegisterPatientWindow : Window
    {
        Database db = new Database();

        public RegisterPatientWindow()
        {
            InitializeComponent();
        }

        // 1. TitleBar Logic
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) btnMaximize_Click(sender, e);
            else this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
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

        private void btnExit_Click(object sender, RoutedEventArgs e) => this.Close();

        // 2. Register Logic
        private void btnDoRegister_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra validate cơ bản
            if (string.IsNullOrWhiteSpace(txtRegUser.Text) || string.IsNullOrWhiteSpace(txtRegPass.Password))
            {
                MessageBox.Show("Vui lòng không để trống Tên đăng nhập và Mật khẩu!");
                return;
            }

            if (txtRegPass.Password != txtRegConfirmPass.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không trùng khớp!");
                return;
            }

            try
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                DataTable dt = db.GetData("SELECT * FROM ACCOUNTS WHERE Username = @u", new SqlParameter("@u", txtRegUser.Text.Trim()));
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại trong hệ thống!");
                    return;
                }

                // Thực hiện Insert (Transaction giả định qua 2 câu lệnh)
                string newID = "PAT" + new Random().Next(100, 999).ToString();
                
                string sqlPatient = @"INSERT INTO PATIENT (Patient_ID, Patient_Name, Gender, Day_Of_Birth, Phone, CID, Address, Curr_Condition) 
                                     VALUES (@id, @name, @gender, @dob, @phone, @cid, @addr, @cond)";
                
                SqlParameter[] p1 = {
                    new SqlParameter("@id", newID),
                    new SqlParameter("@name", txtRegName.Text.Trim()),
                    new SqlParameter("@gender", cbRegGender.Text),
                    new SqlParameter("@dob", dpRegDOB.SelectedDate ?? DateTime.Now),
                    new SqlParameter("@phone", txtRegPhone.Text.Trim()),
                    new SqlParameter("@cid", txtRegCID.Text.Trim()),
                    new SqlParameter("@addr", "N/A"),
                    new SqlParameter("@cond", "Active")
                };

                string sqlAccount = "INSERT INTO ACCOUNTS (Username, Password, Role, OwnerID) VALUES (@u, @p, 'Patient', @oid)";
                SqlParameter[] p2 = {
                    new SqlParameter("@u", txtRegUser.Text.Trim()),
                    new SqlParameter("@p", txtRegPass.Password),
                    new SqlParameter("@oid", newID)
                };

                if (db.Execute(sqlPatient, p1) && db.Execute(sqlAccount, p2))
                {
                    MessageBox.Show("Chúc mừng! Đăng ký tài khoản bệnh nhân thành công.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
            }
        }
    }
}
