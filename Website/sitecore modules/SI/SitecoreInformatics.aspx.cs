using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SI
{
    public partial class SitecoreInformatics : System.Web.UI.Page
    {
        public string SubLayoutFolderPath = string.Empty;
        public string TemplateFolderPath = string.Empty;
        public string MediaFolderPath = string.Empty;
        public string layoutPath = string.Empty;
        public string templatePath = string.Empty;
        public int siteCount;
        public int subLayoutCount;
        public int templateCount;
        public int contentItemCount;
        public int mediaItemCount;
        public int userCount;

        Database DB;
        public Item SiteHome;
        Item[] templateItems;
        Item[] subLayouts = null;
        Item[] templates = null;
        Item[] subLayoutPageItems = null;
        System.Collections.Generic.List<string> systemSites = new System.Collections.Generic.List<string>() { "shell", "login", "admin", "service", "modules_shell", "modules_website", "scheduler", "system", "publisher" };
        public AnalyzeItem searchedItem = new AnalyzeItem();

        private class Sites
        {
            public string siteName { get; set; }
            public string hostName { get; set; }
            public string rootPath { get; set; }
            public string startItem { get; set; }
            public string startPath { get; set; }
            public bool cacheHtml { get; set; }
            public string database { get; set; }
            public string device { get; set; }
            public bool enableAnalytics { get; set; }
            public string language { get; set; }
            public string htmlCacheSize { get; set; }
            public string registryCacheSize { get; set; }
            public string viewStateCacheSize { get; set; }
            public string xslCacheSize { get; set; }
            public string filteredItemsCacheSize { get; set; }
        }

        public class AnalyzeItem
        {
            public string Icon { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Template { get; set; }
            public string IsPublished { get; set; }
            public string LastUpdated { get; set; }
            public string LastUpdatedBy { get; set; }
            public string ReferencePages { get; set; }
            public Image imgIcon { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txtAnalyzeItemSearch.Attributes.Add("onKeyPress", "callSearchOnEnter('" + btnAnalyzeItem.ClientID +"',event)");

                if (Sitecore.Context.User.IsAuthenticated == false)
                {
                    Response.Redirect("/sitecore/login");
                }

                DB = Sitecore.Context.Database;
                SiteHome = DB.Items[Sitecore.Context.Site.StartPath];
                lnkShowSiteOverview.Attributes.Add("class", "element brand");
                lnkShowSite.Attributes.Add("class", "element brand");
                lnkShowSubLayout.Attributes.Add("class", "element brand");
                lnkShowTemplate.Attributes.Add("class", "element brand");
                lnkShowUser.Attributes.Add("class", "element brand");
                lnkShowAnalyzeItem.Attributes.Add("class", "element brand");

                divSiteOverview.Visible = false;
                divSite.Visible = false;
                divSubLayout.Visible = false;
                divTemplate.Visible = false;
                divUsers.Visible = false;
                tblAnalyzeItem.Visible = false;
                divAnalyzeItemSearch.Visible = false;
                divError.Visible = false;

                TemplateFolderPath = Sitecore.Configuration.Settings.GetSetting("TemplateFolderPath");
                SubLayoutFolderPath = Sitecore.Configuration.Settings.GetSetting("SubLayoutFolderPath");
                MediaFolderPath = Sitecore.Configuration.Settings.GetSetting("MediaFolderPath");

                if (TemplateFolderPath == string.Empty)
                {
                    TemplateFolderPath = "/sitecore/templates/User Defined/" + SiteHome.Name;
                }

                if (SubLayoutFolderPath == string.Empty)
                {
                    SubLayoutFolderPath = "/sitecore/layout/Sublayouts/" + SiteHome.Name;
                }

                if (MediaFolderPath == string.Empty)
                {
                    MediaFolderPath = "/sitecore/media library/" + SiteHome.Name;
                }

                if (IsPostBack)
                {
                    string eTarget = Request.Params["__EVENTTARGET"].ToString();
                    if (eTarget.Equals(string.Empty) || eTarget.Equals("lnkShowSubLayout"))
                    {
                        populateSublayouts();
                    }
                    if (eTarget.Equals("lnkShowSiteOverview"))
                    {
                        getSiteOverviewCount();
                    }
                }
                else
                {
                    divSiteOverview.Visible = true;
                    lnkShowSiteOverview.Attributes.Add("class", "element brand active");
                    getSiteOverviewCount();
                }

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics: " + ex.ToString(), this);
            }
        }

        protected void lnkSite_Click(object sender, EventArgs e)
        {
            try
            {
                divSite.Visible = true;
                lnkShowSite.Attributes.Add("class", "element brand active");
                System.Collections.Generic.List<Sites> sites = new System.Collections.Generic.List<Sites>();

                if (Session["sites"] != null)
                {
                    sites = (System.Collections.Generic.List<Sites>)Session["sites"];
                }
                else
                {
                    Sites siteItem = new Sites();
                    siteItem.siteName = Sitecore.Context.Site.Name.ToUpper();
                    siteItem.hostName = "<a href=" + HttpContext.Current.Request.Url.Scheme + "://" + Sitecore.Context.Site.HostName + " target='_blank' >" + Sitecore.Context.Site.HostName + "</a>";
                    siteItem.rootPath = Sitecore.Context.Site.RootPath;
                    siteItem.startItem = Sitecore.Context.Site.StartItem;
                    siteItem.startPath = Sitecore.Context.Site.StartPath;
                    siteItem.cacheHtml = Sitecore.Context.Site.CacheHtml;
                    siteItem.database = Sitecore.Context.Site.Database.Name;
                    siteItem.device = Sitecore.Context.Site.Device;
                    siteItem.enableAnalytics = Sitecore.Context.Site.EnableAnalytics;
                    siteItem.language = Sitecore.Context.Site.Language;
                    siteItem.htmlCacheSize = Sitecore.MainUtil.FormatSize(Sitecore.Context.Site.Caches.HtmlCache.InnerCache.MaxSize);
                    siteItem.registryCacheSize = Sitecore.MainUtil.FormatSize(Sitecore.Context.Site.Caches.RegistryCache.InnerCache.MaxSize);
                    siteItem.viewStateCacheSize = Sitecore.MainUtil.FormatSize(Sitecore.Context.Site.Caches.ViewStateCache.InnerCache.MaxSize);
                    siteItem.xslCacheSize = Sitecore.MainUtil.FormatSize(Sitecore.Context.Site.Caches.XslCache.InnerCache.MaxSize);
                    siteItem.filteredItemsCacheSize = Sitecore.MainUtil.FormatSize(Sitecore.Context.Site.Caches.FilteredItemsCache.InnerCache.MaxSize);

                    sites.Add(siteItem);


                    foreach (Sitecore.Sites.Site s in Sitecore.Sites.SiteManager.GetSites())
                    {
                        siteItem = new Sites();

                        siteItem.siteName = s.Properties["name"].ToUpper();
                        if (siteItem.siteName.Equals(Sitecore.Context.Site.Name.ToUpper()) || systemSites.Contains(siteItem.siteName.ToLower()))
                        {
                            continue;
                        }

                        //siteItem.hostName = s.Properties["hostName"];
                        siteItem.hostName = "<a href=" + HttpContext.Current.Request.Url.Scheme + "://" + s.Properties["hostName"] + " target='_blank' >" + s.Properties["hostName"] + "</a>";
                        siteItem.rootPath = s.Properties["rootPath"];
                        siteItem.startItem = s.Properties["startItem"];
                        siteItem.startPath = s.Properties["startPath"];
                        siteItem.cacheHtml = Convert.ToBoolean(s.Properties["cacheHtml"]);
                        siteItem.database = s.Properties["database"];
                        siteItem.device = s.Properties["device"];
                        siteItem.enableAnalytics = Convert.ToBoolean(s.Properties["enableAnalytics"]);
                        siteItem.language = s.Properties["language"];
                        siteItem.htmlCacheSize = s.Properties["htmlCacheSize"];
                        siteItem.registryCacheSize = s.Properties["registryCacheSize"];
                        siteItem.viewStateCacheSize = s.Properties["viewStateCacheSize"];

                        siteItem.xslCacheSize = s.Properties["xslCacheSize"];
                        siteItem.filteredItemsCacheSize = s.Properties["filteredItemsCacheSize"];
                        sites.Add(siteItem);
                    }

                    Session.Add("sites", sites);
                }

                rptSiteList.DataSource = sites;
                rptSiteList.DataBind();
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Sites: " + ex.ToString(), this);
            }
        }

        protected void lnkSiteOverview_Click(object sender, EventArgs e)
        {
            try
            {
                lnkShowSiteOverview.Attributes.Add("class", "element brand active");
                divSiteOverview.Visible = true;
            }
            catch (Exception)
            {
                divError.Visible = true;
            }
        }

        protected void lnkTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sbScriptBlock = new StringBuilder();
                divTemplate.Visible = true;
                lnkShowTemplate.Attributes.Add("class", "element brand active");
                Table tblJS = new Table();

                if (Session["tblTemplates"] != null)
                {
                    tblJS = (Table)Session["tblTemplates"];
                }
                else
                {
                    tblJS.ID = "tblTemplates";
                    tblJS.Attributes.Add("class", "table striped hovered dataTable bordered fg-black");
                    tblJS.CellSpacing = 0;
                    tblJS.CellPadding = 0;

                    if (Session["templates"] == null)
                    {
                        templatePath = TemplateFolderPath;
                        string query = @"fast:" + templatePath + "//*[@@templatename='Template']";
                        templates = DB.SelectItems(query);
                        Session["templates"] = templates;
                    }
                    else
                    {
                        templates = (Item[])Session["templates"];
                    }

                    TableRow trSummaryCount = new TableRow();

                    //Add Template Header
                    TableHeaderRow trHeader = new TableHeaderRow();
                    TableHeaderCell tcHeaderTemplateName = new TableHeaderCell();
                    tcHeaderTemplateName.Controls.Add(new LiteralControl("Template Name"));

                    TableHeaderCell tcHeaderTemplatePath = new TableHeaderCell();
                    tcHeaderTemplatePath.Controls.Add(new LiteralControl("Template Path"));

                    TableHeaderCell tcHeaderBaseTemplate = new TableHeaderCell();
                    tcHeaderBaseTemplate.Controls.Add(new LiteralControl("Base Templates"));

                    TableHeaderCell tcHeaderPageCreatedFromTemplate = new TableHeaderCell();
                    tcHeaderPageCreatedFromTemplate.Controls.Add(new LiteralControl("Pages Created"));

                    trHeader.Cells.Add(tcHeaderTemplateName);
                    trHeader.Cells.Add(tcHeaderTemplatePath);
                    trHeader.Cells.Add(tcHeaderBaseTemplate);
                    trHeader.Cells.Add(tcHeaderPageCreatedFromTemplate);
                    trHeader.TableSection = TableRowSection.TableHeader;
                    tblJS.Rows.Add(trHeader);

                    //Add Template Details Rows
                    if (templates != null)
                    {
                        foreach (Item tmp in templates)
                        {
                            //Render child template                            
                            TableRow tr = new TableRow();

                            TableCell tcTemplateName = new TableCell();
                            string iconFullPath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + Sitecore.Resources.Images.GetThemedImageSource(tmp.Appearance.Icon);
                            Image img = new Image();
                            img.ImageUrl = iconFullPath;
                            img.CssClass = "templateicon";
                            tcTemplateName.Controls.Add(img);
                            tcTemplateName.Controls.Add(new LiteralControl(tmp.DisplayName));

                            TableCell tcTemplatePath = new TableCell();
                            tcTemplatePath.Controls.Add(new LiteralControl(tmp.Paths.FullPath));

                            TableCell tcBaseTemplate = new TableCell();
                            if (tmp.Fields["__Base template"] != null)
                            {
                                string[] strBaseTemplates = tmp.Fields["__Base template"].Value.Split('|');
                                System.Text.StringBuilder sbBaseTemplates = new System.Text.StringBuilder();

                                foreach (string baseTmp in strBaseTemplates)
                                {
                                    if (!String.IsNullOrEmpty(baseTmp))
                                    {
                                        Item itmBaseTmp = DB.GetItem(baseTmp);
                                        //if (itmBaseTmp == null)
                                        //{
                                        //    sbBaseTemplates.Append("<span class='wrongvalue'>Item NOT found :").Append(baseTmp).Append("</span><br/>");
                                        //}
                                        //else if (itmBaseTmp.Paths.FullPath.ToLower().StartsWith("/sitecore/templates/user defined") && (!itmBaseTmp.Paths.FullPath.ToLower().Contains(SiteHome.Name.ToLower())))
                                        //{     
                                        //    sbBaseTemplates.Append("<span class='wrongvalue'>").Append(itmBaseTmp.Paths.FullPath).Append("</span><br/><span class='icon-warning fg-crimson'></span><span class='fg-crimson'>&nbsp;&nbsp;Non ").Append(SiteHome.Name).Append(" Template</span>");
                                        //}
                                        //else
                                        //{
                                            sbBaseTemplates.Append(itmBaseTmp.Paths.FullPath).Append("<br/>");
                                        //}
                                    }
                                }

                                tcBaseTemplate.Controls.Add(new LiteralControl(sbBaseTemplates.ToString()));
                            }
                            else
                            {
                                tcBaseTemplate.Controls.Add(new LiteralControl(string.Empty));
                            }


                            //13-Oct       
                            string queryPath = string.Empty;
                            if (SiteHome.Parent.Name.Contains("-"))
                            {
                                queryPath = "/sitecore/content/#" + SiteHome.Name + "#";
                            }
                            else
                            {
                                queryPath = "/sitecore/content/" + SiteHome.Name;
                            }


                            string query = @"fast:" + queryPath + "//*[@@templateid='" + tmp.ID + "']";
                            templateItems = DB.SelectItems(query);

                            System.Text.StringBuilder sbTemplateItems = new System.Text.StringBuilder();
                            TableCell tcPageCreatedFromTemplate = new TableCell();
                            sbTemplateItems.Append("<nobr>Pages: ").Append("<b>").Append(templateItems.Length).Append("</b><nobr><br>").Append("&nbsp;");

                            if (templateItems.Length > 0)
                            {
                                StringBuilder sbTemplatePages = new StringBuilder();
                                foreach (Item templateItem in templateItems)
                                {
                                    sbTemplatePages.Append(templateItem.Paths.FullPath).Append("<br/>");
                                }


                                string templatePages = string.Empty;
                                if (sbTemplatePages.ToString().Length > 32765)
                                {
                                    templatePages = Uri.EscapeDataString(sbTemplatePages.ToString().Substring(0, 32760) + "...");
                                }
                                else
                                {
                                    templatePages = Uri.EscapeDataString(sbTemplatePages.ToString());
                                }

                                sbTemplateItems.AppendFormat("<a id='{0}' href='#' class='numberofpages' onclick={1}>View pages</a> <br/>",
                                   tmp.ID.ToShortID(),
                                   "ShowDialog('" + templatePages + "');");
                            }

                            tcPageCreatedFromTemplate.Controls.Add(new LiteralControl(sbTemplateItems.ToString()));
                            tr.Cells.Add(tcTemplateName);
                            tr.Cells.Add(tcTemplatePath);
                            tr.Cells.Add(tcBaseTemplate);
                            tr.Cells.Add(tcPageCreatedFromTemplate);
                            tr.TableSection = TableRowSection.TableBody;
                            tblJS.Rows.Add(tr);

                        } //end outer for loop
                    }

                    Session.Add("tblTemplates", tblJS);
                }

                divTemplate.Controls.Add(tblJS);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "loadTemplateDT", "$('#tblTemplates').DataTable();", true);
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Templates: " + ex.ToString(), this);
            }
        }

        private void populateSublayouts()
        {
            try
            {
                divSubLayout.Visible = true;
                lnkShowSubLayout.Attributes.Add("class", "element brand active");
                Table tblSubLayouts = new Table();

                if (Session["tblSubLayouts"] != null)
                {
                    tblSubLayouts = (Table)Session["tblSubLayouts"];
                    CheckBox headCB = (CheckBox)tblSubLayouts.FindControl("headSublayoutDownload");
                    headCB.Attributes.Add("name", "headsublayoutcheckbox");
                    headCB.Attributes.Add("onclick", "javascript:fnSelectAllCheck('sublayoutcheckbox',this);");
                }
                else
                {

                    if (SiteHome != null)
                    {
                        if (Session["subLayouts"] == null)
                        {
                            layoutPath = SubLayoutFolderPath;
                            string query = @"fast:" + layoutPath + "//*[@@templatename='Sublayout' or @@templatename='Controller rendering' or @@templatename='Item rendering' or @@templatename='Method rendering' or @@templatename='Rendering' or @@templatename='Url rendering' or @@templatename='View rendering' or @@templatename='Webcontrol' or @@templatename='Xsl rendering']";
                            subLayouts = DB.SelectItems(query);
                            Session.Add("subLayouts", subLayouts);
                        }
                        else
                        {
                            subLayouts = (Item[])Session["subLayouts"];
                        }

                    }

                    subLayoutCount = subLayouts.Length;

                    tblSubLayouts.Attributes.Add("class", "table striped hovered dataTable bordered fg-black");
                    tblSubLayouts.CellSpacing = 0;
                    tblSubLayouts.CellPadding = 0;
                    tblSubLayouts.ID = "tblSublayouts";
                    tblSubLayouts.Attributes.Add("width", "100%");

                    //Add subLayouts Header
                    TableHeaderRow trHeader = new TableHeaderRow();

                    TableHeaderCell tcHeaderDownloadCheckbox = new TableHeaderCell();
                    tcHeaderDownloadCheckbox.Attributes.Add("width", "5%");
                    CheckBox headCB = new CheckBox();
                    headCB.Attributes.Add("name", "headsublayoutcheckbox");
                    headCB.Attributes.Add("onclick", "javascript:fnSelectAllCheck('sublayoutcheckbox',this);");
                    headCB.ID = "headSublayoutDownload";
                    tcHeaderDownloadCheckbox.Controls.Add(headCB);

                    TableHeaderCell tcSublayoutHeaderName = new TableHeaderCell();
                    tcSublayoutHeaderName.Controls.Add(new LiteralControl("SubLayout Name"));

                    TableHeaderCell tcSublayoutHeaderPath = new TableHeaderCell();
                    tcSublayoutHeaderPath.Controls.Add(new LiteralControl("SubLayout Path"));

                    TableHeaderCell tcHeaderCaching = new TableHeaderCell();
                    tcHeaderCaching.Controls.Add(new LiteralControl("Caching"));

                    TableHeaderCell tcHeaderASCXPath = new TableHeaderCell();
                    tcHeaderASCXPath.Controls.Add(new LiteralControl("ASCX File Path"));

                    TableHeaderCell tcHeaderPageCreatedFromSubLayout = new TableHeaderCell();
                    tcHeaderPageCreatedFromSubLayout.Controls.Add(new LiteralControl("Pages Created"));

                    trHeader.Cells.Add(tcHeaderDownloadCheckbox);
                    trHeader.Cells.Add(tcSublayoutHeaderName);
                    trHeader.Cells.Add(tcSublayoutHeaderPath);
                    trHeader.Cells.Add(tcHeaderCaching);
                    trHeader.Cells.Add(tcHeaderPageCreatedFromSubLayout);
                    trHeader.Cells.Add(tcHeaderASCXPath);

                    trHeader.TableSection = TableRowSection.TableHeader;
                    tblSubLayouts.Rows.Add(trHeader);


                    //Add subLayouts Details Rows
                    if (subLayouts != null)
                    {
                        foreach (Item subLayout in subLayouts)
                        {

                            TableRow tr = new TableRow();
                            TableCell tcSublayoutName = new TableCell();
                            tcSublayoutName.Controls.Add(new LiteralControl(subLayout.DisplayName));

                            TableCell tcSublayoutPath = new TableCell();
                            tcSublayoutPath.Controls.Add(new LiteralControl(subLayout.Paths.ContentPath));

                            TableCell tcCaching = new TableCell();
                            tcCaching = new TableCell();


                            StringBuilder sbCachingData = new StringBuilder();
                            if (subLayout.Fields["Cacheable"].Value == "1")
                                sbCachingData.Append("Cacheable").Append("<br/>");

                            if (subLayout.Fields["VaryByData"].Value == "1")
                                sbCachingData.Append("VaryByData").Append("<br/>");

                            if (subLayout.Fields["VaryByDevice"].Value == "1")
                                sbCachingData.Append("VaryByDevice").Append("<br/>");

                            if (subLayout.Fields["VaryByLogin"].Value == "1")
                                sbCachingData.Append("VaryByLogin").Append("<br/>");

                            if (subLayout.Fields["VaryByParm"].Value == "1")
                                sbCachingData.Append("VaryByParm").Append("<br/>");

                            if (subLayout.Fields["VaryByQueryString"].Value == "1")
                                sbCachingData.Append("VaryByQueryString").Append("<br/>");

                            if (subLayout.Fields["VaryByUser"].Value == "1")
                                sbCachingData.Append("VaryByUser").Append("<br/>");

                            string finalCacheData = string.Empty;
                            if (sbCachingData.ToString().EndsWith("<br/>"))
                                finalCacheData = sbCachingData.ToString().Substring(0, sbCachingData.ToString().Length - 5);

                            tcCaching.Controls.Add(new LiteralControl(finalCacheData));

                            TableCell tcASCXPath = new TableCell();
                            layoutPath = subLayout["Path"];

                            //if (!layoutPath.ToLower().Contains(SiteHome.Name.ToLower()) || (!layoutPath.EndsWith(subLayout.DisplayName + ".ascx")))
                            //{                              
                                
                            //    //sbBaseTemplates.Append("<span class='wrongvalue' data-hint='Non ").Append(SiteHome.Name).Append(" Template|This base template is referred from other site.' data-hint-position='top'>").Append(itmBaseTmp.Paths.FullPath).Append("</span><br/>");
                            //}
                            //else
                            //{
                                tcASCXPath.Controls.Add(new LiteralControl(layoutPath));
                            //}


                            TableCell tcPageCreated = new TableCell();

                            string query = @"fast://*[@__Renderings='%" + subLayout.ID + "%']";
                            subLayoutPageItems = DB.SelectItems(query);

                            System.Text.StringBuilder sbSublayoutUsedOnPages = new System.Text.StringBuilder();
                            TableCell tcPageCreatedFromSubLayout = new TableCell();
                            sbSublayoutUsedOnPages.Append("<nobr>Pages: ").Append("<b>").Append(subLayoutPageItems.Length).Append("</b><nobr><br>").Append("&nbsp;");

                            if (subLayoutPageItems.Length > 0)
                            {
                                StringBuilder sbSubLayoutPages = new StringBuilder();
                                foreach (Item subLayoutPageItem in subLayoutPageItems)
                                {
                                    sbSubLayoutPages.Append(subLayoutPageItem.Paths.FullPath).Append("<br/>");
                                }

                                string sublayoutPages = string.Empty;
                                if (sbSubLayoutPages.ToString().Length > 32765)
                                {
                                    sublayoutPages = Uri.EscapeDataString(sbSubLayoutPages.ToString().Substring(0, 32760) + "...");
                                }
                                else
                                {
                                    sublayoutPages = Uri.EscapeDataString(sbSubLayoutPages.ToString());
                                }

                                sbSublayoutUsedOnPages.AppendFormat("<a id='{0}' href='#' class='numberofpages' onclick={1}>View pages</a> <br/>",
                                   subLayout.ID.ToShortID(),
                                   "ShowDialog('" + sublayoutPages + "');");
                            }

                            tcPageCreated.Controls.Add(new LiteralControl(sbSublayoutUsedOnPages.ToString()));

                            TableCell tcDownloadCheckbox = new TableCell();
                            headCB = new CheckBox();
                            headCB.Attributes.Add("name", "sublayoutcheckbox");
                            //string sublayoutTrailingName = subLayout.Paths.ContentPath.Substring(subLayout.Paths.ContentPath.IndexOf("/Sublayouts"), subLayout.Paths.ContentPath.Length - subLayout.Paths.ContentPath.IndexOf("/Sublayouts"));
                            headCB.ID = subLayout.ID.ToShortID().ToString();//sublayoutTrailingName.Replace("/", "_");
                            tcDownloadCheckbox.Controls.Add(headCB);

                            tr.Cells.Add(tcDownloadCheckbox);
                            tr.Cells.Add(tcSublayoutName);
                            tr.Cells.Add(tcSublayoutPath);
                            tr.Cells.Add(tcCaching);
                            tr.Cells.Add(tcPageCreated);
                            tr.Cells.Add(tcASCXPath);
                            tr.TableSection = TableRowSection.TableBody;
                            tblSubLayouts.Rows.Add(tr);
                        }
                    }

                    Session.Add("tblSubLayouts", tblSubLayouts);
                }

                divSubLayout.Controls.Add(tblSubLayouts);

                Button btnDownloadZipButton = new Button();
                btnDownloadZipButton.ID = "btnDownloadSublayout";
                btnDownloadZipButton.Text = "Download Sublayout";
                btnDownloadZipButton.CssClass = "info";
                btnDownloadZipButton.Click += new EventHandler(this.btnDownloadZipButton_Click);
                btnDownloadZipButton.OnClientClick = "return checkSublayoutSelected()";
                divSubLayout.Controls.Add(btnDownloadZipButton);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "loadSublayoutDT", "$('#tblSublayouts').DataTable( {  \"order\": [1, 'asc'], \"autoWidth\": false, \"columnDefs\": [{ \"orderable\" : false, \"targets\" : 0}], \"columns\": [{ \"width\" : \"3%\" }, { \"width\" : \"14%\" }, { \"width\" : \"35%\" }, { \"width\" : \"8%\" }, { \"width\" : \"10%\" }, { \"width\" : \"30%\" }]} );", true);
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Sublayouts: " + ex.ToString(), this);
            }

        }

        private void getSiteOverviewCount()
        {
            if (ViewState["siteCount"] == null)
            {
                foreach (Sitecore.Sites.Site s in Sitecore.Sites.SiteManager.GetSites())
                {
                    Sites siteItem = new Sites();

                    siteItem.siteName = s.Properties["name"].ToUpper();
                    if (systemSites.Contains(siteItem.siteName.ToLower()))
                    {
                        continue;
                    }
                    siteCount++;
                }
                ViewState.Add("siteCount", siteCount);
            }
            else
            {
                siteCount = (int)ViewState["siteCount"];
            }

            if (ViewState["contentItemCount"] == null)
            {
                contentItemCount = SiteHome.Axes.GetDescendants().Length;
                ViewState.Add("contentItemCount", contentItemCount);
            }
            else
            {
                contentItemCount = (int)ViewState["contentItemCount"];
            }

            if (ViewState["mediaItemCount"] == null)
            {
                Item mediaRootFolder = DB.GetItem(MediaFolderPath);
                if (mediaRootFolder != null)
                {
                    mediaItemCount = mediaRootFolder.Axes.GetDescendants().Length;
                    ViewState.Add("mediaItemCount", mediaItemCount);
                }
            }
            else
            {
                mediaItemCount = (int)ViewState["mediaItemCount"];
            }

            userCount = Sitecore.Security.Accounts.UserManager.GetUsers().GetCount();


            if (Session["templates"] == null)
            {
                templatePath = TemplateFolderPath;
                string query = @"fast:" + templatePath + "//*[@@templatename='Template']";
                templates = DB.SelectItems(query);
                Session.Add("templates", templates);
            }
            else
            {
                templates = (Item[])Session["templates"];
            }

            templateCount = templates.Length;

            if (Session["subLayouts"] == null)
            {
                layoutPath = SubLayoutFolderPath;
                string query = @"fast:" + layoutPath + "//*[@@templatename='Sublayout' or @@templatename='Controller rendering' or @@templatename='Item rendering' or @@templatename='Method rendering' or @@templatename='Rendering' or @@templatename='Url rendering' or @@templatename='View rendering' or @@templatename='Webcontrol' or @@templatename='Xsl rendering']";
                subLayouts = DB.SelectItems(query);
                Session.Add("subLayouts", subLayouts);
            }
            else
            {
                subLayouts = (Item[])Session["subLayouts"];
            }

            subLayoutCount = subLayouts.Length;
        }

        protected void btnDownloadZipButton_Click(object sender, EventArgs e)
        {
            try
            {
                String strCheckedItem = String.Empty;
                StringCollection scCheckedItems = new StringCollection();
                Table srcTblSublayouts = (Table)this.FindControl("tblSublayouts");

                if (srcTblSublayouts != null)
                {
                    ControlCollection tblRowCollection = srcTblSublayouts.Controls;
                    //if (Session["scCheckedItems"] == null)
                    //{
                    for (int nCount = 2; nCount < tblRowCollection.Count; nCount++)
                    {
                        TableRow tblRow = (TableRow)tblRowCollection[nCount];
                        if (tblRow != null)
                        {
                            TableCell tblCell = (TableCell)tblRow.Controls[0];

                            if (tblCell != null)
                            {
                                CheckBox cbDownload = (CheckBox)tblCell.Controls[0];
                                if (cbDownload != null)
                                {
                                    if (cbDownload.Checked)
                                    {
                                        strCheckedItem = cbDownload.ID;
                                        scCheckedItems.Add(strCheckedItem);
                                    }
                                }
                            }
                        }
                    }
                }


                if (scCheckedItems.Count > 0)
                {
                    //String layoutFolderPath = "/sitecore/layout";
                    String subLayoutCheckboxPath = String.Empty;

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(2000);

                    using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zos = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms))
                    {
                        zos.SetLevel(8);
                        foreach (String stSelectedSublayouts in scCheckedItems)
                        {
                            subLayoutCheckboxPath = stSelectedSublayouts;// layoutFolderPath + stSelectedSublayouts.Replace("_", "/");
                            ID selectedSublayoutID = new Sitecore.Data.ID(subLayoutCheckboxPath);

                            Item layoutPathItem = DB.GetItem(selectedSublayoutID);
                            System.IO.FileInfo fi = new System.IO.FileInfo(Server.MapPath(layoutPathItem["Path"]));
                            //Added file check
                            if (fi.Exists)
                            {
                                System.IO.FileStream fs = fi.OpenRead();
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                                ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(fi.Name);
                                entry.Size = fi.Length;

                                zos.PutNextEntry(entry);
                                zos.Write(buffer, 0, buffer.Length);

                                fs.Close();
                            }
                        }
                    }
                    /* byte[] bufferzip = new byte[ms.Length];
                     ms.Read(bufferzip, 0, bufferzip.Length);
                     ms.Close();*/

                    //if (bufferzip != null && bufferzip.Length > 0)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ClearHeaders();
                        HttpContext.Current.Response.CacheControl = "public";
                        HttpContext.Current.Response.ContentType = "application/octet-stream";//"application/zip";
                        HttpContext.Current.Response.AddHeader("Content-disposition", "attachment; filename=\"" + SiteHome.Name + "_SubLayouts" + ".zip\"");
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.BinaryWrite(ms.ToArray());

                        try
                        {
                            HttpContext.Current.Response.End();
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            //ignore thread abort exception
                            //Added to Fix Backlog item # 572                        
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Sublayout download: " + ex.ToString(), this);
            }
        }

        protected void lnkUsers_Click(object sender, EventArgs e)
        {
            try
            {
                lnkShowUser.Attributes.Add("class", "element brand active");
                divUsers.Visible = true;
                Table tblUsers = new Table();
                if (Session["tblUsers"] != null)
                {
                    tblUsers = (Table)Session["tblUsers"];
                }
                else
                {
                    tblUsers.ID = "tblUsers";
                    tblUsers.Attributes.Add("class", "table striped hovered dataTable bordered fg-black");
                    TableHeaderRow trHeader = new TableHeaderRow();

                    TableHeaderCell tcUsernameHeader = new TableHeaderCell();
                    tcUsernameHeader.Controls.Add(new LiteralControl("User Name"));

                    TableHeaderCell tcDomainHeader = new TableHeaderCell();
                    tcDomainHeader.Controls.Add(new LiteralControl("Domain"));

                    TableHeaderCell tcFullNameHeader = new TableHeaderCell();
                    tcFullNameHeader.Controls.Add(new LiteralControl("FullName"));

                    TableHeaderCell tcEmailHeader = new TableHeaderCell();
                    tcEmailHeader.Controls.Add(new LiteralControl("Email"));

                    TableHeaderCell tcIsAdminHeader = new TableHeaderCell();
                    tcIsAdminHeader.Controls.Add(new LiteralControl("IsAdmin"));

                    TableHeaderCell tcIsLockedHeader = new TableHeaderCell();
                    tcIsLockedHeader.Controls.Add(new LiteralControl("State"));

                    TableHeaderCell tcCommentHeader = new TableHeaderCell();
                    tcCommentHeader.Controls.Add(new LiteralControl("Comment"));

                    trHeader.Cells.Add(tcUsernameHeader);
                    trHeader.Cells.Add(tcDomainHeader);
                    trHeader.Cells.Add(tcFullNameHeader);
                    trHeader.Cells.Add(tcEmailHeader);
                    trHeader.Cells.Add(tcIsAdminHeader);
                    trHeader.Cells.Add(tcIsLockedHeader);
                    trHeader.Cells.Add(tcCommentHeader);

                    trHeader.TableSection = TableRowSection.TableHeader;
                    tblUsers.Rows.Add(trHeader);

                    Sitecore.Common.IFilterable<Sitecore.Security.Accounts.User> users = Sitecore.Security.Accounts.UserManager.GetUsers();
                    //Add subLayouts Details Rows
                    if (users != null)
                    {
                        foreach (Sitecore.Security.Accounts.User u in users)
                        {
                            TableRow tr = new TableRow();
                            tr.TableSection = TableRowSection.TableBody;

                            TableCell tcUsername = new TableCell();
                            tcUsername.Controls.Add(new LiteralControl(u.Name));
                            tr.Cells.Add(tcUsername);

                            TableCell tcDomain = new TableCell();
                            tcDomain.Controls.Add(new LiteralControl(u.Domain.Name));
                            tr.Cells.Add(tcDomain);

                            TableCell tcFullName = new TableCell();
                            tcFullName.Controls.Add(new LiteralControl(u.Profile.FullName));
                            tr.Cells.Add(tcFullName);

                            TableCell tcEmail = new TableCell();
                            tcEmail.Controls.Add(new LiteralControl(u.Profile.Email));
                            tr.Cells.Add(tcEmail);

                            TableCell tcIsAdmin = new TableCell();
                            tcIsAdmin.Controls.Add(new LiteralControl(u.IsAdministrator.ToString()));
                            tr.Cells.Add(tcIsAdmin);

                            TableCell tcIsLocked = new TableCell();
                            tcIsLocked.Controls.Add(new LiteralControl(u.Profile.State.ToString()));
                            tr.Cells.Add(tcIsLocked);

                            TableCell tcComment = new TableCell();
                            tcComment.Controls.Add(new LiteralControl(u.Profile.Comment));
                            tr.Cells.Add(tcComment);


                            tblUsers.Rows.Add(tr);
                            Session.Add("tblUsers", tblUsers);
                        }
                    }
                }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "loadUsers", "$('#tblUsers').DataTable();", true);
                divUsers.Controls.Add(tblUsers);
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Users: " + ex.ToString(), this);
            }

        }

        protected void btnGlobalSearchAnalyzeItem_Click(object sender, EventArgs e)
        {
            try
            {
                lnkShowAnalyzeItem.Attributes.Add("class", "element brand active");
                tblAnalyzeItem.Visible = true;
                divAnalyzeItemSearch.Visible = true;
                if (txtGlobalSearch.Value.Trim() != string.Empty)
                {
                    analyzeSitecoreItem(txtGlobalSearch.Value.Trim());
                    txtAnalyzeItemSearch.Value = txtGlobalSearch.Value.Trim();
                }
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Global Search: " + ex.ToString(), this);
            }
        }

        protected void btnSearchAnalyzeItem_Click(object sender, EventArgs e)
        {
            try
            {
                lnkShowAnalyzeItem.Attributes.Add("class", "element brand active");
                tblAnalyzeItem.Visible = true;
                divAnalyzeItemSearch.Visible = true;
                if (txtAnalyzeItemSearch.Value.Trim() != string.Empty)
                {
                    analyzeSitecoreItem(txtAnalyzeItemSearch.Value.Trim());
                }
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics Analyze Item: " + ex.ToString(), this);
            }
        }

        protected void lnkAnalyzeItem_Click(object sender, EventArgs e)
        {
            try
            {
                lnkShowAnalyzeItem.Attributes.Add("class", "element brand active");
                tblAnalyzeItem.Visible = false;
                divAnalyzeItemSearch.Visible = true;
            }
            catch (Exception)
            {
                divError.Visible = true;
            }
        }

        private void analyzeSitecoreItem(string itemPathORId)
        {
            try
            {
                Item itemToBeSearched = DB.GetItem(itemPathORId);

                string iconFullPath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + Sitecore.Resources.Images.GetThemedImageSource(itemToBeSearched.Appearance.Icon);
                searchedItem.Icon = iconFullPath;

                searchedItem.ID = itemToBeSearched.ID.ToString();
                searchedItem.Name = itemToBeSearched.Name;
                searchedItem.Path = itemToBeSearched.Paths.FullPath;

                Item searchedItemTemplate = itemToBeSearched.Template;
                searchedItem.Template = searchedItemTemplate.Paths.FullPath;
                searchedItem.LastUpdated = itemToBeSearched.Statistics.Updated.ToString();
                searchedItem.LastUpdatedBy = itemToBeSearched["__Updated by"];

                Sitecore.Data.Database webDB = Sitecore.Configuration.Factory.GetDatabase("web");
                Item webItem = webDB.GetItem(itemToBeSearched.ID);
                if ((itemToBeSearched["__Revision"].ToString()) == (webItem["__Revision"].ToString()))
                {
                    searchedItem.IsPublished = "True";
                }
                else
                {
                    searchedItem.IsPublished = "Flase";
                }

                StringBuilder sbRefernceItems = new StringBuilder();
                Sitecore.Links.ItemLink[] referrers = Sitecore.Globals.LinkDatabase.GetReferrers(itemToBeSearched);

                foreach (Sitecore.Links.ItemLink referrerLink in referrers)
                {
                    Item referenceItem = DB.GetItem(referrerLink.SourceItemID);
                    string icon = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + Sitecore.Resources.Images.GetThemedImageSource(referenceItem.Appearance.Icon);
                    Image imgIcon = new Image();
                    imgIcon.ImageUrl = icon;
                    imgIcon.CssClass = "templateicon";
                    tblAnalyzeItem.Rows[8].Cells[1].Controls.Add(imgIcon);
                    tblAnalyzeItem.Rows[8].Cells[1].Controls.Add(new LiteralControl(referenceItem.Paths.FullPath + "<br/>"));
                    //sbRefernceItems.Append(referenceItem.Paths.FullPath).Append("<br/>");
                }

                searchedItem.ReferencePages = sbRefernceItems.ToString();
                //searchedItem.IsPublished = "";
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                Sitecore.Diagnostics.Log.Error("Error occured while accessing SitecoreInformatics: analyzeSitecoreItem()" + ex.ToString(), this);
            }
        }

        protected void lnkSubLayout_Click(object sender, EventArgs e)
        {
        }

        public void Page_Error(object sender, EventArgs e)
        {

        }

        protected void lnkClearCache_ServerClick(object sender, EventArgs e)
        {
            Session.Clear();
            Server.Transfer("\\sitecore modules\\SI\\SitecoreInformatics.aspx");
        }
    }
}