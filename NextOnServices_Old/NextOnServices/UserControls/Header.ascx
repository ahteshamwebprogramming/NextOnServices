<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="UserControls_Header" %>
<html>
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>index</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="hencework" />

    <!-- Favicon -->
    <link rel="shortcut icon" href="favicon.ico">
    <link rel="icon" href="../favicon.png" type="image/x-icon">
    <link href="../vendors/bower_components/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Data table CSS -->
    <link href="../vendors/bower_components/datatables/media/css/jquery.dataTables.min.css" rel="stylesheet" type="text/css" />
    <link href="../vendors/bower_components/datatables.net-responsive/css/responsive.dataTables.min.css" rel="stylesheet" type="text/css" />
    <!-- select2 CSS -->
    <link href="../vendors/bower_components/select2/dist/css/select2.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-select CSS -->
    <link href="../vendors/bower_components/bootstrap-select/dist/css/bootstrap-select.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-tagsinput CSS -->
    <link href="../vendors/bower_components/bootstrap-tagsinput/dist/bootstrap-tagsinput.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-touchspin CSS -->
    <link href="../vendors/bower_components/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="../vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <!-- multi-select CSS -->
    <link href="../vendors/bower_components/multiselect/css/multi-select.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Switches CSS -->
    <link href="../vendors/bower_components/bootstrap-switch/dist/css/bootstrap3/bootstrap-switch.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="../vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <link href="../Scripts/UI/themes/base/jquery.ui.theme.css" rel="stylesheet" />
    <link href="../dist/css/Alert.css" rel="stylesheet" />
    <%--  <link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/base/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />--%>

    <!-- Custom CSS -->
    <link href="../dist/css/style.css" rel="stylesheet" type="text/css">
    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>--%>
</head>
<body>

    <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel1">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h5 class="modal-title" id="exampleModalLabel1">Change Password</h5>
                </div>
                <div class="modal-body">
                    <div class="ml-auto mr-auto no-float">
                        <div class="row">
                            <div class="col-sm-12 col-xs-12">
                                <div class="form-wrap">
                                    <form action="#">
                                        <%--<div class="form-group">
											<label class="control-label mb-10" for="exampleInputEmail_2">User Name</label>
											<input type="email" class="form-control" required id="exampleInputEmail_2" placeholder="User Name">
										</div>--%>
                                        <div class="form-group">
                                            <label class="pull-left control-label mb-10" for="exampleInputpwd_2">Old Password</label>
                                            <div class="clearfix"></div>
                                            <asp:TextBox ID="txtOldPwd" class="form-control" runat="server"></asp:TextBox>
                                            <%--<input type="password" class="form-control" required id="exampleInputpwd_2" placeholder="Password">--%>
                                        </div>
                                        <div class="form-group">
                                            <label class="pull-left control-label mb-10" for="exampleInputpwd_2">New Password</label>
                                            <div class="clearfix"></div>
                                            <asp:TextBox ID="txtNewPwd" class="form-control" runat="server"></asp:TextBox>
                                            <%--<input type="password" class="form-control" required id="Password1" placeholder="New Password">--%>
                                        </div>
                                        <div class="form-group">
                                            <label class="pull-left control-label mb-10" for="exampleInputpwd_2">Confirm New Password</label>
                                            <div class="clearfix"></div>
                                            <asp:TextBox ID="txtConPwd" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="clearfix"></div>
                                        <div class="form-group text-right">
                                            <asp:Button ID="BtnCPWD" runat="server" class="btn btn-info btn-rounded"
                                                Text="Change Password" OnClick="BtnCPWD_Click" OnClientClick="" />
                                            <%--<button type="submit" class="btn btn-info btn-rounded">Submit</button>--%>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="wrapper theme-1-active pimary-color-red">
        <nav class="navbar navbar-inverse navbar-fixed-top">
            <div class="mobile-only-brand pull-left">
                <div class="nav-header pull-left">
                    <div class="logo-wrap">
                        <a href="../nexton.aspx">
                            <img class="brand-img" src="../dist/img/NextonLogo.png" alt="brand" />
                        </a>
                    </div>
                </div>
                <a id="toggle_nav_btn" class="toggle-left-nav-btn inline-block ml-20 pull-left" href="javascript:void(0);"><i class="zmdi zmdi-menu"></i></a>
                <a id="toggle_mobile_search" data-toggle="collapse" data-target="#search_form" class="mobile-only-view" href="javascript:void(0);"><i class="zmdi zmdi-search"></i></a>
                <a id="toggle_mobile_nav" class="mobile-only-view" href="javascript:void(0);"><i class="zmdi zmdi-more"></i></a>

            </div>
            <div id="mobile_only_nav" class="mobile-only-nav pull-right">
                <ul class="nav navbar-right top-nav pull-right">

                    <li class="dropdown auth-drp">
                        <a href="#" class="dropdown-toggle pr-0" data-toggle="dropdown">
                            <img src="../dist/img/user1.png" alt="user_auth" class="user-auth-img img-circle" /><span class="user-online-status"></span></a>
                        <ul class="dropdown-menu user-auth-dropdown" data-dropdown-in="flipInX" data-dropdown-out="flipOutX">
                            <li>
                                <a href="UserProfile.aspx"><i class="zmdi zmdi-account"></i><span>Profile</span></a>
                            </li>
                            <li class="divider"></li>
                            <li>
                                <a href="#" data-toggle="modal" data-target="#exampleModal"><i class="zmdi zmdi-lock"></i><span>Change Password</span></a>
                            </li>
                            <li class="divider"></li>
                            <li>
                                <a href="Index.html"><i class="zmdi zmdi-power"></i><span>Log Out</span></a>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        </nav>
        <!-- /Top Menu Items -->

        <!-- Left Sidebar Menu -->
        <div class="fixed-sidebar-left">
            <ul class="nav navbar-nav side-nav nicescroll-bar">

                <li>
                    <a class="active" href="Dashboard.aspx">
                        <div class="pull-left"><i class="zmdi zmdi-landscape mr-20"></i><span class="right-nav-text">Dashboard</span></div>
                        <div class="clearfix"></div>
                    </a>
                </li>

                <li>
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#table_dr">
                        <div class="pull-left"><i class="zmdi zmdi-google-pages mr-20"></i><span class="right-nav-text">Project</span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="table_dr" class="collapse collapse-level-1 two-col-list">
                        <li>
                            <a href="AddProject.aspx">Add</a>
                        </li>
                        <li>
                            <a href="ProjectDetails.aspx">List</a>
                        </li>
                        <%--  <li>
                            <a href="AddParameter.aspx">Add Parameter</a>
                        </li>--%>
                        <%-- <li>
                            <a href="ProjectMapping.aspx">Sampling</a>
                        </li>--%>
                        <li>
                            <a href="RecontactProjects.aspx">Recontact Projects</a>
                        </li>
                        <li>
                            <a href="RespondentsReport.aspx">ID Search</a>
                        </li>
                        <li>
                            <a href="ManageStatus.aspx">Reconciliation</a>
                        </li>
                        <li>
                            <a href="Redirects.aspx">Redirects</a>
                        </li>
                        <li id="li_CompleteRedirects" runat="server">
                            <a href="CompleteRedirects.aspx">Generate Complete Redirects</a>
                        </li>
                        <li>
                            <a href="ProjectInfoSearc.aspx">ID Mapping</a>
                        </li>
                        <li>
                            <a href="RecontactCreate.aspx">Create Recontact</a>
                        </li>

                        <%--<li>
							<a  href="../ProjectDetails.aspx">Details</a>
						</li>--%>
                    </ul>
                </li>

                <li>
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#sagoapiprojects">
                        <div class="pull-left"><i class="zmdi zmdi-google-pages mr-20"></i><span class="right-nav-text">API Projects</span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="sagoapiprojects" class="collapse collapse-level-1 two-col-list">
                        <li>
                            <a href="SurveysFromLucid.aspx">Surveys(Lucid)</a>
                        </li>
                        <li>
                            <a href="SurveysFromSpectrum.aspx">Surveys(Spectrum)</a>
                        </li>
                       
                        <li>
                            <a href="ProjectsListSago.aspx">List</a>
                        </li>

                        <li>
                            <a href="RecontactProjects.aspx">Recontact Projects</a>
                        </li>
                        <li>
                            <a href="RespondentsReport.aspx">ID Search</a>
                        </li>
                        <li>
                            <a href="ManageStatus.aspx">Reconciliation</a>
                        </li>
                        <li>
                            <a href="Redirects.aspx">Redirects</a>
                        </li>
                        <li id="li2" runat="server">
                            <a href="CompleteRedirects.aspx">Generate Complete Redirects</a>
                        </li>
                        <li>
                            <a href="ProjectInfoSearc.aspx">ID Mapping</a>
                        </li>
                        <li>
                            <a href="RecontactCreate.aspx">Create Recontact</a>
                        </li>

                        <%--<li>
							<a  href="../ProjectDetails.aspx">Details</a>
						</li>--%>
                    </ul>
                </li>


                <li>
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#Supplier">
                        <div class="pull-left"><i class="zmdi zmdi-truck mr-20"></i><span class="right-nav-text">Supplier</span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="Supplier" class="collapse collapse-level-1">
                        <li>
                            <a href="AddSupplier.aspx">Add</a>
                        </li>
                        <li>
                            <a href="SupplierDetails.aspx">List</a>
                        </li>
                        <li>
                            <a href="VendorsHH.aspx">Hashing</a>
                        </li>
                        <%--<li>
                            <a href="AddPixelMgr.aspx">Pixel Mgr</a>
                        </li>--%>
                    </ul>
                </li>
                <li>
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#Client">
                        <div class="pull-left"><i class="zmdi zmdi-assignment-account mr-20"></i><span class="right-nav-text">Client</span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="Client" class="collapse collapse-level-1">
                        <li>
                            <a href="Clients.aspx">Add</a>
                        </li>
                        <li>
                            <a href="ClientsDetails.aspx">List</a>
                        </li>
                    </ul>
                </li>

                <li id="admin_Report" runat="server">
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#app_dr">
                        <div class="pull-left"><i class="zmdi zmdi-book mr-20"></i><span class="right-nav-text">Report </span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="app_dr" class="collapse collapse-level-1">
                        <li>
                            <a href="OverallReport.aspx">Overall</a>
                        </li>
                        <%--<li>
                            <a href="ClientsReport.aspx">Client</a>
                        </li>--%>
                        <%-- <li>
                            <a href="SuppliersReport.aspx">Supplier</a>
                        </li>--%>
                        <li>
                            <a href="ProjectWiseReport.aspx">Project</a>
                        </li>

                    </ul>
                </li>

                <li id="user_admin" runat="server">
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#chart_dr">
                        <div class="pull-left"><i class="zmdi zmdi-account mr-20"></i><span class="right-nav-text">User </span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="chart_dr" class="collapse collapse-level-1 two-col-list">
                        <li>
                            <a href="AddUser.aspx">Add</a>
                        </li>
                        <li>
                            <a href="UsersDetails.aspx">List</a>
                        </li>
                    </ul>
                </li>
                <li id="Li1" runat="server">
                    <a href="javascript:void(0);" data-toggle="collapse" data-target="#screening_dr">
                        <div class="pull-left"><i class="zmdi zmdi-account mr-20"></i><span class="right-nav-text">Questionnaire</span></div>
                        <div class="pull-right"><i class="zmdi zmdi-caret-down"></i></div>
                        <div class="clearfix"></div>
                    </a>
                    <ul id="screening_dr" class="collapse collapse-level-1 two-col-list">
                        <li>
                            <a href="Screening.aspx">Add Questions</a>
                        </li>
                        <li>
                            <a href="QuestionsList.aspx">Question Library</a>
                        </li>
                        <%-- <li>
                            <a href="ProjectQuestionMapping.aspx">Project/Question Mapping</a>
                        </li>--%>
                    </ul>
                </li>
            </ul>
        </div>
        <!-- /Left Sidebar Menu -->
    </div>
</body>
</html>
