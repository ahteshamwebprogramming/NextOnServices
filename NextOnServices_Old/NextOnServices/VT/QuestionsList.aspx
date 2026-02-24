<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QuestionsList.aspx.cs" Inherits="VT_QuestionsList" %>

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
    </script>
      <style type="text/css">
.Gridview
{
font-family:Verdana;
font-size:10pt;
font-weight:normal;
color:black;

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
                                <li class="active"><span>Questions List</span></li>
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
                                        <h6 class="panel-title txt-dark">Questions List:</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">


                                      <div class="row">
                                       
                                           <div class=" col-xs-12">
                                            <div class="form-group">
                                                 
                                                   <asp:GridView ID="gvDetails" runat="server"  AutoGenerateColumns="False" class="table table-hover" 
     DataKeyNames="ID"  OnRowEditing="gvDetails_RowEditing"    OnRowDeleting="gvDetails_RowDeleting" > 
<Columns> 

    <asp:TemplateField HeaderText="Id" Visible="false" >
        <ItemTemplate> 
  <asp:Label ID="lblid" runat="server" Text='<%# Eval("ID") %>'></asp:Label> 
</ItemTemplate> 
    </asp:TemplateField>
     <asp:TemplateField HeaderText="Qtype" Visible="false" >
        <ItemTemplate> 
  <asp:Label ID="lbltype" runat="server" Text='<%# Eval("QuestionType") %>'></asp:Label> 
</ItemTemplate> 
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Question ID" HeaderStyle-Font-Bold="true" >


<ItemTemplate>
  <asp:Label ID="lbltext" runat="server" Text='<%# Eval("QuestionID") %>'   ></asp:Label> 
  </ItemTemplate> 


  
</asp:TemplateField> 
<asp:TemplateField HeaderText="Label" HeaderStyle-Font-Bold="true" > 
<ItemTemplate> 
  <asp:Label ID="lblvalue" runat="server" Text='<%# Eval("Questionlabel") %>'></asp:Label> 
</ItemTemplate> 

</asp:TemplateField> 

    <asp:TemplateField HeaderText="Label Type" HeaderStyle-Font-Bold="true" > 
<ItemTemplate> 
  <asp:Label ID="lblQType" runat="server" Text='<%# Eval("QType") %>'></asp:Label> 
</ItemTemplate> 

</asp:TemplateField> 








    <asp:CommandField HeaderText="Edit"  ShowEditButton="true" ShowHeader="True"  HeaderStyle-Font-Bold="true"  /> 

      <asp:TemplateField  HeaderText="Delete" >
                                       <ItemTemplate>
                                        
                                        <asp:LinkButton ID="Button1" runat="server" CommandName="Delete" OnClientClick="return confirmDelete()" Text="Delete" />
                                          </ItemTemplate>
                                       </asp:TemplateField>
<%--<asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True"  HeaderStyle-Font-Bold="true"  /> --%>

</Columns> 
</asp:GridView>
                                            </div>
                                        </div>

                                          </div>


                                   
                                       
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
    
</body>
</html>