﻿using System.Collections.Generic;

namespace LastSeenWeb.Core
{
	public class ApplicationSettings
	{
		public string ApplicationName { get; set; } = "Last Seen";
		public string Domain { get; set; } = "lastseen.azurewebsites.net";
		public int ItemsPerPage { get; set; } = 12;
		public List<(string, string)> NavBarLoggedOut { get; set; } = new List<(string, string)>
		{
			("/Auth/Register","Register"),
			("/Auth/Login","Log in")
		};
		public List<(string, string)> NavBarLoggedIn { get; set; } = new List<(string, string)>
		{
			("/Auth/Logout","Log out")
		};
	}
}
