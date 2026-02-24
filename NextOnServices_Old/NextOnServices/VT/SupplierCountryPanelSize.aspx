<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierCountryPanelSize.aspx.cs" EnableEventValidation="false" Inherits="SupplierCountryPanelSize" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <script type="text/javascript">

        function validation() {



            var txtSize = document.getElementById('txtSize');
            var ddlCountry = document.getElementById('ddlCountry');

            // alert('Asif');

            var result;




            if (txtSize.value == '') {
                alertt('', 'Sample Size cannot be left blank', 'error');
                document.getElementById('txtSize').focus();
                return false;
            }

            if (ddlCountry.value == '0') {
                alertt('', 'Please select country', 'error');
                document.getElementById('ddlCountry').focus();
                return false;
            }
            return true;
        }
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

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
            return true;
        };

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
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
                            <h5 class="txt-dark">Supplier</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="SupplierDetails.aspx"><span>Supplier</span></a></li>
                                <li class="active"><span>Country Panel Size</span></li>
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
                                        <h6 class="panel-title txt-dark">Country Panel Size</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div class="row">

                                        <div class="col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <asp:DropDownList ID="ddlCountry" runat="server" class="form-control" data-style="form-control ui search selection dropdown">
                                                </asp:DropDownList>

                                            </div>
                                        </div>


                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Panel Size</label>
                                                <asp:TextBox ID="txtSize" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter Panel Size"></asp:TextBox>

                                            </div>
                                        </div>


                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit" OnClick="btnSubmit_Click" />
                                                <%--	<button type="button" class="btn btn-success btn-anim"><i class="icon-rocket"></i><span class="btn-text">submit</span></button>--%>
                                            </div>
                                        </div>

                                        <div class="col-xs-12">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <div class="panel panel-default card-view">
                                                        <div class="panel-heading">
                                                            <div class="pull-left">
                                                                <h6 class="panel-title txt-dark">Supplier Country Panel Size </h6>
                                                            </div>
                                                            <div class="clearfix"></div>
                                                        </div>
                                                        <div class="panel-wrapper collapse in">
                                                            <div class="panel-body">
                                                                <div class="table-wrap">
                                                                    <div class="">
                                                                        <table id="myTable1" class="table table-hover display  pb-30">
                                                                            <thead>
                                                                                <tr>
                                                                                    <th>Supplier Name</th>
                                                                                    <th>Country</th>
                                                                                    <th>Sample Size</th>
                                                                                    <th>Edit</th>
                                                                                </tr>
                                                                            </thead>

                                                                            <tbody></tbody>


                                                                        </table>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>



                                <br />
                            </div>
                        </div>

                    </div>
                    <input type="hidden" value="" runat="server" id="hfPanelIndividualID" />
                    <input type="hidden" runat="server" value="Submit" id="hfPanelButtonval" />

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>

    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script type="text/javascript">




        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});


        jQuery(document).ready(function () {

            $('#ddlCountry').dropdown();

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'SupplierCountryPanelSize.aspx/GetSamplePanelSize',
                cache: false,
                dataType: "json",
                success: function (data) {

                    var tbody = "";
                    for (var i = 0; i < data.d.length; i++) {

                        tbody += '<tr><td>' + data.d[i].Name + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].PSize + '</td><td><input type="button" class="btn samewidth btn-primary" value="Edit" onclick=EDIT("' + data.d[i].ID + '") /></td></tr>';
                        //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    }
                    $("#myTable1 tbody").html(tbody);
                    $("#myTable1").DataTable();
                    //  $('#example').DataTable();
                },
                error: function (result) {
                    alert(result.d);
                }
            });

            //  $('#myTable1').DataTable();

        });

        function EDIT(ID) {
            //  alert(id);
            // $('#ddlCountry option[value="2"]').attr('selected', true);
            //$('#ddlCountry').val('4').change();

            $('#txtSize').val();
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'SupplierCountryPanelSize.aspx/FetchDetailsbyId',
                data: JSON.stringify({ id: ID }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.d.length > 0) {
                        $('#txtSize').val(data.d[0].PSize);
                        //$('#ddlCountry option[value=' + data.d[0].Country + ']').prop('selected', true);
                        $('#ddlCountry').val(data.d[0].Country).change();
                        $('#hfPanelIndividualID').val(data.d[0].ID);
                        $('#btnSubmit').val('Update');
                        $('#hfPanelButtonval').val('Update');
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
    </script>
</body>
</html>
