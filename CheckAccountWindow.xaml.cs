public partial class CheckAccountWindow : Window
{
    public CheckAccountWindow()
    {
        InitializeComponent();
    }

    // Cho phép người dùng nhấn giữ chuột vào bất cứ đâu để di chuyển cửa sổ
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.DragMove();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        // Code mở màn hình Login ở đây
        this.DialogResult = true; 
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        RegisterPatientWindow reg = new RegisterPatientWindow();
        reg.Show();
        this.Close();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
