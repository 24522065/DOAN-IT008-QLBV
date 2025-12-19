using System;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;
using DoAn.ClassData;

namespace DoAn.Windows
{
    public partial class ForgotPasswordWindow : Window
    {
        Database db = new Database();

        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text.Trim();
            string cccd = txtCCCD.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(cccd) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin xác thực!");
                return;
            }

            // Truy vấn kiểm tra khớp Username, CCCD và Phone
            string sql = @"SELECT A.User_Name 
                           FROM ACCOUNT A 
                           JOIN EMPLOYEE E ON A.Emp_ID = E.Emp_ID 
                           WHERE A.User_Name = @u AND E.CID = @cid AND E.Phone = @p";

            SqlParameter[] p = {
                new SqlParameter("@u", user),
                new SqlParameter("@cid", cccd),
                new SqlParameter("@p", phone)
            };

            DataTable dt = db.GetData(sql, p);

            if (dt.Rows.Count > 0)
            {
                // Reset mật khẩu về '123' và đánh dấu phải đổi pass ở lần đăng nhập tới
                string updateSql = "UPDATE ACCOUNT SET Password = '123', IsFirstLogin = 1 WHERE User_Name = @u";
                db.ExecuteNonQuery(updateSql, new SqlParameter[] { new SqlParameter("@u", user) });

                MessageBox.Show("Xác thực thành công!\nMật khẩu của bạn đã được reset về: 123\nHãy dùng mật khẩu này để đăng nhập lại.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Thông tin xác thực không chính xác. Vui lòng kiểm tra lại!");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
