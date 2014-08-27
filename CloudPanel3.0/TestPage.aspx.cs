using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Lync;
using CloudPanel.Modules.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Lync2013HPv2 lync = null;

            try
            {
                lync = new Lync2013HPv2("lync.lab.local", @"LAB\Administrator", "Password1", Modules.Base.Class.Enumerations.ConnectionType.SSL, "DC.lab.local");

                LyncPlan p = new LyncPlan();
                p.EnableOutsideVoice = true;

                lync.Enable_Company("OU=LAR,OU=KNO,OU=Hosting,DC=lab,DC=local", "LAR", p);
            }
            catch (Exception ex)
            {
                Label1.Text = ex.ToString();
            }
        }
    }
}