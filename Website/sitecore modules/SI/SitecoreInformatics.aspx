<%@ Page Language="C#" AutoEventWireup="true" Debug="false" CodeBehind="SitecoreInformatics.aspx.cs" Inherits="SI.SitecoreInformatics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">
    <meta name="product" content="Metro UI CSS Framework">
    <meta name="description" content="Simple responsive css framework">
    <meta name="author" content="Sergey S. Pimenov, Ukraine, Kiev">

    <link type="text/css" rel="Stylesheet" href="css/style.css" media="all" />
    <link href='http://fonts.googleapis.com/css?family=Roboto:400,500' rel='stylesheet' type='text/css'>

    <link href="css/metro-bootstrap.css" rel="stylesheet">
    <link href="css/metro-bootstrap-responsive.css" rel="stylesheet">
    <link href="css/iconFont.css" rel="stylesheet">

    <!-- Load JavaScript Libraries -->
    <script src="js/jquery/jquery.min.js"></script>
    <script src="js/jquery/jquery.widget.min.js"></script>
    <script src="js/jquery/jquery.mousewheel.js"></script>
    <script src="js/jquery/jquery.dataTables.js"></script>

    <!-- Metro UI CSS JavaScript plugins -->
    <script src="js/load-metro.js" type="text/javascript"></script>

    <!-- Local JavaScript -->
    <script src="js/github.info.js"></script>

    <title>Sitecore Informatics</title>
    <style>
        .container {
            width: 940px;
        }

        .metro .table th, .metro .table td {
            font-size: 12px;
            padding: 4px 10px;
        }

        .metro .window .content {
            max-height: 700px;
            overflow-y: auto;
        }
    </style>

    <script type="text/javascript">
        function ShowDialog(content) {
            $.Dialog({
                shadow: true,
                overlay: true,
                icon: '<span class="icon-link"></span>',
                title: 'Referrer Pages',
                width: 500,
                padding: 10,
                content: unescape(content)
            });
        }

        function ShowSettings() {
            $.Dialog({
                shadow: true,
                overlay: true,
                icon: '<span class="icon-cog"></span>',
                title: 'Sitcore Informatics Configuration Settings',
                width: 500,
                padding: 10,
                content: 'Your current settings are:<br/> <b>Sublayouts root folder path:</b> <%=SubLayoutFolderPath%> <br/><b>Template root folder path:</b> <%=TemplateFolderPath%> <br/><b>Media root folder path:</b> <%=MediaFolderPath%> <br/><br/> If you are seeing <b>0 count</b> then change your settings at:<br/> <b>Website\\App_Config\\Include\\SitecoreInformatics.config</b>'
            });
        }

        function callSearchOnEnter(buttonName, e) {//the purpose of this function is to allow the enter key to point to the correct button to click.
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                //Get the button the user wants to have clicked
                var btn = document.getElementById(buttonName);
                if (btn != null) { //If we find the button click it
                    btn.click();
                    event.keyCode = 0
                }
            }
        }

    </script>




    <script type="text/javascript" language="JavaScript">
        function fnSelectAllCheck(sublayoutcheckboxname, source) {
            checkboxspans = document.getElementsByName(sublayoutcheckboxname);
            var selected = source.checked;
            for (var i = 0; i < checkboxspans.length; i++) {

                checkboxes = checkboxspans[i].getElementsByTagName('input');
                checkboxes[0].checked = selected;
            }
        }

        function checkSublayoutSelected() {
            checkboxspans = document.getElementsByName("sublayoutcheckbox");
            for (var i = 0; i < checkboxspans.length; i++) {

                checkboxes = checkboxspans[i].getElementsByTagName('input');
                if (checkboxes[0].checked) {
                    return true;
                }
            }

            alert("Please select sublayout to download");
            return false;
        }

        function showProgress() {
            try {
                document.getElementById("divProgress").style.visibility = "visible";
                document.body.style.backgroundColor = "gray";
                document.body.style.filter = "alpha(opacity=60)";
                document.body.style.opacity = " 0.6";
            } catch (exception) {
            }
        }

    </script>
</head>

<body class="metro">
    <form id="frmSiteInfo" runat="server">
        <div class="container">
            <div class="row">
                <div class="place-right">
                    <div class="input-control text size5 margin20 nrm">
                        <form>
                            <input type="text" placeholder="Search Sitecore Item..." id="txtGlobalSearch" runat="server">
                            <button id="Button1" class="btn-search" runat="server" onserverclick="btnGlobalSearchAnalyzeItem_Click"></button>
                        </form>
                    </div>
                </div>
                <a class="place-left" href="#" title="">
                    <h1>Sitecore Informatics</h1>
                </a>
            </div>

            <div class="row">
                <div class="tile-group no-margin no-padding clearfix span12">
                    <nav class="navigation-bar dark">
                                <nav class="navigation-bar-content no-margin no-padding">                                    
                                     <a class="element brand" href="#" id="lnkShowSiteOverview" runat="server" onclick="showProgress();" onserverclick="lnkSiteOverview_Click"><span class="icon-home"></span>&nbsp;&nbsp;<%=SiteHome.Name.ToUpper() %>&nbsp;-&nbsp;Site Overview</a>                                    
                                     <span class="element-divider"></span>                                    
                                     <a class="element brand" href="#" id="lnkShowSite" runat="server" onclick="showProgress();" onserverclick="lnkSite_Click"><span class="icon-earth"></span>&nbsp;&nbsp;Sites</a>                                    
                                     <span class="element-divider"></span>    
                                    <a class="element brand" href="#" id="lnkShowSubLayout" runat="server" onclick="showProgress();" onserverclick="lnkSubLayout_Click"><span class="icon-file"></span>&nbsp;&nbsp;Sublayouts</a>
                                    <span class="element-divider"></span>                                    
                                    <a class="element brand" href="#" id="lnkShowTemplate" runat="server" onclick="showProgress();" onserverclick="lnkTemplate_Click"><span class="icon-window"></span>&nbsp;&nbsp;Templates</a>                                    
                                    <span class="element-divider"></span>                                    
                                    <a class="element brand" href="#" id="lnkShowUser" runat="server" onclick="showProgress();" onserverclick="lnkUsers_Click"><span class="icon-user"></span>&nbsp;&nbsp;Users</a>
                                      <span class="element-divider"></span>                                    
                                    <a class="element brand" href="#" id="lnkShowAnalyzeItem" runat="server" onclick="showProgress();" onserverclick="lnkAnalyzeItem_Click"><span class="icon-eye"></span>&nbsp;&nbsp;Analyze Sitecore Item</a>

                                    <span class="element-divider"></span> 
                                     <a runat="server" onserverclick="lnkClearCache_ServerClick" data-hint="Refresh|Sitcore Informatics caches the data on first request. Click this button to clear the cached data. This will impact on performance as it will pull fresh data from sitecore on next request." data-hint-position="bottom" class="element brand" id="lnkClearCache"><li class="icon-spin"></li>
                                    </a>
                                        
                                    <span class="element-divider"></span>                                    
                                    <a class="element brand place-right" id="lnkShowSetting" onclick="ShowSettings()"><li class="icon-cog"></li></a>
                                </nav>
                    </nav>
                </div>
            </div>

            <div class="clearfix"></div>

            <div id="divSiteOverview" class="row" runat="server">
                <div class="margin10 nrm nlm">


                    <div class="tile ol-transparent bg-darkGreen span4" onclick="showProgress();javascript:__doPostBack('lnkShowSite','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=siteCount.ToString() %>
                            </p>
                            <span class="name">Sites</span>
                        </div>
                    </div>



                    <%if (subLayoutCount.ToString() != "0")
                      {%>
                    <div class="tile ol-transparent bg-teal span4" onclick="showProgress();javascript:__doPostBack('lnkShowSubLayout','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=subLayoutCount.ToString() %>
                            </p>
                            <span class="name">Sublayouts</span>

                        </div>
                    </div>
                    <% }
                      else
                      { %>
                    <div class="tile ol-transparent bg-teal span4" onclick="ShowSettings();">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=subLayoutCount.ToString() %>
                            </p>
                            <span class="name">Sublayouts</span>
                            <div class="badge attention"></div>
                        </div>
                    </div>
                    <%} %>



                    <%if (templateCount.ToString() != "0")
                      {%>
                    <div class="tile ol-transparent bg-magenta span4" onclick="showProgress();javascript:__doPostBack('lnkShowTemplate','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=templateCount.ToString() %>
                            </p>
                            <span class="name">Templates</span>
                        </div>
                    </div>
                    <% }
                      else
                      { %>
                    <div class="tile ol-transparent bg-magenta span4" onclick="ShowSettings();">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=templateCount.ToString() %>
                            </p>
                            <span class="name">Templates</span>
                            <div class="badge attention"></div>
                        </div>
                    </div>
                    <%} %>

                    <div class="tile ol-transparent bg-cyan span4" onclick="showProgress();javascript:__doPostBack('lnkShowUser','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=userCount.ToString() %>
                            </p>
                            <span class="name">Users</span>
                        </div>
                    </div>

                    <div class="tile ol-transparent bg-violet span4" onclick="showProgress();javascript:__doPostBack('lnkShowAnalyzeItem','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=contentItemCount.ToString() %>
                            </p>
                            <span class="name">Content Items</span>
                        </div>
                    </div>

                    <%if (mediaItemCount.ToString() != "0")
                      {%>
                    <div class="tile ol-transparent bg-darkOrange span4" onclick="showProgress();javascript:__doPostBack('lnkShowAnalyzeItem','');">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=mediaItemCount.ToString() %>
                            </p>
                            <span class="name">Media Items</span>
                        </div>
                    </div>
                    <% }
                      else
                      { %>
                    <div class="tile ol-transparent bg-darkOrange span4" onclick="ShowSettings();">
                        <div class="tile-status">
                            <p class="header text-center fg-white">
                                <%=mediaItemCount.ToString() %>
                            </p>
                            <span class="name">Media Items</span>
                            <div class="badge attention"></div>
                        </div>
                    </div>
                    <% } %>
                </div>
            </div>

            <div id="Div1" class="row" runat="server">
                <div id="divSite" runat="server" class="accordion with-marker span12 place-left" data-closeany="false" data-role="accordion">
                    <asp:Repeater ID="rptSiteList" runat="server">
                        <ItemTemplate>
                            <div class="accordion-frame">
                                <a class="heading bg-lightBlue fg-white collapsed" href="#"><%#DataBinder.Eval(Container.DataItem,"siteName") %></a>
                                <div class="content">
                                    <table id="tblSiteOverview" cellpadding="0" cellspacing="0" class="table striped hovered bordered fg-black" border="1">
                                        <tr>
                                            <td class="headingColumn">Host Name:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"hostName") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Root Path:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"rootPath") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Start Item:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"startItem") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Start Path:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"startPath") %></td>
                                        </tr>

                                        <tr>
                                            <td class="headingColumn">Cache Html:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"cacheHtml") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Database:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"database") %></td>
                                        </tr>

                                        <tr>
                                            <td class="headingColumn">Device:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"device") %></td>
                                        </tr>

                                        <tr>
                                            <td class="headingColumn">Enable Analytics:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"enableAnalytics") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Language:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"language") %></td>
                                        </tr>

                                        <tr>
                                            <td class="headingColumn">Html CacheSize:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"htmlCacheSize") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Registry CacheSize:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"registryCacheSize") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">ViewState CacheSize:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"viewStateCacheSize") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">Xsl CacheSize:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"xslCacheSize") %></td>
                                        </tr>
                                        <tr>
                                            <td class="headingColumn">FilteredItems CacheSize:</td>
                                            <td><%#DataBinder.Eval(Container.DataItem,"filteredItemsCacheSize") %></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div id="divTemplate" class="span12" runat="server">
                </div>

                <div id="divSubLayout" class="span12" runat="server">
                </div>

                <div id="divUsers" class="span12" runat="server">
                </div>

                <div id="divAnalyzeItem" class="span12" runat="server">
                    <div class="clearfix"></div>
                    <div class="margin10 nrm nlm">
                        <div class="input-control text size12" id="divAnalyzeItemSearch" runat="server">
                            <input type="text" placeholder="Search Sitecore Item..." id="txtAnalyzeItemSearch" runat="server">
                            <button id="btnAnalyzeItem" class="btn-search" runat="server" onserverclick="btnSearchAnalyzeItem_Click"></button>
                        </div>
                    </div>

                    <table id="tblAnalyzeItem" runat="server" cellpadding="0" cellspacing="0" class="table striped hovered bordered fg-black" border="1">
                        <tr>
                            <td colspan="2">
                                <img src="<%=searchedItem.Icon %>" alt="" class="templateicon" />
                                <b><%=searchedItem.Name %></b>
                            </td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item ID:</td>
                            <td><%=searchedItem.ID %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item Name:</td>
                            <td><%=searchedItem.Name %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item Path:</td>
                            <td><%=searchedItem.Path %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item Template:</td>
                            <td><%=searchedItem.Template %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item Last Updated At:</td>
                            <td><%=searchedItem.LastUpdated %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Item Last Updated By:</td>
                            <td><%=searchedItem.LastUpdatedBy %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Is Published?:</td>
                            <td><%=searchedItem.IsPublished %></td>
                        </tr>
                        <tr>
                            <td class="headingColumn">Referrer Pages:</td>
                            <td></td>
                        </tr>
                    </table>


                </div>

                <div id="divError" class="notice bg-lightBlue fg-white" runat="server">
                    <h2>Oops!</h2>
                    <div>It seems that some error occured while requesting this page. Please check the logs for more details.</div>
                </div>
            </div>
        </div>

        <div id="divProgress" class="progressBar" style="visibility: hidden;" runat="server">
        </div>
    </form>
</body>
</html>
