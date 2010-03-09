using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using BlogEngine.Core;

namespace Tikal
{
    partial class MongoDBBlogProvider
	{
		public override AuthorProfile SelectProfile(string id)
		{
			string fileName = _Folder + "profiles" + Path.DirectorySeparatorChar + id.ToString() + ".xml";
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			AuthorProfile profile = new AuthorProfile(id);

			if (doc.SelectSingleNode("//DisplayName") != null)
				profile.DisplayName = doc.SelectSingleNode("//DisplayName").InnerText;

			if (doc.SelectSingleNode("//FirstName") != null)
				profile.FirstName = doc.SelectSingleNode("//FirstName").InnerText;

			if (doc.SelectSingleNode("//MiddleName") != null)
				profile.MiddleName = doc.SelectSingleNode("//MiddleName").InnerText;

			if (doc.SelectSingleNode("//LastName") != null)
				profile.LastName = doc.SelectSingleNode("//LastName").InnerText;

			//profile.Address1 = doc.SelectSingleNode("//Address1").InnerText;
			//profile.Address2 = doc.SelectSingleNode("//Address2").InnerText;
			if (doc.SelectSingleNode("//CityTown") != null)
				profile.CityTown = doc.SelectSingleNode("//CityTown").InnerText;

			if (doc.SelectSingleNode("//RegionState") != null)
				profile.RegionState = doc.SelectSingleNode("//RegionState").InnerText;

			if (doc.SelectSingleNode("//Country") != null)
				profile.Country = doc.SelectSingleNode("//Country").InnerText;

			if (doc.SelectSingleNode("//Birthday") != null)
			{
				DateTime date;
				if (DateTime.TryParse(doc.SelectSingleNode("//Birthday").InnerText, out date))
					profile.Birthday = date;
			}

			if (doc.SelectSingleNode("//AboutMe") != null)
				profile.AboutMe = doc.SelectSingleNode("//AboutMe").InnerText;

			if (doc.SelectSingleNode("//PhotoURL") != null)
				profile.PhotoURL = doc.SelectSingleNode("//PhotoURL").InnerText;

			if (doc.SelectSingleNode("//Company") != null)
				profile.Company = doc.SelectSingleNode("//Company").InnerText;

			if (doc.SelectSingleNode("//EmailAddress") != null)
				profile.EmailAddress = doc.SelectSingleNode("//EmailAddress").InnerText;

			if (doc.SelectSingleNode("//PhoneMain") != null)
				profile.PhoneMain = doc.SelectSingleNode("//PhoneMain").InnerText;

			if (doc.SelectSingleNode("//PhoneMobile") != null)
				profile.PhoneMobile = doc.SelectSingleNode("//PhoneMobile").InnerText;

			if (doc.SelectSingleNode("//PhoneFax") != null)
				profile.PhoneFax = doc.SelectSingleNode("//PhoneFax").InnerText;

            if (doc.SelectSingleNode("//IsPrivate") != null)
                profile.IsPrivate = doc.SelectSingleNode("//IsPrivate").InnerText == "true";

			//page.DateCreated = DateTime.Parse(doc.SelectSingleNode("page/datecreated").InnerText, CultureInfo.InvariantCulture);
			//page.DateModified = DateTime.Parse(doc.SelectSingleNode("page/datemodified").InnerText, CultureInfo.InvariantCulture);

			return profile;
		}

		public override void InsertProfile(AuthorProfile profile)
		{
			if (!Directory.Exists(_Folder + "profiles"))
				Directory.CreateDirectory(_Folder + "profiles");

			string fileName = _Folder + "profiles" + Path.DirectorySeparatorChar + profile.Id + ".xml";
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(fileName, settings))
			{
				writer.WriteStartDocument(true);
				writer.WriteStartElement("profileData");

				writer.WriteElementString("DisplayName", profile.DisplayName);
				writer.WriteElementString("FirstName", profile.FirstName);
				writer.WriteElementString("MiddleName", profile.MiddleName);
				writer.WriteElementString("LastName", profile.LastName);

				writer.WriteElementString("CityTown", profile.CityTown);
				writer.WriteElementString("RegionState", profile.RegionState);
				writer.WriteElementString("Country", profile.Country);

				writer.WriteElementString("Birthday", profile.Birthday.ToString("yyyy-MM-dd"));
				writer.WriteElementString("AboutMe", profile.AboutMe);
				writer.WriteElementString("PhotoURL", profile.PhotoURL);

				writer.WriteElementString("Company", profile.Company);
				writer.WriteElementString("EmailAddress", profile.EmailAddress);
				writer.WriteElementString("PhoneMain", profile.PhoneMain);
				writer.WriteElementString("PhoneMobile", profile.PhoneMobile);
				writer.WriteElementString("PhoneFax", profile.PhoneFax);

                writer.WriteElementString("IsPrivate", profile.IsPrivate.ToString());

				writer.WriteEndElement();
			}
		}

		public override void UpdateProfile(AuthorProfile profile)
		{
			InsertProfile(profile);
		}

		public override void DeleteProfile(AuthorProfile profile)
		{
			string fileName = _Folder + "profiles" + Path.DirectorySeparatorChar + profile.Id + ".xml";
			if (File.Exists(fileName))
				File.Delete(fileName);

			if (AuthorProfile.Profiles.Contains(profile))
				AuthorProfile.Profiles.Remove(profile);
		}

		public override List<AuthorProfile> FillProfiles()
		{
			string folder = _Folder + "profiles" + Path.DirectorySeparatorChar;
			List<AuthorProfile> profiles = new List<AuthorProfile>();

			foreach (string file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly))
			{
				FileInfo info = new FileInfo(file);
				string username = info.Name.Replace(".xml", string.Empty);
				AuthorProfile profile = AuthorProfile.Load(username);
				profiles.Add(profile);
			}

			return profiles;
		}
	}
}
