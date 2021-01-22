using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class Registration : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Web.UI.ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        LoadingImage.Attributes.CssStyle.Add("opacity", "0");
        submit.Attributes.CssStyle.Add("cursor", "allowed");
    }

    protected void submit_Click(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid)
        {
            
            switch(IsUserAndEmailExists(Server.HtmlEncode(TextBoxUN.Text), Server.HtmlEncode(TextBoxEmail.Text)))
            {
                case 0:
                    AddUser();
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                    break;
                case -1:
                    InfoLabel.Text = "<p style = 'color:red'>Both email & usermane are exists<p>";
                    break;
                case -2:
                    InfoLabel.Text = "<p style = 'color:red'>Email Exists<p>";
                    break;
                case -3:
                    InfoLabel.Text = "<p style = 'color:red'>Username Exists<p>";
                    break;
                default:
                    InfoLabel.Text = "<p style = 'color:red'>Something went wrong!! Please Try again later<p>";
                    break;
            }  

        }
        else
        {
            LoadingImage.Attributes.CssStyle.Add("opacity", "0");
            submit.Attributes.CssStyle.Add("cursor", "allowed");
        }

    }

    protected void AddUser()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;
        SqlConnection sqlConnect = new SqlConnection(connectionString);
        
        try
        {      
            
            sqlConnect.Open();

            string SQLCommand = "Insert into UserDatabase (Username,Email,Password,Country) values(@username, @email, @pass, @country)";
            SqlCommand sqlcommand = new SqlCommand(SQLCommand, sqlConnect);

            sqlcommand.Parameters.AddWithValue("@username", Server.HtmlEncode(TextBoxUN.Text));
            sqlcommand.Parameters.AddWithValue("@email", Server.HtmlEncode(TextBoxEmail.Text));
            sqlcommand.Parameters.AddWithValue("@pass", Server.HtmlEncode(EncodePasswordToBase64(TextBoxPass.Text)));
            sqlcommand.Parameters.AddWithValue("@country", Server.HtmlEncode(DropDownListCountry.SelectedValue));

            sqlcommand.ExecuteNonQuery();
            sqlConnect.Close();
            
        }
        catch(Exception ex)
        {
            InfoLabel.Text = "<p style='color:red' > Error: "+ex.Message+"</p>";
        }
        finally
        {
            sqlConnect.Close();
        }

    }


    protected int IsUserAndEmailExists(string username, string email)
    {
        string stringconnection = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;
        SqlConnection sqlconn = new SqlConnection(stringconnection);

        try
        {
            string command_string = "ProcIsUserExist @Uname, @EId";
            SqlCommand sqlcom = new SqlCommand(command_string, sqlconn);
            sqlcom.Parameters.AddWithValue("@Uname", username);
            sqlcom.Parameters.AddWithValue("@Eid", email);

            sqlconn.Open();
            int n = Convert.ToInt32(sqlcom.ExecuteScalar());
            sqlconn.Close();
            return n;
        }
        catch (Exception ex)
        {
            InfoLabel.Text = "<p style='color:red' > Error: " + ex.Message + "</p>";
            return -5;
        }
        finally
        {
            sqlconn.Close();
        }
    }

    public static string EncodePasswordToBase64(string password)
    {
        try
        {
            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in base64Encode" + ex.Message);
        }
    } 

    public string DecodeFrom64(string encodedData)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        System.Text.Decoder utf8Decode = encoder.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(encodedData);
        int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        string result = new String(decoded_char);
        return result;
    }
}
