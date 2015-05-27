using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    SqlConnection conn = new SqlConnection(@"Server=aa7885c7-b2f0-457d-8300-a4a500ae08c3.sqlserver.sequelizer.com;Database=dbaa7885c7b2f0457d8300a4a500ae08c3;User ID=qskbhfwqtngqpqek;Password=TepLYeWuqbWcKq4jv2X8sTHUf6RBRJndwusjbvVH5fWrxXcCKesSWkgaCCE3vtLN;");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            FillGridView();
            BindDDLState();
        }
    }

    private void BindDDLState()
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("select * from state", conn);
        ddlState.DataSource = cmd.ExecuteReader();
        ddlState.DataTextField = "state";
        ddlState.DataValueField = "id";
        ddlState.DataBind();
        conn.Close();

        ListItem li = new ListItem();
        li.Text = "--Select--";
        li.Value = "0";
        ddlState.Items.Insert(0, li);
    }

    private void FillGridView()
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("select * from myreg", conn);
        GridView1.DataSource = cmd.ExecuteReader();
        GridView1.DataBind();
        conn.Close();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string fname = FileUploadProfilePic.FileName;
        string fextension = Path.GetExtension(fname);
        if (FileUploadProfilePic.HasFiles)
        {
           // string fname = FileUploadProfilePic.FileName;
           // string fextension = Path.GetExtension(fname);
            if (fextension == ".jpg")
            {
                FileUploadProfilePic.SaveAs(Server.MapPath("Files/") + fname);
                lbpic.Text = "File Uploaded Successfully..";
            }

            else
            {
                lbpic.Text = "Please Upload JPG Files Only..";
            }
        }
        else
        {
            lbpic.Text = "Please Upload File First...";
        }

        conn.Open();
        SqlCommand sc = new SqlCommand("insert into myreg values(@name,@state,@city,@area,@pic)", conn);
        sc.Parameters.AddWithValue("@name", txtName.Text);
        sc.Parameters.AddWithValue("@state", ddlState.SelectedItem.Text);
        sc.Parameters.AddWithValue("@city", ddlCity.SelectedItem.Text);
        sc.Parameters.AddWithValue("@area", ddlArea.SelectedItem.Text);
        sc.Parameters.AddWithValue("@pic", FileUploadProfilePic.FileBytes);
        sc.ExecuteNonQuery();
        conn.Close();
        FillGridView();


        
    }
    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("select * from city where state_id=@sid", conn);
        cmd.Parameters.AddWithValue("@sid", ddlState.SelectedItem.Value);
        ddlCity.DataSource = cmd.ExecuteReader();
        ddlCity.DataTextField = "city";
        ddlCity.DataValueField = "c_id";
        ddlCity.DataBind();
        conn.Close();

        ListItem li = new ListItem();
        li.Text = "--Select--";
        li.Value = "0";
        ddlCity.Items.Insert(0, li);
    }
    protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("select * from Area where c_id=@cid", conn);
        cmd.Parameters.AddWithValue("@cid", ddlCity.SelectedItem.Value);
        ddlArea.DataSource = cmd.ExecuteReader();
        ddlArea.DataTextField = "area_name";
        ddlArea.DataValueField = "a_id";
        ddlArea.DataBind();
        conn.Close();

        ListItem li = new ListItem();
        li.Text = "--Select--";
        li.Value = "0";
        ddlArea.Items.Insert(0, li);
    }
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("delete from myreg where id=@id", conn);
        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value));
        cmd.ExecuteNonQuery();
        conn.Close();
        FillGridView();
    }
    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        FillGridView();
    }
    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView1.EditIndex = -1;
        FillGridView();
    }
    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("update myreg set name=@nm,state=@st,city=@ct,area=@ar,@pic where id=@id", conn);
        //TextBox id = (TextBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0];
        TextBox name = (TextBox)GridView1.Rows[e.RowIndex].FindControl("TextBox1");
        TextBox state = (TextBox)GridView1.Rows[e.RowIndex].FindControl("TextBox2");
        TextBox city = (TextBox)GridView1.Rows[e.RowIndex].FindControl("TextBox3");
        TextBox area = (TextBox)GridView1.Rows[e.RowIndex].FindControl("TextBox4");
        FileUpload fup = (FileUpload)GridView1.Rows[e.RowIndex].FindControl("FileUploadUpdate");
       // Image im = (Image)GridView1.Rows[e.RowIndex].FindControl("Image2");
        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value));
        cmd.Parameters.AddWithValue("@nm", name.Text);
        cmd.Parameters.AddWithValue("@st", state.Text);
        cmd.Parameters.AddWithValue("@ct", city.Text);
        cmd.Parameters.AddWithValue("@ar", area.Text);
        cmd.Parameters.AddWithValue("@pic", fup.FileBytes);
        cmd.ExecuteNonQuery();
        conn.Close();
        GridView1.EditIndex = -1;
        FillGridView();
    }
}