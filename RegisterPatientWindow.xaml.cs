using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using DoAn.ClassData; // Đảm bảo namespace chứa class Database của bạn

namespace DoAn
{
    public partial class RegisterPatientWindow : Window
    {
        Database db = new Database();

        public RegisterPatientWindow()
        {
            InitializeComponent();
        }

        // Cho phép kéo di chuyển cửa sổ
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void btnDoRegister_Click(object sender, RoutedEventArgs e)
        {
            // --- 1. KIỂM TRA DỮ LIỆU ĐẦU VÀO (VALIDATION) ---

            // Kiểm tra các trường trống
            if (string.IsNullOrWhiteSpace(txtRegUser.Text) || 
                string.IsNullOrWhiteSpace(txtRegName.Text) ||
                string.IsNullOrWhiteSpace(txtRegPhone.Text) ||
                string.IsNullOrWhiteSpace(txtRegCID.Text) ||
                string.IsNullOrWhiteSpace(txtRegPass.Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả các thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra số điện thoại (phải là số và đủ 10 chữ số)
            if (txtRegPhone.Text.Length != 10 || !long.TryParse(txtRegPhone.Text, out _))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (phải đủ 10 chữ số)!", "Lỗi định dạng");
                return;
            }

            // Kiểm tra CCCD (12 chữ số)
            if (txtRegCID.Text.Length != 12 || !long.TryParse(txtRegCID.Text, out _))
            {
                MessageBox.Show("Số CCCD không hợp lệ (phải đủ 12 chữ số)!", "Lỗi định dạng");
                return;
            }

            // Kiểm tra ngày sinh
            if (dpRegDOB.SelectedDate == null || dpRegDOB.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi dữ liệu");
                return;
            }

            // Kiểm tra mật khẩu khớp nhau
            if (txtRegPass.Password != txtRegConfirmPass.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi bảo mật");
                return;
            }

            try
            {
                // --- 2. KIỂM TRA TÊN ĐĂNG NHẬP ĐÃ TỒN TẠI CHƯA ---
                string checkUserSql = "SELECT COUNT(*) FROM ACCOUNTS WHERE Username = @user";
                var exists = db.GetData(checkUserSql, new SqlParameter("@user", txtRegUser.Text));
                if (Convert.ToInt32(exists.Rows[0][0]) > 0)
                {
                    MessageBox.Show("Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác!");
                    return;
                }

                // --- 3. THỰC THI LƯU DỮ LIỆU ---
                
                // Tạo ID bệnh nhân ngẫu nhiên
                string newID = "PAT" + new Random().Next(100, 999).ToString();

                // SQL cho Bệnh nhân
                string sqlPatient = @"INSERT INTO PATIENT (Patient_ID, Patient_Name, Gender, Day_Of_Birth, Phone, CID, Address, Curr_Condition) 
                                     VALUES (@id, @name, @gender, @dob, @phone, @cid, @addr, @cond)";
                
                SqlParameter[] p1 = {
                    new SqlParameter("@id", newID),
                    new SqlParameter("@name", txtRegName.Text.Trim()),
                    new SqlParameter("@gender", cbRegGender.Text),
                    new SqlParameter("@dob", dpRegDOB.SelectedDate.Value),
                    new SqlParameter("@phone", txtRegPhone.Text.Trim()),
                    new SqlParameter("@cid", txtRegCID.Text.Trim()),
                    new SqlParameter("@addr", ""), 
                    new SqlParameter("@cond", "Mới đăng ký")
                };

                // SQL cho Tài khoản
                string sqlAccount = "INSERT INTO ACCOUNTS (Username, Password, Role, OwnerID) VALUES (@user, @pass, 'Patient', @owner)";
                SqlParameter[] p2 = {
                    new SqlParameter("@user", txtRegUser.Text.Trim()),
                    new SqlParameter("@pass", txtRegPass.Password),
                    new SqlParameter("@owner", newID)
                };

                // Thực thi đồng thời (Sử dụng Transaction nếu Database class của bạn hỗ trợ)
                if (db.Execute(sqlPatient, p1) && db.Execute(sqlAccount, p2))
                {
                    MessageBox.Show("Đăng ký thành công! Chào mừng bạn đến với hệ thống.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
