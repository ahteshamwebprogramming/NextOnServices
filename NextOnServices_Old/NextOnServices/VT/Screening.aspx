<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Screening.aspx.cs" Inherits="VT_Screening" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <script>
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        function success(message, typee) {
            swal({
                position: 'top-center',
                type: typee,
                title: message,
                showConfirmButton: false,
                timer: 2000
            })
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete this?');
        }


        function validation() {

            var txtQuestion = document.getElementById('txtQuestion');
            var ddlQType = document.getElementById('ddlQType');
            var QLabel = document.getElementById('QLabel');



            //alert(txtQuestion.value);
            //alert(ddlQType.value);
            //alert(QLabel.value);

            var result;



            if (txtQuestion.value == '') {
                alertt('', 'Question ID cannot be left blank', 'error');
                document.getElementById('txtQuestion').focus();
                return false;
            }
            if (ddlQType.value == '0') {
                alertt('', 'Please select question type', 'error');
                document.getElementById('ddlQType').focus();
                return false;
            }
            if (QLabel.value == '') {
                alertt('', 'Question label cannot be left blank', 'error');
                document.getElementById('QLabel').focus();
                return false;
            }

            return true;
        }
    </script>
    <style type="text/css">
        .Gridview {
            font-family: Verdana;
            font-size: 10pt;
            font-weight: normal;
            color: black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1">&nbsp;</div>
        </div>
        <!--/Preloader-->
        <div class="wrapper theme-1-active pimary-color-red">

            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>

            <div class="page-wrapper">
                <div class="container-fluid">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Pre-Screening</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="#"><span>Pre Screening</span></a></li>
                                <li class="active"><span>Screening</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->

                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Screening:</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">

                                    <div class="row">

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">

                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Question ID</label>
                                                <asp:TextBox ID="txtQuestion" runat="server" class="form-control" placeholder="Question"></asp:TextBox>

                                            </div>
                                        </div>



                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Question Type</label>
                                                <asp:DropDownList ID="ddlQType" runat="server" class="form-control" data-style="form-control ui search selection dropdown">
                                                    <asp:ListItem Value="0">--Select--</asp:ListItem>

                                                    <asp:ListItem Value="1">Single</asp:ListItem>

                                                    <asp:ListItem Value="2">Multi</asp:ListItem>

                                                    <asp:ListItem Value="3">Open Text</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Label</label>
                                                <asp:TextBox ID="QLabel" runat="server" class="form-control" placeholder="Question"></asp:TextBox>

                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">

                                        <div class=" col-xs-12">
                                            <div class="form-group">
                                                <br />
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim"
                                                    Text="Submit" OnClick="btnSubmit_Click" OnClientClick="return validation();" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                 
                                               
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row" id="dvoptions" runat="server">

                                        <div class=" col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Options</label>
                                                <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="False" class="table table-hover"
                                                    DataKeyNames="ID" OnRowCancelingEdit="gvDetails_RowCancelingEdit" OnRowDataBound="gvDetails_RowDataBound" OnRowEditing="gvDetails_RowEditing"
                                                    OnRowUpdating="gvDetails_RowUpdating" OnRowCommand="gvDetails_RowCommand" ShowFooter="True" OnRowDeleting="gvDetails_RowDeleting">
                                                    <Columns>



                                                        <%--  <asp:TemplateField HeaderText="Question Id"  ControlStyle-Width="200px">


<ItemTemplate>
  <asp:Label ID="lblcollabel" runat="server" Text='<%# Bind("ColumnName") %>'></asp:Label> 
  </ItemTemplate> 
   
  </asp:TemplateField> --%>
                                                        <asp:TemplateField HeaderText="Id" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblid" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Label" HeaderStyle-Font-Bold="true">


                                                            <ItemTemplate>
                                                                <asp:Label ID="lbltext" runat="server" Text='<%# Eval("OptionLabel") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <EditItemTemplate>
                                                                <asp:TextBox ID="txttext" runat="server" Text='<%# Eval("OptionLabel") %>' class="form-control"></asp:TextBox>
                                                            </EditItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="txtnewtext" runat="server" Text='<%# Eval("OptionLabel") %>' class="form-control"></asp:TextBox>
                                                            </FooterTemplate>

                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Code" HeaderStyle-Font-Bold="true">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblvalue" runat="server" Text='<%# Eval("OptionCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <EditItemTemplate>
                                                                <asp:TextBox ID="txtvalue" runat="server" class="form-control" Text='<%# Eval("OptionCode") %>'></asp:TextBox>
                                                            </EditItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="txtNewvalue" runat="server" class="form-control" Text='<%# Eval("OptionCode") %>'></asp:TextBox>
                                                                <%--  <cc1:FilteredTextBoxExtender ID="FtlQty" runat="server" FilterMode="ValidChars"
                                                                    ValidChars="$0123456789" TargetControlID="txtNewvalue">
                                                                </cc1:FilteredTextBoxExtender>--%>
                                                            </FooterTemplate>
                                                        </asp:TemplateField>








                                                        <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-Font-Bold="true">
                                                            <EditItemTemplate>
                                                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>
                                                                <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                                                            </EditItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="AddNew" Text="Add New"></asp:LinkButton>
                                                            </FooterTemplate>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%--<asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True"  HeaderStyle-Font-Bold="true"  /> --%>

                                                        <asp:TemplateField HeaderText="Delete">
                                                            <ItemTemplate>

                                                                <asp:LinkButton ID="Button1" runat="server" CommandName="Delete" OnClientClick="return confirmDelete()" Text="Delete" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>

                                    </div>

                                    <%--                                      <div class="row">
                                       
                                           <div class=" col-xs-12">
                                            <div class="form-group">
                                                  <br />
                                              
                                                 <button type="button" class="btn btn-success btn-anim" id="btnnew" name="btnnew" >Add more</button>
                                               
                                            </div>
                                        </div>

                                          </div>--%>
                                </div>
                            </div>
                            <br />
                        </div>
                    </div>
                </div>
            </div>
            <!-- Footer -->
            <uc2:Footer ID="Footer1" runat="server" />
            <!-- /Footer -->
        </div>

    </form>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('#ddlQType').dropdown();

            //$('#btnnew').hide();

            //$('#btnSubmit').click(function () {
            //    $('#btnnew').show();
            //});

            //$('#btnnew').click(function () {
            //    $('#btnSubmit').show();
            //});
        });


    </script>
</body>
</html>
