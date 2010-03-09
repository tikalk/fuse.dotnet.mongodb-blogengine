#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BlogEngine.Core;

#endregion

public partial class admin_Pages_referrers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (BlogSettings.Instance.EnableReferrerTracking)
            {
                BindDays();
                BindReferrers();
            }
            else
            {
                ddlDays.Enabled = false;
            }

            txtNumberOfDays.Text = BlogSettings.Instance.NumberOfReferrerDays.ToString();
            cbEnableReferrers.Checked = BlogSettings.Instance.EnableReferrerTracking;
        }

        btnSave.Click += new EventHandler(btnSave_Click);
        btnSaveTop.Click += new EventHandler(btnSave_Click);

        ddlDays.SelectedIndexChanged += new EventHandler(ddlDays_SelectedIndexChanged);
        cbEnableReferrers.CheckedChanged += new EventHandler(cbEnableReferrers_CheckedChanged);
        Page.Title = Resources.labels.referrers;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        int days;
        if (int.TryParse(txtNumberOfDays.Text, out days))
        {
            BlogSettings.Instance.NumberOfReferrerDays = days;

            BlogSettings.Instance.Save();
        }
        Response.Redirect(Request.RawUrl, true);
    }

    private void cbEnableReferrers_CheckedChanged(object sender, EventArgs e)
    {
        if (cbEnableReferrers.Checked)
        {
            BindDays();
            BindReferrers();
        }
        else
        {
            ddlDays.Enabled = false;
        }

        BlogSettings.Instance.EnableReferrerTracking = cbEnableReferrers.Checked;
        BlogSettings.Instance.Save();
    }

    void ddlDays_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindReferrers();
    }

    private void BindDays()
    {
        ddlDays.DataSource = Referrer.ReferrersByDay.Keys;
        ddlDays.DataBind();
        foreach (ListItem item in ddlDays.Items)
        {
            if (item.Text == DateTime.Today.ToShortDateString())
            {
                item.Selected = true;
            }
        }
    }

    private void BindReferrers()
    {
        if (ddlDays.SelectedIndex >= 0 & Referrer.Referrers.Count > 0)
        {
            List<Referrer> referrers = Referrer.ReferrersByDay[DateTime.Parse(ddlDays.SelectedItem.Text)];
            if (referrers != null)
            {
                DataTable table = new DataTable();
                table.Columns.Add("url", typeof(string));
                table.Columns.Add("shortUrl", typeof(string));
                table.Columns.Add("target", typeof(string));
                table.Columns.Add("shortTarget", typeof(string));
                table.Columns.Add("hits", typeof(int));

                DataTable spamTable = table.Clone();

                DataRow tableRow;
                foreach (Referrer refer in referrers)
                {
                    if (refer.PossibleSpam)
                    {
                        tableRow = spamTable.NewRow();
                        populateRow(tableRow, refer);
                        spamTable.Rows.Add(tableRow);
                    }
                    else
                    {
                        tableRow = table.NewRow();
                        populateRow(tableRow, refer);
                        table.Rows.Add(tableRow);
                    }
                }

                BindTable(table, grid);
                BindTable(spamTable, spamGrid);
            }
        }
    }

    private void populateRow(DataRow tableRow, Referrer refer)
    {
        tableRow["url"] = Server.HtmlEncode(refer.ReferrerUrl.ToString());
        tableRow["shortUrl"] = MakeShortUrl(refer.ReferrerUrl.ToString());
        tableRow["target"] = Server.HtmlEncode(refer.Url.ToString());
        tableRow["shortTarget"] = MakeShortUrl(refer.Url.ToString());
        tableRow["hits"] = refer.Count;
    }

    private void BindTable(DataTable table, GridView grid)
    {
        string total = table.Compute("sum(hits)", null).ToString();

        DataView view = new DataView(table);
        view.Sort = "hits desc";

        grid.DataSource = view;
        grid.DataBind();

        if (grid.Rows.Count > 0)
        {
            grid.FooterRow.Cells[0].Text = "Total";
            grid.FooterRow.Cells[grid.FooterRow.Cells.Count - 1].Text = total;
        }

        PaintRows(grid, 3);
    }

    private string MakeShortUrl(string url)
    {
        if (url.Length > 150)
            return url.Substring(0, 150) + "...";

        return Server.HtmlEncode(url.Replace("http://", string.Empty).Replace("www.", string.Empty));
    }

    /// <summary>
    /// Paints the background color of the alternate rows
    /// in the gridview.
    /// </summary>
    private void PaintRows(GridView grid, int alternateRows)
    {
        if (grid.Rows.Count == 0)
            return;

        int count = 0;
        for (int i = 0; i < grid.Controls[0].Controls.Count - 1; i++)
        {
            if (count > alternateRows)
                (grid.Controls[0].Controls[i] as WebControl).CssClass = "alt";

            count++;

            if (count == alternateRows + alternateRows + 1)
                count = 1;
        }
    }
}
