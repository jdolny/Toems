using System;
using Toems_Common.Entity;
using System.IO;
using System.Collections.Generic;

namespace Toems_FrontEnd.BasePages
{
    public class Users : PageBaseMaster
    {
        public EntityToemsUser ToemsUser { get; set; }
        public EntityToemsUserGroup ToemsUserGroup { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ToemsUser = !string.IsNullOrEmpty(Request["userid"])
                ? Call.ToemsUserApi.Get(Convert.ToInt32(Request.QueryString["userid"]))
                : null;
            //Don't Check Authorization here for admin because reset pass for users won't work.
            ToemsUserGroup = !string.IsNullOrEmpty(Request["groupid"])
                ? Call.UserGroupApi.Get(Convert.ToInt32(Request.QueryString["groupid"]))
                : null;
        }

        public List<string> GetThemes()
        {
            var themes = new List<string>();
            try
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content", "css", "themes");
                var directories = Directory.GetDirectories(basePath);
                foreach (var directory in directories)
                {
                    themes.Add(Path.GetFileName(directory));
                }
            }
            catch
            {
                //ignored
            }
           
            return themes;

        }
    }
}