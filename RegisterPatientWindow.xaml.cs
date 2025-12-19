protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
{
    base.OnMouseLeftButtonDown(e);
    if (e.ButtonState == MouseButtonState.Pressed)
        this.DragMove();
}
private void btnDoRegister_Click(object sender, RoutedEventArgs e)
{
    // 1. Kiểm tra mật khẩu khớp nhau
    if (txtRegPass.Password != txtRegConfirmPass.Password) {
        MessageBox.Show("Mật khẩu xác nhận không khớp!");
        return;
    }

    try {
        // 2. Tạo ID tự động (Ví dụ: PAT + số ngẫu nhiên)
        string newID = "PAT" + new Random().Next(100, 999).ToString();

        // 3. Insert vào bảng PATIENT
        string sqlPatient = @"INSERT INTO PATIENT (Patient_ID, Patient_Name, Gender, Day_Of_Birth, Phone, CID, Address, Curr_Condition) 
                             VALUES (@id, @name, @gender, @dob, @phone, @cid, @addr, @cond)";
        
        SqlParameter[] p1 = {
            new SqlParameter("@id", newID),
            new SqlParameter("@name", txtRegName.Text),
            new SqlParameter("@gender", cbRegGender.Text),
            new SqlParameter("@dob", dpRegDOB.SelectedDate ?? DateTime.Now),
            new SqlParameter("@phone", txtRegPhone.Text),
            new SqlParameter("@cid", txtRegCID.Text),
            new SqlParameter("@addr", ""), // Để trống cho người dùng cập nhật sau
            new SqlParameter("@cond", "Mới đăng ký")
        };

        // 4. Insert vào bảng ACCOUNTS (Giả sử bảng ACCOUNTS có cột Username, Password, Role, OwnerID)
        string sqlAccount = "INSERT INTO ACCOUNTS (Username, Password, Role, OwnerID) VALUES (@user, @pass, 'Patient', @owner)";
        SqlParameter[] p2 = {
            new SqlParameter("@user", txtRegUser.Text),
            new SqlParameter("@pass", txtRegPass.Password),
            new SqlParameter("@owner", newID)
        };

        if (db.Execute(sqlPatient, p1) && db.Execute(sqlAccount, p2)) {
            MessageBox.Show("Đăng ký thành công! Hãy đăng nhập bằng tài khoản vừa tạo.");
            this.Close();
        }
    } catch (Exception ex) {
        MessageBox.Show("Lỗi: " + ex.Message);
    }
}
