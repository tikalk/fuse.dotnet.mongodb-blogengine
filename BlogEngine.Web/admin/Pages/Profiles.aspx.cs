#region Using

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Security;
using System.Web.UI.WebControls;
using BlogEngine.Core;
using Page = System.Web.UI.Page;

#endregion

public partial class admin_profiles : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			ClearFormControls();
			BindCountries();
			SetDDLUser();
			SetProfile(User.Identity.Name);
			dropdown.Visible = Page.User.IsInRole(BlogSettings.Instance.AdministratorRole);
		}

		lbSaveProfile.Text = Resources.labels.saveProfile;
		Page.Title = Resources.labels.profile;
	}

	private void SetProfile(string name)
	{
		AuthorProfile pc = AuthorProfile.GetProfile(name);
		if (pc != null)
		{
			cbIsPublic.Checked = pc.IsPrivate;
			tbDisplayName.Text = pc.DisplayName;
			tbFirstName.Text = pc.FirstName;
			tbMiddleName.Text = pc.MiddleName;
			tbLastName.Text = pc.LastName;

			if (pc.Birthday != DateTime.MinValue)
				tbBirthdate.Text = pc.Birthday.ToString("yyyy-MM-dd");

			tbPhotoUrl.Text = pc.PhotoURL;
			tbPhoneMain.Text = pc.PhoneMain;
			tbPhoneMobile.Text = pc.PhoneMobile;
			tbPhoneFax.Text = pc.PhoneFax;
			tbEmailAddress.Text = pc.EmailAddress;
			tbCityTown.Text = pc.CityTown;
			tbRegionState.Text = pc.RegionState;
			ddlCountry.SelectedValue = pc.Country;
			tbCompany.Text = pc.Company;
			tbAboutMe.Text = pc.AboutMe;
		}
		else
		{
			// Clear any data in the form controls remaining from the last profile selected.
			ClearFormControls();
		}

		// Sync the dropdownlist user with the selected profile user.  This is
		// particularily needed on the initial page load (!IsPostBack).
		ListItem SelectedUser = FindItemInListControlByValue(ddlUserList, name);
		if (SelectedUser != null)
			SelectedUser.Selected = true;

		// Store the selected profile name so changes are saved to this
		// profile and not another profile the user may later select and
		// forget to click the lbChangeUserProfile button for.
		ViewState["selectedProfile"] = name;
	}

	/// <summary>
	/// Returns the ListItem that has a value matching the Value passed in
	/// via a OrdinalIgnoreCase search.
	/// </summary>
	private ListItem FindItemInListControlByValue(ListControl control, string Value)
	{
		foreach (ListItem li in control.Items)
		{
			if (string.Equals(Value, li.Value, StringComparison.OrdinalIgnoreCase))
				return li;
		}
		return null;
	}

	private void ClearFormControls()
	{
		cbIsPublic.Checked = false;
		tbDisplayName.Text = string.Empty;
		tbFirstName.Text = string.Empty;
		tbMiddleName.Text = string.Empty;
		tbLastName.Text = string.Empty;
		tbBirthdate.Text = string.Empty;
		tbPhotoUrl.Text = string.Empty;
		tbPhoneMain.Text = string.Empty;
		tbPhoneMobile.Text = string.Empty;
		tbPhoneFax.Text = string.Empty;
		tbEmailAddress.Text = string.Empty;
		tbCityTown.Text = string.Empty;
		tbRegionState.Text = string.Empty;
		tbCompany.Text = string.Empty;
		tbAboutMe.Text = string.Empty;

		if (ddlCountry.Items.Count > 0)
			ddlCountry.SelectedIndex = 0;
		SetDefaultCountry();
	}

	private void SetDDLUser()
	{
		foreach (MembershipUser user in Membership.GetAllUsers())
		{
			ListItem li = new ListItem(user.UserName, user.UserName);
			ddlUserList.Items.Add(li);
		}
	}

	protected void lbSaveProfile_Click(object sender, EventArgs e)
	{
		string userProfileToSave = ViewState["selectedProfile"] as string;
		AuthorProfile pc = AuthorProfile.GetProfile(userProfileToSave);
		if (pc == null)
			pc = new AuthorProfile(userProfileToSave);

		pc.IsPrivate = cbIsPublic.Checked;
		pc.DisplayName = tbDisplayName.Text;
		pc.FirstName = tbFirstName.Text;
		pc.MiddleName = tbMiddleName.Text;
		pc.LastName = tbLastName.Text;

		DateTime date;
		if (DateTime.TryParse(tbBirthdate.Text, out date))
			pc.Birthday = date;

		pc.PhotoURL = tbPhotoUrl.Text;
		pc.PhoneMain = tbPhoneMain.Text;
		pc.PhoneMobile = tbPhoneMobile.Text;
		pc.PhoneFax = tbPhoneFax.Text;
		pc.EmailAddress = tbEmailAddress.Text;
		pc.CityTown = tbCityTown.Text;
		pc.RegionState = tbRegionState.Text;
		pc.Country = ddlCountry.SelectedValue;
		pc.Company = tbCompany.Text;
		pc.AboutMe = tbAboutMe.Text;

		pc.Save();
	}

	protected void lbChangeUserProfile_Click(object sender, EventArgs e)
	{
		SetProfile(ddlUserList.SelectedValue);
	}


	/// <summary>
	/// Binds the country dropdown list with countries retrieved
	/// from the .NET Framework.
	/// </summary>
	public void BindCountries()
	{
		StringDictionary dic = new StringDictionary();
		List<string> col = new List<string>();

		foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
		{
			RegionInfo ri = new RegionInfo(ci.Name);
			if (!dic.ContainsKey(ri.EnglishName))
				dic.Add(ri.EnglishName, ri.TwoLetterISORegionName.ToLowerInvariant());

			if (!col.Contains(ri.EnglishName))
				col.Add(ri.EnglishName);
		}

		// Add custom cultures
		if (!dic.ContainsValue("bd"))
		{
			dic.Add("Bangladesh", "bd");
			col.Add("Bangladesh");
		}

		col.Sort();

		ddlCountry.Items.Add(new ListItem("[Not specified]", ""));
		foreach (string key in col)
		{
			ddlCountry.Items.Add(new ListItem(key, dic[key]));
		}

		SetDefaultCountry();
	}

	private void SetDefaultCountry()
	{
		if (ddlCountry.SelectedIndex == 0 && Request.UserLanguages != null && Request.UserLanguages[0].Length == 5)
		{
            // Do a case-insensitive search for the country name.  Some browsers (like Chrome) report
            // their UserLanguages in uppercase.
            ListItem countryToSelect = FindItemInListControlByValue(ddlCountry, Request.UserLanguages[0].Substring(3));
            if (countryToSelect != null)
                ddlCountry.SelectedValue = countryToSelect.Value;
            SetFlagImageUrl();
		}
	}
	private void SetFlagImageUrl()
	{
		//if (!string.IsNullOrEmpty(ddlCountry.SelectedValue))
		//{
		//  imgFlag.ImageUrl = Utils.RelativeWebRoot + "pics/flags/" + ddlCountry.SelectedValue + ".png";
		//}
		//else
		//{
		//  imgFlag.ImageUrl = Utils.RelativeWebRoot + "pics/pixel.png";
		//}
	}
}