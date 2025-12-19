using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Data;
using DoAn.ClassData; // Đảm bảo class Database nằm ở đây

namespace DoAn
{
    public partial class RegisterPatientWindow : Window
    {
        Database db = new Database();

        public RegisterPatientWindow()
        {
            InitializeComponent();
        }

        // Kéo thả cửa sổ
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed) this.DragMove();
        }

        private void btnDoRegister_Click(object sender, RoutedEventArgs e)
        {
            // 1. Kiểm tra để trống
            if (string.IsNullOrWhiteSpace(txtRegUser.Text) || string.IsNullOrWhiteSpace(txtRegName.Text) || 
                string.IsNullOrWhiteSpace(txtRegPass.Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!");
                return;
            }

            // 2. Kiểm tra mật khẩu khớp
            if (txtRegPass.Password != txtRegConfirmPass.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không chính xác!");
                return;
            }

            try {
                // 3. Kiểm tra tên đăng nhập tồn tại
                DataTable dtCheck = db.GetData("SELECT * FROM ACCOUNTS WHERE Username = @u", 
                                    new SqlParameter("@u", txtRegUser.Text.Trim()));
                if (dtCheck.Rows.Count > 0) {
                    MessageBox.Show("Tên đăng nhập đã tồn tại!");
                    return;
                }

                // 4. Tạo ID mới ngẫu nhiên
                string newID = "PAT" + new Random().Next(100, 999).ToString();

                // 5. Câu lệnh SQL Insert (Dựa theo cấu trúc bảng bạn cung cấp)
                string sqlPatient = @"INSERT INTO PATIENT (Patient_ID, Patient_Name, Gender, Day_Of_Birth, Phone, CID, Address, Curr_Condition) 
                                     VALUES (@id, @name, @gender, @dob, @phone, @cid, @addr, @cond)";
                
                SqlParameter[] p1 = {
                    new SqlParameter("@id", newID),
                    new SqlParameter("@name", txtRegName.Text.Trim()),
                    new SqlParameter("@gender", cbRegGender.Text),
                    new SqlParameter("@dob", dpRegDOB.SelectedDate ?? DateTime.Now),
                    new SqlParameter("@phone", txtRegPhone.Text.Trim()),
                    new SqlParameter("@cid", txtRegCID.Text.Trim()),
                    new SqlParameter("@addr", "Chưa cập nhật"),
                    new SqlParameter("@cond", "New Registration")
                };

                // SQL cho bảng tài khoản
                string sqlAcc = "INSERT INTO ACCOUNTS (Username, Password, Role, OwnerID) VALUES (@u, @p, 'Patient', @oid)";
                SqlParameter[] p2 = {
                    new SqlParameter("@u", txtRegUser.Text.Trim()),
                    new SqlParameter("@p", txtRegPass.Password),
                    new SqlParameter("@oid", newID)
                };

                // 6. Thực hiện lưu
                if (db.Execute(sqlPatient, p1) && db.Execute(sqlAcc, p2)) {
                    MessageBox.Show("Đăng ký thành công!");
                    this.Close();
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

                   // Nút Thu nhỏ
            private void btnMinimize_Click(object sender, RoutedEventArgs e)
            {
                this.WindowState = WindowState.Minimized;
            }
            
            // Nút Phóng to / Thu nhỏ lại
            private void btnMaximize_Click(object sender, RoutedEventArgs e)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    btnMax.Content = "▢"; // Biểu tượng ô vuông đơn
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    btnMax.Content = "❐"; // Biểu tượng hai ô vuông chồng nhau
                }
            }
            
            // Nút Thoát (Đã có trong code cũ nhưng hãy đảm bảo nó hoạt động)
            private void btnExit_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
                }
}
