using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class User_controls_xmanager_Parameters : UserControl
{
  #region Private members
  private string _extensionName = string.Empty;
  protected ExtensionSettings _settings;
  #endregion

  public string SettingName { get { return _extensionName; } set { _extensionName = value; } }
  public bool GenerateDeleteButton = true;
  public bool GenerateEditButton = true;

  /// <summary>
  /// Dynamically loads form controls or
  /// data grid and binds data to controls
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  protected void Page_Load(object sender, EventArgs e)
  {
    _extensionName = ID;
    _settings = ExtensionManager.GetSettings(_extensionName);

    GenerateDeleteButton = _settings.ShowDelete;
    GenerateEditButton = _settings.ShowEdit;

    if(_settings.ShowAdd)
      CreateFormFields();

    if (!Page.IsPostBack)
    {
      if (_settings.IsScalar)
      {
        BindScalar();
      }
      else
      {
        CreateTemplatedGridView();
        BindGrid();
      }
    }

    if (_settings.IsScalar)
    {
      btnAdd.Text = Resources.labels.save;
    }
    else
    {
      if (_settings.ShowAdd)
      {
        grid.RowEditing += grid_RowEditing;
        grid.RowUpdating += grid_RowUpdating;
        grid.RowCancelingEdit += delegate { Response.Redirect(Request.RawUrl); };
        grid.RowDeleting += grid_RowDeleting;
        btnAdd.Text = Resources.labels.add;
      }
      else
        btnAdd.Visible = false;
    }
    btnAdd.Click += btnAdd_Click;
  }

  /// <summary>
  /// Handels adding a new value(s)
  /// </summary>
  /// <param name="sender">Button</param>
  /// <param name="e">Arguments</param>
  void btnAdd_Click(object sender, EventArgs e)
  {
    if (IsValidForm())
    {
      foreach (Control ctl in phAddForm.Controls)
      {
        if (ctl.GetType().Name == "TextBox")
        {
          TextBox txt = (TextBox)ctl;

          if (_settings.IsScalar)
            _settings.UpdateScalarValue(txt.ID, txt.Text);
          else
            _settings.AddValue(txt.ID, txt.Text);
        }
        else if (ctl.GetType().Name == "CheckBox")
        {
          CheckBox cbx = (CheckBox)ctl;
          _settings.UpdateScalarValue(cbx.ID, cbx.Checked.ToString());
        }
        else if (ctl.GetType().Name == "DropDownList")
        {
          DropDownList dd = (DropDownList)ctl;
          _settings.UpdateSelectedValue(dd.ID, dd.SelectedValue);
        }
        else if (ctl.GetType().Name == "ListBox")
        {
          ListBox lb = (ListBox)ctl;
          _settings.UpdateSelectedValue(lb.ID, lb.SelectedValue);
        }
        else if (ctl.GetType().Name == "RadioButtonList")
        {
          RadioButtonList rbl = (RadioButtonList)ctl;
          _settings.UpdateSelectedValue(rbl.ID, rbl.SelectedValue);
        }
      }
      ExtensionManager.SaveSettings(_extensionName, _settings);
      if (_settings.IsScalar)
      {
        InfoMsg.InnerHtml = "The values has been saved";
        InfoMsg.Visible = true;
      }
      else
      {
        BindGrid();
      }
    }
  }

  /// <summary>
  /// Deliting row in the data grid
  /// </summary>
  /// <param name="sender">Grid View</param>
  /// <param name="e">Arguments</param>
  void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
  {
    int paramIndex = ParameterValueIndex(sender, e.RowIndex);

    foreach (ExtensionParameter par in _settings.Parameters)
    {
      par.DeleteValue(paramIndex);
    }
    ExtensionManager.SaveSettings(_extensionName, _settings);
    Response.Redirect(Request.RawUrl);
  }

  /// <summary>
  /// Updating row in the grid
  /// </summary>
  /// <param name="sender">Grid View</param>
  /// <param name="e">Event args</param>
  void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
  {
    // extract and store input values in the collection
    StringCollection updateValues = new StringCollection();
    foreach (DataControlFieldCell cel in grid.Rows[e.RowIndex].Controls)
    {
      foreach (Control ctl in cel.Controls)
      {
        if (ctl.GetType().Name == "TextBox")
        {
          TextBox txt = (TextBox)ctl;
          updateValues.Add(txt.Text);
        }
      }
    }

    int paramIndex = ParameterValueIndex(sender, e.RowIndex);

    for (int i = 0; i < _settings.Parameters.Count; i++)
    {
      string parName = _settings.Parameters[i].Name;
      if (_settings.IsRequiredParameter(parName) && string.IsNullOrEmpty(updateValues[i]))
      {
        // throw error if required field is empty
        ErrorMsg.InnerHtml = "\"" + _settings.GetLabel(parName) + "\" is a required field";
        ErrorMsg.Visible = true;
        e.Cancel = true;
        return;
      }
      else if (parName == _settings.KeyField && _settings.IsKeyValueExists(updateValues[i]))
      {
        // check if key value was changed; if not, it's ok to update
        if (!_settings.IsOldValue(parName, updateValues[i], paramIndex))
        {
          // trying to update key field with value that already exists
          ErrorMsg.InnerHtml = "\"" + updateValues[i] + "\" is already exists";
          ErrorMsg.Visible = true;
          e.Cancel = true;
          return;
        }

      }
      else
        _settings.Parameters[i].Values[paramIndex] = updateValues[i];
    }

    ExtensionManager.SaveSettings(_extensionName, _settings);
    Response.Redirect(Request.RawUrl);
  }

  /// <summary>
  /// Returns index of the parameter calculated 
  /// based on the page number and size
  /// </summary>
  /// <param name="sender">GridView object</param>
  /// <param name="rowindex">Index of the row in the grid</param>
  /// <returns>Index of the parameter</returns>
  private static int ParameterValueIndex(object sender, int rowindex)
  {
      int paramIndex = rowindex;
      GridView gv = (GridView)sender;
      if (gv.PageIndex > 0)
      {
          paramIndex = gv.PageIndex * gv.PageSize + rowindex;
      }
      return paramIndex;
  }

  /// <summary>
  /// Editing data in the data grid
  /// </summary>
  /// <param name="sender">Grid View</param>
  /// <param name="e">Event args</param>
  void grid_RowEditing(object sender, GridViewEditEventArgs e)
  {
    grid.EditIndex = e.NewEditIndex;
    BindGrid();
  }

  /// <summary>
  /// Handles page changing event in the data grid
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
  {
      grid.PageIndex = e.NewPageIndex;
      grid.DataSource = _settings.GetDataTable();
      grid.DataBind();
  }

  /// <summary>
  /// Binds settings values formatted as
  /// data table to grid view
  /// </summary>
  private void BindGrid()
  {
    if (GenerateEditButton)
      grid.AutoGenerateEditButton = true;

    if (GenerateDeleteButton)
      grid.AutoGenerateDeleteButton = true;

    grid.DataKeyNames = new string[] { _settings.KeyField };
    grid.DataSource = _settings.GetDataTable();
    grid.DataBind();
  }

  /// <summary>
  /// Binds single value parameters
  /// to text boxes
  /// </summary>
  private void BindScalar()
  {
    foreach (ExtensionParameter par in _settings.Parameters)
    {
      foreach (Control ctl in phAddForm.Controls)
      {
        if (ctl.GetType().Name == "CheckBox")
        {
          CheckBox cbx = (CheckBox)ctl;
          if (cbx.ID.ToLower() == par.Name.ToLower())
          {
            if (par.Values != null && par.Values.Count > 0)
            {
              cbx.Checked = bool.Parse(par.Values[0]);
            }
          }
        }

        if (ctl.GetType().Name == "TextBox")
        {
          TextBox txt = (TextBox)ctl;
          if (txt.ID.ToLower() == par.Name.ToLower())
          {
            if (par.Values != null)
            {
              if (par.Values.Count == 0)
                txt.Text = string.Empty;
              else
                txt.Text = par.Values[0];
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Creates template for data grid view
  /// </summary>
  void CreateTemplatedGridView()
  {
    foreach (ExtensionParameter par in _settings.Parameters)
    {
      BoundField col = new BoundField();
      col.DataField = par.Name;
      col.HeaderText = par.Name;
      grid.Columns.Add(col);
    }
  }

  /// <summary>
  /// Dynamically add controls to the form
  /// </summary>
  void CreateFormFields()
  {
    foreach (ExtensionParameter par in _settings.Parameters)
    {
      ErrorMsg.InnerHtml = string.Empty;
      ErrorMsg.Visible = false;
      InfoMsg.InnerHtml = string.Empty;
      InfoMsg.Visible = false;

      // add label
      if (par.ParamType != ParameterType.Boolean)
      {
        AddLabel(par.Label, "");
      }

      if (par.ParamType == ParameterType.Boolean)
      {
        // add checkbox
        CheckBox cb = new CheckBox();
        cb.Checked = false;
        cb.ID = par.Name;
        cb.CssClass = "mgrCheck";
        phAddForm.Controls.Add(cb);
        AddLabel(par.Label, "mgrCheckLbl");
      }
      else if (par.ParamType == ParameterType.DropDown)
      {
        // add dropdown
        DropDownList dd = new DropDownList();
        foreach (string item in par.Values)
        {
          dd.Items.Add(item);
        }
        dd.SelectedValue = par.SelectedValue;
        dd.ID = par.Name;
        dd.Width = 250;
        phAddForm.Controls.Add(dd);
      }
      else if (par.ParamType == ParameterType.ListBox)
      {
        ListBox lb = new ListBox();
        lb.Rows = par.Values.Count;
        foreach (string item in par.Values)
        {
          lb.Items.Add(item);
        }
        lb.SelectedValue = par.SelectedValue;
        lb.ID = par.Name;
        lb.Width = 250;
        phAddForm.Controls.Add(lb);
      }
      else if (par.ParamType == ParameterType.RadioGroup)
      {
        RadioButtonList rbl = new RadioButtonList();
        foreach (string item in par.Values)
        {
          rbl.Items.Add(item);
        }
        rbl.SelectedValue = par.SelectedValue;
        rbl.ID = par.Name;
        rbl.RepeatDirection = RepeatDirection.Horizontal;
        rbl.CssClass = "mgrRadioList";
        phAddForm.Controls.Add(rbl);
      }
      else
      {
        // add textbox
        TextBox bx = new TextBox();
        bx.Text = string.Empty;
        bx.ID = par.Name;
        bx.Width = new Unit(250);
        bx.MaxLength = par.MaxLength;
        phAddForm.Controls.Add(bx);
      }

      Literal br2 = new Literal();
      br2.Text = "<br />";
      phAddForm.Controls.Add(br2);
    }
  }

  private void AddLabel(string txt, string cls)
  {
    Label lbl = new Label();
    lbl.Width = new Unit("250");
    lbl.Text = txt;
    if (!string.IsNullOrEmpty(cls)) lbl.CssClass = cls;
    phAddForm.Controls.Add(lbl);

    Literal br = new Literal();
    br.Text = "<br />";
    phAddForm.Controls.Add(br);
  }

  /// <summary>
  /// Validate the form
  /// </summary>
  /// <returns>True if valid</returns>
  private bool IsValidForm()
  {
    bool rval = true;
    ErrorMsg.InnerHtml = string.Empty;
    foreach (Control ctl in phAddForm.Controls)
    {
      if (ctl.GetType().Name == "TextBox")
      {
        TextBox txt = (TextBox)ctl;
        // check if required
        if (_settings.IsRequiredParameter(txt.ID) && string.IsNullOrEmpty(txt.Text.Trim()))
        {
          ErrorMsg.InnerHtml = "\"" + _settings.GetLabel(txt.ID) + "\" is a required field";
          ErrorMsg.Visible = true;
          rval = false;
          break;
        }
        // check data type
        if (!string.IsNullOrEmpty(txt.Text) && !ValidateType(txt.ID, txt.Text))
        {
          ErrorMsg.InnerHtml = "\"" + _settings.GetLabel(txt.ID) + "\" must be a " + _settings.GetParameterType(txt.ID);
          ErrorMsg.Visible = true;
          rval = false;
          break;
        }
        if (!_settings.IsScalar)
        {
          if (_settings.KeyField == (txt.ID) && _settings.IsKeyValueExists(txt.Text.Trim()))
          {
            ErrorMsg.InnerHtml = "\"" + txt.Text + "\" is already exists";
            ErrorMsg.Visible = true;
            rval = false;
            break;
          }
        }
      }
    }
    return rval;
  }

  protected bool ValidateType(string parameterName, object val)
  {
    bool retVal = true;
    try
    {
      switch (_settings.GetParameterType(parameterName))
      {
        case ParameterType.Boolean:
          bool.Parse(val.ToString());
          break;
        case ParameterType.Integer:
          int.Parse(val.ToString());
          break;
        case ParameterType.Long:
          long.Parse(val.ToString());
          break;
        case ParameterType.Float:
          float.Parse(val.ToString());
          break;
        case ParameterType.Double:
          double.Parse(val.ToString());
          break;
        case ParameterType.Decimal:
          decimal.Parse(val.ToString());
          break;
      }
    }
    catch (Exception)
    {
      retVal = false;
    }
    return retVal;
  }

  /// <summary>
  /// Gets a handle on grid data just before
  /// bound them to grid view
  /// </summary>
  /// <param name="sender">Grid view</param>
  /// <param name="e">Event args</param>
  protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    AddConfirmDelete((GridView)sender, e);
  }

  /// <summary>
  /// Adds confirmation box to delete buttons
  /// in the data grid
  /// </summary>
  /// <param name="gv">Data grid view</param>
  /// <param name="e">Event args</param>
  protected static void AddConfirmDelete(GridView gv, GridViewRowEventArgs e)
  {
    if (e.Row.RowType != DataControlRowType.DataRow)
      return;

    foreach (DataControlFieldCell dcf in e.Row.Cells)
    {
      if (string.IsNullOrEmpty(dcf.Text.Trim()))
      {
        foreach (Control ctrl in dcf.Controls)
        {
          LinkButton deleteButton = ctrl as LinkButton;
          if (deleteButton != null && deleteButton.Text == "Delete")
          {
            deleteButton.Attributes.Add("onClick", "return confirm('Are you sure you want to delete this row?');");
            break;
          }
        }
        break;
      }
    }
  }
}
